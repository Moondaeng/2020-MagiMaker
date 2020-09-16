﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CEnemyPara))]

public class CEnemyFSM : MonoBehaviour
{
    #region Properties
    protected Animator _anim;                          // 몬스터가 가지고있는 애니메이터
    protected AnimatorStateInfo _currentBaseState;     // 기본 레이어에 사용되는 애니메이터의 현재 상테에 대한 참조
    protected GameObject _player;
    protected CEnemyPara _myPara;
    protected CPlayerPara _playerPara;
    [HideInInspector] public GameObject _myRespawn;

    protected bool _lookAtPlayer; // 어떤 행동시에, 플레이어를 바라보게 하는 TurnToDestination을 OFF하게 하려는 목적을 가진 bool
    protected bool _coolDown;     // 평타 쿨타임 판단
    protected bool _skill1, _skill2, _skill3; // 스킬 행동의 중간단계의 동작을 관리하기 위한 bool
    protected bool _skillCoolDown1, _skillCoolDown2, _skillCoolDown3; // 스킬 쿨타임 판단
    protected bool _anotherAction; // 공격 사용 후, 플레이어의 거리가 멀어지면 chase 아니면, 그대로 공격하게 하는걸 판단
    protected bool _actionStart; // 플레이어와 조우 전에, 스킬의 쿨타임이 미리 도는 것을 방지하려는 것

    public int _spawnID { get; set; } // CRespawn에서 판별한 SpawnID
    protected float _rotAnglePerSecond = 360f; //1초에 플레이어의 방향을 360도 회전
    public float _moveSpeed { get; set; } //초당 ~미터의 속도로 이동
    public float _attackDistance { get; set; } // 공격 거리 (적과의 거리)
    protected float _attackRadius { get; set; } // 공격 범위

    #region 모션들
    protected static int _idleState { get; set; }
    protected static int _standState { get; set; }
    protected static int _walkState { get; set; }
    protected static int _runState { get; set; }
    protected static int _attackState1 { get; set; }
    protected static int _attackState2 { get; set; }
    protected static int _waitState { get; set; }
    protected static int _skillState1 { get; set; }
    protected static int _skillWaitState1 { get; set; }
    protected static int _skillState2 { get; set; }
    protected static int _skillState3 { get; set; }
    protected static int _gethitState { get; set; } 
    protected static int _deadState1 { get; set; }
    protected static int _deadState2 { get; set; }
    protected float _cooltime { get; set; }
    protected float _originCooltime { get; set; }
    protected float _skillCooltime1 { get; set; }
    protected float _originSkillCooltime1 { get; set; }
    protected float _skillCooltime2 { get; set; }
    protected float _originSkillCooltime2 { get; set; }
    protected float _skillCooltime3 { get; set; }
    protected float _originSkillCooltime3 { get; set; }
    #endregion

    protected List<GameObject> _players = new List<GameObject>(); // 플레이어들의 GameObject를 담는 리스트
    protected List<float> _distances = new List<float>(); // 플레이어와의 거리 정보를 담는 리스트
    #endregion

    #region Start
    void Start()
    {
        InitStat();
    }

    protected virtual void InitStat()
    {

    }
    #endregion

    #region 이동 관련
    // 이동함수
    protected virtual void MoveState()
    {
        TurnToDestination();
        MoveToDestination();
    }

