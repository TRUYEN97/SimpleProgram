using CPEI_MFG.Common;
using CPEI_MFG.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPEI_MFG.Services
{
    internal class LogAnalysis
    {
        private readonly List<IService> services;
        public LogAnalysis()
        {
            services = new List<IService>();
        }
        public void AddService(IService service)
        {
            if (service == null) return;
            services.Add(service);
        }
        public bool Init()
        {
            return services.All(s => s.Init());
        }
        public ResultModel Analysis(string path)
        {
            ResultModel testModel = new ResultModel(path);
            FileUtil.TailLogFile(path, (line) =>
            {
                OnNewLine?.Invoke(line);
                foreach (var service in services)
                {
                    service.Check(line);
                }
                return line.Contains("] ===All DONE================================") || line.Equals("quit FTU..");
            });
            string errorCode;
            if (TryGetErrorCode(out errorCode))
            {
                testModel.SetFail(errorCode);
            }
            else
            {
                testModel.SetPass();
            }
            return testModel;
        }

        private bool TryGetErrorCode(out string errorCode)
        {
            errorCode = null;
            foreach (var service in services)
            {
                if (service.TryGetErrorcode(out errorCode) && !string.IsNullOrWhiteSpace(errorCode))
                {
                    return true;
                }
            }
            return false;
        }

        public event Action<string> OnNewLine;
        public void Reset()
        {
            foreach (var service in services)
            {
                service.Reset();
            }
        }
    }
}
