using UnityEngine;

public interface ICollisionHandler
{
    void HandleCollision(GameObject obj, Collision c);
}

public class CCollision : MonoBehaviour
{
    public ICollisionHandler CollisionHandler;

    public void OnCollisionEnter(Collision col)
    {
        CollisionHandler?.HandleCollision(gameObject, col);
    }
}