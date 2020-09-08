using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionHandler
{
    void HandleCollision(GameObject obj, Collision c);
}

public class CMonsterCollision : MonoBehaviour
{
    public ICollisionHandler CollisionHandler;

    public void OnCollisionEnter(Collision col)
    {
        CollisionHandler.HandleCollision(gameObject, col);
    }
}
