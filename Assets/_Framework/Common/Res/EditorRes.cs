using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ResLoadMgr
{
    public class EditorRes : IRes
    {
        public event Action<UnityEngine.Object> LoadFinishEvent;

        // 传入的必须是在asset文件夹下的文件全路径加后缀
        public T Load<T>(string name) where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(name);
            if (null == asset)
            {
                Debug.LogError($"项目中没有名为:<- {name} ->的资源");
            }
            return asset;
        }

        // 传入的必须是在asset文件夹下的文件全路径加后缀
        public async void LoadAsync<T>(string name) where T : UnityEngine.Object
        {
            await Task.Delay(2); // 模拟异步加载
            T asset = AssetDatabase.LoadAssetAtPath<T>(name);
            if (null == asset)
            {
                Debug.LogError($"项目中没有名为:<- {name} ->的资源");
            }
            else
            {
                LoadFinishEvent?.Invoke(asset);
                LoadFinishEvent = null;
            }
        }
    }
}