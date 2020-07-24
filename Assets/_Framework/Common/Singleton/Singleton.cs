using System;

/// <summary>
/// 泛型单例，使用懒加载类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> where T: class, new()
{
    private static readonly Lazy<T> lazy = new Lazy<T>(() => new T());
    protected Singleton() { }
    public static T Instance
    {
        get { return lazy.Value; }
    }
 }

