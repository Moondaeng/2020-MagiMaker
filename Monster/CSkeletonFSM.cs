using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkeletonFSM : CEnemyFSM
{
    protected override void InitStat()
    {
        _moveSpeed = 4f;
        _attackDistance = 5f;
        _attackAngle = 20f;
        _anim = GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _myPara.deadEvent.AddListener(CallDeadEvent);

        // 몬스터 마다 다른 행동양식들
        _idleState = Animator.StringToHash("Base Layer.Idle");
        _walkState = Animator.StringToHash("Base Layer.Walk");
        _standState = Animator.StringToHash("Base Layer.Stand");
        _runState = Animator.StringToHash("Base Layer.Run");
        _attackState1 = Animator.StringToHash("Base Layer.Attack");
        _waitState = Animator.StringToHash("Base Layer.Wait");
        _skillState1 = Animator.StringToHash("Base Layer.SkillWait1");
        _skillState2 = Animator.StringToHash("Base Layer.SkillWait2");
        _gethitState = Animator.StringToHash("Base Layer.Hit");
        _deadState1 = Animator.StringToHash("Base Layer.Dead");

        _cooltime = .5f;
        _skillCooltime1 = Random.Range(8f, 12f);
        _skillCooltime2 = Random.Range(4f, 6f);
        _originCooltime = _cooltime;
        _originSkillCooltime1 = _skillCooltime1;
        _originSkillCooltime2 = _skillCooltime2;
    }
    
    #region 통상적인 State 관련 함수들
    protected override void UpdateState()
    {
        if (_actionStart)
        {
            _skillCooltime1 -= Time.deltaTime;
            _skillCooltime2 -= Time.deltaTime;
            _skillCoolDown1 = true;
            _skillCoolDown2 = true;
        }

        if (_currentBaseState.fullPathHash == _idleState) IdleState();
        else if (_currentBaseState.fullPathHash == _walkState)           ChaseState();
        else if (_currentBaseState.fullPathHash == _attackState1)    AttackState1();
        else if (_currentBaseState.fullPathHash == _waitState)      AttackWaitState();
        else if (_currentBaseState.fullPathHash == _skillState1)    SkillState1();
        else if (_currentBaseState.fullPathHash == _skillState2)    SkillState2();
        else if (_currentBaseState.fullPathHash == _deadState1)    DeadState1();

        if (_skillCooltime1 < 0f)
        {
            _anim.SetTrigger("Skill1");
            _skillCoolDown1 = false;
        }
        if (_skillCooltime2 < 0f)
        {
            _anim.SetTrigger("Skill2");
            _skillCoolDown2 = false;
        }
    }
    
    protected override void ChaseState()
    {
        base.ChaseState();
        if (_currentBaseState.fullPathHash != _deadState1) MoveState();
    }
    
    private void SkillState1()
    {
        if (_myState != EState.Skill1)
        {
            _myState = EState.Skill1;
        }
        if (_lookAtPlayer)
        {
            _lookAtPlayer = false;
        }
       
        _skillCooltime1 = _originSkillCooltime1;
        _skillCoolDown1 = true;
    }

    private void Skill2AttackCheck()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (IsTargetInSight(_attackAngle, _players[i].transform) && IsInAttackDistance(_attackDistance, _players[i].transform))
            {
                var DamagedPlayerPara = _players[i].GetComponent<CPlayerPara>();
                Skill2AttackDamage(DamagedPlayerPara);
            }
        }
    }
    
    private void Skill2AttackDamage(CPlayerPara c)
    {
        c.DamegedRegardDefence((_myPara.RandomAttackDamage() * 15 / 10));
    }

    private void Skill2AttackCheck2()
    {
        bool exist = false;
        for (int i = 0; i < _players.Count; i++)
        {
            if (IsTargetInSight(_attackAngle, _players[i].transform) && IsInAttackDistance(_attackDistance, _players[i].transform))
            {
                var DamagedPlayerPara = _players[i].GetComponent<CPlayerPara>();
                Skill2AttackDamage2(DamagedPlayerPara);
            }
        }
    }
    
    private void Skill2AttackDamage2(CPlayerPara c)
    {
        c.DamegedRegardDefence(_myPara.RandomAttackDamage() * 2);
    }

    private void SkillState2()
    {
        if (_myState != EState.Skill2)
        {
            _myState = EState.Skill2;
        }
        if (_lookAtPlayer)
        {
            _lookAtPlayer = false;
        }
        _skillCooltime2 = _originSkillCooltime2;
        _skillCoolDown2 = true;
    }


    #endregion

    #region Hit 애니메이션 Reciever 오류 피하기용
    private void Hit()
    {
        
    }
    #endregion

    protected override void Update()
    {
        if (_currentBaseState.fullPathHash == _deadState1) return;
        _anim.SetBool("CoolDown", _coolDown);
        _anim.SetBool("CoolDown1", _skillCoolDown1);
        _anim.SetBool("CoolDown2", _skillCoolDown2);
        _anim.SetBool("AnotherAction", _anotherAction);
        if (_currentBaseState.fullPathHash != _deadState1)
            IsLookPlayer();
        base.Update();
    }
}