// 聊天客户端 - Unity版本
// 用法：挂到空物体上，运行后输入IP连接服务器
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public class ChatClient : MonoBehaviour
{
    public string serverIP = "127.0.0.1";  // 服务器IP
    public string myName = "玩家";          // 我的名字
    
    TcpClient me;
    NetworkStream stream;
    List<string> messages = new List<string>();  // 消息列表
    string inputText = "";                        // 输入框
    bool connected = false;
    Vector2 scroll;

    void Start()
    {
        AddMsg("点击[连接]按钮连接服务器");
    }

    // 连接服务器
    void Connect()
    {
        try
        {
            me = new TcpClient(serverIP, 8888);
            stream = me.GetStream();
            connected = true;
            AddMsg("连接成功!");
            new Thread(ReceiveMsg).Start();
        }
        catch
        {
            AddMsg("连接失败! 检查服务器是否启动");
        }
    }

    // 接收消息线程
    void ReceiveMsg()
    {
        byte[] buf = new byte[1024];
        while (connected && me.Connected)
        {
            try
            {
                if (stream.DataAvailable)
                {
                    int n = stream.Read(buf, 0, buf.Length);
                    if (n > 0)
                        AddMsg(Encoding.UTF8.GetString(buf, 0, n));
                }
            }
            catch { break; }
            Thread.Sleep(50);
        }
    }

    // 发送消息
    void SendMsg(string msg)
    {
        if (!connected || string.IsNullOrEmpty(msg)) return;
        string full = myName + ": " + msg;
        byte[] data = Encoding.UTF8.GetBytes(full);
        stream.Write(data, 0, data.Length);
        AddMsg(full);
    }

    void AddMsg(string s) { lock (messages) messages.Add(s); }

    // 显示界面
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 500));
        GUILayout.Label("===== 聊天客户端 =====");
        
        // 连接设置
        GUILayout.BeginHorizontal();
        GUILayout.Label("IP:", GUILayout.Width(30));
        serverIP = GUILayout.TextField(serverIP, GUILayout.Width(120));
        GUILayout.Label("名字:", GUILayout.Width(40));
        myName = GUILayout.TextField(myName, GUILayout.Width(80));
        if (!connected && GUILayout.Button("连接", GUILayout.Width(60)))
            Connect();
        GUILayout.EndHorizontal();
        
        GUILayout.Label(connected ? "已连接" : "未连接");
        
        // 消息列表
        scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(350));
        lock (messages)
        {
            foreach (var m in messages)
                GUILayout.Label(m);
        }
        GUILayout.EndScrollView();

        // 输入框
        GUILayout.BeginHorizontal();
        inputText = GUILayout.TextField(inputText, GUILayout.Width(300));
        if (GUILayout.Button("发送", GUILayout.Width(80)))
        {
            SendMsg(inputText);
            inputText = "";
        }
        GUILayout.EndHorizontal();
        
        GUILayout.EndArea();
    }

    void OnDestroy()
    {
        connected = false;
        me?.Close();
    }
}
