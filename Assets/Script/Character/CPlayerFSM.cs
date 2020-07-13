using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerFSM : CharacterFSM
{
    private static CLogComponent _logger;

    float skillDelay, skillTimer;
    //마우스 클릭 지점, 플레이어가 이동할 목적지의 좌표를 저장할 예정
    Vector3 curTargetPos;
    GameObject curEnemy;

    CPlayerAni myAni;
    CPlayerPara myPara;
    CEnemyPara curEnemyPara;

    private void Awake()
    {
        _logger = new CLogComponent(ELogType.Character);
    }

    public override void InitStat()
    {
        moveSpeed = 3f;
        attackDelay = 1f;
        attackTimer = 0f;
        attackDistance = 4.5f;

        myAni = GetComponent<CPlayerAni>();
        myPara = GetComponent<CPlayerPara>();
        myPara.InitPara();
        myPara.deadEvent.AddListener(ChangeToPlayerDead);
        ChangeState(EState.Idle, CPlayerAni.ANI_IDLE);
    }

    public void currentEnemyName()
    {

    }

    public void ChangeToPlayerDead()
    {
        _logger.Log("player was dead");
        ChangeState(EState.Dead, CPlayerAni.ANI_DEATH);
    }

    public void CurrentEnemyDead()
    {
        ChangeState(EState.Idle, CPlayerAni.ANI_IDLE);

        curEnemy = null;
    }

    /*void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Punch")
        {
        }
    }*/

    public override void AttackCalculate()
    {
        if (curEnemy == null)
        {
            return;
        }
        curEnemy.GetComponent<CEnemyPara>().ShowHitEffect();

        float attackPower = myPara.GetRandomAttack(curEnemyPara._eType, myPara._eType);
        curEnemyPara.SetEnemyAttack(attackPower);
    }

    public override void AttackEnemy(GameObject Enemy)
    {
        //Debug.Log(Enemy);
        if (curEnemy != null && curEnemy == Enemy)
        {
            return;
        }
        // 적(몬스터)의 파라미터를 변수에 저장
        curEnemyPara = Enemy.GetComponent<CEnemyPara>();

        if (curEnemyPara._isDead == false)
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

    public override void ChangeState(EState newState, int aniNumber)
    {
        if (currentState == newState)
        {
            return;
        }
        myAni.ChangeAni(aniNumber);
        currentState = newState;
        _logger.Log(currentState);
    }

    //캐릭터의 상태가 바뀌면 어떤 일이 일어날지 를 미리 정의
    public override void UpdateState()
    {
        switch (currentState)
        {
            case EState.Idle:
                IdleState();
                break;
            case EState.Move:
                MoveState();
                break;
            case EState.Attack:
                AttackState();
                break;
            case EState.AttackWait:
                AttackWaitState();
                break;
            case EState.Skill:
                SkillState();
                break;
            case EState.SkillWait:
                SkillWaitState();
                break;
            case EState.Warp:
                WarpState();
                break;
            case EState.Dead:
                DeadState();
                break;
            default:
                break;
        }
    }

    public override void AttackState()
    {
        attackTimer = 0f;

        //transform.LookAt(목표지점 위치) 목표지점을 향해 오브젝트를 회전시키는 함수
        transform.LookAt(curTargetPos);
        ChangeState(EState.AttackWait, CPlayerAni.ANI_ATKIDLE);
    }

    public override void AttackWaitState()
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

    public void SkillState()
    {
        skillTimer = 0f;
        transform.LookAt(curTargetPos);
        ChangeState(EState.SkillWait, CPlayerAni.ANI_ATKIDLE);
    }

    public void SkillWaitState()
    {
        if (skillTimer > skillDelay)
        {
            ChangeState(EState.Skill, CPlayerAni.ANI_SKILL);
        }
        skillTimer += Time.deltaTime;
    }

    public override void MoveTo(Vector3 tPos)
    {
        if (currentState == EState.Dead)
        {
            return;
        }
        curEnemy = null;
        curTargetPos = tPos;
        ChangeState(EState.Move, CPlayerAni.ANI_RUN);
    }

    public void TurnTo(Vector3 tPos)
    {
        curTargetPos = tPos;
        TurnToDestination();
    }

    public override void MoveToDestination()
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

    public override void TurnToDestination()
    {
        // 눌렀을 떄,  y축으로 캐릭터가 기울어짐을 방지하기 위함
        curTargetPos = curTargetPos - new Vector3(0f, curTargetPos.y, 0f);
        // Quaternion lookRotation(회전할 목표 방향) : 목표 방향은 목적지 위치에서 자신의 위치를 빼면 구함
        Quaternion lookRotation = Quaternion.LookRotation(curTargetPos - transform.position);
        //Quaternion.RotateTowards(현재의 rotation값, 최종 목표 rotation 값, 최대 회전각)
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation,
                                                        Time.deltaTime * rotAnglePerSecond);
    }
}