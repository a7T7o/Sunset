// 聊天服务器 - Unity版本
// 用法：挂到空物体上，运行即启动服务器
using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ChatServer : MonoBehaviour
{
    TcpListener server;
    List<TcpClient> people = new List<TcpClient>();  // 连接的人
    List<string> log = new List<string>();           // 日志
    bool running = false;

    void Start()
    {
        // 启动服务器
        server = new TcpListener(IPAddress.Any, 8888);
        server.Start();
        running = true;
        AddLog("服务器启动 端口8888");
        
        // 开线程接受连接
        new Thread(AcceptPeople).Start();
    }

    // 接受新连接
    void AcceptPeople()
    {
        while (running)
        {
            if (server.Pending())
            {
                TcpClient newguy = server.AcceptTcpClient();
                people.Add(newguy);
                AddLog("有人来了! 现在" + people.Count + "人");
                new Thread(() => HandlePerson(newguy)).Start();
            }
            Thread.Sleep(100);
        }
    }

    // 处理一个人的消息
    void HandlePerson(TcpClient person)
    {
        NetworkStream s = person.GetStream();
        byte[] buf = new byte[1024];
        
        while (running && person.Connected)
        {
            try
            {
                if (s.DataAvailable)
                {
                    int n = s.Read(buf, 0, buf.Length);
                    if (n > 0)
                    {
                        string msg = Encoding.UTF8.GetString(buf, 0, n);
                        AddLog(msg);
                        
                        // 转发给其他人
                        foreach (var p in people.ToArray())
                        {
                            if (p != person && p.Connected)
                            {
                                try { p.GetStream().Write(buf, 0, n); } catch { }
                            }
                        }
                    }
                }
            }
            catch { break; }
            Thread.Sleep(50);
        }
        
        people.Remove(person);
        AddLog("有人走了");
    }

    void AddLog(string s) { lock (log) log.Add(s); }

    // 显示界面
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 500));
        GUILayout.Label("===== 聊天服务器 =====");
        GUILayout.Label("在线人数: " + people.Count);
        GUILayout.Space(10);
        
        lock (log)
        {
            foreach (var l in log)
                GUILayout.Label(l);
        }
        GUILayout.EndArea();
    }

    void OnDestroy()
    {
        running = false;
        foreach (var p in people) p.Close();
        server?.Stop();
    }
}
