using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 滑动切页功能(目前暂时不支持与无限循环列表结合使用) http://www.ngui.cc/51cto/show-93219.html
/// </summary>

public class PageScroller : MonoBehaviour, IEndDragHandler, IBeginDragHandler,IDragHandler
{
    #region 声明对象
    public enum ScrolType
    {
        //滑动类型（分为两种 横纵两种）
        HorizontalType,
        VerticalType
    }

    ScrollRect m_rect;
    private int m_pageCount;//选项个数
    
    private float[] m_pages;//位置
    private float m_space;

    [Header("滚动参数")]
    public float MoveSpeed = 0.3f;//移动速度
    public float AnimationTime = 0;//动画过渡时间长度
    private int m_currentindex = 0;//当前所处页数
    private float m_timer = 0;//计时器
    [Range(0, 1f)]
    public float PercentOfDis = 0; // 滑动距离百分比
    private bool m_isMoving = false;

    private bool m_isDraging = false;//是否在拖动中

    [Header("程序调试用")]
    public float StartMovePos;//开始移动的位置
    [Header("自动滚动参数")]
    public bool IsAutoScrol = true;//是否自动滚动
    public float AutoTime = 2f;//自动滚动时间间隔
    public float AutoTimer = 0;//自动计时器

    [Header("滑动类型")]
    public ScrolType scroltype = ScrolType.HorizontalType;

    public Action<int> SnappedAction;
    #endregion

    public void Init(int pageCount = 0)
    {
        m_rect = GetComponent<ScrollRect>();
        if (m_rect == null)
        {
            Debug.LogError("不存在组件ScrollRect");

        }
        m_pageCount = pageCount;//子节点数量
        m_pages = new float[m_pageCount];//初始化
        m_space = 1.0f / (m_pageCount - 1);

        if (m_pages.Length == 1)
        {
            Debug.LogError("只有一页 不用滚动");
        }
        for (int i = 0; i < m_pages.Length; i++)
        {
            switch (scroltype)
            {
                case ScrolType.HorizontalType:
                    m_pages[i] = i * m_space;//给位置的数组赋值
                    break;
                case ScrolType.VerticalType:
                    m_pages[i] = 1 - i * m_space;//给位置的数组赋值
                    break;
            }
        }
    }

    // 跳转至某一页
    public void ScrollToPage(int page)
    {
        if (page < 0 || page > m_pageCount - 1 || m_isMoving) // 超出页签范围
        {
            return;
        }
        m_isMoving = true;
        this.m_currentindex = page;
        m_timer = 0;


        switch (scroltype)
        {
            case ScrolType.HorizontalType:
                StartMovePos = m_rect.horizontalNormalizedPosition;
                break;
            case ScrolType.VerticalType:
                StartMovePos = m_rect.verticalNormalizedPosition;
                break;
        }
    }

    // 直接跳转到指定页（不带动画）
    public void JumpToNoAni(int pageIndex)
    {
        switch (scroltype)
        {
            case ScrolType.HorizontalType:
                m_rect.horizontalNormalizedPosition = m_pages[pageIndex];
                break;
            case ScrolType.VerticalType:
                m_rect.verticalNormalizedPosition = m_pages[pageIndex];
                break;
        }
        m_currentindex = pageIndex;
        SnappedAction?.Invoke(pageIndex);
    }

    //跳转至下一页
    public void ScrollToNextPage()
    {
        ScrollToPage(m_currentindex + 1);
    }

    // 跳转至上一页
    public void ScrollToPreviousPage()
    {
        ScrollToPage(m_currentindex - 1);
    }

    // 获取当前所在页索引
    public int GetCurIndex()
    {
        return m_currentindex;
    }

    //取消拖动的接口
    public void OnEndDrag(PointerEventData eventData)
    {
        int pageIndex = m_currentindex;
        float pos = m_pages[m_currentindex];
        switch (scroltype)
        {
            case ScrolType.HorizontalType:
                if (Mathf.Abs(pos - m_rect.horizontalNormalizedPosition) > m_space*PercentOfDis)
                {
                    pageIndex = m_currentindex - 1* GetSymbolVal(pos, m_rect.horizontalNormalizedPosition);
                }
                break;
            case ScrolType.VerticalType:
                if (Mathf.Abs(pos - m_rect.verticalNormalizedPosition) > m_space * PercentOfDis)
                {
                    pageIndex = m_currentindex - 1 * GetSymbolVal(pos, m_rect.horizontalNormalizedPosition);
                }
                break;
        }

        ScrollToPage(pageIndex);
        m_isDraging = false;
        AutoTimer = 0;
    }

    //开始拖动的接口
    public void OnBeginDrag(PointerEventData eventData)
    {
        m_isDraging = true;
    }

    // 拖拽中
    public void OnDrag(PointerEventData eventData)
    {
        float pos = m_pages[m_currentindex];
        switch (scroltype)
        {
            case ScrolType.HorizontalType:
                if (Mathf.Abs(pos - m_rect.horizontalNormalizedPosition) > m_space)
                {
                    m_rect.horizontalNormalizedPosition = pos - GetSymbolVal(pos, m_rect.horizontalNormalizedPosition) * m_space * 0.95f; //  不要全部到位，留一点点空白出来
                }
                break;
            case ScrolType.VerticalType:
                if (Mathf.Abs(pos - m_rect.verticalNormalizedPosition) > m_space)
                {
                    m_rect.verticalNormalizedPosition = pos - GetSymbolVal(pos, m_rect.horizontalNormalizedPosition) * m_space * 0.95f;//  不要全部到位，留一点点空白出来
                }
                break;
        }
    }

    private void Update()
    {
        ListenerMove();
        //listenerAutoScroll();//关闭自动滑动
    }

    private void ListenerMove()//监听滑动
    {
        if (m_isMoving)
        {
            m_timer += Time.deltaTime * MoveSpeed;

            //灵魂就在这里
            switch (scroltype)
            {
                case ScrolType.HorizontalType:
                    m_rect.horizontalNormalizedPosition = Mathf.Lerp(StartMovePos, m_pages[m_currentindex], m_timer);
                    break;
                case ScrolType.VerticalType:
                    m_rect.verticalNormalizedPosition = Mathf.Lerp(StartMovePos, m_pages[m_currentindex], m_timer);
                    break;
            }

            if (m_timer >= AnimationTime)
            {
                SnappedAction?.Invoke(m_currentindex);
                m_isMoving = false;
            }
        }
    }

    private void ListenerAutoScroll()//自动滑动
    {
        if (m_isDraging)
        {
            return;
        }
        if (IsAutoScrol)
        {
            AutoTimer += Time.deltaTime;
            if (AutoTimer > AutoTime)
            {
                AutoTimer = 0;
                m_currentindex++;
                m_currentindex %= m_pageCount;//取余形成循环
                ScrollToPage(m_currentindex);
            }
        }
    }

    private int GetSymbolVal(float val_1, float val_2)
    {
        float val = val_1 - val_2;
        return val == 0 ? 0 : (int)(val / Mathf.Abs(val));
    }
}