using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CGolemFSM : CEnemyFSM
{
    #region 골렘만 필요한 Properties
    [SerializeField] readonly GameObject _hand;
    [SerializeField] readonly GameObject _rock;
    GameObject Rock;
    ThrowObject RockScript;
    bool _holding;
    bool _shooting;
    #endregion
    

    protected override void InitStat()
    {
        _moveSpeed = 4f;
        _attackDistance = 7f;
        _attackAngle = 20f;

        _anim = GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _myPara.deadEvent.AddListener(CallDeadEvent);
        _myPara.hitEvent.AddListener(CallHitEvent);

        _idleState = Animator.StringToHash("Base Layer.Idle");
        _walkState = Animator.StringToHash("Base Layer.Walk");
        _attackState1 = Animator.StringToHash("Base Layer.Attack");
        _waitState = Animator.StringToHash("Base Layer.AttackWait");
        _skillState1 = Animator.StringToHash("Base Layer.Skill1");
        _skillState2 = Animator.StringToHash("Base Layer.Skill2");
        _gethitState = Animator.StringToHash("Base Layer.GetHit");
        _deadState1 = Animator.StringToHash("Base Layer.Dead");

        _cooltime = 1f;
        _skillCooltime1 = 10f;
        _skillCooltime2 = 15f;
        _originCooltime = _cooltime;
        _originSkillCooltime1 = _skillCooltime1;
        _originSkillCooltime2 = _skillCooltime2;
    }

    #region State
    protected override void UpdateState()
    {
        if (_actionStart)
        {
            _skillCooltime1 -= Time.deltaTime;
            _skillCoolDown1 = true;
            //_skillCooltime2 -= Time.deltaTime;
            //_skillCoolDown2 = true;
        }
        if (_currentBaseState.fullPathHash == _walkState) ChaseState();
        else if (_currentBaseState.fullPathHash == _attackState1) AttackState1();
        else if (_currentBaseState.fullPathHash == _skillState1) SkillState1();
        else if (_currentBaseState.fullPathHash == _skillState2) SkillState2();
        else if (_currentBaseState.fullPathHash == _waitState) AttackWaitState();
        else if (_currentBaseState.fullPathHash == _deadState1) DeadState1();

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

    private void ChaseWaitState()
    {

    }

    protected override void ChaseState()
    {
        base.ChaseState();
        if (_currentBaseState.fullPathHash != _deadState1) MoveState();
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
                _skillCooltime1 = _originSkillCooltime1;
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
        if (_currentBaseState.fullPathHash != _deadState1)
            IsLookPlayer();
        base.Update();
    }
}