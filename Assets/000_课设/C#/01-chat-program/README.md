# 题目1：网络聊天程序 (C#版本)

## 功能说明
基于TCP Socket实现的多人聊天程序，包含服务器端和客户端。

## 文件说明
- `ChatServer.cs` - 聊天服务器，负责接收连接和消息转发
- `ChatClient.cs` - 聊天客户端，负责发送和接收消息

## 编译方法
```bash
# 编译服务器
csc ChatServer.cs -out:ChatServer.exe

# 编译客户端
csc ChatClient.cs -out:ChatClient.exe
```

## 运行方法
1. 先启动服务器：`ChatServer.exe`
2. 再启动客户端：`ChatClient.exe`
3. 可以启动多个客户端进行聊天

## 技术要点
1. **TCP Socket通信**：使用TcpListener和TcpClient
2. **多线程**：服务器为每个客户端创建独立线程
3. **消息广播**：服务器将消息转发给所有在线客户端
4. **线程安全**：使用lock保护共享资源

## 程序流程
```
服务器端:
1. 创建TcpListener监听端口
2. 循环Accept接受客户端连接
3. 为每个客户端创建处理线程
4. 接收消息并广播给其他客户端

客户端:
1. 创建TcpClient连接服务器
2. 发送用户名
3. 创建接收线程持续接收消息
4. 主线程读取用户输入并发送
```
