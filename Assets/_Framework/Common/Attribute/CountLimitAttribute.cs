using System;

/// <summary>
/// 类对象回收个数限制
/// PS:若是需要约束在指定类型上，可以把该属性放在指定类型里面
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class CountLimitAttribute: Attribute
{
    public CountLimitAttribute(int count)
    {
        Count = count;
    }

    public int Count { get; set; }
}
