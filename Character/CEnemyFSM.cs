using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemyFSM : MonoBehaviour
{
    protected Animator _anim;
    protected AnimatorStateInfo _currentBaseState;     // 기본 레이어에 사용되는 애니메이터의 현재 상테에 대한 참조

    protected GameObject _player;
    protected CEnemyPara _myPara;
    protected CPlayerPara _playerPara;
    protected GameObject _myRespawn;

    protected bool _lookAtPlayer;
    protected bool _lookFlag;
    protected bool _coolDown;
    protected bool _skill1, _skill2, _skill3;
    protected bool _skillCoolDown1, _skillCoolDown2, _skillCoolDown3;
    protected bool _anotherAction;
    protected bool _actionStart;

    public int _spawnID { get; set; }
    protected float _rotAnglePerSecond = 360f; //1초에 플레이어의 방향을 360도 회전
    public float _moveSpeed { get; set; } //초당 2미터의 속도로 이동
    public float _attackDelay { get; set; } // 공격을 한번 하고 다시 공격할 때까지의 지연
    public float _attackTimer { get; set; } //공격을 하고 난 뒤에 경과되는 시간을 계산하기 위한 변수
    public float _attackDistance { get; set; } // 공격 거리 (적과의 거리)

    public static int _idleState { get; set; }
    public static int _standState { get; set; }
    public static int _walkState { get; set; }
    public static int _runState { get; set; }
    public static int _attackState1 { get; set; }
    public static int _attackState2 { get; set; }
    public static int _waitState { get; set; }
    public static int _skillState1 { get; set; }
    public static int _skillWaitState1 { get; set; }
    public static int _skillState2 { get; set; }
    public static int _skillState3 { get; set; }
    public static int _gethitState { get; set; } 
    public static int _deadState1 { get; set; }
    public static int _deadState2 { get; set; }
    protected float _cooltime { get; set; }
    protected float _originCooltime { get; set; }
    protected float _skillCooltime1 { get; set; }
    protected float _originSkillCooltime1 { get; set; }
    protected float _skillCooltime2 { get; set; }
    protected float _originSkillCooltime2 { get; set; }
    protected float _skillCooltime3 { get; set; }
    protected float _originSkillCooltime3 { get; set; }

    public int _idleBattleState { get; set; }
    public int _attackState { get; set; }
    public int _skillState { get; set; }

    protected List<GameObject> _players = new List<GameObject>();
    protected List<float> _distances = new List<float>();

    void Start()
    {
        InitStat();
    }

    protected virtual void InitStat()
    {

    }
    
    protected void OnOffLookAtPlayer()
    {
        _lookAtPlayer |= _lookAtPlayer;
    }

    protected virtual void MoveState()
    {
        TurnToDestination();
        MoveToDestination();
    }

    protected virtual void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            _player.transform.position, _moveSpeed * Time.deltaTime);
    }

    protected virtual void TurnToDestination()
    {
        Quaternion lookRotation =
            Quaternion.LookRotation(_player.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            lookRotation, Time.deltaTime * _rotAnglePerSecond);
    }

    public List<GameObject> DetectPlayer(List<GameObject> players)
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

    public List<float> CalculateDistance(List<GameObject> players)
    {
        List<float> distances = new List<float>();
        for (int i = 0; i < players.Count; i++)
        {
            distances.Add(Vector3.Distance(
                players[i].transform.position, transform.position));
        }
        //_distances.Sort();
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

    protected bool IsInAttackDistance(float distance, Transform Target)
    {
        float result = Vector3.Distance(transform.position, Target.position);
        if (result <= distance) return true;
        else return false;
    }

    //플레이어와 거리을 재는 함수
    protected float GetDistanceFromPlayer(List<float> distances)
    {
        for (int i = 0; i < distances.Count - 1; i++)
        {
            int j, tmp = i;
            float tempDis;
            GameObject nullGameObj;

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

            nullGameObj = _players[tmp];
            _players[tmp] = _players[i];
            _players[i] = nullGameObj;
        }
        _player = _players[0];
        _playerPara = _players[0].GetComponent<CPlayerPara>();
        return distances[0];
    }
    
    protected virtual void UpdateState()
    {

    }

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
    
    protected void AttackCalculate()
    {
        _playerPara.SetEnemyAttack(_myPara.GetRandomAttack(_playerPara._eType, _myPara._eType));
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