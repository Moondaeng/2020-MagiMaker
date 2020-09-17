using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkeletonFSM : CEnemyFSM
{
    protected override void InitStat()
    {
        _moveSpeed = 4f;
        _attackDistance = 3f;
        _attackRadius = 20f;
        _anim = GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _myPara.deadEvent.AddListener(CallDeadEvent);

        _spawnID = _myPara.GetComponent<CEnemyPara>()._spawnID;
        _myRespawn = _myPara.GetComponent<CEnemyPara>()._myRespawn;
        
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

        _cooltime = 1f;
        _skillCooltime1 = 10f;
        _skillCooltime2 = 5f;
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

        if (_currentBaseState.fullPathHash == _walkState)           ChaseState();
        else if (_currentBaseState.fullPathHash == _attackState1)    AttackState();
        else if (_currentBaseState.fullPathHash == _waitState)      AttackWaitState();
        else if (_currentBaseState.fullPathHash == _skillState1)    SkillState1();
        else if (_currentBaseState.fullPathHash == _skillState2)    SkillState2();

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

    private void ChaseState()
    {
        if (!_actionStart) _actionStart = true;
        if (_currentBaseState.fullPathHash != _deadState1) MoveState();
    }

    protected override void MoveState()
    {
        _anotherAction = false;
        base.MoveState();
    }
    
    private void AttackState()
    {
        if (!_lookAtPlayer)
        {
            _lookAtPlayer = false;
        }
        _coolDown = true;
    }
    
    private void AttackWaitState()
    {
        _cooltime -= Time.deltaTime;
        _lookAtPlayer = true;
        if (_cooltime < 0)
        {
            _coolDown = false;
            _cooltime = _originCooltime;
        }
        else if (GetDistanceFromPlayer(_distances) > _attackDistance)
        {
            _coolDown = false;
            _anotherAction = true;
            _cooltime = _originCooltime;
        }
    }

    private void SkillState1()
    {
        _skillCooltime1 = _originSkillCooltime1;
        _skillCoolDown1 = true;
    }

    private void SkillState2()
    {
        _skillCooltime2 = _originSkillCooltime2;
        _skillCoolDown2 = true;
    }
    #endregion

    protected override void Update()
    {
        _anim.SetBool("CoolDown", _coolDown);
        _anim.SetBool("CoolDown1", _skillCoolDown1);
        _anim.SetBool("CoolDown2", _skillCoolDown2);
        _anim.SetBool("AnotherAction", _anotherAction);
        base.Update();
    }
}