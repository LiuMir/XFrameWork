using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ResLoadMgr
{
    public class ResManager:Singleton<ResManager>
    {
        // 加载中的资源单位
        public class LoadingResUnit {
            public IRes Res;
            public AssetBundleCreateRequest request;
            public LoadingResUnit(IRes res, AssetBundleCreateRequest createRequest)
            {
                Res = res;
                request = createRequest;
            }
        }

        private Dictionary<string, IRes> m_LoadedRes;
        private Dictionary<string, LoadingResUnit> m_LoadingRes;
        private AssetBundleManifest m_manifest;

        public Dictionary<string, LoadingResUnit> LoadingRes{get => m_LoadingRes;} // 加载中的资源 

        public Dictionary<string, IRes> LoadedRes { get => m_LoadedRes; } // 已经加载过的资源 

        //TODO 待尝试AssetBundleManifest的assetbundle调用unload(false)
        public void Init(string manifestPath)
        {
            m_manifest = AssetBundle.LoadFromFile(manifestPath).LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        public IRes Load(string path, string name)
        {
#if UNITY_EDITOR
                IRes res = new EditorRes();
#else
            if (!m_LoadedRes.TryGetValue(path, out IRes res))
            {
                string[] deps = m_manifest.GetAllDependencies(path);
                int len = deps.Length;
                for (int i = 0; i < len; i++)
                {
                    Load(deps[i], "");
                }

                AssetBundle bundle = null;
                if (m_LoadingRes.TryGetValue(path, out LoadingResUnit loadingRes))//正在异步加载的资源，需要打断加载
                {
                    bundle = loadingRes.request.assetBundle;// 直接获取assetBundle会从异步转同步 --> 5.6以后
                    m_LoadingRes.Remove(path);
                }
                else
                {
                    bundle = AssetBundle.LoadFromFile(path);
                }

                if (null == bundle)
                {
                    Debug.LogError($"assetbundle加载失败，没有路径为:<- {path} ->的assetbundle");
                }
                else
                {
                    res = new BundleRes();
                    (res as BundleRes).SetBundle(bundle);
                    m_LoadedRes.Add(path, res);
                }
            }
            if (null != res)
            {
                (res as BundleRes).AddRef();
            }
#endif
            return res;
        }

        public async Task<IRes> LoadAsync(string path, string name, Action<UnityEngine.Object> action, Action countAction = null)
        {
#if UNITY_EDITOR
            IRes result = new EditorRes();  
#else
            string[] deps = m_manifest.GetAllDependencies(path);
            int count = 0;
            int len = deps.Length;
            for (int i = 0; i < len; i++)
            {
                LoadAsync(deps[i], "", null, () => { count++; }).Wait();
            }

            if (m_LoadingRes.TryGetValue(path, out LoadingResUnit resUnit)) //资源加载中
            {
                if (null != resUnit.Res)
                {
                    resUnit.Res.LoadFinishEvent += action;
                }
                return null;
            }

            if (!LoadedRes.TryGetValue(path, out IRes result))
            {
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
                result = new BundleRes();
                m_LoadingRes.Add(path, new LoadingResUnit(result, request));
                await request;// 等待AB加载完毕
                (result as BundleRes).SetBundle(request.assetBundle);
                m_LoadingRes.Remove(path);
                m_LoadedRes.Add(path, result);
            }
            if (null != result)
            {
                (result as BundleRes).AddRef();
            }
            countAction?.Invoke();
            while (count!= len) // 等待所有AB加载完毕
            {
                await Task.Yield();
            }
#endif
            return result;
        }

        public void UnloadRes(string path)
        {
#if !UNITY_EDITOR
            string[] deps = m_manifest.GetAllDependencies(path);
            int len = deps.Length;
            for (int i = 0; i < len; i++)
            {
                UnloadRes(deps[i]);
            }

            if (m_LoadingRes.TryGetValue(path, out LoadingResUnit loadingRes))//理论上不应该会走到这里(没有加载完不会实例化，不会实例化就不会卸载)
            {
                loadingRes.request.assetBundle.Unload(true);// 直接获取assetBundle会从异步转同步 --> 5.6以后
                m_LoadingRes.Remove(path);
            }

            if (LoadedRes.TryGetValue(path, out IRes result))
            {
                int refCount = (result as BundleRes).Unload();
                if (refCount <= 0)
                {
                    LoadedRes.Remove(path);
                }
            } 
#endif
        }

    }
}