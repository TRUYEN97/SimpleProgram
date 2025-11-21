

using CPEI_MFG.Communicate.Interface;

namespace CPEI_MFG.Communicate
{
    public abstract class BaseCommunicate : BaseInOutStream, IConnectable
    {
        public abstract bool Connect();
        public abstract bool IsConnect {  get; }
        public abstract bool Disconnect();
    }
}
