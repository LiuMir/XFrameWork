using System;

namespace ResLoadMgr
{
    public class AssetLoadManager:Singleton<AssetLoadManager>
    {
        // 同步加载unity中的Asset
        public T Load<T>(string path, string name) where T : UnityEngine.Object
        {
            IRes res = ResManager.Instance.Load(path, name);
            if (null == res)
            {
                return null;
            }
            return res.Load<T>(name);
        }

        // 异步加载unity中的Asset
        public async void LoadAsync<T>(string path, string name, Action<UnityEngine.Object> action) where T : UnityEngine.Object
        {
            IRes res = await ResManager.Instance.LoadAsync(path, name, action);
            if (null != res)
            {
                res.LoadFinishEvent += action;
                res.LoadAsync<T>(name);
            }
        }

        public void Unload(string path, string name)
        {
            ResManager.Instance.UnloadRes(path);
        }

    }
}