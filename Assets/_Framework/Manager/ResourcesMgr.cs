using System;
using UnityEngine;
using System.Collections;

public class ResourcesMgr:Singleton<ResourcesMgr>
{
    public GameObject LoadGameObj(string path, Transform parent = null, Action<GameObject> action = null)
    {
        ABMgr.Instance.LoadABsync(path);
        AssetBundle assetBundle = ABMgr.Instance.TryGetBundle(path);
        assetBundle.LoadAsset<GameObject>(path);
        return null;
    }

    // 同步加载资源  
    // TODO 加载bundle传的是路径，加载资源传的是名字
    public void LoadResSync<T>(string path, Action<T> completed) where T : UnityEngine.Object
    {
        ABMgr.Instance.LoadABsync(path);
        AssetBundle assetBundle = ABMgr.Instance.TryGetBundle(path);
        completed?.Invoke(assetBundle.LoadAsset<T>(path));
    }

    // 异步加载资源
    // TODO 加载bundle传的是路径，加载资源传的是名字
    public void LoadResAsync<T>(string path, Action<T> completed) where T : UnityEngine.Object
    {
        CoroutineMgr.Instance.StartCoroutine("LoadResCoroutine", LoadResCoroutine(path, completed));
    }

    // 异步加载资源采用协程处理
    private IEnumerator LoadResCoroutine<T>(string path, Action<T> completed) where T : UnityEngine.Object
    {
        yield return ABMgr.Instance.LoadABAsync(path);
        AssetBundle assetBundle = ABMgr.Instance.TryGetBundle(path);
        if (AssertUtility.Instance.AssertNull(assetBundle))
        {
            AssetBundleRequest assetBundleRequest = assetBundle.LoadAssetAsync<T>(path);
            yield return assetBundleRequest;
            T asset = assetBundleRequest.asset as T;
            completed?.Invoke(asset);
        }
        else
        {
            Debug.LogError("异步加载失败，同步加载资源试一下！！！");
            LoadResSync(path, completed);
        }
    }

}
