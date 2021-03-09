using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMiniSlimeFSM : CEnemyFSM
{
    #region MonsterFSM에서 공유되는 것들
    protected override void InitStat()
    {
        _moveSpeed = 4f;
        _anim = GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _myPara.deadEvent.AddListener(CallDeadEvent);

        _spawnID = _myPara.GetComponent<CEnemyPara>()._spawnID;
        _myRespawn = _myPara.GetComponent<CEnemyPara>()._myRespawn;
        
        // 몬스터 마다 다른 행동양식들
        _idleState = Animator.StringToHash("Base Layer.Idle");
        _standState = Animator.StringToHash("Base Layer.Stand");
        _runState = Animator.StringToHash("Base Layer.Run");
        _attackState1 = Animator.StringToHash("Base Layer.Attack");
        _waitState = Animator.StringToHash("Base Layer.Wait");
        _gethitState = Animator.StringToHash("Base Layer.Hit");
        _deadState1 = Animator.StringToHash("Base Layer.Dead");

        _cooltime = 1f;
        _skillCooltime1 = 20f;
        _originCooltime = _cooltime;
        _originSkillCooltime1 = _skillCooltime1;
    }
    #endregion

    #region 통상적인 State 관련 함수들
    protected override void UpdateState()
    {
        if (_actionStart)
        {
            _skillCooltime1 -= Time.deltaTime;
            _skillCoolDown1 = true;
        }

        if (_currentBaseState.fullPathHash == _runState) ChaseState();
        else if (_currentBaseState.fullPathHash == _attackState1) AttackState1();
        else if (_currentBaseState.fullPathHash == _waitState) AttackWaitState();
        else if (_currentBaseState.fullPathHash == _skillState1) SkillState1();
        else if (_currentBaseState.fullPathHash == _deadState1) DeadState1();

        if (_skillCooltime1 < 0f)
        {
            //_anim.SetTrigger("Skill1");
            //_skillCoolDown1 = false;
        }
    }

    protected override void ChaseState()
    {
        base.ChaseState();
        if (_currentBaseState.fullPathHash != _deadState1) MoveState();
    }
    
    #endregion

    #region Skill 관련 State들
    private void SkillState1()
    {
        _skillCooltime1 = _originSkillCooltime1;
    }

    private void SkillState2()
    {
        _skillCooltime2 = _originSkillCooltime2;
    }
    #endregion

    protected override void Update()
    {
        _anim.SetBool("CoolDown", _coolDown);
        _anim.SetBool("CoolDown1", _skillCoolDown1);
        _anim.SetBool("AnotherAction", _anotherAction);
        base.Update();
    }
}