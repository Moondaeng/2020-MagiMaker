using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CThrowObject : MonoBehaviour
{
    [SerializeField] float _firingAngle = 45f;
    [SerializeField] float _gravity = 9.8f;
    [HideInInspector] public Transform _target;
    [HideInInspector] public Transform _holding;
    bool _hold = true;

    private void Update()
    {
        if (_hold)
            transform.position = _holding.position;
    }

    void StartShot()
    {
        _hold = false;
        Debug.Log("Shooting");
        StartCoroutine(SimulateProjectile());
    }

    IEnumerator SimulateProjectile()
    {
        // 투사체를 던지는 물체의 위치로 이동 + 필요한 경우 오프셋을 추가
        transform.position = transform.position + new Vector3(0, 0.5f, 0);
        // 타겟과의 거리계산
        float target_Distance = Vector3.Distance(transform.position, _target.position);
        // 던지는 속도 계산 각도에 영향을 받음
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * _firingAngle * Mathf.Deg2Rad) / _gravity);
        // 속도의 X Y 요소 추출
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(_firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(_firingAngle * Mathf.Deg2Rad);
        // 비행주기 계산
        float flightDuration = target_Distance / Vx;
        // 대상을 향하도록 발사체를 회전
        transform.rotation = Quaternion.LookRotation(_target.position - transform.position);
        float elapse_time = 0;
        while (elapse_time < flightDuration)
        {
            transform.Translate(0, (Vy - (_gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
            elapse_time += Time.deltaTime;
            yield return null;
        }
        SendMessage("Explosion");
        RemoveMe();
    }

    void RemoveMe()
    {
        StartCoroutine(Dest(1f));
    }

    IEnumerator Dest(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
}
