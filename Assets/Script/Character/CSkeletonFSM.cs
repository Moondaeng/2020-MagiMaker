using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkeletonFSM : MonoBehaviour
{
    public enum EState
    {
        Idle,  //정지
        Chase,  //추적
        Attack,  //공격
        Dead,   //사망
    }
    
    // 현재 상태를 나타내는 변수
    public EState currentState = EState.Idle;

    CSkeletonAni myAni;

    Transform player;

    CSkeletonPara myPara;
    CPlayerPara playerPara;

    float chaseDistance = 5f; // 플레이어를 향해 몬스터가 추적을 시작할 거리
    float attackDistance = 2.5f; // 플레이어가 안쪽으로 들오오게 되면 공격을 시작
    float reChaseDistance = 3f; // 플레이어가 도망 갈 경우 얼마나 떨어져야 다시 추적

    float rotAnglePerSecond = 360f; // 초당 회전 각도
    float moveSpeed = 2.3f; // 몬스터의 이동 속도

    float attackDelay = 2f; // 몬스터 공격 딜레이
    float attackTimer = 0f; // 몬스터 공격 타이머 (미구현)

    public ParticleSystem hitEffect;
    public GameObject selection;

    GameObject myRespawn;
    public int spawnID { get; set; }
    Vector3 originPos;

    void Start()
    {
        // 스켈레톤의 Animation을 받아옴
        myAni = GetComponent<CSkeletonAni>();
        // 스켈레톤의 스텟을 받아옴
        myPara = GetComponent<CSkeletonPara>();

        myPara.deadEvent.AddListener(CallDeadEvent);

        // 첫 상태를 Idle
        ChangeState(EState.Idle, CSkeletonAni.IDLE);

        // player 태그를 가진 사람을 변수로 받음
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // player의 스텟을 받아옴
        playerPara = player.gameObject.GetComponent<CPlayerPara>();
        // 타격 시 이펙트 이거안해두면 Looping함.
        hitEffect.Stop();
        HideSelection();
    }

    public void HideSelection()
    {
        selection.SetActive(false);
    }

    public void ShowSelection()
    {
        selection.SetActive(true);
    }

    public void SetRespawn(GameObject respawn, int spawnID, Vector3 originPos)
    {
        myRespawn = respawn;
        this.spawnID = spawnID;
        this.originPos = originPos;
        Debug.Log(respawn + " " + spawnID + " " + originPos + " ");
    }

    void CallDeadEvent()
    {
        ChangeState(EState.Dead, CSkeletonAni.DEATH);
        player.gameObject.SendMessage(myPara.name + " is Dead");
        StartCoroutine("RemoveMe");
    }

    IEnumerable RemoveMe()
    {
        yield return new WaitForSeconds(1f);

        ChangeState(EState.Idle, CSkeletonAni.IDLE);

        myRespawn.GetComponent<CRespawn>().RemoveMonster(spawnID);
    }

    public void AttackCalculate()
    {
        playerPara.SetEnemyAttack(myPara.GetRandomAttack(playerPara.eType, myPara.eType));
    }

    // 타격 이펙트 함수
    public void ShowHitEffect()
    {
        hitEffect.Play();
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case EState.Idle:
                IdleState();
                break;
            case EState.Chase:
                ChaseState();
                break;
            case EState.Attack:
                AttackState();
                break;
            case EState.Dead:
                DeadState();
                break;
        }
    }

    public void ChangeState(EState newState, string aniName)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
        myAni.ChangeAni(aniName);
    }
    void IdleState()
    {
        if (GetDistanceFromPlayer() < chaseDistance)
        {
            ChangeState(EState.Chase, CSkeletonAni.WALK);
        }
    }

    void ChaseState()
    {
        myAni.ChangeAni(CSkeletonAni.WALK);
        //몬스터가 공격 가능 거리 안으로 들어가면 공격 상태
        if (GetDistanceFromPlayer() < attackDistance)
        {
            ChangeState(EState.Attack, CSkeletonAni.ATTACK);
        }
        else
        {
            TurnToDestination();
            MoveToDestination();
        }
    }

    // 거리재는 함수로 재추격 거리보다 길 경우 뛰어가고
    // 아니면 공격하는데, 이게 쿨타임이 있음 2초 설정
    // 추후 이 함수 안에서, 패턴을 나눠야 할 듯 하다.
    void AttackState()
    {
        if (player.GetComponent<CPlayerFSM>().currentState == CPlayerFSM.EState.Dead)
        {
            ChangeState(EState.Idle, CSkeletonAni.IDLE);
        }
        if (GetDistanceFromPlayer() > reChaseDistance)
        {
            attackTimer = 0f;
            ChangeState(EState.Chase, CSkeletonAni.RUN);
        }
        else
        {
            if (attackTimer > attackDelay)
            {
                transform.LookAt(player.position);
                myAni.ChangeAni(CSkeletonAni.ATTACK);

                attackTimer = 0f;
            }

            attackTimer += Time.deltaTime;
        }
    }
    
    // 죽으면 선택안되게끔 하려고함
    void DeadState()
    {
        GetComponent<BoxCollider>().enabled = false;
    }
    
    // 빙글빙글
    void TurnToDestination()
    {
        Quaternion lookRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotAnglePerSecond);
    }

    // 종착 위치 계산
    // 이걸 서버로 보내면 될듯?
    void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    //플레이어와 거리을 재는 함수
    float GetDistanceFromPlayer()
    {
        return Vector3.Distance(transform.position, player.position);   
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }
}