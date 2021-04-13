using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkeletonFSM : CEnemyFSM
{
    protected override void InitStat()
    {
        base.InitStat();
        _moveSpeed = 4f;
        _attackDistance = 5f;
        _attackAngle = 20f;
        AttackTrail1 = transform.GetChild(2).GetComponent<CMonstermeleeChecker>();
    }
    
    #region 통상적인 State 관련 함수들
    protected override void UpdateState()
    {
        base.UpdateState();

        if (_skillCoolTime[0] < 0f)
        {
            _anim.SetTrigger("Skill1");
            _skillCoolDown1 = false;
        }
        if (_skillCoolTime[2] < 0f)
        {
            _anim.SetTrigger("Skill2");
            _skillCoolDown2 = false;
        }
    }
    
    protected override void ChaseState()
    {
        base.ChaseState();
        if (_currentBaseState.fullPathHash != _deadState) MoveState();
    }

    protected override void SkillState1()
    {
        if (_myState != EState.Skill1)
        {
            _myState = EState.Skill1;
        }
        if (_lookAtPlayer)
        {
            _lookAtPlayer = false;
        }

        _skillCoolTime[1] = _originSkillCoolTime[0];
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

    protected override  void SkillState2()
    {
        if (_myState != EState.Skill2)
        {
            _myState = EState.Skill2;
        }
        if (_lookAtPlayer)
        {
            _lookAtPlayer = false;
        }
        _skillCoolTime[1] = _skillCoolTime[1];
        _skillCoolDown2 = true;
    }


    #endregion

    protected override void Update()
    {
        if (_currentBaseState.fullPathHash == _deadState) return;
        _anim.SetBool("CoolDown", _coolDown);
        _anim.SetBool("CoolDown1", _skillCoolDown1);
        _anim.SetBool("CoolDown2", _skillCoolDown2);
        _anim.SetBool("AnotherAction", _anotherAction);
        if (_currentBaseState.fullPathHash != _deadState)
            IsLookPlayer();
        base.Update();
    }
}