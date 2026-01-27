// 滑动窗口协议仿真 - Unity版本
// 用法：挂到空物体上，点击按钮模拟发送
using UnityEngine;
using System.Collections.Generic;

public class SlidingWindow : MonoBehaviour
{
    // 参数
    int winSize = 4;      // 窗口大小
    int maxSeq = 8;       // 序号范围0-7
    
    // 发送方变量
    int sendBase = 0;     // 发送窗口起点
    int nextSend = 0;     // 下一个要发的
    bool[] gotAck = new bool[8];  // 收到ACK没
    
    // 接收方变量
    int recvBase = 0;     // 接收窗口起点
    bool[] gotData = new bool[8]; // 收到数据没
    
    // 数据
    string[] data = {"A", "B", "C", "D", "E", "F", "G", "H"};
    int sent = 0;         // 发了几个
    
    // 日志
    List<string> log = new List<string>();
    Vector2 scroll;
    bool done = false;

    void Start()
    {
        Log("===== 滑动窗口协议仿真 =====");
        Log("窗口大小: " + winSize + ", 序号: 0-7");
        Log("点击[发送下一帧]模拟传输");
        Log("20%概率丢包");
    }

    // 发送一帧
    void SendOneFrame()
    {
        if (done) return;
        
        // 检查窗口是否满了
        if ((nextSend - sendBase + maxSeq) % maxSeq >= winSize)
        {
            Log("[窗口满] 等待ACK...");
            return;
        }
        
        if (sent >= data.Length)
        {
            Log("===== 所有数据发送完毕 =====");
            done = true;
            return;
        }
        
        // 模拟丢包
        bool lost = Random.value < 0.2f;
        
        Log("发送帧" + nextSend + "(" + data[sent] + ") -> " + (lost ? "X丢了!" : "OK"));
        
        if (!lost)
        {
            gotData[nextSend] = true;
            Log("  接收方收到帧" + nextSend);
            
            // 发ACK
            gotAck[nextSend] = true;
            Log("  ACK" + nextSend + " 确认");
        }
        
        nextSend = (nextSend + 1) % maxSeq;
        sent++;
        
        // 滑动发送窗口
        while (gotAck[sendBase])
        {
            gotAck[sendBase] = false;
            sendBase = (sendBase + 1) % maxSeq;
        }
        
        // 滑动接收窗口并交付
        while (gotData[recvBase])
        {
            Log("  交付帧" + recvBase);
            gotData[recvBase] = false;
            recvBase = (recvBase + 1) % maxSeq;
        }
        
        Log("[状态] 发送窗口=" + sendBase + ", 接收窗口=" + recvBase);
        Log("");
    }

    void Log(string s) { log.Add(s); }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 500, 600));
        
        GUILayout.Label("===== 滑动窗口协议 =====");
        GUILayout.Label("发送窗口: " + sendBase + " ~ " + ((sendBase + winSize - 1) % maxSeq));
        GUILayout.Label("接收窗口: " + recvBase);
        GUILayout.Label("已发送: " + sent + "/" + data.Length);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("发送下一帧", GUILayout.Width(150)))
            SendOneFrame();
        
        if (GUILayout.Button("自动发送全部", GUILayout.Width(150)))
            InvokeRepeating("SendOneFrame", 0, 0.5f);
        
        GUILayout.Space(10);
        
        scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(400));
        foreach (var l in log)
            GUILayout.Label(l);
        GUILayout.EndScrollView();
        
        GUILayout.EndArea();
    }
}
