using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class AwaiterExtension
{
    // 扩展AsyncOperation为可等待方式
    public static AsyncOperationAwaiter GetAwaiter(this AsyncOperation op)
    {
        return new AsyncOperationAwaiter(op);
    }
}

public struct AsyncOperationAwaiter : INotifyCompletion
{
    private readonly AsyncOperation m_operation;

    public AsyncOperationAwaiter(AsyncOperation operation)
    {
        this.m_operation = operation;
    }

    public bool IsCompleted { get { return m_operation.isDone; } }

    public void GetResult() { }

    public void OnCompleted(Action continuation)
    {
        m_operation.completed += (operation)=> { continuation?.Invoke(); };
    }
}
