using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFlyingFSM : CharacterFSM
{
    CFlyingAni myAni;
    Transform player;
    CEnemyPara myPara;
    CPlayerPara playerPara;

    float chaseDistance = 15f; // 플레이어를 향해 몬스터가 추적을 시작할 거리
    float attackDistance = 2.5f; // 플레이어가 안쪽으로 들오오게 되면 공격을 시작
    float reChaseDistance = 5f; // 플레이어가 도망 갈 경우 얼마나 떨어져야 다시 추적

    GameObject myRespawn;
    public int spawnID { get; set; }
    Vector3 originPos;
    int WhoFlag;

    public override void InitStat()
    {
        moveSpeed = 10f;
        attackDelay = 2.5f;
        attackTimer = 3f;
        attackDistance = 4.5f;

        myAni = GetComponent<CFlyingAni>();
        myPara = GetComponent<CEnemyPara>();
        myPara.deadEvent.AddListener(CallDeadEvent);
        RandomMotion(EState.Idle, "idle");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerPara = player.gameObject.GetComponent<CPlayerPara>();
    }

    private void RandomMotion(string motion)
    {
        int judge;
        judge = Random.Range(1, 2);
        motion = motion + judge;
        myAni.ChangeAni(motion);
    }

    private void RandomMotion(EState state, string motion)
    {
        int judge;
        judge = Random.Range(1, 2);
        motion = motion + judge;
        ChangeState(state, motion);
    }

    public void SetRespawn(GameObject respawn, int spawnID, Vector3 originPos)
    {
        myRespawn = respawn;
        this.spawnID = spawnID;
        this.originPos = originPos;
        //Debug.Log(respawn + " " + spawnID + " " + originPos + " ");
    }

    void CallDeadEvent()
    {
        ChangeState(EState.Dead, CFlyingAni.DEATH);
<<<<<<< HEAD
        //석래 추가
        gameObject.SendMessage("InstatiateItem"); //죽으면 아이템 생성
        //석래 끝
=======
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        player.gameObject.SendMessage("CurrentEnemyDead");
        StartCoroutine("RemoveMe");
    }

    IEnumerable RemoveMe()
    {
        yield return new WaitForSeconds(2f);
        
        myRespawn.GetComponent<CRespawn>().RemoveMonster(spawnID);
    }

    public override void AttackCalculate()
    {
<<<<<<< HEAD
        playerPara.SetEnemyAttack(myPara.GetRandomAttack(playerPara.eType, myPara.eType));
=======
        playerPara.SetEnemyAttack(myPara.GetRandomAttack(playerPara._eType, myPara._eType));
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    public void ChangeState(EState newState, string aniName)
    {
        if (currentState == newState)
        {
            return;
        }
        myAni.ChangeAni(aniName);
        currentState = newState;
    }

    public override void UpdateState()
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

    public override void IdleState()
    {
        if (GetDistanceFromPlayer() < chaseDistance)
        {
            ChangeState(EState.Chase, CFlyingAni.FLYING);
        }
    }

    public void ChaseState()
    {
        myAni.ChangeAni(CFlyingAni.FLYING);
        //몬스터가 공격 가능 거리 안으로 들어가면 공격 상태
        if (GetDistanceFromPlayer() < attackDistance)
        {
            RandomMotion(EState.Attack, "atack");
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
    public override void AttackState()
    {
        if (player.GetComponent<CPlayerFSM>().currentState == CPlayerFSM.EState.Dead)
        {
            RandomMotion(EState.Idle, "idle");
        }
        if (GetDistanceFromPlayer() > reChaseDistance)
        {
            attackTimer = 0f;
            ChangeState(EState.Chase, CFlyingAni.FLYING);
        }
        else
        {
            if (attackTimer > attackDelay)
            {
                transform.LookAt(player.position);
                RandomMotion("atack");
                Invoke("TimerReset", 1.5f);
            }
            else
            {
                RandomMotion("idle");
            }
            attackTimer += Time.deltaTime;
        }
    }


    // 죽으면 선택안되게끔 하려고함
    public override void DeadState()
    {
<<<<<<< HEAD
        //GetComponent<BoxCollider>().enabled = false;
=======
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        GetComponent<Collider>().enabled = false;
    }

    public override void TurnToDestination()
    {
        Quaternion lookRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotAnglePerSecond);
    }

    public override void MoveToDestination()
    {
        Vector3 newPos = new Vector3(transform.position.x, 2f, transform.position.z);
        transform.position = Vector3.MoveTowards(newPos, player.position, moveSpeed * Time.deltaTime);
    }

    //플레이어와 거리을 재는 함수
    float GetDistanceFromPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
    }
}