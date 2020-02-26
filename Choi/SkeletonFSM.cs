using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonFSM : MonoBehaviour
{
    public enum State
    {
        Idle,  //정지
        Chase,  //추적
        Attack,  //공격
        Dead,   //사망
    }
    
    // 현재 상태를 나타내는 변수
    public State currentState = State.Idle;

    SkeletonAni myAni;

    Transform player;

    SkeletonPara myPara;
    PlayerPara playerPara;

    float chaseDistance = 5f; // 플레이어를 향해 몬스터가 추적을 시작할 거리
    float attackDistance = 2.5f; // 플레이어가 안쪽으로 들오오게 되면 공격을 시작
    float reChaseDistance = 3f; // 플레이어가 도망 갈 경우 얼마나 떨어져야 다시 추적

    float rotAnglePerSecond = 360f; // 초당 회전 각도
    float moveSpeed = 2.3f; // 몬스터의 이동 속도

    float attackDelay = 2f; // 몬스터 공격 딜레이
    float attackTimer = 0f; // 몬스터 공격 타이머 (미구현)

    public ParticleSystem hitEffect;

    void Start()
    {
        // 스켈레톤의 Animation을 받아옴
        myAni = GetComponent<SkeletonAni>();
        // 스켈레톤의 스텟을 받아옴
        myPara = GetComponent<SkeletonPara>();

        myPara.deadEvent.AddListener(CallDeadEvent);

        // 첫 상태를 Idle
        ChangeState(State.Idle, SkeletonAni.IDLE);

        // player 태그를 가진 사람을 변수로 받음
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // player의 스텟을 받아옴
        playerPara = player.gameObject.GetComponent<PlayerPara>();
        // 타격 시 이펙트 이거안해두면 Looping함.
        hitEffect.Stop();
    }

    void CallDeadEvent()
    {
        ChangeState(State.Dead, SkeletonAni.DEATH);
        player.gameObject.SendMessage(myPara.name + " is Dead");
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
            case State.Idle:
                IdleState();
                break;
            case State.Chase:
                ChaseState();
                break;
            case State.Attack:
                AttackState();
                break;
            case State.Dead:
                DeadState();
                break;
        }
    }

    public void ChangeState(State newState, string aniName)
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
            ChangeState(State.Chase, SkeletonAni.WALK);
        }
    }

    void ChaseState()
    {
        myAni.ChangeAni(SkeletonAni.WALK);
        //몬스터가 공격 가능 거리 안으로 들어가면 공격 상태
        if (GetDistanceFromPlayer() < attackDistance)
        {
            ChangeState(State.Attack, SkeletonAni.ATTACK);
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
        if (GetDistanceFromPlayer() > reChaseDistance)
        {
            attackTimer = 0f;
            ChangeState(State.Chase, SkeletonAni.RUN);
        }
        else
        {
            if (attackTimer > attackDelay)
            {
                transform.LookAt(player.position);
                myAni.ChangeAni(SkeletonAni.ATTACK);

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
        float distance = Vector3.Distance(transform.position, player.position);
        return distance;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }
}