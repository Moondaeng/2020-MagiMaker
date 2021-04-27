using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventBulletController : MonoBehaviour
{ 
    [Tooltip("y 로테이션 중간값 설정")]
    public int middleRotationY;
    [Tooltip("x 로테이션 최대값 설정")]
    public int maxRotationX;
    [Tooltip("x 로테이션 최소값 설정")]
    public int minRotationX;
    [Tooltip("총알 속도")]
    public float speed;
    [Tooltip("총알 데미지")]
    public int bulletDamage;
    // Start is called before the first frame update
    void Awake()
    {
        if (middleRotationY == 0)
            middleRotationY = 0;
        if (maxRotationX == 0)
            maxRotationX = 100;
        if (minRotationX == 0)
            minRotationX = 70;
        if (speed == 0)
            speed = 6;
        if (bulletDamage == 0)
            bulletDamage = 100;

        transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(minRotationX, maxRotationX),
            UnityEngine.Random.Range(middleRotationY - 90, middleRotationY + 90), transform.rotation.z));
    }
}
