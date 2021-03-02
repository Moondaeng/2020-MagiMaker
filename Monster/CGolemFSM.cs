using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CGolemFSM : CEnemyFSM
{
    #region 골렘만 필요한 Properties
    [SerializeField] GameObject _hand;
    [SerializeField] GameObject _rock;
    GameObject Rock;
    ThrowObject RockScript;
    bool _holding;
    bool _shooting;
    #endregion
    
    protected override void InitStat()
    {
        base.InitStat();
        _moveSpeed = 4f;
        _attackDistance = 7f;
        _attackAngle = 20f;
        var S1 = new SetSkillCoolTime
        {
            skillCoolDownDown = 6f,
            skillCoolDownUp = 8f
        };
        var S2 = new SetSkillCoolTime
        {
            skillCoolDownDown = 6f,
            skillCoolDownUp = 8f
        };
        SetSkillCoolTimeList.Add(S1);
        SetSkillCoolTimeList.Add(S2);
        SetCoolTime();
        _myPara.hitEvent.AddListener(CallHitEvent);
    }

    #region State
    protected override void UpdateState()
    {
        if (_actionStart)
        {
            _skillCoolTime[0] -= Time.deltaTime;
            _skillCoolDown1 = true;
            //_skillCooltime2 -= Time.deltaTime;
            //_skillCoolDown2 = true;
        }
        if (_currentBaseState.fullPathHash == _walkState) ChaseState();
        else if (_currentBaseState.fullPathHash == _attackState1) AttackState1();
        else if (_currentBaseState.fullPathHash == _skillState1) SkillState1();
        else if (_currentBaseState.fullPathHash == _skillState2) SkillState2();
        else if (_currentBaseState.fullPathHash == _waitState) AttackWaitState();
        else if (_currentBaseState.fullPathHash == _deadState) DeadState();

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

    private void ChaseWaitState()
    {

    }

    protected override void ChaseState()
    {
        base.ChaseState();
    }
    #region Skill
    private void SkillState1()
    {
        _myState = EState.Skill1;
        _lookAtPlayer = false;
        if (_holding) Rock.transform.position = _hand.transform.position;
        else
        {
            if (_shooting)
            {
                Rock.SendMessage("StartShot");
                _shooting = false;
                _skillCoolTime[0] = _originSkillCoolTime[0];
                _skillCoolDown1 = false;
            }
        }
    }
    private void SkillState2()
    {
        _myState = EState.Skill2;
    }
    private void CreateRock()
    {
        Rock = Instantiate(_rock, _hand.transform.position, Quaternion.identity) as GameObject;
        RockScript = Rock.GetComponent<ThrowObject>();
        RockScript.Target = _player.transform;
        RockScript.Projectile = Rock.transform;
        RockScript._myTransform = _hand.transform;
    }
    
    private void OnHold()
    {
        _holding = true;
    }

    private void OffHold()
    {
        _holding = false;
        _shooting = true;
    }
    
    #endregion
    #endregion
    
    protected override void Update()
    {
        _anim.SetBool("CoolDown", _coolDown);
        _anim.SetBool("CoolDown1", _skillCoolDown1);
        _anim.SetBool("CoolDown2", _skillCoolDown2);
        _anim.SetBool("AnotherAction", _anotherAction);
        _anim.SetBool("Hit", _getHit);
        if (_getHit)  _getHit = false;
        if (_currentBaseState.fullPathHash != _deadState)
            IsLookPlayer();
        base.Update();
    }
}