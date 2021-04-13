using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CEnemyNavFSM : MonoBehaviour
{
    //private class HostInfo
    //{
    //    bool sendMessage;
    //    public HostInfo(bool send)
    //    {
    //        sendMessage = send;
    //    }

    //    public void SetMessageCheck(bool check)
    //    {
    //        sendMessage = check;
    //    }

    //    public bool GetMessageCheck()
    //    {
    //        return sendMessage;
    //    }
    //}
    //HostInfo _hostMessage;
    public bool IsHost = true;

    public enum 
        CurrentState { idleEnter, idle, idleExit
            , traceEnter, trace, traceExit
            , attackEnter, attack, attackExit
            , skill1Enter, skill1, skill1Exit
            , skill2Enter, skill2, skill2Exit
            , skill3Enter, skill3, skill3Exit
            , dead };
    protected CurrentState curState = CurrentState.idle;

    protected NavMeshAgent nvAgent;
    protected CEnemyPara _myPara;
    protected Animator _anim;
    protected AnimatorStateInfo _currentBaseState;      // 기본 레이어에 사용되는 애니메이터의 현재 상테에 대한 참조

    /// 추적에 필요한 속성
    [Tooltip(" 추적 사정거리 " )]
    public float _traceDist;
    protected GameObject _player;
    protected float _nearestPlayer;
    /***************************/

    /* 공격에 필요한 속성 */
    [HideInInspector]
    public bool _isAttack = false;
    [Tooltip(" 공격 사정거리 " )]
    public float _attackDist;
    [Tooltip(" 기본 공격 쿨타임 ")]
    public float _cooltime;
    // 애니메이션 배속
    protected float _attackMultiply = 1f;
    [Tooltip("기본 공격 시, 멈칫 하게 하는 배수")]
    public float _basicAttackMultiply = .3f;
    protected float _originCooltime;
    // 조우
    protected bool _actionStart = false;

    // 몬스터 기본 공격 Trail
    protected CMonstermeleeChecker AttackTrail1, AttackTrail2;
    
    // 스킬 개수 및 스킬 쿨타임 설정
    protected List<float> _skillCoolTime = new List<float>();
    protected List<float> _originSkillCoolTime = new List<float>();

    protected float _skillMultiply = 1f;
    [HideInInspector]
    public bool _isSkill1 = false;
    protected float _skillMultiply2 = 1f;
    [HideInInspector]
    public bool _isSkill2 = false;
    [Tooltip("스킬1 공격 시, 멈칫 하게 하는 배수")]
    public float _basicSkillMultiply = .3f;
    [Tooltip("스킬2 공격 시, 멈칫 하게 하는 배수")]
    public float _basicSkillMultiply2 = .3f;

    // 히트 여부
    [HideInInspector]
    public bool _isHit = false;
    // 사망 여부
    [HideInInspector]
    public bool _isDead = false;


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

        // 기본 값 2초
        _originCooltime = _cooltime != 0f ? _cooltime : 2f;
        _myPara.deadEvent.AddListener(CallDeadEvent);

        Debug.Log(_player.name);
    }

    protected void SetCoolTime(float[] Skillcooltime)
    {
        _originSkillCoolTime.Add(UnityEngine.Random.Range(Skillcooltime[0], Skillcooltime[1]));
        _skillCoolTime.Add(UnityEngine.Random.Range(Skillcooltime[0], Skillcooltime[1]));
    }

    protected virtual void AttackDisabledCollider()
    {
        if (AttackTrail1 != null && AttackTrail1._attackedPlayer.Count > 0)
            AttackTrail1.DiscardList();
        if (AttackTrail2 != null && AttackTrail2._attackedPlayer.Count > 0)
            AttackTrail2.DiscardList();
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

    protected IEnumerator SelectPlayerforChase()
    {
        while (!_isDead)
        {
            yield return new WaitForSeconds(.1f);
            List<GameObject> player = CPlayerManager.instance.GetPlayerObjects();
            List<ChasePlayerInfo> chaseList = new List<ChasePlayerInfo>();

            for (int i = 0; i < CPlayerManager.instance.GetPlayerCount(); i++)
            {
                chaseList.Add(new ChasePlayerInfo(player[i], player[i].transform.position
                    , Vector3.Distance(player[i].transform.position, transform.position)));
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
                && IsTargetInSight(60f, _player.transform))
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
                    _anim.SetBool("Move", false);
                    _anim.SetBool("Attack", false);
                    curState = CurrentState.idle;
                    break;
                case CurrentState.idle:
                    nvAgent.Stop();
                    break;
                case CurrentState.traceEnter:
                    if (!_actionStart) _actionStart = true;
                    _anim.SetBool("Move", true);
                    curState = CurrentState.trace;
                    break;
                case CurrentState.trace:
                    TurnToDestination(this.transform.position, _player.transform.position);
                    nvAgent.destination = _player.transform.position;
                    nvAgent.Resume();
                    break;
                case CurrentState.attackEnter:
                    nvAgent.Stop();
                    _anim.SetBool("Move", false);
                    curState = CurrentState.attack;
                    break;
                case CurrentState.attack:
                    _anim.SetBool("Attack", true);
                    break;
                case CurrentState.attackExit:
                    _anim.SetBool("Attack", false);
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

    protected void ResetCooltime() { _cooltime = _originCooltime; }

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

    /// <summary>
    /// CheckState에서 정의한 현재 상태에 따른 액션
    /// Animator의 Parameter값을 조절하여 애니메이션 출력을 함.,
    /// 처음 진입은 무조건 ~~Enter State로 진입하여 Host의 판단 후, 본래의 State로 진입하게 함.
    /// </summary>
    /// <returns></returns>
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
        _currentBaseState = _anim.GetCurrentAnimatorStateInfo(0);
        if (_cooltime >= 0f) _cooltime -= Time.deltaTime;
        _anim.SetFloat("AttackMultiply", _attackMultiply);
        _anim.SetBool("Hit", _isHit);
    }

    public void ReleaseAllAnimatorBools()
    {
        _anim.SetBool("Dead", false);
        _anim.SetBool("Move", false);
        _anim.SetBool("Hit", false);
        _anim.SetBool("Attack", false);
        nvAgent.Stop();
    }

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

    protected IEnumerator BoolClicker(int Number)
    {
        Debug.Log("Start BoolClicker");
        switch(Number)
        {
            case 1:
                _isAttack = true;
                yield return new WaitForSeconds(.2f);
                _isAttack = false;
                break;
            case 2:
                _isSkill1 = true;
                yield return new WaitForSeconds(.2f);
                _isSkill1 = false;
                break;
            case 3:
                _isSkill2 = true;
                yield return new WaitForSeconds(.2f);
                _isSkill2 = false;
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

    public void AttackEvent()
    {
        StartCoroutine(BoolClicker(1));
    }
    
    public void SkillEvent1()
    {
        StartCoroutine(BoolClicker(2));
    }

    public void SkillEvent2()
    {
        StartCoroutine(BoolClicker(3));
    }

    public void HitEvent()
    {
        StartCoroutine(BoolClicker(4));
    }

    void SetAttackMultiply() { _attackMultiply = _basicAttackMultiply; }
    
    void ReleaseAttackMultiply() { _attackMultiply = 1f; }

}
