using System.Collections.Generic;

namespace CPEI_MFG.Services
{
    public interface IService
    {
        IReadOnlyCollection<string> Errorcodes {  get; }
        bool TryGetErrorcode(out string Errorcode);
        bool Init();
        void AddModel(object model);
        void Check(string line);
        void Reset();
    }
}