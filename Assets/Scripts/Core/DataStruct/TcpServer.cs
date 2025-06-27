using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MNP.Managers;

namespace MNP.Core.DataStruct
{
    public class TcpServer
    {
        private TcpListener listener;
        private bool isRunning;

        public int ID = -1;
        public int port = -1;

        public string receivedRespond = "";

        public TcpServer(string ip = "127.0.0.1", int port = 6132)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            isRunning = false;
            this.port = port;
        }

        public void Start()
        {
            if (isRunning)
            {
                LogManager.LogWarn($"Tcp服务器：Tcp服务器#{ID}已启动");
                return;
            }
            listener.Start();
            isRunning = true;
            LogManager.LogInfo($"Tcp服务器：Tcp服务器#{ID}已启动监听，等待客户端连接");

            Task.Run(() => AcceptClientsAsync());
        }

        private async Task AcceptClientsAsync()
        {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    LogManager.LogInfo($"Tcp服务器：Tcp服务器#{ID}与客户端连接成功");
                    while (isRunning)
                    {
                        await HandleClientAsync(client);
                    }
                }
                catch (Exception e)
                {
                    LogManager.LogError($"Tcp服务器：Tcp服务器#{ID}于等待客户端连接中发生错误，错误信息为{Environment.NewLine}{e.Message}");
                }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    receivedRespond = dataReceived;
                    /*
                    // Send response to client
                    string response = "Hello from server!";
                    byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                    await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    */
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"Tcp服务器：Tcp服务器#{ID}于处理客户端发送的信息中发生错误，错误信息为{Environment.NewLine}{e.Message}");
            }
        }

        public void Stop()
        {
            isRunning = false;
            listener.Stop();
            LogManager.LogInfo($"Tcp服务器：Tcp服务器#{ID}已停止");
        }

        public bool IsRunning => isRunning;
    }
}