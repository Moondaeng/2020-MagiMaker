using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHitScanBase : MonoBehaviour
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
        public float arg1;
        public float arg2;
    }

    public List<AttackArgumentsList> AttackArguments;

    private float size = 30f;
    private float speed = 10f;
    
    public float userAttackPower = 15f;

    public Vector3 TargetPos;

    private float lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Death", 300f);
    }

    private void Update()
    {
        if (this.gameObject.name == "RockNeedle")
        {
            if (this.gameObject.transform.position.y < 5f)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                                     transform.position + Vector3.up, 50f * Time.deltaTime);
            }
        }
    }

    // 자동 소멸
    private void Death()
    {
        //Debug.Log("Projectile Death");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag == "Monster")
        {
            if(other.CompareTag("Player"))
            {
                // 이벤트 처리 : 네트워크한테 충돌 알림

                Debug.Log("Player Hit");
                // 몬스터 타격 관련 함수
                var playerPara = other.GetComponent<CharacterPara>();
                foreach (var attackArg in AttackArguments)
                {
                    switch(attackArg.type)
                    {
                        case AttackType.damage:
                            playerPara.SetEnemyAttack(userAttackPower * attackArg.arg1);
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
}
