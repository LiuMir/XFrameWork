using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResLoadMgr
{
    public class BundleRes : IRes
    {
        private AssetBundle m_Bundle;
        private int m_RefCount;
        private List<string> m_LoadingAsset;

        public event Action<UnityEngine.Object> LoadFinishEvent;

        public AssetBundle Bundle{get => m_Bundle;} // asset bundle

        public int RefCount{get => m_RefCount;} // 引用计数
 
        public BundleRes()
        {
            m_RefCount = 1;
            m_LoadingAsset = new List<string>();
        }

        public void SetBundle(AssetBundle bundle)
        {
            m_Bundle = bundle;
        }

        public void AddRef()
        {
            m_RefCount++;
        }

        public void SubRef()
        {
            m_RefCount--;
        }

        public T Load<T>(string name) where T : UnityEngine.Object
        {
            T asset = m_Bundle.LoadAsset<T>(name);
            if (null == asset)
            {
                Debug.LogError($"{m_Bundle.name} 中没有名为:<- {name} ->的资源");
            }
            return asset;
        }

        public async void LoadAsync<T>(string name) where T : UnityEngine.Object
        {
            if (!m_LoadingAsset.Contains(name))
            {
                AssetBundleRequest request = Bundle.LoadAssetAsync<T>(name);
                m_LoadingAsset.Add(name);
                await request;
                T asset = request.asset as T;
                m_LoadingAsset.Remove(name);
                if (null == asset)
                {
                    Debug.LogError($"{m_Bundle.name} 中没有名为:<- {name} ->的资源");
                }
                else
                {
                    LoadFinishEvent?.Invoke(request.asset);
                    LoadFinishEvent = null;
                }
            }
        }

        public int Unload()
        {
            SubRef();
            if (m_RefCount <= 0)
            {
                m_Bundle.Unload(true);
            }
            return m_RefCount;
        }
        
    }
}