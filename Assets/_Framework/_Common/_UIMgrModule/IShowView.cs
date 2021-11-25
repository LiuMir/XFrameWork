using System;
using System.Collections.Generic;
using UnityEngine;

public interface IShowView
{
    void OnShow(UIView view, bool isNew, BaseArgs args = null, Action finishAction = null);
    void OnHide(UIView view);
    UIView GetTopUIView();
}

// windowLayer 策略
public class UIWindowLayerContent : IShowView
{
    private readonly Stack<UIView> windowView;
    private GameObject uiRoot;

    public UIWindowLayerContent(GameObject uiRoot)
    {
        windowView = new Stack<UIView>();
        this.uiRoot = uiRoot;
    }

    public UIView GetTopUIView()
    {
        return windowView.Peek();
    }

    public void OnHide(UIView view)
    {
        if (windowView.Count > 0)
        {
            UIView popView = windowView.Pop();//理论上只能删除最顶上的UIview TODO 最好做一下检测 看看删除的是不是最顶上的uiview
            if (popView.UIName != view.UIName)
            {
                Debug.LogWarning($"[WindowLayer]当前想要隐藏的ui与实际隐藏的ui不一致，想要隐藏的UIName->{view.UIName}，实际隐藏的UIName->{popView.UIName}");
            }
            popView?.RealUIView?.Close();
        }
        else
        {
            Debug.LogError($"{view.UIName}-> 隐藏时 都没有任何winLayer存在");
        }
    }

    public void OnShow(UIView view, bool isNew, BaseArgs args = null, Action finishAction = null)
    {
        GameObject winRoot = new GameObject(view.UIName + "_root"); // TODO 暂时先这么写名字 每一个winLayer都有WindowRoot 
        winRoot.layer = LayerMask.NameToLayer("UI");
        winRoot.transform.SetParent(uiRoot.transform, false);
        AddRectComponent(winRoot);
        view.transform.SetParent(winRoot.transform, false); // 不先把实例父节点设置UI坐标系下 无法设置canvas相关参数

        UIConfig uiConfig = UIPathConfig.GetUIConfig(view.UIName);// 走到这里就不可能在UIPathConfig中找不到
        Canvas canvas = view.Canvas;
        BaseUI realView = view.RealUIView;
        if (isNew)
        {
            UIMgr.Instance.AddCanvas(view);
            canvas = view.gameObject.GetComponent<Canvas>();
            realView = Activator.CreateInstance(uiConfig.RealUIViewType) as BaseUI;
        }
        view.InitParm(realView, canvas);
        view.gameObject.SetActive(true);// 恢复生命周期函数调用
        canvas.overrideSorting = true; // 父节点必须显示否则 无法设置canvas相关参数
        canvas.sortingOrder = GlobalVariable.MinWindowLayerOrder + windowView.Count + 1;
        view.ParentObj = winRoot;
        realView.Show(args); // 因为Awake不会每次都被调用 得有自己的show、close
        finishAction?.Invoke();
        windowView.Push(view);
    }

    //添加 RectTransform
    private void AddRectComponent(GameObject gb)
    {
        RectTransform rect = gb.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}

// PopLayer 策略
public class UIPopLayerContent : IShowView
{
    private readonly Dictionary<int, Stack<UIView>> allPopLayers; // 用uiwindowLayerGameobject的instanceID作为key

    public UIPopLayerContent()
    {
        allPopLayers = new Dictionary<int, Stack<UIView>>();
    }

    public UIView GetTopUIView()
    {
        UIView topWindowLayer = UIMgr.Instance.GetLayerTopView(UIWindowLayer.WindowLayer);
        int instanceID = topWindowLayer.gameObject.GetInstanceID();
        if (!allPopLayers.TryGetValue(instanceID, out Stack<UIView> popLayers))
        {
            popLayers = new Stack<UIView>();
            allPopLayers.Add(instanceID, popLayers);
        }
        return popLayers.Peek();
    }

