// 网络路由模拟器 - Unity版本
// 用法：挂到空物体上，模拟数据包转发
using UnityEngine;
using System.Collections.Generic;

public class RouterSim : MonoBehaviour
{
    // 路由表: 路由器名 -> [(目标网络, 下一跳)]
    Dictionary<string, List<Route>> routes = new Dictionary<string, List<Route>>();
    
    struct Route
    {
        public string net;   // 目标网络
        public string next;  // 下一跳
        public Route(string n, string x) { net = n; next = x; }
    }
    
    List<string> log = new List<string>();
    Vector2 scroll;
    string fromRouter = "R1";
    string destIP = "192.168.4.10";

    void Start()
    {
        // 建立路由表
        routes["R1"] = new List<Route> {
            new Route("192.168.1.0", "直连"),
            new Route("192.168.2.0", "R2"),
            new Route("192.168.3.0", "R2"),
            new Route("192.168.4.0", "R2")
        };
        routes["R2"] = new List<Route> {
            new Route("192.168.1.0", "R1"),
            new Route("192.168.2.0", "直连"),
            new Route("192.168.3.0", "R3"),
            new Route("192.168.4.0", "R3")
        };
        routes["R3"] = new List<Route> {
            new Route("192.168.1.0", "R2"),
            new Route("192.168.2.0", "R2"),
            new Route("192.168.3.0", "直连"),
            new Route("192.168.4.0", "R4")
        };
        routes["R4"] = new List<Route> {
            new Route("192.168.1.0", "R3"),
            new Route("192.168.2.0", "R3"),
            new Route("192.168.3.0", "R3"),
            new Route("192.168.4.0", "直连")
        };
        
        Log("===== 网络路由模拟器 =====");
        Log("拓扑: [PC1]-R1--R2--R3--R4-[PC4]");
        Log("");
        ShowRouteTables();
    }

    void ShowRouteTables()
    {
        foreach (var r in routes)
        {
            Log(r.Key + " 路由表:");
            foreach (var e in r.Value)
                Log("  " + e.net + "/24 -> " + e.next);
            Log("");
        }
    }

    // 模拟转发
    void Forward()
    {
        log.Clear();
        ShowRouteTables();
        
        Log("===== 转发: " + fromRouter + " -> " + destIP + " =====");
        
        // 获取目标网络
        int lastDot = destIP.LastIndexOf('.');
        string destNet = destIP.Substring(0, lastDot) + ".0";
        
        string cur = fromRouter;
        List<string> path = new List<string> { cur };
        
        for (int i = 0; i < 10; i++)  // 最多跳10次
        {
            // 查路由表
            string next = "找不到";
            foreach (var r in routes[cur])
            {
                if (r.net == destNet)
                {
                    next = r.next;
                    break;
                }
            }
            
            Log(cur + " 查表: " + destNet + " -> " + next);
            
            if (next == "直连")
            {
                Log("到达目的网络!");
                Log("路径: " + string.Join(" -> ", path) + " -> [目的地]");
                return;
            }
            
            if (next == "找不到")
            {
                Log("路由不可达!");
                return;
            }
            
            cur = next;
            path.Add(cur);
        }
        
        Log("路由环路!");
    }

    void Log(string s) { log.Add(s); }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 450, 550));
        
        GUILayout.Label("===== 网络路由模拟器 =====");
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("从:", GUILayout.Width(30));
        if (GUILayout.Button(fromRouter, GUILayout.Width(50)))
        {
            // 切换路由器
            if (fromRouter == "R1") fromRouter = "R2";
            else if (fromRouter == "R2") fromRouter = "R3";
            else if (fromRouter == "R3") fromRouter = "R4";
            else fromRouter = "R1";
        }
        GUILayout.Label("到:", GUILayout.Width(30));
        destIP = GUILayout.TextField(destIP, GUILayout.Width(150));
        if (GUILayout.Button("转发", GUILayout.Width(60)))
            Forward();
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(450));
        foreach (var l in log)
            GUILayout.Label(l);
        GUILayout.EndScrollView();
        
        GUILayout.EndArea();
    }
}
