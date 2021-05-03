using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DebugingDelegate();

public class CEnemyFSM : MonoBehaviour
{
    // 상태들을 클래스화 함. TypeSafe Enum Pattern
    public class EState
    {
        public static readonly EState Idle = new EState("Idle");
        public static readonly EState Chase = new EState("Chase");
        public static readonly EState Move = new EState("Move");
        public static readonly EState Attack = new EState("Attack");
        public static readonly EState AttackWait = new EState("AttackWait");
        public static readonly EState Skill1 = new EState("Skill1");
        public static readonly EState SkillWait1 = new EState("SkillWait1");
        public static readonly EState Skill2 = new EState("Skill2");
        public static readonly EState SkillWait2 = new EState("SkillWait2");
        public static readonly EState Skill3 = new EState("Skill3");
        public static readonly EState SkillWait3 = new EState("SkillWait3");
        public static readonly EState Dead = new EState("Dead");

        public override string ToString()
        {
            return Value;
        }

        protected EState(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }

    public class DebugingMyState
    {
        private EState _stateName;

        public DebugingMyState(EState name)
        {
            this._stateName = name;
        }

        public void DisplayToConsole()
        {
            Debug.Log(this._stateName);
        }
    }
    public enum StateType
    {
        idle, stand, walk, run, attackState1, attackState2, wait
        , skill1, skillWait1, skill2, skillWait2, skillState3, skillWait3
        , gethit, dead1, dead2
    }
    [SerializeField]
    public struct StateInfo
    {
        public StateType type;
    }
    #region Properties
    protected Animator _anim;                           // 몬스터가 가지고있는 애니메이터
    protected AnimatorStateInfo _currentBaseState;      // 기본 레이어에 사용되는 애니메이터의 현재 상테에 대한 참조
    protected GameObject _player;                       // 가장 가까운 플레이어의 정보를 저장
    protected CPlayerPara _playerPara;                  // 가장 가가움 플레이어의 Para
    protected CEnemyPara _myPara;                       // 보스 몬스터를 제외한 Para
    //protected CBossPara _myBossPara;                    // 보스 몬스터 전용 Para
    protected bool _lookAtPlayer; // 어떤 행동시에, 플레이어를 바라보게 하는 TurnToDestination을 OFF하게 하려는 목적을 가진 bool
    protected bool _coolDown;     // 평타 쿨타임 판단
    protected bool _skill1, _skill2, _skill3; // 스킬 행동의 중간단계의 동작을 관리하기 위한 bool

    protected bool _skillCoolDown1; // 스킬 쿨타임 판단
    protected bool _skillCoolDown2; // 스킬 쿨타임 판단
    protected bool _skillCoolDown3; // 스킬 쿨타임 판단

    [SerializeField]
    public struct SetSkillCoolTime
    {
        [Tooltip("Skill 쿨타임 최소")] public float skillCoolDownUp; // 스킬 쿨타임 판단
        [Tooltip("Skill 쿨타임 최대")] public float skillCoolDownDown; // 스킬 쿨타임 판단
    }

    [Tooltip("몬스터 기본 공격 간격")]
    [SerializeField] protected static float _cooltime;
    protected float _originCooltime;

    protected bool _anotherAction; // 공격 사용 후, 플레이어의 거리가 멀어지면 chase 아니면, 그대로 공격하게 하는걸 판단
    protected bool _actionStart; // 플레이어와 조우 전에, 스킬의 쿨타임이 미리 도는 것을 방지하려는 것
    protected bool _getHit;      // 히트 애니메이션 관련 bool
    protected float _rotAnglePerSecond = 360f; //1초에 플레이어의 방향을 360도 회전
    public float _moveSpeed { get; set; } //초당 ~미터의 속도로 이동
    public float _attackDistance { get; set; } // 공격 거리 (적과의 거리)
    protected float _attackAngle { get; set; } // 공격 범위

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
    protected static int _skillWaitState2 { get; set; }
    protected static int _skillState3 { get; set; }
    protected static int _skillWaitState3 { get; set; }
    protected static int _gethitState { get; set; }
    protected static int _deadState { get; set; }

    #endregion

    protected EState _myState, _myOldState = EState.Idle;
    protected List<GameObject> _players = new List<GameObject>(); // 플레이어들의 GameObject를 담는 리스트
    protected List<float> _distances = new List<float>(); // 플레이어와의 거리 정보를 담는 리스트
    [Tooltip("스킬 개수 및 스킬 쿨타임 설정")]
    [SerializeField] protected List<SetSkillCoolTime> SetSkillCoolTimeList = new List<SetSkillCoolTime>();
    [HideInInspector] protected List<float> _skillCoolTime = new List<float>();
    [HideInInspector] protected List<float> _originSkillCoolTime = new List<float>();

