using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using UnityEngine;

public class CPlantFSM : MonoBehaviour
{
    protected CEnemyPara _myPara;
    protected Animator _anim;
    protected AnimatorStateInfo _currentBaseState;      // 기본 레이어에 사용되는 애니메이터의 현재 상테에 대한 참조

    bool IsHost = true;

    public enum
        CurrentState
    {
        idle, targeting, attack, skill1, skill2, skill3, hit, dead
    };
    protected CurrentState curState = CurrentState.idle;

    // 어떤 행동 뒤에 Idle 패턴으로 돌아가기 위한 변수
    protected bool _idle = false;

    [Header("기본 속성")]
    /// 탐색에 필요한 속성
    [Tooltip("회전 속도 기본 값 120")] [Range(60f, 360f)] public float _angularSpeed = 120f;
    [Tooltip("탐색 사정거리 ")] public float _targetDist = 20f;
    protected GameObject _player;
    protected float _nearestPlayer;

    /***************************/

    /* 공격에 필요한 속성 */
    [HideInInspector] public bool _isAttack = false;
    [Tooltip(" 공격 사정거리 ")] public float _attackDist = 5f;
    [Tooltip(" 기본 공격 쿨타임 ")] public float _cooltime;
    [Tooltip("기본 공격 범위")] [SerializeField] [Range(10f, 360f)] float _attackRadius = 10f;
    // 애니메이션 배속
    protected float _attackMultiply = 1f;
    protected float _originCooltime;
    [SerializeField] protected float _basicAttackMultiply = 1f;
    // 조우
    protected bool _actionStart = false;

    // 몬스터 기본 공격 Trail
    protected CMonsterAttackChecker AttackTrail1, AttackTrail2;

    // 스킬 개수 및 스킬 쿨타임 설정
    [System.Serializable]
    private class SkillProperty
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
    [SerializeField] private List<SkillProperty> _skillList = new List<SkillProperty>();
    [Tooltip("스킬 시작 위치")] [SerializeField] Transform _shootingPoint;
    // AI 정지 여부를 위해 존재하는 변수.
    protected bool _IsActiveCo = false;
    protected Coroutine Check1, Check2;

    // 히트 여부
    [HideInInspector]
    public bool _isHit = false;
    // 사망 여부
    [HideInInspector]
    public bool _isDead = false;

    private class TargetPlayerInfo
    {
        public GameObject pObject;
        public Vector3 pPosition;
        public float pDist;

        public TargetPlayerInfo(GameObject Object, Vector3 position, float Distance)
        {
            this.pObject = Object;
            this.pPosition = position;
            this.pDist = Distance;
        }
    }

    //CMonsterSkillDispenser _mySkill;
    protected void Start()
    {
        _myPara = GetComponent<CEnemyPara>();
        _anim = GetComponent<Animator>();
        if (IsHost)
        {
            _player = GameObject.Find("Player1");
            StartCoroutine(SelectPlayerforChase());
        }
        OnCoroutine();
        SetSkillProperty();
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
    protected virtual void AttackDisabledCollider()
    {
        if (AttackTrail1 != null && AttackTrail1._attackedPlayer.Count > 0)
            AttackTrail1.DiscardList();
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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * _angularSpeed);
    }

    #region Coroutine
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
            List<GameObject> player = CPlayerManager.instance.GetPlayerObjects();
            List<TargetPlayerInfo> chaseList = new List<TargetPlayerInfo>();

            for (int i = 0; i < CPlayerManager.instance.GetPlayerCount(); i++)
            {
                chaseList.Add(new TargetPlayerInfo(player[i], player[i].transform.position
                    , Vector3.Distance(player[i].transform.position, transform.position)));
            }

