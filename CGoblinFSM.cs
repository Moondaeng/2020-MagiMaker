using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGoblinFSM : CEnemyFSM
{
    
    Transform player;
    CEnemyPara myPara;
    CPlayerPara playerPara;

    float chaseDistance = 5f; // 플레이어를 향해 몬스터가 추적을 시작할 거리
    float attackDistance = 1f; // 플레이어가 안쪽으로 들어오게 되면 공격을 시작

    GameObject myRespawn;
    public int spawnID { get; set; }

    private Animator anim;
    private AnimatorStateInfo currentBaseState;     // 기본 레이어에 사용되는 애니메이터의 현재 상테에 대한 참조
    public int idleBattleState = Animator.StringToHash("Base Layer.Idle_battle");
    public int walkState = Animator.StringToHash("Base Layer.Run");
    public int deadState1 = Animator.StringToHash("Base Layer.Death1");
    public int deadState2 = Animator.StringToHash("Base Layer.Death2");
    public int attackState = Animator.StringToHash("Base Layer.Attack1");

    public override void InitStat()
    {
        _moveSpeed = 5f;
        anim = GetComponent<Animator>();
        myPara = GetComponent<CEnemyPara>();
        myPara.deadEvent.AddListener(CallDeadEvent);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerPara = player.gameObject.GetComponent<CPlayerPara>();

        spawnID = myPara.GetComponent<CEnemyPara>()._spawnID;
        myRespawn = myPara.GetComponent<CEnemyPara>()._myRespawn;
    }

    public void CallDeadEvent()
    {
        int n = Random.Range(0, 2);
        if (n == 0)
        {
            anim.SetInteger("Dead", 1);
        }
        else { anim.SetInteger("Dead", 2); }
        GetComponent<Collider>().enabled = false;
        player.gameObject.SendMessage("CurrentEnemyDead");
        Invoke("RemoveMe", 2f);
    }
    void RemoveMe()
    {
        myRespawn.GetComponent<CRespawn>().RemoveMonster(spawnID);
        anim.SetInteger("Dead", 0);
    }

    public void AttackCalculate()
    {
        playerPara.SetEnemyAttack(myPara.GetRandomAttack(playerPara._eType, myPara._eType));
    }
    
    public void UpdateState()
    {  
        if (currentBaseState.nameHash == walkState)
        {
            ChaseState();
        }
        else if (currentBaseState.nameHash == attackState)
        {
            AttackState();
        }
    }

    public void ChaseState()
    {
        MoveState();
    }

    public void AttackState()
    {/*
        if (player.GetComponent<CPlayerFSM>().currentState == EState.Dead)
        {
            anim.SetBool("PlayerDead", true);
        }*/
    }

    public override void TurnToDestination()
    {
        Quaternion lookRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
            lookRotation, Time.deltaTime * _rotAnglePerSecond);
    }

    public override void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            player.position, _moveSpeed * Time.deltaTime);
    }

    //플레이어와 거리을 재는 함수
    float GetDistanceFromPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
    }

    public void Update()
    {
        anim.SetFloat("DistanceFromPlayer", GetDistanceFromPlayer());
        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
        UpdateState();
    }
}