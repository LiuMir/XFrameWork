using UnityEngine;

public class WorldComponent
{
    public GameObject MonoGameObject { get { return Entity.GameObj; } }
    public WorldObject Entity { get; set; }
}