            List<TargetPlayerInfo> SortedList = chaseList.OrderBy(x => x.pDist).ToList();

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
                curState = CurrentState.idle;
            }
            else if (_nearestPlayer <= _attackDist
                && _cooltime < 0f
                && IsTargetInSight(_attackRadius, _player.transform))
            {
                curState = CurrentState.attack;
            }
            else if (_nearestPlayer <= _targetDist)
            {
                curState = CurrentState.targeting;
            }
            else
            {
                curState = CurrentState.idle;
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
                case CurrentState.idle:
                    break;
                case CurrentState.targeting:
                    if (!_actionStart) _actionStart = true;
                    TurnToDestination(this.transform.position, _player.transform.position);
                    break;
                case CurrentState.attack:
                    _anim.SetBool("Attack", true);
                    break;
            }
            yield return null;
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
            //Check1 = StartCoroutine(CheckState());
            //Check2 = StartCoroutine(CheckStateForAction());
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

    protected virtual IEnumerator BoolClicker(int Number)
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
                _isHit = true;
                yield return new WaitForSeconds(.2f);
                _isHit = false;
                break;
            default:
                break;
        }
    }
    #endregion

    #region State 
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

    protected virtual void FixedUpdate()
    {
        _currentBaseState = _anim.GetCurrentAnimatorStateInfo(0);
        if (_cooltime >= 0f) _cooltime -= Time.deltaTime;

        SetAnimatorBoolValue();
        DecreaseCoolTime();

        for (int i = 0; i < _skillList.Count; i++)
        {
            if (_skillList[i].skillCoolTime <= 0f && _IsActiveCo && _nearestPlayer <= _targetDist)
            {
                OffCoroutine();
                StartCoroutine(BoolClicker(2));
                SetOriginSkillCoolTime(i, 2f);
                StartCoroutine(ReleaseStiffnessAfterSkill(4f));
            }
        }

        if (_currentBaseState.fullPathHash == CMonsterManager._idleState
            && !_IsActiveCo && !CMonsterManager.instance._IsOrder && _idle)
        {
            OnCoroutine();
        }
    }

    private void SetOriginSkillCoolTime(int number, float seconds)
    {
        _skillList[number].skillCoolTime = _skillList[number].originSkillCoolTime + seconds;
        _cooltime = _originCooltime;
    }

    void DecreaseCoolTime()
    {
        if (_actionStart)
        {
            for (int i = 0; i < _skillList.Count; i++)
            {
                _skillList[i].skillCoolTime -= Time.deltaTime;
            }
        }
    }

    void SetAnimatorBoolValue()
    {
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

    #region Animator function
    //기본 공격 쿨타임 재갱신
    protected void ResetCooltime() { _cooltime = _originCooltime; }
    /// 기본 공격 배율 조절
    void SetAttackMultiply() { _attackMultiply = _basicAttackMultiply; }
    void ReleaseAttackMultiply() { _attackMultiply = 1f; }

    void SetSkillMultiply(int i) { _skillList[i].skillMultiply = _skillList[i].basicSkillMultiply; }

    void ReleaseSkill(int i) { _skillList[i].skillMultiply = 1f; }

    void Skill1()
    {
        Vector3 Direction = Vector3.Normalize(transform.position - _player.transform.position);
        CParticleProjectile pp = _skillList[0].skill.GetComponent<CParticleProjectile>();
        pp._direction = Direction;
        CMonsterAttackChecker ma = _skillList[0].skill.GetComponent<CMonsterAttackChecker>();
        ma._creator = gameObject;
        ma._isPenetrated = false;

        GameObject temp = Instantiate(_skillList[0].skill, _shootingPoint.position, Quaternion.identity);
    }

    void Skill2()
    {
        Vector3 Direction = Vector3.Normalize(transform.position - _player.transform.position);
        CParticleProjectile tmp = _skillList[1].skill.GetComponent<CParticleProjectile>();
        tmp._direction = Direction;
        CMonsterAttackChecker ma = _skillList[1].skill.GetComponent<CMonsterAttackChecker>();
        ma._creator = gameObject;
        ma._isPenetrated = true;

        GameObject temp = Instantiate(_skillList[0].skill, _shootingPoint.position, Quaternion.identity);
    }

    #endregion
}