using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGolemFSM : CEnemyFSM
{
    GameObject _player;
    CEnemyPara _myPara;
    CPlayerPara _playerPara;
    GameObject myRespawn;

    public Animator _anim;
    public AnimatorStateInfo _currentBaseState;     // 기본 레이어에 사용되는 애니메이터의 현재 상테에 대한 참조
    public int _idleState = Animator.StringToHash("Base Layer.Idle");
    public int _walkState = Animator.StringToHash("Base Layer.Walk");
    public int _attackState = Animator.StringToHash("Base Layer.Attack");
    public int _skillState = Animator.StringToHash("Base Layer.Skill");
    public int _gethitState = Animator.StringToHash("Base Layer.GetHit");
    public int _deadState = Animator.StringToHash("Base Layer.Dead");

    public List<GameObject> _players = new List<GameObject>();
    public List<float> _distances = new List<float>();
    
    public override void InitStat()
    {
        _moveSpeed = 4f;
        _anim = GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _myPara.deadEvent.AddListener(CallDeadEvent);

        _spawnID = _myPara.GetComponent<CEnemyPara>()._spawnID;
        myRespawn = _myPara.GetComponent<CEnemyPara>()._myRespawn;
        GetDistanceFromPlayer();
        _anim.SetFloat("DistanceFromPlayer", GetDistanceFromPlayer());
    }
    
    private void CallDeadEvent()
    {
        _anim.SetBool("Dead", true);
        GetComponent<Collider>().enabled = false;
        _player.gameObject.SendMessage("CurrentEnemyDead");
        Invoke("RemoveMe", 3f);
    }

    private void RemoveMe()
    {
        myRespawn.GetComponent<CRespawn>().RemoveMonster(_spawnID);
        _anim.SetBool("Dead", false);
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
        if (_currentBaseState.nameHash != _deadState)
        {
            MoveState();
        }
    }

    private void AttackState()
    {
        transform.LookAt(_player.transform.position);
    }

    public override void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position, 
            _player.transform.position, _moveSpeed * Time.deltaTime);
    }

    public override void TurnToDestination()
    {
        Quaternion lookRotation = 
            Quaternion.LookRotation(_player.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
            lookRotation, Time.deltaTime * _rotAnglePerSecond);
    }

    //플레이어와 거리을 재는 함수
    private float GetDistanceFromPlayer()
    {
        for (int i = 0; i < _distances.Count-1; i++)
        {
            int j;
            int tmp = i;
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

    public void Update()
    {
        _currentBaseState = _anim.GetCurrentAnimatorStateInfo(0);
        _players = DetectPlayer(_players);
        _distances = CalculateDistance(_players);
        Debug.Log(_players.Count);
        _anim.SetInteger("PlayerCount", _players.Count);
        if (_players.Count > 1 )
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