    // 목표 방향으로 이동하는 함수
    protected virtual void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            _player.transform.position, _moveSpeed * Time.deltaTime);
    }

    // 목표 방향으로 회전하는 함수
    protected virtual void TurnToDestination()
    {
        Quaternion lookRotation =
            Quaternion.LookRotation(_player.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            lookRotation, Time.deltaTime * _rotAnglePerSecond);
    }

    // 공격 시, turn하지 않도록 bool값을 보정
    protected void OnOffLookAtPlayer()
    {
        _lookAtPlayer |= _lookAtPlayer;
    }
    #endregion

    #region 탐색
    // 게임 씬 내의 플레이어 캐릭터 찾는 함수
    // 플레이어 이름이 Player1, Player2.. 이런식으로 뒤에 숫자가 붙어야함
    protected List<GameObject> DetectPlayer(List<GameObject> players)
    {
        players.Clear();
        GameObject temp = null;
        for (int i = 1; i < 5; i++)
        {
            if ((temp = GameObject.Find("Player" + i.ToString())) != null)
            {
                players.Add(temp);
            }
        }
        return players;
    }

    // 플레이어와의 거리를 재는 함수
    protected List<float> CalculateDistance(List<GameObject> players)
    {
        List<float> distances = new List<float>();
        for (int i = 0; i < players.Count; i++)
        {
            distances.Add(Vector3.Distance(
                players[i].transform.position, transform.position));
        }
        return distances;
    }

    // 시야각에 있는지 체크
    protected bool IsTargetInSight(float SightAngle, Transform Target)
    {
        //타겟의 방향 
        Vector3 targetDir = (Target.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, targetDir);

        //내적을 이용한 각 계산하기
        // thetha = cos^-1( a dot b / |a||b|)
        float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (theta <= SightAngle) return true;
        else return false;
    }

    // 타겟이 공격 거리에 들어왔는지 확인하는 함수
    protected bool IsInAttackDistance(float distance, Transform Target)
    {
        if (Vector3.Distance(transform.position, Target.position) <= distance) return true;
        else return false;
    }

    // 플레이어와 거리을 재는 함수
    // 이 함수의 경우 Sort를 하여 가장 가까운 플레이어의 _player, _playerpara 정보를 가져온다.
    // return하는 거리의 값도 마찬가지로 가장 가까운 플레이어와의 거리를 return한다.
    protected float GetDistanceFromPlayer(List<float> distances)
    {
        for (int i = 0; i < distances.Count - 1; i++)
        {
            int j, tmp = i;
            float tempDis;
            GameObject tempGameObj;

            for (j = i + 1; j < distances.Count; j++)
            {
                if (distances[tmp] >= distances[j] + 4f)
                {
                    tmp = j;
                }
            }

            tempDis = distances[tmp];
            distances[tmp] = distances[i];
            distances[i] = tempDis;

            tempGameObj = _players[tmp];
            _players[tmp] = _players[i];
            _players[i] = tempGameObj;
        }
        _player = _players[0];
        _playerPara = _players[0].GetComponent<CPlayerPara>();
        return distances[0];
    }

    // chase의 경우를 제외하고 나머지 행동들에서 플레이어에게 turn을 하는가를 체크함.
    // _lookAtPlayer는 UpdateState들의 state들에서 OnOff처리를 해야한다.
    protected void IsLookPlayer()
    {
        if (_lookAtPlayer) transform.LookAt(_player.transform.position);
    }
    #endregion

    #region 이벤트
    protected virtual void CallDeadEvent()
    {
        _anim.SetBool("Dead", true);
        this.tag = "Untagged";
        Invoke("RemoveMe", 3f);
    }
    protected virtual void RemoveMe()
    {
        _myRespawn.GetComponent<CRespawn>().RemoveMonster(_spawnID);
        _anim.SetBool("Dead", false);
    }
    #endregion

    #region 공격력 전달함수
    // 애니메이션에 추가되는 함수로써, _attackRadius, _attackDistance의 값을 충족하면 데미지를 주게한다.
    protected void AttackCheck()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (IsTargetInSight(_attackRadius, _players[i].transform) && IsInAttackDistance(_attackDistance, _players[i].transform))
            {
                _playerPara = _players[i].GetComponent<CPlayerPara>();
                AttackCalculate();
            }
        }
    }

    // 대폭적인 추가 필요
    protected void AttackCalculate()
    {
        _playerPara.SetEnemyAttack(_myPara.GetRandomAttack(_playerPara._eType, _myPara._eType));
    }
    #endregion

    // state의 업데이트는 하위 클래스인 몬스터들마다 세분화되어 관리되므로 Virtual로 남겨두었다.
    protected virtual void UpdateState()
    {

    }

    protected virtual void Update()
    {
        _players = DetectPlayer(_players);
        _currentBaseState = _anim.GetCurrentAnimatorStateInfo(0);
        _distances = CalculateDistance(_players);
        _anim.SetInteger("PlayerCount", _players.Count);
        // 다인큐용
        if (_players.Count > 1)
        {
            _anim.SetFloat("DistanceFromPlayer", GetDistanceFromPlayer(_distances));
            IsLookPlayer();
            UpdateState();
        }
        // 솔플용
        else if (_players.Count == 1)
        {
            _player = _players[0];
            _playerPara = _players[0].GetComponent<CPlayerPara>();
            _anim.SetFloat("DistanceFromPlayer", _distances[0]);
            IsLookPlayer();
            UpdateState();
        }
        else if (_players.Count == 0)
        {
            _anim.SetFloat("DistanceFromPlayer", 999f);
            _actionStart = false;
        }
    }
}