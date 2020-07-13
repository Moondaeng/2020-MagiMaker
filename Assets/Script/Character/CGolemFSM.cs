using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGolemFSM : CEnemyFSM
{
    public List<float> _distances = new List<float>();
    GameObject _dummyNull;
    [SerializeField]
    GameObject _dummyDeath;
    public override void InitStat()
    {
        _moveSpeed = 4f;
        _anim = GetComponent<Animator>();
        //_anim = transform.GetChild(3).GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _myPara.deadEvent.AddListener(CallDeadEvent);

        _spawnID = _myPara.GetComponent<CEnemyPara>()._spawnID;
        _myRespawn = _myPara.GetComponent<CEnemyPara>()._myRespawn;
        
        _idleState = Animator.StringToHash("Base Layer.Idle");
        _walkState = Animator.StringToHash("Base Layer.Walk");
        _attackState = Animator.StringToHash("Base Layer.Attack");
        _skillState = Animator.StringToHash("Base Layer.Skill");
        _gethitState = Animator.StringToHash("Base Layer.GetHit");
        _deadState1 = Animator.StringToHash("Base Layer.Dead");
    }
    
    private void CallDeadEvent()
    {
        _anim.SetBool("Dead", true);
        GetComponent<Collider>().enabled = false;
        _player.gameObject.SendMessage("CurrentEnemyDead");
        Invoke("RemoveMe", 0.0f);
    }

    private void RemoveMe()
    {
        _dummyDeath.SetActive(true);
        _dummyNull = Instantiate(_dummyDeath, transform.position,
            transform.rotation);
        _myRespawn.GetComponent<CRespawn>().RemoveMonster(_spawnID);
        _anim.SetBool("Dead", false);
        Invoke("RemoveDummy", 3f);
    }

    private void RemoveDummy()
    {
        _dummyDeath.SetActive(false);
        _dummyNull.SetActive(false);
    }

    private void AttackCalculate()
    {
        _playerPara.SetEnemyAttack(_myPara.GetRandomAttack(_playerPara._eType, _myPara._eType));
    }

    private void UpdateState()
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

    private void ChaseState()
    {
        if (_currentBaseState.nameHash != _deadState1)
        {
            MoveState();
        }
    }

    private void AttackState()
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
        Vector3 position = new Vector3(0f, _player.transform.position.y, 0f);
        Quaternion lookRotation = Quaternion.LookRotation(_player.transform.position - position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
            lookRotation, Time.deltaTime * _rotAnglePerSecond);
    }

    //플레이어와 거리을 재는 함수
    private float GetDistanceFromPlayer()
    {
        for (int i = 0; i < _distances.Count-1; i++)
        {
            int j, tmp = i;
            float tempDis;
            GameObject nullGameObj;
            
            for (j = i + 1; j < _distances.Count; j++)
            {
                if(_distances[tmp] >= _distances[j] + 4f)
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

        // 다인큐용
        if (_players.Count > 1 )
        {
            _anim.SetFloat("DistanceFromPlayer", GetDistanceFromPlayer());
            UpdateState();
        }
        // 솔플용
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