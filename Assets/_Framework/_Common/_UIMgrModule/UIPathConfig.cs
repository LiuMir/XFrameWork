using System;
using System.Collections.Generic;
using UnityEngine;

public class UIConfig
{
    public string Path; //prefab路径
    public Type RealUIViewType;
}

public class UIPathConfig
{
    private static readonly Dictionary<string, UIConfig> UIConfigList = new Dictionary<string, UIConfig> {
        //{"MainUI", new UIConfig {Path = "_Res/_UIPrefabs/MainUI", RealUIViewType = typeof(MainView) } },  这是个例子
    };

    // 获取UI配置
    public static UIConfig GetUIConfig(string UIName)
    {
        if (!UIConfigList.TryGetValue(UIName, out UIConfig config))
        {
            Debug.LogError(UIName + " 没有在UIPathConfig类中的UIConfigList配置");
        }
        return config;
    }
}
