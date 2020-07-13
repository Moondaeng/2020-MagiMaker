using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGolemFSM : CEnemyFSM
{
    public List<float> _distances = new List<float>();
    int _attackCount;
    public GameObject DropItem;
    private CGolemSkill _skill;
    protected override void InitStat()
    {
        _attackCount = 0;
        _moveSpeed = 4f;
        _anim = GetComponent<Animator>();
        //_anim = transform.GetChild(3).GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _myPara.deadEvent.AddListener(CallDeadEvent);
        _skill = GetComponent<CGolemSkill>();
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
        GetComponent<BoxCollider>().enabled = false;
        GameObject.Find("Controller").GetComponent<CController>().DeselectEnemy();
        Instantiate(DropItem, gameObject.transform.position, gameObject.transform.rotation);
        //_player.gameObject.SendMessage("CurrentEnemyDead");
        Invoke("RemoveMe", 3.0f);
    }

    private void RemoveMe()
    {
        _myRespawn.GetComponent<CRespawn>().RemoveMonster(_spawnID);
        _anim.SetBool("Dead", false);
    }

    private void AttackCalculate()
    {
        _playerPara.SetEnemyAttack(_myPara.GetRandomAttack(_playerPara._eType, _myPara._eType));
    }

    protected void AttackCheck()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (IsTargetInSight(10f, _players[i].transform))
            {
                _playerPara = _players[i].GetComponent<CPlayerPara>();
                AttackCalculate();
            }
        }
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

    IEnumerator CheckAnimationState()
    {
        while (!_anim.GetCurrentAnimatorStateInfo(0)
        .IsName("Attack"))
        {
            yield return null;
        }
        while (_anim.GetCurrentAnimatorStateInfo(0)
        .normalizedTime < 0.5f)
        {
            //애니메이션 재생 중 실행되는 부분
            yield return null;
        }
        yield break;
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

    protected override void Update()
    {
        base.Update();
        _currentBaseState = _anim.GetCurrentAnimatorStateInfo(0);
        _distances = CalculateDistance(_players);
        Debug.Log(_attackCount);
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

        //if (_distances[0] < 4f)
        if (_distances.Count != 0 && _distances[0] < 4f)
        {
            StartCoroutine("CheckAnimationState");
        }
    }
}