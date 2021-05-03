using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CEnemyNavFSM : MonoBehaviour
{
    public bool IsHost = true;
    public enum 
        CurrentState { idleEnter, idle, idleExit
            , traceEnter, trace, traceExit
            , attackEnter, attack, attackExit
            , skill1Enter, skill1, skill1Exit
            , skill2Enter, skill2, skill2Exit
            , skill3Enter, skill3, skill3Exit
            , hit ,dead };
    protected CurrentState curState = CurrentState.idle;

    protected NavMeshAgent nvAgent;
    protected CEnemyPara _myPara;
    protected Animator _anim;
    // 기본 레이어에 사용되는 애니메이터의 현재 상테에 대한 참조
    protected AnimatorStateInfo _currentBaseState;      

    // 어떤 행동 뒤에 Idle 패턴으로 돌아가기 위한 변수
    protected bool _idle = false;

    [Header("기본 속성")]
    /// 추적에 필요한 속성
    [Tooltip(" 추적 사정거리 " )]
    public float _traceDist;
    protected GameObject _player;
    protected float _nearestPlayer;
    [HideInInspector]public bool _isMove = false;
    /***************************/
    /* 공격에 필요한 속성 */
    [HideInInspector]public bool _isAttack = false;
    [Tooltip(" 공격 사정거리 " )] public float _attackDist;
    [Tooltip(" 기본 공격 쿨타임 ")] public float _cooltime;
    [Tooltip("기본 공격 범위")][SerializeField] [Range(10f, 360f)] float _attackRadius = 60f;
    // 애니메이션 배속
    protected float _attackMultiply = 1f;
    [Tooltip("기본 공격 시, 멈칫 하게 하는 배수")]
    public float _basicAttackMultiply = .3f;
    protected float _originCooltime;
    // 조우
    protected bool _actionStart = false;

    // 몬스터 기본 공격 Trail
    protected CMonsterAttackChecker AttackTrail1, AttackTrail2;

    // 스킬 개수 및 스킬 쿨타임 설정
    [System.Serializable]
    protected class SkillProperty
    {
        // 스킬 오브젝트
        public GameObject skill;
        // 애니메이터에서 사용할 스킬 bool 값
        [HideInInspector] public bool IsSkill;
        // 애니메이터에서 사용할 스킬 배율

        [Header("애니메이션 재생 배수")]
        public float basicSkillMultiply;
        [HideInInspector] public float skillMultiply;
        // 스킬 쿨타임

        [Header("스킬 쿨타임 범위")]
        [HideInInspector] public float skillCoolTime;
        [HideInInspector] public float originSkillCoolTime;
        public float[] SkillCoolTimeRange = new float[2];
    }

    [Header("몬스터 스킬 속성 설정")]
    [SerializeField] protected List<SkillProperty> _skillList = new List<SkillProperty>();

    // 히트 여부
    [HideInInspector]public bool _isHit = false;
    // 사망 여부
    [HideInInspector]public bool _isDead = false;

    // AI 정지 여부를 위해 존재하는 변수.
    protected bool _IsActiveCo = false;
    protected Coroutine Check1, Check2;

    private class ChasePlayerInfo
    {
        public GameObject pObject;
        public Vector3 pPosition;
        public float pDist;

        public ChasePlayerInfo(GameObject Object, Vector3 position, float Distance)
        {
            this.pObject = Object;
            this.pPosition = position;
            this.pDist = Distance;
        }
    }
    string tmp;
    // Use this for initialization
    protected virtual void Start()
    {
        nvAgent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _anim.SetFloat("AttackMultiply", _attackMultiply);
        CMonsterManager.instance.AttackEvent.AddListener(AttackEvent);
        CMonsterManager.instance.HitEvent.AddListener(HitEvent);
        CMonsterManager.instance.SkillEvent1.AddListener(SkillEvent1);
        CMonsterManager.instance.SkillEvent2.AddListener(SkillEvent2);
        //CMonsterManager.instance._actionEvent.AddListener(OffCoroutine);

        // 호스트만 거리판별을 함.
        if (IsHost)
        {
            _player = GameObject.Find("Player1");
            StartCoroutine(SelectPlayerforChase());
        }
        else
        {
            //_hostMessage = new HostInfo(false);
        }
        OnCoroutine();
        SetSkillProperty();
        // 기본 값 2초
        _originCooltime = _cooltime != 0f ? _cooltime : 2f;
        _myPara.deadEvent.AddListener(CallDeadEvent);
    }

    void SetSkillProperty()
    {
        for (int i = 0; i < _skillList.Count; i++)
        {
            _skillList[i].originSkillCoolTime =
                UnityEngine.Random.Range(_skillList[i].SkillCoolTimeRange[0], _skillList[i].SkillCoolTimeRange[1]);
            _skillList[i].skillCoolTime =
                UnityEngine.Random.Range(_skillList[i].SkillCoolTimeRange[0], _skillList[i].SkillCoolTimeRange[1]);
            _skillList[i].skillMultiply = _skillList[i].basicSkillMultiply;
        }
    }
    protected bool IsTargetInSight(float SightAngle, Transform Target)
    {
        Vector3 targetDir = (Target.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, targetDir);

        float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (theta <= SightAngle) return true;
        else return false;
    }
    protected virtual void TurnToDestination(Vector3 MyPosition, Vector3 TargetPosition)
    {
        Quaternion lookRotation = Quaternion.LookRotation(TargetPosition - MyPosition);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * nvAgent.angularSpeed);
    }
    protected void SetOriginSkillCoolTime(int number, float seconds)
    {
        _skillList[number].skillCoolTime = _skillList[number].originSkillCoolTime + seconds;
        _cooltime = _originCooltime;
    }
    // 몇 초 뒤에 Idle로 가는 명령
    // 사용이유 : Idle 상태일 경우, OnCoroutine이 호출되는데
    // 이 때, 호출할 수 있는 Idle 과, 모션만 같은 State를 하나 더주고
    // OnCoroutine() 호출 타이밍을 조절하기 위해 작성
    protected IEnumerator ReleaseStiffnessAfterSkill(float seconds)
    {
        _idle = false;
        yield return new WaitForSeconds(seconds);
        _idle = true;
        yield return new WaitForSeconds(1f);
        _idle = false;
        yield return null;
    }
    protected IEnumerator SelectPlayerforChase()
    {
        while (!_isDead)
        {
            yield return new WaitForSeconds(.1f);
            List<GameObject> alivePlayers = CPlayerCommand.instance.AlivePlayers;
            List<ChasePlayerInfo> chaseList = new List<ChasePlayerInfo>();

            for (int i = 0; i < alivePlayers.Count; i++)
            {
                chaseList.Add(new ChasePlayerInfo(alivePlayers[i], alivePlayers[i].transform.position
                    , Vector3.Distance(alivePlayers[i].transform.position, transform.position)));
            }

            List<ChasePlayerInfo> SortedList = chaseList.OrderBy(x => x.pDist).ToList();

            _player = SortedList[0].pObject;
            _nearestPlayer = SortedList[0].pDist;
        }
    }
    protected IEnumerator SingleCheckState()
    {
        while (!_isDead)
        {
            yield return new WaitForSeconds(0.2f);

            if (_cooltime > 0f)
            {
                curState = CurrentState.idleEnter;
            }
            else if (_nearestPlayer <= _attackDist
                && _cooltime < 0f
                && IsTargetInSight(_attackRadius, _player.transform))
            {
                curState = CurrentState.attackEnter;
            }
            else if (_nearestPlayer <= _traceDist)
            {
                if (!_actionStart)
                {
                    _actionStart = true;
                    Debug.Log(_actionStart);
                }
                curState = CurrentState.traceEnter;
            }
            else
            {
                curState = CurrentState.idleEnter;
            }
        }
    }
    protected IEnumerator SingleCheckStateForAction()
    {
        while (!_isDead)
        {
            //Debug.Log(curState);
            switch (curState)
            {
                case CurrentState.idleEnter:
                    curState = CurrentState.idle;
                    break;
                case CurrentState.idle:
                    _idle = true;
                    StopNavAgent();
                    break;
                case CurrentState.traceEnter:
                    if (!_actionStart) _actionStart = true;
                    ResetNavAgent();
                    curState = CurrentState.trace;
                    break;
                case CurrentState.trace:
                    _isMove = true;
                    TurnToDestination(this.transform.position, _player.transform.position);
                    nvAgent.destination = _player.transform.position;
                    break;
                case CurrentState.attackEnter:
                    StopNavAgent();
                    curState = CurrentState.attack;
                    break;
                case CurrentState.attack:
                    StartCoroutine(BoolClicker(1));
                    break;
            }

            yield return null;
        }
    }

    protected IEnumerator CheckState()
    {
        while (!_isDead)
        {
            yield return new WaitForSeconds(0.2f);

            if (_nearestPlayer <= _attackDist)
            {
                curState = CurrentState.attackEnter;
            }
            else if (_nearestPlayer <= _traceDist)
            {
                curState = CurrentState.traceEnter;
            }
            else
            {
                curState = CurrentState.idleEnter;
            }
        }
    }
    protected IEnumerator CheckStateForAction()
    {
        while (!_isDead)
        {
            switch (curState)
            {
                case CurrentState.idleEnter:
                    if (IsHost)
                    {
                        CMonsterManager.instance.OrderAction
                            (_myPara._spawnID, CMonsterManager._idleState);
                        //_hostMessage.SetMessageCheck(true);
                        curState = CurrentState.idle;
                    }
                    else if (!IsHost)
                    {

                    }
                    break;
                case CurrentState.idle:
                    //if (_hostMessage.GetMessageCheck()) _hostMessage.SetMessageCheck(false);
                    _anim.SetBool("Move", false);
                    nvAgent.Stop();
                    break;

                case CurrentState.traceEnter:
                    if (IsHost)
                    {
                        CMonsterManager.instance.OrderAction
                            (_myPara._spawnID, CMonsterManager._traceState);
                        //_hostMessage.SetMessageCheck(true);
                    }
                    else
                    {
                        //if (_hostMessage.GetMessageCheck()) _hostMessage.SetMessageCheck(false);

                        curState = CurrentState.trace;
                    }
                    break;
                case CurrentState.trace:
                    _anim.SetBool("Move", true);
                    nvAgent.Resume();
                    break;
                case CurrentState.attack:
                    break;
            }

            yield return null;
        }
    }

    protected virtual void FixedUpdate()
    {
        SetAnimatorBoolValue();
        DecreaseCoolTime();
    }
    // Animator Bool 값 조절 만약 필요하다면 상속 클래스에서 override 해서 따로 쓰기
    protected virtual void SetAnimatorBoolValue()
    {
        _currentBaseState = _anim.GetCurrentAnimatorStateInfo(0);
        if (_cooltime >= 0f) _cooltime -= Time.deltaTime;
        _anim.SetBool("Move", _isMove);
        _anim.SetFloat("AttackMultiply", _attackMultiply);
        _anim.SetBool("Hit", _isHit);
        _anim.SetBool("Idle", _idle);
        _anim.SetBool("Attack", _isAttack);
        for (int i = 0; i < _skillList.Count; i++)
        {
            _anim.SetBool("Skill" + (i + 1).ToString(), _skillList[i].IsSkill);
            _anim.SetFloat("SkillMultiply" + (i + 1).ToString(), _skillList[i].skillMultiply);
        }
    }

    // 기본적인 스킬 호출 프로세스
    // Update구문에서 사용
    protected void SkillChecker(int number, float seconds)
    {
        if (_skillList[number].skillCoolTime <= 0f && _IsActiveCo 
            && IsTargetInSight(_attackRadius, _player.transform))
        {
            OffCoroutine();
            StopNavAgent();
            SetOriginSkillCoolTime(number, 4f);
            StartCoroutine(BoolClicker(number + 2));
            StartCoroutine(ReleaseStiffnessAfterSkill(seconds));
        }
    }
    protected void DecreaseCoolTime()
    {
        if (_actionStart && _IsActiveCo)
        {
            for (int i = 0; i < _skillList.Count; i++)
            {
                _skillList[i].skillCoolTime -= Time.deltaTime;
            }
        }
    }
    public void OnCoroutine()
    {
        Debug.Log("Start Coroutine");
        if (IsHost)
        {
            Check1 = StartCoroutine(SingleCheckState());
            Check2 = StartCoroutine(SingleCheckStateForAction());
        }
        else
        {
            Check1 = StartCoroutine(CheckState());
            Check2 = StartCoroutine(CheckStateForAction());
        }
        _IsActiveCo = true;
        _actionStart = true;
    }
    public void OffCoroutine()
    {
        Debug.Log("Stop Coroutine");
        StopCoroutine(Check1);
        StopCoroutine(Check2);
        _IsActiveCo = false;
        _actionStart = false;
    }
    // Navigator Agent 관리 미끄러지는 현상을 방지.
    protected void StopNavAgent()
    {
        _isMove = false;
        nvAgent.isStopped = true;
        nvAgent.updatePosition = false;
        nvAgent.updateRotation = false;
        nvAgent.velocity = Vector3.zero;
    }
    protected void ResetNavAgent()
    {
        nvAgent.ResetPath();
        nvAgent.isStopped = false;
        nvAgent.updatePosition = true;
        nvAgent.updateRotation = true;
    }
    protected IEnumerator BoolClicker(int Number)
    {
        Debug.Log("Start BoolClicker");
        switch (Number)
        {
            case 1:
                _isAttack = true;
                yield return new WaitForSeconds(.2f);
                _isAttack = false;
                break;
            case 2:
                _skillList[0].IsSkill = true;
                yield return new WaitForSeconds(.2f);
                _skillList[0].IsSkill = false;
                break;
            case 3:
                _skillList[1].IsSkill = true;
                yield return new WaitForSeconds(.2f);
                _skillList[1].IsSkill = false;
                break;
            case 4:
                _skillList[2].IsSkill = true;
                yield return new WaitForSeconds(.2f);
                _skillList[2].IsSkill = false;
                break;
            case 0:
                _isHit = true;
                yield return new WaitForSeconds(.2f);
                _isHit = false;
                break;
            default:
                break;
        }
        yield return null;
    }

    #region Para Event(Dead)
    protected void CallDeadEvent()
    {
        _isDead = true;
        if (_myPara != null)
        {
            Debug.Log(_myPara.name + " " + _myPara._spawnID + " is dead!");
        }

        Debug.Log("SetBool true");
        _anim.SetBool("Dead", true);
        gameObject.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("DeadBody");
        Invoke(nameof(RemoveMe), .5f);
    }
    protected virtual void RemoveMe()
    {
        Debug.Log("SetBool false");
        _anim.SetBool("Dead", false);
    }
    #endregion

    #region Monster Manager Call
    public void ReleaseAllAnimatorBools()
    {
        _isHit = false;
        _isAttack = false;
        _isHit = false;
        _isMove = false;
        _isDead = false;
        nvAgent.Stop();
    }
    public void AttackEvent() { StartCoroutine(BoolClicker(1)); }
    public void SkillEvent1() { StartCoroutine(BoolClicker(2)); }
    public void SkillEvent2() { StartCoroutine(BoolClicker(3)); }
    public void HitEvent() { StartCoroutine(BoolClicker(4)); }
    #endregion

    #region Animator Function
    protected virtual void AttackDisabledCollider()
    {
        if (AttackTrail1 != null && AttackTrail1._attackedPlayer.Count > 0)
            AttackTrail1.DiscardList();
        if (AttackTrail2 != null && AttackTrail2._attackedPlayer.Count > 0)
            AttackTrail2.DiscardList();
    }
    protected void ResetCooltime() { _cooltime = _originCooltime; }
    protected void SetAttackMultiply() { _attackMultiply = _basicAttackMultiply; }
    protected void ReleaseAttackMultiply() { _attackMultiply = 1f; }
    protected void SetSkillMultiply(int i) { _skillList[i].skillMultiply = _skillList[i].basicSkillMultiply; }
    protected void ReleaseSkill(int i) { _skillList[i].skillMultiply = 1f; }
    #endregion

}
