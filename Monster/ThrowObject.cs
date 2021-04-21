using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThrowObject : MonoBehaviour, ICollisionHandler
{
    [SerializeField] float firingAngle = 45f;
    [SerializeField] float gravity = 9.8f;
    public Transform Projectile;
    [HideInInspector] public Transform Target;
    [HideInInspector] public Transform _myTransform;
    private void Start()
    {
        ICollisionHandler handler = (this as ICollisionHandler);
        //CCollision Checker = GetComponent<CCollision>();
        //if (Checker != null)
        //{
        //    Checker.CollisionHandler = handler;
        //}
    }

    void StartShot()
    {
        StartCoroutine(SimulateProjectile());
    }

    IEnumerator SimulateProjectile()
    {
        // 투사체를 던지는 물체의 위치로 이동 + 필요한 경우 오프셋을 추가
        Projectile.position = transform.position + new Vector3(0, 0.5f, 0);
        // 타겟과의 거리계산
        float target_Distance = Vector3.Distance(Projectile.position, Target.position);
        // 던지는 속도 계산 각도에 영향을 받음
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);
        // 속도의 X Y 요소 추출
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
        // 비행주기 계산
        float flightDuration = target_Distance / Vx;
        // 대상을 향하도록 발사체를 회전
        Projectile.rotation = Quaternion.LookRotation(Target.position - Projectile.position);
        float elapse_time = 0;
        while (elapse_time < flightDuration)
        {
            Projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
            elapse_time += Time.deltaTime;
            yield return null;
        }
        //SendMessage("Explosion");
        Invoke("RemoveMe", 1f);
    }

    void RemoveMe()
    {
        Destroy(this.gameObject);
    }

    public void HandleCollision(GameObject obj, Collision c)
    {
        if (c.contacts.Length != 0)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                StopAllCoroutines();
                //SendMessage("Explosion");
                Invoke("RemoveMe", 1f);
                CCntl hitscan = c.gameObject.GetComponent<CCntl>();
                hitscan.CCController("Stun", 2f);
            }
        }
    }
}
