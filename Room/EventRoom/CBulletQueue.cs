using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBulletQueue : MonoBehaviour
{
    private Queue<GameObject> _bullets;
    public static CBulletQueue instance = null;
    // Start is called before the first frame update
    void Awake()
    {
        _bullets = new Queue<GameObject>();
        if (instance == null)
            instance = this;
    }

    public void BulletEnqueue(GameObject bullet)
    {
        if (bullet == null)
        {
            Debug.Log("bullet is null");
            return;
        }
        bullet = Object.Instantiate(bullet, bullet.transform.position, bullet.transform.rotation);
        _bullets.Enqueue(bullet);
        bullet.transform.SetParent(transform.parent.Find("Bullet"));
        bullet.SetActive(false);
    }

    public GameObject BulletDequeue()
    {
        if(_bullets.Count == 0)
        {
            return null;
        }
        GameObject bullet = _bullets.Dequeue();

        bullet.SetActive(true);

        return bullet;
    }
}
