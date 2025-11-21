using UiTest.Service.Communicate.Implement.SocketSv.Client;

namespace UiTest.Service.Communicate.Implement.SocketSv.Server
{
    public interface ISocketManagement
    {
        void AddNewClient(ClientSocket clientSocket);
        int Logback {  get; }
    }
}
