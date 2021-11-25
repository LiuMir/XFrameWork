using System;
using System.Diagnostics;

public class TimeUtility:Singleton<TimeUtility>
{
    public void CostTime(string flag, Action action)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        action?.Invoke();
        sw.Stop();
        UnityEngine.Debug.LogError($"{flag}一共耗时{sw.ElapsedMilliseconds}ms");
    }
}

