using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlantFSM : CEnemyFSM
{
    //CMonsterSkillDispenser _mySkill;
    protected override void InitStat()
    {
        base.InitStat();
        _moveSpeed = 0f;
        _attackDistance = 5f;
        _attackAngle = 10f;
        var S1 = new SetSkillCoolTime
        {
            skillCoolDownDown = 3f,
            skillCoolDownUp = 4f
        };
        var S2 = new SetSkillCoolTime
        {
            skillCoolDownDown = 30f,
            skillCoolDownUp = 40f
        };
        SetSkillCoolTimeList.Add(S1);
        SetSkillCoolTimeList.Add(S2);
        SetCoolTime();
        //_mySkill = GetComponent<CMonsterSkillDispenser>();
    }

    #region State 
    protected override void UpdateState()
    {
        if (_actionStart)
        {
            _skillCoolTime[0] -= Time.deltaTime;
            _skillCoolTime[1] -= Time.deltaTime;
            _skillCoolDown1 = true;
            _skillCoolDown2 = true;
        }

        if (_currentBaseState.fullPathHash == _standState)          StartState();
        else if (_currentBaseState.fullPathHash == _attackState1)   AttackState1();
        else if (_currentBaseState.fullPathHash == _waitState)      AttackWaitState();
        else if (_currentBaseState.fullPathHash == _skillState1)    SkillState1();
        else if (_currentBaseState.fullPathHash == _skillState2)    SkillState2();

        if (_skillCoolTime[0] < 0f)
        {
            _anim.SetTrigger("Skill1");
            _skillCoolDown1 = false;
        }
        if (_skillCoolTime[1] < 0f)
        {
            _anim.SetTrigger("Skill2");
            _skillCoolDown2 = false;
        }
    }

    private void StartState()
    {
        _actionStart = true;
    }

    private void SkillState1()
    {
        _skillCoolTime[0] = _originSkillCoolTime[0];
        _skillCoolDown1 = true;
        _lookAtPlayer = true;
    }

    private void RangeSkillShot()
    {
        //_mySkill.BeginEffect(0);
    }

    private void SkillState2()
    {
        _skillCoolTime[1] = _originSkillCoolTime[1];
        _skillCoolDown2 = true;
    }
    
    private void FiledSkill()
    {
        //_mySkill.BeginEffect(1);
    }
    #endregion

    protected override void Update()
    {
        _anim.SetBool("CoolDown", _coolDown);
        _anim.SetBool("CoolDown1", _skillCoolDown1);
        _anim.SetBool("CoolDown2", _skillCoolDown2);
        if (_currentBaseState.fullPathHash != _deadState)
            IsLookPlayer();
        base.Update();
    }
}