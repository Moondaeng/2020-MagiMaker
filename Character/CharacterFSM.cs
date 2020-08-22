using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFSM : MonoBehaviour
{
    public enum EState
    {
        Idle,
        Move,
        Attack,
        AttackWait,
        Warp,
        Skill,
        SkillWait,
        Chase,
        Dead
    }
    public float rotAnglePerSecond = 360f; //1초에 플레이어의 방향을 360도 회전
    public float moveSpeed { get; set; } //초당 2미터의 속도로 이동
    public float attackDelay { get; set; } // 공격을 한번 하고 다시 공격할 때까지의 지연
    public float attackTimer { get; set; } //공격을 하고 난 뒤에 경과되는 시간을 계산하기 위한 변수
    public float attackDistance { get; set; } // 공격 거리 (적과의 거리)

    //idle 상태를 기본 상태로 지정
    public EState currentState = EState.Idle;

    void Start()
    {
        InitStat();
    }

    public virtual void InitStat()
    {

    }

    public void TimerReset()
    {
        attackTimer = 0f;
    }

    public virtual void AttackCalculate()
    {

    }

    // 적을 공격하기 위한 함수 
    public virtual void AttackEnemy(GameObject Enemy)
    {

    }

    public virtual void ChangeState(EState newState, int aniNumber)
    {

    }

    public virtual void UpdateState()
    {

    }

    public virtual void IdleState()
    {
        ChangeState(EState.Idle, CPlayerAni.ANI_IDLE);
    }

    public virtual void MoveState()
    {
        TurnToDestination();
        MoveToDestination();
    }

    public virtual void AttackState()
    {

    }

    public virtual void AttackWaitState()
    {

    }

    public virtual void DeadState()
    {

    }

    //MoveTo(캐릭터가 이동할 목표 지점의 좌표)
    public virtual void MoveTo(Vector3 tPos)
    {

    }

    public virtual void TurnToDestination()
    {

    }

    // 종착 위치 계산
    // 이걸 서버로 보내면 될듯?
    public virtual void MoveToDestination()
    {

    }

    public void Update()
    {
        UpdateState();
    }
}
