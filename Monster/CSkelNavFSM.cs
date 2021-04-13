using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkelNavFSM : CEnemyNavFSM
{
    [Tooltip("스킬1 쿨타임 범위")]
    public float[] SkillCooltime1 = new float[2];
    [Tooltip("스킬2 쿨타임 범위")]
    public float[] SkillCooltime2 = new float[2];
    public CMonstermeleeChecker SkillTrail1, SkillTrail2;
    Coroutine _skillChecker;
    protected override void Start()
    {
        AttackTrail1 = transform.GetChild(2).GetComponent<CMonstermeleeChecker>();
        SetCoolTime(SkillCooltime1);
        SetCoolTime(SkillCooltime2);
        Debug.Log(_skillCoolTime[0]);
        Debug.Log(_skillCoolTime[1]);
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        _anim.SetBool("Attack", _isAttack);
        _anim.SetBool("Skill1", _isSkill1);
        _anim.SetBool("Skill2", _isSkill2);
        _anim.SetFloat("SkillMultiply1", _skillMultiply);
        _anim.SetFloat("SkillMultiply2", _skillMultiply2);
        DecreaseCoolTime();

        if (_skillCoolTime[0] <= 0f && _IsActiveCo && _nearestPlayer <= _attackDist)
        {
            nvAgent.Stop();
            OffCoroutine();
            StartCoroutine(BoolClicker(2));
            _skillCoolTime[0] = _originSkillCoolTime[0] + 2f;
        }
        if (_skillCoolTime[0] <= 0f && _IsActiveCo && _nearestPlayer <= _attackDist)
        {
            nvAgent.Stop();
            OffCoroutine();
            StartCoroutine(BoolClicker(3));
            _skillCoolTime[1] = _originSkillCoolTime[1] + 2f;
        }

        if (_currentBaseState.fullPathHash == CMonsterManager._idleState
            && !_IsActiveCo && !CMonsterManager.instance._IsOrder)
        {
            OnCoroutine();
        }

    }

    void DecreaseCoolTime()
    {
        if (_actionStart)
        {
            if (_skillCoolTime[0] > 0f
                && _currentBaseState.fullPathHash != CMonsterManager._skillState2)
                _skillCoolTime[0] -= Time.deltaTime;
            if (_skillCoolTime[1] > 0f
                && _currentBaseState.fullPathHash != CMonsterManager._skillState1)
                _skillCoolTime[1] -= Time.deltaTime;
        }
    }

    #region Animator Event Functions
    void SkillDisabledCollider1()
    {
        if (SkillTrail1 != null && SkillTrail1._attackedPlayer.Count > 0)
            SkillTrail1.DiscardList();
    }
    void SkillDisabledCollider2()
    {
        if (SkillTrail2 != null && SkillTrail2._attackedPlayer.Count > 0)
            SkillTrail2.DiscardList();
    }
    void SetSkillMultiply1() { _skillMultiply = _basicSkillMultiply; }
    void SetSkillMultiply2() { _skillMultiply2 = _basicSkillMultiply2; }

    void ReleaseSkill1() { _skillMultiply = 1f; }
    void ReleaseSkill2() { _skillMultiply2 = 1f; }

    #endregion
}