using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ResourcesMgr:Singleton<ResourcesMgr>
{
    public GameObject LoadGameObj(string path, Transform parent = null, Action<GameObject> action = null)
    {
        GameObject obj = Resources.Load<GameObject>(path);
        GameObject inst = UnityEngine.Object.Instantiate(obj, parent);
        action?.Invoke(inst);
        return inst;
    }
}
