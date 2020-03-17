using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerFSM : MonoBehaviour
{
    public enum EState
    {
        Idle,
        Move,
        Attack,
        AttackWait,
        Warp,
        Dead
    }
    //idle 상태를 기본 상태로 지정
    public EState currentState = EState.Idle;

    //마우스 클릭 지점, 플레이어가 이동할 목적지의 좌표를 저장할 예정
    Vector3 curTargetPos;

    GameObject curEnemy;

    public float rotAnglePerSecond = 360f; //1초에 플레이어의 방향을 360도 회전
    public float moveSpeed = 2f; //초당 2미터의 속도로 이동
    float attackDelay = 2f; // 공격을 한번 하고 다시 공격할 때까지의 지연
    float attackTimer = 0f; //공격을 하고 난 뒤에 경과되는 시간을 계산하기 위한 변수
    float attackDistance = 4.5f; // 공격 거리 (적과의 거리)
//    float chaseDistance = 2.5f; // 전투 중 적이 도망가면 다시 추적을 시작하기 위한 거리

    CPlayerAni myAni;
    CPlayerPara myPara;
    CSkeletonPara curEnemyPara;
//    GoblinPara curEnemyPara2;

    void Start()
    {
        myAni = GetComponent<CPlayerAni>();
        myPara = GetComponent<CPlayerPara>();
        myPara.InitPara();
        myPara.deadEvent.AddListener(ChangeToPlayerDead);

        ChangeState(EState.Idle, CPlayerAni.ANI_IDLE);
    }

    public void ChangeToPlayerDead()
    {
        print("player was dead");
        ChangeState(EState.Dead, CPlayerAni.ANI_DEATH);
    }

    public void CurrentEnemyDead()
    {
        ChangeState(EState.Idle, CPlayerAni.ANI_IDLE);

        curEnemy = null;
    }

    public void AttackCalculate()
    {
        if (curEnemy == null)
        {
            return;
        }
        
        curEnemy.GetComponent<CSkeletonFSM>().ShowHitEffect();

        float attackPower = myPara.GetRandomAttack(curEnemyPara.eType, myPara.eType);
        curEnemyPara.SetEnemyAttack(attackPower);
    }

    // 적을 공격하기 위한 함수 
    public void AttackEnemy(GameObject Enemy)
    {
        Debug.Log(Enemy);
        if (curEnemy != null && curEnemy == Enemy)
        {
            return;
        }
        // 적(몬스터)의 파라미터를 변수에 저장
        curEnemyPara = Enemy.GetComponent<CSkeletonPara>();

        if (curEnemyPara.isDead == false)
        {
            curEnemy = Enemy;
            curTargetPos = curEnemy.transform.position;
            CManager.instance.ChangeCurrentTarget(curEnemy);
            ChangeState(EState.Move, CPlayerAni.ANI_RUN);
        }
        else
        {
            curEnemyPara = null;
        }
    }

    void ChangeState(EState newState, int aniNumber)
    {
        if (currentState == newState)
        {
            return;
        }
        myAni.ChangeAni(aniNumber);
        currentState = newState;
    }

    //캐릭터의 상태가 바뀌면 어떤 일이 일어날지 를 미리 정의
    void UpdateState()
    {
        switch (currentState)
        {
            case EState.Idle:
                IdleState();
                myPara.isAnotherAction = false;
                break;
            case EState.Move:
                MoveState();
                myPara.isAnotherAction = true;
                break;
            case EState.Attack:
                AttackState();
                myPara.isAnotherAction = true;
                break;
            case EState.AttackWait:
                AttackWaitState();
                myPara.isAnotherAction = true;
                break;
            case EState.Warp:
                WarpState();
                myPara.isAnotherAction = true;
                break;
            case EState.Dead:
                DeadState();
                myPara.isAnotherAction = true;
                break;
            default:
                break;
        }
    }

    public void IdleState()
    {
        ChangeState(EState.Idle, CPlayerAni.ANI_IDLE);
    }

    void MoveState()
    {
        TurnToDestination();
        MoveToDestination();
    }

    void AttackState()
    {
        attackTimer = 0f;

        //transform.LookAt(목표지점 위치) 목표지점을 향해 오브젝트를 회전시키는 함수
        transform.LookAt(curTargetPos);
        ChangeState(EState.AttackWait, CPlayerAni.ANI_ATTACK);
    }

    void AttackWaitState()
    {
        if (attackTimer > attackDelay)
        {
            ChangeState(EState.Attack, CPlayerAni.ANI_ATTACK);

        }
        attackTimer += Time.deltaTime;
    }

    public void WarpState()
    {
        ChangeState(EState.Warp, CPlayerAni.ANI_WARP);
    }

    void DeadState()
    {

    }


    //MoveTo(캐릭터가 이동할 목표 지점의 좌표)
    public void MoveTo(Vector3 tPos)
    {
        if (currentState == EState.Dead)
        {
            return;
        }
        curEnemy = null;
        curTargetPos = tPos;
        ChangeState(EState.Move, CPlayerAni.ANI_RUN);
    }

    void TurnToDestination()
    {
        // Quaternion lookRotation(회전할 목표 방향) : 목표 방향은 목적지 위치에서 자신의 위치를 빼면 구함
        Quaternion lookRotation = Quaternion.LookRotation(curTargetPos - transform.position);
        //Quaternion.RotateTowards(현재의 rotation값, 최종 목표 rotation 값, 최대 회전각)
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 
                                                        Time.deltaTime * rotAnglePerSecond);
    }

    void MoveToDestination()
    {
        //Vector3.MoveTowards(시작 지점, 목표지점, 최대 이동거리)
        transform.position = Vector3.MoveTowards(transform.position, 
                             curTargetPos, moveSpeed * Time.deltaTime);
        
        if (curEnemy == null)
        {
            //플레이어의 위치와 목표지점의 위치가 가까우면, 상태를 Idle 상태로 바꾸라는 명령
            if (Vector3.Distance(transform.position, curTargetPos) < 1f)
            {
                ChangeState(EState.Idle, CPlayerAni.ANI_IDLE);
            }
            
        }
        else if (Vector3.Distance(transform.position, curTargetPos) < attackDistance)
        {
            ChangeState(EState.Attack, CPlayerAni.ANI_ATTACK);
        }
    }
    void Update()
    {
        UpdateState();
    }
}