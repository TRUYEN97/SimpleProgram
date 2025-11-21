using System;
using System.Threading.Tasks;
using CPEI_MFG.Config;
using CPEI_MFG.Config.Errorcode;

namespace CPEI_MFG.Services
{
    public abstract class BaseInApprox<T> : BaseService<T> where T : AbsApproxCommand
    {
        protected override void CheckModel(T model, string line)
        {
            if (model?.IsEnable == true)
            {
                if (line.Contains(model.TextTrigget))
                {
                    model.Count = 0;
                    model.Start = true;
                }
                if (model.Start)
                {
                    if (line.Contains(model.Keywork))
                    {
                        OnTriggerStart(model, line);
                    }
                    if (++model.Count > model.InApprox)
                    {
                        model.Start = false;
                    }
                }
            }
        }
        protected override void ResetModel(T model)
        {
            if (model != null)
            {
                model.Count = 0;
                model.Start = false;
            }
        }
        protected abstract void OnTriggerStart(T model, string line);
    }
}
