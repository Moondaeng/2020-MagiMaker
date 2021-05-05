using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGolemNavFSM : CEnemyNavFSM
{
    #region 골렘만 필요한 Properties
    [Header("던지는 손 GameObject")]
    [SerializeField] GameObject _hand;
    GameObject _rock, _warning;

    bool _shooting = false;
    #endregion

    protected override void Start()
    {
        AttackTrail1 = transform.GetChild(2).GetComponent<CMonsterAttackChecker>();
        //_skillList[0].skill = Resources.Load("");
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        SkillChecker(0, 4f);
        SkillChecker(1, 6f);

        if (!_IsActiveCo && !CMonsterManager.instance._IsOrder && _idle)
        {
            OnCoroutine();
        }

        if (_currentBaseState.fullPathHash == CMonsterManager._skillState1)
        {
            TurnToDestination(this.transform.position, _player.transform.position);
        }

        if (_shooting)
        {
            _rock.SendMessage("StartShot");
            _shooting = false;
        }
    }

    #region Animator Event Functions
    void CreateRock()
    {
        _rock = Instantiate(_skillList[0].skill, _hand.transform.position, Quaternion.identity); 
        CThrowObject to = _rock.GetComponent<CThrowObject>();
        to._target = _player.transform;
        to._holding = _hand.transform;
        CMonsterAttackChecker mc = _rock.GetComponent<CMonsterAttackChecker>();
        mc._creator = gameObject;
        mc._isPenetrated = false;
    }
    void OnHold() { }
    void OffHold() { _shooting = true; }
    void CreateWarning()
    {
        CMonsterSkillChecker ms = _skillList[1].skill.GetComponent<CMonsterSkillChecker>();
        ms._distance = 8f;
        ms._time = 3f;
        CMonsterAttackChecker ma = _skillList[1].skill.GetComponent<CMonsterAttackChecker>();
        ma._creator = this.gameObject;
        ma._isPenetrated = true;
        _warning = Instantiate(_skillList[1].skill, transform.position, Quaternion.identity);
        _warning.SendMessage("Starting");
    }

    #endregion
}