    #endregion

    protected virtual void Start()
    {
        InitStat();
    }

    protected virtual void InitStat()
    {
        if (gameObject.tag == "Boss")
        {
            //_myBossPara = GetComponent<CBossPara>();
        }
        else
        {
            _myPara = GetComponent<CEnemyPara>();
        }

        _anim = GetComponent<Animator>();
        _myPara.deadEvent.AddListener(CallDeadEvent);
        _idleState = Animator.StringToHash("Base Layer.Idle");
        _standState = Animator.StringToHash("Base Layer.MovingSub.Stand");
        _walkState = Animator.StringToHash("Base Layer.MovingSub.Walk");
        _runState = Animator.StringToHash("Base Layer.MovingSub.Run");
        _attackState1 = Animator.StringToHash("Base Layer.AttackSub.Attack1");
        _waitState = Animator.StringToHash("Base Layer.AttackSub.AttackWait");
        _attackState2 = Animator.StringToHash("Base Layer.AttackSub.Attack2");
        _skillState1 = Animator.StringToHash("Base Layer.AnySub.Skill1");
        _skillState2 = Animator.StringToHash("Base Layer.AnySub.Skill2");
        _skillState2 = Animator.StringToHash("Base Layer.AnySub.Skill3");
        _skillWaitState1 = Animator.StringToHash("Base Layer.AnySub.SkillWait1");
        _skillWaitState1 = Animator.StringToHash("Base Layer.AnySub.SkillWait2");
        _skillWaitState1 = Animator.StringToHash("Base Layer.AnySub.SkillWait3");
        _gethitState = Animator.StringToHash("Base Layer.AnySub.GetHit");
        _deadState = Animator.StringToHash("Base Layer.AnySub.Dead");

    }

    // 몬스터 기본 공격, 스킬 쿨타임을 인스펙터 상에서 조절 가능하게 함.
    // 랜덤하게 조절
    protected void SetCoolTime()
    {
        _originCooltime = _cooltime != 0f ? _cooltime : .5f;

        for (int i = 0; i < SetSkillCoolTimeList.Count; i++)
        {
            _originSkillCoolTime.Add(UnityEngine.Random.Range(SetSkillCoolTimeList[i].skillCoolDownDown,
                SetSkillCoolTimeList[i].skillCoolDownUp));
            _skillCoolTime.Add(_originSkillCoolTime[i]);
        }
    }

    #region State
    // 이동함수
    protected virtual void MoveState()
    {
        if (_anotherAction)
        {
            _anotherAction = false;
        }
        if (!_lookAtPlayer)
        {
            _lookAtPlayer = true;
        }
        MoveToDestination(transform.position - new Vector3(0f, transform.position.y, 0f), _player.transform.position);
    }

