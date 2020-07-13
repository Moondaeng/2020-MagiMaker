using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGoblinFSM : CEnemyFSM
{
    public List<float> _distances = new List<float>();

    public override void InitStat()
    {
        _moveSpeed = 5f;
        _anim = GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _myPara.deadEvent.AddListener(CallDeadEvent);

        _spawnID = _myPara.GetComponent<CEnemyPara>()._spawnID;
        _myRespawn = _myPara.GetComponent<CEnemyPara>()._myRespawn;

        _walkState = Animator.StringToHash("Base Layer.Run");
        _idleBattleState = Animator.StringToHash("Base Layer.Idle_battle");
        _attackState = Animator.StringToHash("Base Layer.Attack1");
        _deadState1 = Animator.StringToHash("Base Layer.Death1");
        _deadState2 = Animator.StringToHash("Base Layer.Death2");
    }

    public void CallDeadEvent()
    {
        int n = Random.Range(0, 2);
        if (n == 0)
        {
            _anim.SetInteger("Dead", 1);
        }
        else { _anim.SetInteger("Dead", 2); }
        GetComponent<Collider>().enabled = false;
        _player.gameObject.SendMessage("CurrentEnemyDead");
        Invoke("RemoveMe", 2f);
    }
    void RemoveMe()
    {
        _myRespawn.GetComponent<CRespawn>().RemoveMonster(_spawnID);
        _anim.SetInteger("Dead", 0);
    }

    public void AttackCalculate()
    {
        _playerPara.SetEnemyAttack(_myPara.GetRandomAttack(_playerPara._eType, _myPara._eType));
    }
    
    public void UpdateState()
    {  
        if (_currentBaseState.nameHash == _walkState)
        {
            ChaseState();
        }
        else if (_currentBaseState.nameHash == _attackState)
        {
            AttackState();
        }
    }

    public void ChaseState()
    {
        MoveState();
    }

    public void AttackState()
    {
        transform.LookAt(_player.transform.position);
    }

    protected override void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            _player.transform.position, _moveSpeed * Time.deltaTime);
    }

    protected override void TurnToDestination()
    {
        Quaternion lookRotation =
            Quaternion.LookRotation(_player.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            lookRotation, Time.deltaTime * _rotAnglePerSecond);
    }

    //플레이어와 거리을 재는 함수
    private float GetDistanceFromPlayer()
    {
        for (int i = 0; i < _distances.Count - 1; i++)
        {
            int j, tmp = i;
            float tempDis;
            GameObject nullGameObj;

            for (j = i + 1; j < _distances.Count; j++)
            {
                if (_distances[tmp] >= _distances[j] + 4f)
                {
                    tmp = j;
                }
            }

            tempDis = _distances[tmp];
            _distances[tmp] = _distances[i];
            _distances[i] = tempDis;

            nullGameObj = _players[tmp];
            _players[tmp] = _players[i];
            _players[i] = nullGameObj;
        }
        _player = _players[0];
        _playerPara = _players[0].GetComponent<CPlayerPara>();
        return _distances[0];
    }

    public override void Update()
    {
        base.Update();
        _currentBaseState = _anim.GetCurrentAnimatorStateInfo(0);
        _distances = CalculateDistance(_players);
        Debug.Log(_players.Count);
        _anim.SetInteger("PlayerCount", _players.Count);
        if (_players.Count > 1)
        {
            _anim.SetFloat("DistanceFromPlayer", GetDistanceFromPlayer());
            UpdateState();
        }
        else if (_players.Count == 1)
        {
            _player = _players[0];
            _playerPara = _players[0].GetComponent<CPlayerPara>();
            _anim.SetFloat("DistanceFromPlayer", _distances[0]);
            UpdateState();
        }
        else if (_players.Count == 0)
        {
            _anim.SetFloat("DistanceFromPlayer", 999f);
        }
    }
}