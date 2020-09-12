using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGoblinFSM : CEnemyFSM
{
    bool _runEnd;
    Vector3 _dest;
    float second = 1f;
    protected override void InitStat()
    {
        _moveSpeed = 5f;
        _anim = GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _myPara.deadEvent.AddListener(CallDeadEvent);

        _spawnID = _myPara.GetComponent<CEnemyPara>()._spawnID;
        _myRespawn = _myPara.GetComponent<CEnemyPara>()._myRespawn;

        _walkState = Animator.StringToHash("Base Layer.Run");
        _waitState = Animator.StringToHash("Base Layer.AttackWait");
        _standState = Animator.StringToHash("Base Layer.Idle_battle");
        _skillState1 = Animator.StringToHash("Base Layer.Skill1");
        _skillWaitState1 = Animator.StringToHash("Base Layer.SkillWait1");
        _attackState1 = Animator.StringToHash("Base Layer.Attack1");
        _attackState2 = Animator.StringToHash("Base Layer.Attack2");
        _deadState1 = Animator.StringToHash("Base Layer.Death1");
        _deadState2 = Animator.StringToHash("Base Layer.Death2");
        
        _cooltime = 1f;
        _skillCooltime1 = 5f;
        _originCooltime = _cooltime;
        _originSkillCooltime1 = _skillCooltime1;
    }

    protected override void CallDeadEvent()
    {
        int n = Random.Range(0, 2);
        if (n == 0)
        {
            _anim.SetInteger("Dead", 1);
        }
        else { _anim.SetInteger("Dead", 2); }
        this.tag = "Untagged";
        Invoke("RemoveMe", 2f);
    }

    protected override void RemoveMe()
    {
        _myRespawn.GetComponent<CRespawn>().RemoveMonster(_spawnID);
        _anim.SetInteger("Dead", 0);
    }
    
    protected void AttackCheck()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (IsTargetInSight(10f, _players[i].transform) && IsInAttackDistance(3f, _players[i].transform))
            {
                _playerPara = _players[i].GetComponent<CPlayerPara>();
                AttackCalculate();
            }
        }
    }

    protected override void UpdateState()
    {
        if (_actionStart)
        {
            _skillCooltime1 -= Time.deltaTime;
            _skillCoolDown1 = true;
        }
        if (_currentBaseState.nameHash == _walkState) ChaseState();
        else if (_currentBaseState.nameHash == _attackState1
            || _currentBaseState.nameHash == _attackState2) AttackState();
        else if (_currentBaseState.nameHash == _waitState) AttackWaitState();
        else if (_currentBaseState.nameHash == _skillWaitState1) SkillWaitState1();
        else if (_currentBaseState.nameHash == _skillState1) SkillState1();

        if (_skillCooltime1 < 0f)
        {
            _anim.SetBool("Skill1", true);
            _skillCoolDown1 = false;
        }
    }

    protected void ChaseState()
    {
        _runEnd = false;
        if (!_actionStart) _actionStart = true;
        if (_currentBaseState.nameHash != _deadState1 
            || _currentBaseState.nameHash != _deadState2)
            MoveState();
    }

    protected override void MoveState()
    {
        _anotherAction = false;
        base.MoveState();
    }

    protected void AttackState()
    {
        if (!_lookAtPlayer)
            transform.LookAt(_player.transform.position
                - new Vector3(0f, _player.transform.position.y, 0f));
        _coolDown = true;
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
    
    protected void SkillWaitState1()
    {
        _skill1 = false;
        float runDistance = 10f;
        Vector3 Distance = Vector3.forward * runDistance;
        Quaternion Rotate = Quaternion.Euler(0f, 180f, 0f);
        Vector3 TargetPoint = Rotate * Distance;
        _dest = transform.position + TargetPoint;
    }

    protected void SkillState1()
    {
        second -= Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position,
            _dest, _moveSpeed * 2f * Time.deltaTime);

        Quaternion lookRotation =
                Quaternion.LookRotation(_dest - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            lookRotation, Time.deltaTime * _rotAnglePerSecond);

        if (Vector3.Distance(transform.position, _dest) < 0.5f || second < 0f)
        {
            _runEnd = true;
            _skillCooltime1 = _originSkillCooltime1;
            second = 1f;
        }
    }

    protected override void Update()
    {
        _anim.SetBool("Skill1", _skill1);
        _anim.SetBool("CoolDown", _coolDown);
        _anim.SetBool("CoolDown1", _skillCoolDown1);
        _anim.SetBool("RunEnd", _runEnd);
        _anim.SetBool("AnotherAction", _anotherAction);
        base.Update();
    }
}