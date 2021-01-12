using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGolemFSM : CEnemyFSM
{
    #region 골렘만 필요한 Properties
    int _attackCount;
    [SerializeField] GameObject _hand;
    [SerializeField] GameObject _rock;
    GameObject Rock;
    ThrowObject RockScript;
    bool _holding;
    bool _shooting;
    #endregion
    
    protected override void InitStat()
    {
        _attackCount = 0;
        _moveSpeed = 4f;
        _attackDistance = 4f;
        _attackRadius = 20f;

        _anim = GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _myPara.deadEvent.AddListener(CallDeadEvent);
        _spawnID = _myPara.GetComponent<CEnemyPara>()._spawnID;
        _myRespawn = _myPara.GetComponent<CEnemyPara>()._myRespawn;

        _idleState = Animator.StringToHash("Base Layer.Idle");
        _walkState = Animator.StringToHash("Base Layer.Walk");
        _attackState1 = Animator.StringToHash("Base Layer.Attack");
        _attackState2 = Animator.StringToHash("Base Layer.Throw");
        _skillState1 = Animator.StringToHash("Base Layer.Skill");
        _gethitState = Animator.StringToHash("Base Layer.GetHit");
        _deadState1 = Animator.StringToHash("Base Layer.Dead");
    }

    #region 통상적인 State 관련 함수들
    protected override void UpdateState()
    {
        if (_currentBaseState.fullPathHash == _walkState) ChaseState();
        else if (_currentBaseState.fullPathHash == _attackState1) AttackState1();
        else if (_currentBaseState.fullPathHash == _attackState2) AttackState2();
        else if (_currentBaseState.fullPathHash == _waitState) AttackWaitState();
    }

    private void ChaseState()
    {
        if (_currentBaseState.fullPathHash != _deadState1)
        {
            MoveState();
        }
    }

    private void AttackState1()
    {
        if (!_lookAtPlayer)
        {
            _lookAtPlayer = false;
            transform.LookAt(_player.transform.position);
        }
        _coolDown = true;
    }

    private void AttackWaitState()
    {
        _cooltime -= Time.deltaTime;
        _lookAtPlayer = true;
        if (_cooltime < 0)
        {
            _coolDown = false;
            _cooltime = _originCooltime;
        }
        else if (GetDistanceFromPlayer(_distances) > _attackDistance)
        {
            _coolDown = false;
            _anotherAction = true;
            _cooltime = _originCooltime;
        }
    }
    #endregion

    #region 스킬 관련 함수들
    private void AttackState2()
    {
        if (!_lookAtPlayer)
        {
            _lookAtPlayer = false;
        }
        if (_holding) Rock.transform.position = _hand.transform.position;
        else
        {
            if (_shooting)
            {
                Rock.SendMessage("StartShot");
                _shooting = false;
            }
        }
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

    protected override void Update()
    {
        _anim.SetInteger("PlayerCount", _players.Count);
        _anim.SetInteger("AttackCount", _attackCount);
        base.Update();
    }
}