    // 목표 방향으로 이동하는 함수
    protected virtual void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position - new Vector3(0f, transform.position.y, 0f),
            _player.transform.position, _moveSpeed * Time.deltaTime);
    }

    protected virtual void MoveToDestination(Vector3 MyPosition, Vector3 TargetPostion)
    {
        transform.position = Vector3.MoveTowards(MyPosition, _player.transform.position, _moveSpeed * Time.deltaTime);
    }

    protected virtual void MoveToDestination(Vector3 MyPosition, Vector3 TargetPostion, float Speed)
    {
        transform.position = Vector3.MoveTowards(MyPosition, _player.transform.position, Speed * Time.deltaTime);
    }

    // 목표 방향으로 회전하는 함수
    protected virtual void TurnToDestination()
    {
        Quaternion lookRotation =
            Quaternion.LookRotation((_player.transform.position - new Vector3(0f, _player.transform.position.y, 0f))
            - (transform.position - new Vector3(0f, transform.position.y, 0f)));
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            lookRotation, Time.deltaTime * _rotAnglePerSecond);
    }

    protected virtual void TurnToDestination(Vector3 MyPosition, Vector3 TargetPosition)
    {
        Quaternion lookRotation = Quaternion.LookRotation(TargetPosition - MyPosition);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * _rotAnglePerSecond);
    }

    // chase의 경우를 제외하고 나머지 행동들에서 플레이어에게 turn을 하는가를 체크함.
    // _lookAtPlayer는 UpdateState들의 state들에서 OnOff처리를 해야한다.
    protected void IsLookPlayer()
    {
        if (_lookAtPlayer)
        {
            TurnToDestination();
        }
    }

    protected virtual void IdleState()
    {
        if (_myState != EState.Idle)
        {
            _myState = EState.Idle;
        }
        if (!_lookAtPlayer)
        {
            _lookAtPlayer = true;
        }
    }

    protected virtual void ChaseState()
    {
        if (_myState != EState.Chase)
        {
            _myState = EState.Chase;
        }

        if (!_actionStart)
        {
            _actionStart = true;
        }

        if (!_lookAtPlayer)
        {
            _lookAtPlayer = true;
        }

        if (_currentBaseState.fullPathHash != _deadState)
        {
            MoveState();
        }
    }

    protected virtual void AttackState1()
    {
        _cooltime = _originCooltime;
        if (_myState != EState.Attack)
        {
            _myState = EState.Attack;
        }
        if (_lookAtPlayer)
        {
            _lookAtPlayer = false;
        }
        if (!_coolDown)
        {
            _coolDown = true;
        }
    }

    protected virtual void AttackWaitState()
    {
        if (_myState != EState.AttackWait)
        {
            _myState = EState.AttackWait;
        }
        _cooltime -= Time.deltaTime;
        if (!_lookAtPlayer)
        {
            _lookAtPlayer = true;
        }
        if (_cooltime < 0)
        {
            _coolDown = false;
            _anotherAction = false;
            _cooltime = _originCooltime;
        }
        else if (GetDistanceFromPlayer(_distances) > _attackDistance)
        {
            _coolDown = false;
            _anotherAction = true;
            _cooltime = _originCooltime;
        }
    }

    protected virtual void DeadState()
    {
        if (_myState != EState.Dead)
        {
            _myState = EState.Dead;
        }
        if (_lookAtPlayer)
        {
            _lookAtPlayer = false;
        }
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

    #endregion

    #region 이벤트

    protected void CallDeadEvent()
    {
        if (_myPara != null)
        {
            Debug.Log(_myPara._name + " " + _myPara._spawnID + " is dead!");
        }
        else
        {
            //Debug.Log(_myBossPara._name + " is dead!");
        }

        Debug.Log("SetBool true");
        _anim.SetBool("Dead", true);
        gameObject.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("DeadBody");
        Invoke("RemoveMe", .5f);
    }
    protected virtual void RemoveMe()
    {
        Debug.Log("SetBool false");
        _anim.SetBool("Dead", false);
    }

    protected virtual void CallHitEvent()
    {
        _getHit = true;
    }

    #endregion

    #region 공격력 전달함수
    // 애니메이션에 추가되는 함수로써, _attackAngle, _attackDistance의 값을 충족하면 데미지를 주게한다.
    protected void AttackCheck()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (IsTargetInSight(_attackAngle, _players[i].transform) && IsInAttackDistance(_attackDistance, _players[i].transform))
            {
                _playerPara = _players[i].GetComponent<CPlayerPara>();
                //Debug.Log(_players[i].name);
                AttackCalculate(_playerPara);
            }
        }
    }

    // 기본 공격에 관한 체크
    protected void AttackCalculate(CPlayerPara c)
    {
        if (_myPara != null)
        {
            c.DamegedRegardDefence(_myPara.RandomAttackDamage());
        }
        else
        {
            //c.DamegedRegardDefence(_myBossPara.RandomAttackDamage());
        }
    }

    protected virtual void AttackDisabledCollider()
    {
        SendMessage("DiscardList");
    }
    #endregion

    // state의 업데이트는 하위 클래스인 몬스터들마다 세분화되어 관리되므로 Virtual로 남겨두었다.
    protected virtual void UpdateState()
    {

    }


    protected virtual void DebugState()
    {
        if (_myOldState != _myState)
        {
            DebugingMyState oldState = new DebugingMyState(_myState);
            void displayLog() => oldState.DisplayToConsole();
            displayLog();
            _myOldState = _myState;
        }
        else if (_myOldState == _myState)
        {
            return;
        }
    }

    protected virtual void Update()
    {
        //DebugState();
        _players = DetectPlayer(_players);
        _currentBaseState = _anim.GetCurrentAnimatorStateInfo(0);
        _distances = CalculateDistance(_players);
        _anim.SetInteger("PlayerCount", _players.Count);

        if (_myPara != null)
        {
            _anim.SetInteger("Hp", _myPara.CurrentHp);
        }

        if (_currentBaseState.fullPathHash != _deadState)
        {
            IsLookPlayer();
        }
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
            _actionStart = false;
        }
    }
}