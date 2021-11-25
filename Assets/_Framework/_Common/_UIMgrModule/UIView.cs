using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseArgs { }

public interface IMono
{
    void Awake();
    void OnEnable();
    void OnDisable();
}

public class BaseUI : IMono
{
    public UIView viewMono;

    public virtual void Awake(){}

    public virtual void Show(BaseArgs Args = null) { }
    public virtual void Close() { }

    public void InitParam(UIView mono)
    {
        viewMono = mono;
    }

    public virtual void OnEnable(){}

    public virtual void OnDisable(){}
}

public class UIView:MonoBehaviour
{
    [HideInInspector]
    public string UIName = ""; // 与挂载的transform 名字一直 且不可修改
    [Header("UI显示的层级")]
    public UIWindowLayer WindowLayer = UIWindowLayer.WindowLayer;
    
    [HideInInspector]
    public Canvas Canvas; // 有些Layer不需要canvas
    [HideInInspector]
    public GameObject ParentObj;
    [HideInInspector]
    public BaseUI RealUIView;
    
    // 在awake前先把相关的参数初始化好
    public void InitParm(BaseUI uiView, Canvas canvas = null)
    {
        this.Canvas = canvas;
        this.RealUIView = uiView;
    }

    private void Awake()
    {
        RealUIView.InitParam(this);
        RealUIView.Awake();
    }

    private void OnEnable()
    {
        RealUIView.OnEnable();
    }

    private void OnDisable()
    {
        RealUIView.OnDisable();
    }

    private void OnDestroy()
    {
        RealUIView = null;
    }

}