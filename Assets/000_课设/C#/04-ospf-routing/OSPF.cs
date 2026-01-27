// OSPF路由协议仿真 - Unity版本
// 用法：挂到空物体上，显示路由表计算过程
using UnityEngine;
using System.Collections.Generic;

public class OSPF : MonoBehaviour
{
    // 图的邻接表
    Dictionary<string, Dictionary<string, int>> graph = new Dictionary<string, Dictionary<string, int>>();
    
    // 结果
    List<string> log = new List<string>();
    Vector2 scroll;
    string selectedNode = "A";

    void Start()
    {
        // 建图: A-B:2, A-D:5, B-C:1, B-E:2, C-F:8, D-E:1, E-F:4
        AddEdge("A", "B", 2);
        AddEdge("A", "D", 5);
        AddEdge("B", "C", 1);
        AddEdge("B", "E", 2);
        AddEdge("C", "F", 8);
        AddEdge("D", "E", 1);
        AddEdge("E", "F", 4);
        
        Log("===== OSPF路由协议仿真 =====");
        Log("网络拓扑:");
        Log("  A--2--B--1--C");
        Log("  |     |     |");
        Log("  5     2     8");
        Log("  |     |     |");
        Log("  D--1--E--4--F");
        Log("");
        Log("点击节点按钮查看路由表");
    }

    void AddEdge(string a, string b, int cost)
    {
        if (!graph.ContainsKey(a)) graph[a] = new Dictionary<string, int>();
        if (!graph.ContainsKey(b)) graph[b] = new Dictionary<string, int>();
        graph[a][b] = cost;
        graph[b][a] = cost;
    }

    // Dijkstra算法
    void CalcRoute(string src)
    {
        log.Clear();
        Log("===== 节点" + src + "的路由表 =====");
        Log("(使用Dijkstra算法计算)");
        Log("");
        
        // 初始化
        Dictionary<string, int> dist = new Dictionary<string, int>();
        Dictionary<string, string> nextHop = new Dictionary<string, string>();
        HashSet<string> done = new HashSet<string>();
        
        foreach (var n in graph.Keys)
        {
            dist[n] = int.MaxValue;
            nextHop[n] = "";
        }
        dist[src] = 0;
        
        // 主循环
        while (done.Count < graph.Count)
        {
            // 找最小的
            string u = null;
            int min = int.MaxValue;
            foreach (var n in graph.Keys)
            {
                if (!done.Contains(n) && dist[n] < min)
                {
                    min = dist[n];
                    u = n;
                }
            }
            if (u == null) break;
            
            done.Add(u);
            
            // 更新邻居
            foreach (var kv in graph[u])
            {
                string v = kv.Key;
                int w = kv.Value;
                if (dist[u] + w < dist[v])
                {
                    dist[v] = dist[u] + w;
                    nextHop[v] = (u == src) ? v : nextHop[u];
                }
            }
        }
        
        // 输出路由表
        foreach (var n in graph.Keys)
        {
            if (n != src)
            {
                Log("到" + n + ": 下一跳=" + nextHop[n] + ", 开销=" + dist[n]);
            }
        }
    }

    void Log(string s) { log.Add(s); }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 500));
        
        GUILayout.Label("===== OSPF路由协议 =====");
        GUILayout.Label("选择节点查看路由表:");
        
        GUILayout.BeginHorizontal();
        foreach (var n in graph.Keys)
        {
            if (GUILayout.Button(n, GUILayout.Width(40)))
            {
                selectedNode = n;
                CalcRoute(n);
            }
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(380));
        foreach (var l in log)
            GUILayout.Label(l);
        GUILayout.EndScrollView();
        
        GUILayout.EndArea();
    }
}