    public void OnHide(UIView view)
    {
        UIView topWindowLayer = UIMgr.Instance.GetLayerTopView(UIWindowLayer.WindowLayer);
        int instanceID = topWindowLayer.gameObject.GetInstanceID();
        if (!allPopLayers.TryGetValue(instanceID, out Stack<UIView> popLayers))
        {
            popLayers = new Stack<UIView>();
            allPopLayers.Add(instanceID, popLayers);
        }
        if (popLayers.Count > 0)
        {
            UIView popView = popLayers.Pop();//理论上只能删除最顶上的UIview TODO 最好做一下检测 看看删除的是不是最顶上的uiview
            if (popView.UIName != view.UIName)
            {
                Debug.LogWarning($"[PopLayer]当前想要隐藏的ui与实际隐藏的ui不一致，想要隐藏的UIName->{view.UIName}，实际隐藏的UIName->{popView.UIName}");
            }
            popView?.RealUIView?.Close();
        }
        else
        {
            Debug.LogError($"{view.UIName}-> 隐藏时 {topWindowLayer.UIName}下都没有任何PopLayer存在");
        }
    }

    public void OnShow(UIView view, bool isNew, BaseArgs args = null, Action finishAction = null)
    {
        UIConfig uiConfig = UIPathConfig.GetUIConfig(view.UIName);// 走到这里就不可能在UIPathConfig中找不到
        Canvas canvas = view.Canvas;
        BaseUI realView = view.RealUIView;
        UIView topWindowLayer = UIMgr.Instance.GetLayerTopView(UIWindowLayer.WindowLayer);
        view.gameObject.transform.SetParent(topWindowLayer.ParentObj.transform, false);
        if (isNew)
        {
            UIMgr.Instance.AddCanvas(view);
            canvas = view.gameObject.GetComponent<Canvas>();
            realView = Activator.CreateInstance(uiConfig.RealUIViewType) as BaseUI;
        }

        int instanceID = topWindowLayer.gameObject.GetInstanceID();
        if (!allPopLayers.TryGetValue(instanceID, out Stack<UIView> popLayers))
        {
            popLayers = new Stack<UIView>();
            allPopLayers.Add(instanceID, popLayers);
        }
        view.InitParm(realView, canvas);
        view.ParentObj = topWindowLayer.ParentObj;
        view.gameObject.SetActive(true);// 恢复生命周期函数调用
        canvas.overrideSorting = true;
        canvas.sortingOrder = topWindowLayer.Canvas?.sortingOrder ?? 0; // 打开的popLayer与最顶层的windowlayer相同层级
        realView.Show(args); // 因为Awake不会每次都被调用 得有自己的show、close
        finishAction?.Invoke();
        popLayers.Push(view);
    }

    // 关闭当前windowLayer下的所有的PopLayer
    public void OnCloseAllPopLayerInWinLayer(UIView windowUIView, Action<UIView> recycleAction = null)
    {
        int instanceID = windowUIView.gameObject.GetInstanceID();
        if (allPopLayers.TryGetValue(instanceID, out Stack<UIView> popLayers))
        {
            int count = popLayers.Count;
            for (int i = 0; i < count; i++)
            {
                UIView popView = popLayers.Peek();
                OnHide(popView);
                recycleAction?.Invoke(popView);
            }
        }
    }
}

// MsgLayer 策略
public class MsgLayerContent : IShowView
{
    private readonly Dictionary<int, UIView> allMsgLayers;
    private GameObject msgLayerRoot;

    public MsgLayerContent(GameObject msgLayerRoot)
    {
        this.msgLayerRoot = msgLayerRoot;
        allMsgLayers = new Dictionary<int, UIView>();
    }

    // onThing暂时不实现这个方法 -> 目前没想到需要使用这个方法的场景
    public UIView GetTopUIView()
    {
        throw new NotImplementedException();
    }

