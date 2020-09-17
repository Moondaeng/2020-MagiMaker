using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGolemFSM : CEnemyFSM
{
    int _attackCount;
    [SerializeField] GameObject _hand;
    [SerializeField] GameObject _rock;
    GameObject Rock;
    ThrowObject RockScript;
    bool _holding;
    protected override void InitStat()
    {
        _attackCount = 0;
        _moveSpeed = 4f;
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

    protected override void CallDeadEvent()
    {
        _anim.SetBool("Dead", true);
        this.tag = "Untagged";
        Invoke("RemoveMe", 3.0f);
    }

    protected void AttackCheck()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (IsTargetInSight(20f, _players[i].transform) && IsInAttackDistance(4f, _players[i].transform))
            {
                _playerPara = _players[i].GetComponent<CPlayerPara>();
                AttackCalculate();
            }
        }
    }

    protected override void UpdateState()
    {
        if (_currentBaseState.nameHash == _walkState) ChaseState();
        else if (_currentBaseState.nameHash == _attackState1) AttackState1();
        else if (_currentBaseState.nameHash == _attackState2) AttackState2();
        else if (_currentBaseState.nameHash == _waitState) AttackWaitState();
    }

    private void ChaseState()
    {
        if (_currentBaseState.nameHash != _deadState1)
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

    private void AttackState2()
    {
        if (!_lookAtPlayer)
        {
            _lookAtPlayer = false;
            transform.LookAt(_player.transform.position);
        }
        if (_holding) Rock.transform.position = _hand.transform.position;
        else
        {
            Rock.SendMessage("StartShot");
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
    }
    
    private void AttackWaitState()
    {
        _cooltime -= Time.deltaTime;
        if (_cooltime < 0)
        {
            _coolDown = false;
            _cooltime = _originCooltime;
        }
        else if (GetDistanceFromPlayer(_distances) > 3f)
        {
            _coolDown = false;
            _anotherAction = true;
            _cooltime = _originCooltime;
        }
    }
    protected override void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            _player.transform.position, _moveSpeed * Time.deltaTime);
    }

    protected override void TurnToDestination()
    {
        Vector3 position = new Vector3(0f, _player.transform.position.y, 0f);
        Quaternion lookRotation = Quaternion.LookRotation(_player.transform.position - position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            lookRotation, Time.deltaTime * _rotAnglePerSecond);
    }

    protected override void Update()
    {
        _anim.SetInteger("PlayerCount", _players.Count);
        _anim.SetInteger("AttackCount", _attackCount);
        base.Update();
    }
}