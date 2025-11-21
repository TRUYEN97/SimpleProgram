using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UiTest.Service.Communicate.Implement.SocketSv.Client;

namespace UiTest.Service.Communicate.Implement.SocketSv.Server
{
    public class ServerSocket : IDisposable
    {
        private readonly Socket serverSocket;
        public readonly ISocketManagement SocketManagement;
        private Task currentTask;
        public ServerSocket(ISocketManagement socketManagement)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketManagement = socketManagement;
        }

        public void Listen(int port)
        {
            if (IsRunning) return;
            currentTask = new Task(() =>
            {
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                serverSocket.Listen(SocketManagement.Logback);
                Socket socket;
                while ((socket = serverSocket.Accept()) != null)
                {
                    SocketManagement?.AddNewClient(new ClientSocket(socket));
                }
            });
        }

        public bool IsRunning => currentTask != null && !currentTask.IsCompleted;

        public void Close()
        {
            try
            {
                serverSocket?.Close();
            }
            catch (Exception)
            {

            }
        }

        public void Dispose()
        {
            try
            {
                Close();
                serverSocket?.Dispose();
            }
            catch (Exception)
            {

            }
        }
    }
}
