using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGoblinFSM : CEnemyFSM
{
    #region 고블린만 가지는 Properties
    bool _runEnd;
    Vector3 _dest;
    float second = 1f;
    #endregion

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
        _moveSpeed = 5f;
        _attackDistance = 5f;
        _attackAngle = 10f;
        AttackTrail1 = transform.GetChild(3).GetComponent<CMonstermeleeChecker>();
        AttackTrail2 = transform.GetChild(4).GetComponent<CMonstermeleeChecker>();
    }
    

    #region State
    protected override void UpdateState()
    {
        if (_actionStart)
        {
            _skillCoolTime[0] -= Time.deltaTime;
            _skillCoolDown1 = true;
        }
        if (_currentBaseState.fullPathHash == _runState) ChaseState();
        else if (_currentBaseState.fullPathHash == _attackState1
            || _currentBaseState.fullPathHash == _attackState2) AttackState1();
        else if (_currentBaseState.fullPathHash == _waitState) AttackWaitState();
        else if (_currentBaseState.fullPathHash == _skillWaitState1) SkillWaitState1();
        else if (_currentBaseState.fullPathHash == _skillState1) SkillState1();
        else if (_currentBaseState.fullPathHash == _deadState) DeadState();

        if (_skillCoolTime[0] < 0f)
        {
            _anim.SetBool("Skill1", true);
            _skillCoolDown1 = false;
        }
    }

    protected override void ChaseState()
    {
        if (_runEnd)
        {
            _runEnd = false;
        }
        base.ChaseState();
    }
    
    #endregion

    #region 스킬관련 함수들
    protected void SkillWaitState1()
    {
        if (_myState != EState.SkillWait1)
        {
            _myState = EState.SkillWait1;
        }
        _skill1 = false;
        float runDistance = 10f;
        Vector3 Distance = Vector3.forward * runDistance;
        Quaternion Rotate = Quaternion.Euler(0f, 180f, 0f);
        Vector3 TargetPoint = Rotate * Distance;
        _dest = transform.position + TargetPoint;
    }

    protected void SkillState1()
    {
        if (_myState != EState.Skill1)
        {
            _myState = EState.Skill1;
        }
        second -= Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position,
            _dest, _moveSpeed * 2f * Time.deltaTime);

        Quaternion lookRotation =
                Quaternion.LookRotation(_dest - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            lookRotation, Time.deltaTime * _rotAnglePerSecond);

        if (Vector3.Distance(transform.position, _dest) < 0.5f || second < 0f)
        {
            _runEnd = true;
            _skillCoolTime[0] = _originSkillCoolTime[0];
            second = 1f;
        }
    }

    #endregion

    protected override void Update()
    {
        _anim.SetBool("Skill1", _skill1);
        _anim.SetBool("CoolDown", _coolDown);
        _anim.SetBool("CoolDown1", _skillCoolDown1);
        _anim.SetBool("RunEnd", _runEnd);
        _anim.SetBool("AnotherAction", _anotherAction);
        base.Update();
    }
}