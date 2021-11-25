using UnityEngine;

/// <summary>
/// Mono单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingle<T> : MonoBehaviour where T:MonoBehaviour
{
       
    private static T _instance;
    private static bool _applicationIsQuitting = false;

    protected MonoSingle() { }

    public static T Instance
    {

        get
        {
            if (_instance == null && !_applicationIsQuitting)
            {

                _instance = Create();
            }
            return _instance;
        }
    }

    private static T Create()
    {
        var go = new GameObject(typeof(T).Name, typeof(T));
        DontDestroyOnLoad(go);
        return go.GetComponent<T>();
    }

    protected virtual void Awake()
    {
        _instance = this as T;
        this.gameObject.AddComponent<StaticSceneObj>();
    }

    protected virtual void OnApplicationQuit()
    {
        if (_instance == null) return;
        Destroy(_instance.gameObject);
        _instance = null;
    }

    protected virtual void OnDestory()
    {
        _applicationIsQuitting = true;
    }
}
