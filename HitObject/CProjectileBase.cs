using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 모든 투사체형 공격 클래스
 * 
 */
public class CProjectileBase : MonoBehaviour
{
    // 공격에 해당하는 모든 경우들
    // 데미지, 스턴 등
    public enum AttackType
    {
        damage, stun
    }

    // 공격에 해당하는 경우들에 필요한 정보들 구조체
    // 데미지 - 데미지 계수 / 스턴 - 시간 / 슬로우 - 슬로우량, 시간 등
    // 필요에 따라 추가하기
    [System.Serializable]
    public struct AttackArgumentsList
    {
        public AttackType type;
        public int arg1;
        public int arg2;
    }

    public List<AttackArgumentsList> AttackArguments;

    public float range = 30f;
    public float speed = 10f;
    
    public int userAttackPower = 15;

    public Vector3 TargetPos;

    private float lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        lifeTime = range / speed;
        Invoke("Death", lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = transform.position + transform.rotation.eulerAngles.normalized * speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, TargetPos, speed * Time.deltaTime);
    }

    // 발사 위치 지정
    public void SetTargetPos(Vector3 targetPos)
    {
        // 주어진 지점까지만 가기
        //TargetPos = targetPos;
        // 주어진 지점을 기반으로 최대 거리까지 늘리기
        Vector3 targetDistanceVector = transform.position - targetPos;
        float scaleDistance = range / targetDistanceVector.magnitude;
        TargetPos = transform.position - Vector3.Scale(targetDistanceVector, Vector3.one * scaleDistance);
    }

    // 자동 소멸
    private void Death()
    {
        //Debug.Log("Projectile Death");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag == "Player" || gameObject.tag == "Allies")
        {
            if(other.CompareTag("Monster"))
            {
                // 이벤트 처리 : 네트워크한테 충돌 알림

                Debug.Log("Monster Hit");
                // 몬스터 타격 관련 함수
                var enemyPara = other.GetComponent<CharacterPara>();
                foreach (var attackArg in AttackArguments)
                {
                    switch(attackArg.type)
                    {
                        case AttackType.damage:
                            enemyPara.DamegedRegardDefence(userAttackPower * attackArg.arg1);
                            break;
                        case AttackType.stun:
                            Debug.Log("stun");
                            break;
                    }
                }
                Death();
            }
        }
        else if (gameObject.tag == "Monster")
        {

        }
    }

    //// 충돌 시 효과
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(gameObject.tag == "Player" || gameObject.tag == "Alies")
    //    {

    //    }
    //    else if(gameObject.tag == "Monster")
    //    {

    //    }
    //}
}
