using System.Collections.Generic;
using UnityEngine;

public  class WorldObject 
{
    private List<WorldComponent> m_GameComponets;

    public GameObject GameObj { get; set; }

    public WorldObject()
    {
        m_GameComponets = new List<WorldComponent>();
    }

    public T AddComponent<T>(T com) where T : WorldComponent
    {
        m_GameComponets.Add(com);
        com.Entity = this;
        return com;
    }

   public T GetComponent<T>() where T : class
    {
        return m_GameComponets.Find((com)=> {
            return com is T;
        }) as T;
    }

    public void RemoveComponent<T>() where T : WorldComponent
    {
        m_GameComponets.RemoveAll((com) =>
        {
            return com is T;
        });
    }

}