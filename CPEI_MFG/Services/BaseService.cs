using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CPEI_MFG.Services
{
    public abstract class BaseService<T> : IService
    {
        protected readonly List<T> models;
        protected readonly List<string> errorCodes;
        protected BaseService()
        {
            models = new List<T>();
            errorCodes = new List<string>();
            IsEnable = true;
        }
        public abstract bool IsEnable { get; protected set; }
        public abstract bool Init();
        public virtual bool TryGetErrorcode(out string errorcode)
        {
            errorcode = null;
            if (errorCodes.Count > 0)
            {
                foreach (var item in errorCodes)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        errorcode = item;
                        return true;
                    }
                }
            }
            return false;
        }
        public IReadOnlyCollection<string> Errorcodes => errorCodes;

        public virtual void AddModel(object model)
        {
            if (model is T)
            {
                models.Add((T) model);
            }
        }

        public virtual void Check(string line)
        {
            if (!IsEnable || string.IsNullOrWhiteSpace(line))
            {
                return;
            }
            Task.Run(() =>
            {
                foreach (T model in models)
                {
                    CheckModel(model, line);
                }
            });
        }

        public virtual void Reset()
        {
            errorCodes.Clear();
            foreach (T model in models)
            {
                ResetModel(model);
            }
        }
        protected abstract void CheckModel(T model, string line);
        protected abstract void ResetModel(T model);
    }
}
