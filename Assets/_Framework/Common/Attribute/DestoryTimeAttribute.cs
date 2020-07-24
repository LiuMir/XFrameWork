using System;

/// <summary>
/// 对象在回收池里的时间限制
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DestoryTimeAttribute: Attribute
{
    public DestoryTimeAttribute(float time)
    {
        Time = time;
    }

    public float Time { get; set; }
}

