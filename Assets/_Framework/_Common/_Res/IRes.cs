using System;

namespace ResLoadMgr
{
    public interface IRes
    {
        event Action<UnityEngine.Object> LoadFinishEvent;

        T Load<T>(string name) where T : UnityEngine.Object;
        void LoadAsync<T>(string name) where T : UnityEngine.Object;
    }
}