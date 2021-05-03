using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkelNavFSM : CEnemyNavFSM
{
    CMonsterAttackChecker SkillTrail1, SkillTrail2;
    Coroutine _skillChecker;
    protected override void Start()
    {
        AttackTrail1 = transform.GetChild(2).GetComponent<CMonsterAttackChecker>();
        SkillTrail1 = _skillList[0].skill.GetComponent<CMonsterAttackChecker>();
        SkillTrail2 = _skillList[1].skill.GetComponent<CMonsterAttackChecker>();
        SkillTrail1._creator = gameObject;
        SkillTrail2._creator = gameObject;
        base.Start();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        SkillChecker(0, 2f);
        SkillChecker(1, 2f);
        if (!_IsActiveCo && !CMonsterManager.instance._IsOrder && _idle)
        {
            OnCoroutine();
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
    #endregion
}