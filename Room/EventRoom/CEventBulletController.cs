using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventBulletController : MonoBehaviour
{
    [Tooltip("y 로테이션 최대값 설정")]
    public int maxRotationY;
    [Tooltip("x 로테이션 최대값 설정")]
    public int maxRotationX;
    [Tooltip("x 로테이션 최소값 설정")]
    public int minRotationX;
    // Start is called before the first frame update
    void Start()
    {
        if (maxRotationY == 0)
            maxRotationY = 89;
        if (maxRotationX == 0)
            maxRotationX = 60;
        if (minRotationX == 0)
            minRotationX = -30;

        transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(minRotationX, maxRotationX),
            UnityEngine.Random.Range(180-maxRotationY, 180+maxRotationY), transform.rotation.z));
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "WALL" || other.gameObject.tag == "TILE")
    //    {
    //        Destroy(gameObject);
    //    }

    //    if (other.tag == "Player")
    //    {
    //        CController.instance.player.GetComponent<CPlayerPara>().DamagedDisregardDefence(bulletDamage);
    //        Debug.Log("Bullet");
    //        Destroy(gameObject);
    //    }
    //}
}
