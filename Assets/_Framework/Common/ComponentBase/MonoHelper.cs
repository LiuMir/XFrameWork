using UnityEngine;

public static class MonoHelper
{
    public static T AddWorldObject<T>(this MonoBehaviour monoBehaviour, T com) where T:WorldObject
    {
        com.GameObj = monoBehaviour.gameObject;
        return com;
    }
}