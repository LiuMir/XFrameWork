using System;
using System.Collections.Generic;
using UnityEditor; // AssetDatabase
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UIMgr:Singleton<UIMgr>
{
    private readonly Dictionary<int, IShowView> createUIMethods = new Dictionary<int, IShowView>(); // 所有创建不同层的ui策略
    private readonly Dictionary<string, List<UIView>> UIViewPool = new Dictionary<string, List<UIView>>(); // UI池
    private GameObject poolRootTransform;
    private int count = 1;

    public void SetPoolRoot(GameObject poolRoot)
    {
        poolRootTransform = poolRoot;
    }

    // 添加创建不同层的ui策略
    public void AddCreateUIMethod(UIWindowLayer windowLayer, IShowView showView)
    {
        int layerIndex = (int)windowLayer;
        if (!createUIMethods.ContainsKey(layerIndex))
        {
            createUIMethods.Add(layerIndex, showView);
        }
    }

    // 打开UI 暂时还未实现打开新的winLayer隐藏上一个Winlayer（问题：overdraw严重）
    public void OnShowUI(string uiName, BaseArgs args = null, Action finishAction = null)
    {
        UIView view =  GetUIView(uiName, out bool isNew);
        if (null == view)
        {
            Debug.LogError($"显示界面->{uiName}失败");
            return;
        }
        view.gameObject.name = uiName + "_" + count;
        SetPopLayerViewVisiableInWinLayer(view, false);
        GetCreateUIMethod(view.WindowLayer)?.OnShow(view, isNew, args, finishAction);
        count++;
    }

    // 关闭UI
    public void OnCloseUI(UIView view)
    {
        ClosePopLayerViewInWinLayer(view);
        GetCreateUIMethod(view.WindowLayer)?.OnHide(view);
        SetPopLayerViewVisiableInWinLayer(view, true);
        RecycleUIView(view);
    }

    // 获取某一层最顶层的uiview
    public UIView GetLayerTopView(UIWindowLayer windowLayer)
    {
        int layerIndex = (int)windowLayer;
        createUIMethods.TryGetValue(layerIndex, out IShowView showView);
        return showView.GetTopUIView();
    }

    // 给UIView挂上Canvas
    public void AddCanvas(UIView view)
    {
        GraphicRaycaster graphicRaycaster = view.gameObject.AddComponent<GraphicRaycaster>(); // 会自动检测Canvas若无则自动添加
        graphicRaycaster.ignoreReversedGraphics = false; // 不管图形哪一面都接受点击
    }

    //获取创建ui创建策略
    private IShowView GetCreateUIMethod(UIWindowLayer windowLayer)
    {
        int layerIndex = (int)windowLayer;
        createUIMethods.TryGetValue(layerIndex, out IShowView showView);
        return showView;
    }

    // 获取一个UIView（池子没有就创建一个新的实例）
    private UIView GetUIView(string uiName, out bool isNew)
    {
        isNew = false;
        UIView view = null;
        UIConfig uiConfig = UIPathConfig.GetUIConfig(uiName);
        if (null == uiConfig)
        {
            return view;
        }

        if (!UIViewPool.TryGetValue(uiName, out List<UIView> curUIViewPool))
        {
            curUIViewPool = new List<UIView>();
            UIViewPool.Add(uiName, curUIViewPool);
        }
        int poolCount = curUIViewPool.Count;
        if (poolCount > 0) //池子中有
        {
            int index = poolCount - 1;
            view = curUIViewPool[index]; // 取最后一个并删除
            SetUIViewVisiable(view, true);// 为了模拟OnEnable 、OnDisable
            curUIViewPool.RemoveAt(index);
        }
        else
        {
            GameObject UIPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GetUIPath(uiConfig.Path));
            UIPrefab.SetActive(false); // 先禁用是为了阻止实例化后 生命周期函数调用
            GameObject inst = Object.Instantiate(UIPrefab);
            inst.layer = LayerMask.NameToLayer("UI");
            view = inst.GetComponent<UIView>();
            isNew = true;
        }
        return view;
    }

    //回收一个UIView
    private void RecycleUIView(UIView view)
    {
        if (!UIViewPool.TryGetValue(view.UIName, out List<UIView> curUIViewPool))
        {
            curUIViewPool = new List<UIView>();
            UIViewPool.Add(view.UIName, curUIViewPool);
        }
        view.transform.SetParent(poolRootTransform.transform, false);
        if (view.WindowLayer == UIWindowLayer.WindowLayer)
        {
            Object.Destroy(view.ParentObj);
        }
        SetUIViewVisiable(view, false);// 为了模拟OnEnable 、OnDisable
        curUIViewPool.Add(view);
    }

    // 设置uiView的显隐
    private void SetUIViewVisiable(UIView view, bool isVisiable)
    {
        view.enabled = isVisiable;
    }

    // 当关闭的windowLayer时 关闭当前windowLayer下的所有的PopLayer
    private void ClosePopLayerViewInWinLayer(UIView view)
    {
        if (view.WindowLayer == UIWindowLayer.WindowLayer)
        {
            IShowView showView = GetCreateUIMethod(UIWindowLayer.PopLayer);
            (showView as UIPopLayerContent)?.OnCloseAllPopLayerInWinLayer(view, (recycleView)=> { RecycleUIView(recycleView); });
        }
    }

    // 获取最顶层winLayer UIView
    private UIView GetTopWinLayerView()
    {
        return GetCreateUIMethod(UIWindowLayer.WindowLayer).GetTopUIView();
    }

    // 当打开、关闭的windowLayer时 显示、隐藏当前windowLayer下的所有的PopLayer
    private void SetPopLayerViewVisiableInWinLayer(UIView view, bool isShow)
    {
        if (view.WindowLayer == UIWindowLayer.WindowLayer)
        {
            IShowView showView = GetCreateUIMethod(UIWindowLayer.PopLayer);
            (showView as UIPopLayerContent)?.SetLayersVisiableUnderTopWinLayer(GetTopWinLayerView(), isShow);
        }
    }

    // 获取ui路径
    private string GetUIPath(string UIName)
    {
        return "Assets/" + UIName+".prefab";
    }

}