    public void OnHide(UIView view)
    {
        int instanceID = view.gameObject.GetInstanceID();
        if (!allMsgLayers.ContainsKey(instanceID))
        {
            Debug.LogError($"从未保存过的MsgLayer->{view.UIName}需要隐藏"); // 提示一下 引起注意
        }
        else
        {
            allMsgLayers.Remove(instanceID);
            view?.RealUIView?.Close();
        }
    }

    public void OnShow(UIView view, bool isNew, BaseArgs args = null, Action finishAction = null)
    {
        int instanceID = view.gameObject.GetInstanceID();
        if (allMsgLayers.ContainsKey(instanceID))
        {
            Debug.LogError($"出现显示相同instanceID 的MsgLayer->{view.UIName}"); // 重复了就不显示 暴露bug
        }
        else
        {
            UIConfig uiConfig = UIPathConfig.GetUIConfig(view.UIName);// 走到这里就不可能在UIPathConfig中找不到
            Canvas canvas = view.Canvas;
            BaseUI realView = view.RealUIView;
            view.gameObject.transform.SetParent(msgLayerRoot.transform, false);
            if (isNew)
            {
                UIMgr.Instance.AddCanvas(view);
                canvas = view.gameObject.GetComponent<Canvas>();
                realView = Activator.CreateInstance(uiConfig.RealUIViewType) as BaseUI;
            }
            view.ParentObj = msgLayerRoot;
            view.InitParm(realView, canvas);
            view.gameObject.SetActive(true);// 恢复生命周期函数调用
            canvas.overrideSorting = true;
            canvas.sortingOrder = GlobalVariable.MinMsgLayerOrder + allMsgLayers.Count + 1;
            allMsgLayers.Add(instanceID, view);
            realView.Show(args); // 因为Awake不会每次都被调用 得有自己的show、close
            finishAction?.Invoke();
        }
    }
}

// 挂载的小物件类（气泡……）不需要为每一个挂载一个canvas 最多采用分类的方式(一类一个canvas)
public class OnThingLayerContent : IShowView
{
    private readonly Dictionary<int, UIView> allOnThingLayers;
    private GameObject onThingRoot;

    public OnThingLayerContent(GameObject onThingRoot)
    {
        allOnThingLayers = new Dictionary<int, UIView>();
        this.onThingRoot = onThingRoot;
    }

    // onThing暂时不实现这个方法 -> 目前没想到需要使用这个方法的场景
    public UIView GetTopUIView()
    {
        throw new NotImplementedException();
    }

    public void OnHide(UIView view)
    {
        int instanceID = view.gameObject.GetInstanceID();
        if (!allOnThingLayers.ContainsKey(instanceID))
        {
            Debug.LogError($"从未保存过的onThingLayer->{view.UIName}需要隐藏"); // 提示一下 引起注意
        }
        else
        {
            allOnThingLayers.Remove(instanceID);
            view?.RealUIView?.Close();
        }
    }

    public void OnShow(UIView view, bool isNew, BaseArgs args = null, Action finishAction = null)
    {
        int instanceID = view.gameObject.GetInstanceID();
        if (allOnThingLayers.ContainsKey(instanceID))
        {
            Debug.LogError($"出现显示相同instanceID 的onThingLayer->{view.UIName}"); // 重复了就不显示 暴露bug
        }
        else
        {
            UIConfig uiConfig = UIPathConfig.GetUIConfig(view.UIName);// 走到这里就不可能在UIPathConfig中找不到
            BaseUI realView = view.RealUIView;
            view.gameObject.transform.SetParent(onThingRoot.transform, false);
            if (isNew)
            {
                realView = Activator.CreateInstance(uiConfig.RealUIViewType) as BaseUI;
            }
            view.ParentObj = onThingRoot;
            view.InitParm(realView, null);
            view.gameObject.SetActive(true);// 恢复生命周期函数调用
            allOnThingLayers.Add(instanceID, view);
            realView.Show(args); // 因为Awake不会每次都被调用 得有自己的show、close
            finishAction?.Invoke();
        }
    }
}

