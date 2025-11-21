

namespace CPEI_MFG.Communicate.Interface
{
    internal interface IConnectable
    {
        bool Connect();
        bool IsConnect { get; }
        bool Disconnect();
    }
}
