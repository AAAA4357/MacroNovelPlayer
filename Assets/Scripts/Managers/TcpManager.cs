using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MNP.Core.DataStruct;

namespace MNP.Managers
{
    public class TcpManager : MonoBehaviour
    {
        public Dictionary<int, TcpServer> servers;

        private List<int> startedPorts;
        int counter = 0;

        void Awake()
        {
            servers = new();
            startedPorts = new();

            LogManager.LogInfo("VNPTcp管理器：初始化完成");

            //int id = InitNewServer();
            //StartServer(id);
        }

        void OnApplicationQuit()
        {
            foreach (var id in servers.Keys)
            {
                StopServer(id);
            }
        }

        public int InitNewServer(string ip = "127.0.0.1", int port = 6132)
        {
            if (startedPorts.Contains(port))
            {
                LogManager.LogError("VNPTcp管理器：注册端口已被使用的Tcp服务器");
            }
            int id = counter;
            counter++;
            TcpServer server = new(ip, port)
            {
                ID = id
            };
            servers.Add(id, server);
            startedPorts.Add(port);
            LogManager.LogInfo("VNPTcp管理器：注册新的Tcp服务器，ID#" + id);
            return id;
        }

        public void StartServer(int id)
        {
            if (!servers.ContainsKey(id))
            {
                LogManager.LogError("VNPTcp管理器：启动未知的Tcp服务器");
                return;
            }
            TcpServer server = servers[id];
            server.Start();
        }

        public void StopServer(int id)
        {
            if (!servers.ContainsKey(id))
            {
                LogManager.LogError("VNPTcp管理器：停止未知的Tcp服务器");
                return;
            }
            TcpServer server = servers[id];
            server.Stop();
        }

        public void ReleaseServer(int id)
        {
            if (!servers.ContainsKey(id))
            {
                LogManager.LogError("VNPTcp管理器：释放未经注册的Tcp服务器");
                return;
            }
            TcpServer server = servers[id];
            if (server.IsRunning)
            {
                LogManager.LogError("VNPTcp管理器：关闭运行中的Tcp服务器#" + id);
                return;
            }
            servers.Remove(id);
            startedPorts.Remove(server.port);
        }
    }
}
