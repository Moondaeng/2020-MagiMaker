using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSlimeFSM : CEnemyFSM
{
    #region MonsterFSM에서 공유되는 것들

    protected override void InitStat()
    {
        base.InitStat(); 
        var S1 = new SetSkillCoolTime
        {
            skillCoolDownDown = 6f,
            skillCoolDownUp = 8f
        };
        SetSkillCoolTimeList.Add(S1);
        SetCoolTime();
        _moveSpeed = 4f;
    }
    #endregion

    #region 통상적인 State 관련 함수들
    protected override void UpdateState()
    {
        if (_actionStart)
        {
            _skillCoolTime[0] -= Time.deltaTime;
            _skillCoolDown1 = true;
        }

        if (_currentBaseState.fullPathHash == _runState) ChaseState();
        else if (_currentBaseState.fullPathHash == _attackState1) AttackState1();
        else if (_currentBaseState.fullPathHash == _waitState) AttackWaitState();
        else if (_currentBaseState.fullPathHash == _skillState1) SkillState1();
        else if (_currentBaseState.fullPathHash == _deadState) DeadState();

        if (_skillCoolTime[0] < 0f)
        {
            _anim.SetTrigger("Skill1");
            _skillCoolDown1 = false;
        }
    }

    protected override void ChaseState()
    {
        base.ChaseState();
        if (_currentBaseState.fullPathHash != _deadState) MoveState();
    }
    
    #endregion

    #region Skill 관련 State들
    private void SkillState1()
    {
        Debug.Log("fuck");
        _skillCoolTime[0] = _originSkillCoolTime[0];
    }

    #endregion

    protected override void Update()
    {
        _anim.SetBool("CoolDown", _coolDown);
        _anim.SetBool("AnotherAction", _anotherAction);
        base.Update();
    }
}