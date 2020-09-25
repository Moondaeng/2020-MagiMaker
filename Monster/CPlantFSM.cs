using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlantFSM : CEnemyFSM
{
    CMonsterSkillDispenser _mySkill;
    protected override void InitStat()
    {
        _moveSpeed = 0f;
        _attackDistance = 5f;
        _attackRadius = 10f;
        _anim = GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _myPara.deadEvent.AddListener(CallDeadEvent);

        _spawnID = _myPara.GetComponent<CEnemyPara>()._spawnID;
        _myRespawn = _myPara.GetComponent<CEnemyPara>()._myRespawn;
        _mySkill = GetComponent<CMonsterSkillDispenser>();

        // 몬스터 마다 다른 행동양식들
        _idleState = Animator.StringToHash("Base Layer.Idle");
        _standState = Animator.StringToHash("Base Layer.Stand");
        _attackState1 = Animator.StringToHash("Base Layer.Attack1");
        _waitState = Animator.StringToHash("Base Layer.Wait");
        _skillState1 = Animator.StringToHash("Base Layer.Skill1");
        _skillState2 = Animator.StringToHash("Base Layer.Skill2");
        _deadState1 = Animator.StringToHash("Base Layer.Death");

        _cooltime = 1f;
        
        _skillCooltime1 = 3f;
        _skillCooltime2 = 5f;
        _originCooltime = _cooltime;
        _originSkillCooltime1 = _skillCooltime1;
        _originSkillCooltime2 = _skillCooltime2;
    }

    protected override void CallDeadEvent()
    {
        _anim.SetTrigger("DeathTrigger");
        base.CallDeadEvent();
    }

    #region State 
    protected override void UpdateState()
    {
        if (_actionStart)
        {
            _skillCooltime1 -= Time.deltaTime;
            _skillCooltime2 -= Time.deltaTime;
            _skillCoolDown1 = true;
            _skillCoolDown2 = true;
        }

        if (_currentBaseState.fullPathHash == _standState)          StartState();
        else if (_currentBaseState.fullPathHash == _attackState1)   AttackState1();
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

    private void StartState()
    {
        _actionStart = true;
    }

    private void SkillState1()
    {
        _skillCooltime1 = _originSkillCooltime1;
        _skillCoolDown1 = true;
        _lookAtPlayer = true;
    }

    private void RangeSkillShot()
    {
        _mySkill.BeginEffect(0);
    }

    private void SkillState2()
    {
        _skillCooltime2 = _originSkillCooltime2;
        _skillCoolDown2 = true;
    }
    
    private void FiledSkill()
    {
        _mySkill.BeginEffect(1);
    }
    #endregion

    protected override void Update()
    {
        _anim.SetBool("CoolDown", _coolDown);
        _anim.SetBool("CoolDown1", _skillCoolDown1);
        _anim.SetBool("CoolDown2", _skillCoolDown2);
        base.Update();
    }
}