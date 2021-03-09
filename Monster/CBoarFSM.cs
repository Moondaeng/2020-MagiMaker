using UnityEngine;
using System.Collections.Generic;

/*  요약
 * 드래곤유체 Boar 라고 되어 있다.
 * 잡몹으로 등장할 것 같고, 드래곤 보스때 이놈들을 소환하는 패턴도 고려
 * 패턴 개요
 * 1. 기본공격 
 * 2. 울부짖기(울부짖고 몇 초 후에 방을 벗어나지 못하면 친구 소환 or 디버프 용도)
 * 3. 돌진 박치기 (플레이어가 날아가버리는 상호작용을 개발 예정)
 * 
 */

public class CBoarFSM : CEnemyFSM
{
    // 어슬렁 거리는 모션에 사용
    private bool _randomWalk;
    private bool _search;
    private bool _origin;
    private float _originTimer = 3f;
    private static int _searchingState;
    private static int _toOriginPositionState;
    private Vector3 _walkPoint;
    private Vector3 _originVector;
    
    // 돌진 공격에 사용
    private float _hornAttackDistance;
    private float _hornAttackAngle;
    private bool _hornAttackCheck;
    private Vector3 _hornPoint;
    private List<GameObject> InTriggerUnit;

    protected override void InitStat()
    {
        _moveSpeed = 5f;
        _attackDistance = 6f;
        _attackAngle = 5f;
        _hornAttackDistance = 2f;
        _hornAttackAngle = 180f;
        
        _anim = GetComponent<Animator>();
        _myPara = GetComponent<CEnemyPara>();
        _myPara.deadEvent.AddListener(CallDeadEvent);
        _myPara.hitEvent.AddListener(CallHitEvent);
        
        _idleState = Animator.StringToHash("Base Layer.Idle");
        _runState = Animator.StringToHash("Base Layer.Run");
        _searchingState = Animator.StringToHash("Base Layer.Searching");
        _walkState = Animator.StringToHash("Base Layer.Walk");
        _attackState1 = Animator.StringToHash("Base Layer.Attack");
        _waitState = Animator.StringToHash("Base Layer.AttackWait");
        _skillState1 = Animator.StringToHash("Base Layer.Scream");
        _skillState2 = Animator.StringToHash("Base Layer.HornAttack");
        _gethitState = Animator.StringToHash("Base Layer.GetHit");
        _deadState1 = Animator.StringToHash("Base Layer.Dead");
        _toOriginPositionState = Animator.StringToHash("Base Layer. ToOriginPosition");

        _cooltime = 1f;
        _skillCooltime1 = 20f;
        _originCooltime = _cooltime;
        _originSkillCooltime1 = _skillCooltime1;
        _skillCooltime2 = 8f;
        _originSkillCooltime2 = _skillCooltime2;
        _originVector = transform.position;
    }
    
    protected override void CallDeadEvent()
    {
        Debug.Log("he is dead");
        _anim.SetBool("Dead", true);
        this.gameObject.tag = "Untagged";
        this.gameObject.layer = LayerMask.NameToLayer("DeadBody");
        Invoke("OffDead", 0.1f);
    }

    private void OffDead()
    {
        _anim.SetBool("Dead", false);
    }

    #region State
    protected override void UpdateState()
    {
        _originTimer -= Time.deltaTime;
        if (_actionStart)
        {
            _skillCooltime1 -= Time.deltaTime;
            _skillCooltime2 -= Time.deltaTime;
        }
        
        if (_anim.GetFloat("DistanceFromPlayer") > 15f)
        {
            _randomWalk = true;
        }
        else
        {
            _randomWalk = false;
        }

        if (_currentBaseState.fullPathHash == _idleState) IdleState();
        else if (_currentBaseState.fullPathHash == _walkState) WalkState();
        else if (_currentBaseState.fullPathHash == _searchingState) SearchingState();
        else if (_currentBaseState.fullPathHash == _runState) ChaseState();
        else if (_currentBaseState.fullPathHash == _attackState1) AttackState1();
        else if (_currentBaseState.fullPathHash == _attackState1) AttackWaitState();
        else if (_currentBaseState.fullPathHash == _skillState1) SkillState1();
        else if (_currentBaseState.fullPathHash == _skillState2) SkillState2();
        else if (_currentBaseState.fullPathHash == _waitState) AttackWaitState();
        else if (_currentBaseState.fullPathHash == _deadState1) DeadState1();
        else if (_currentBaseState.fullPathHash == _toOriginPositionState) WalkOriginPosition();

        if (_skillCooltime1 < 0f && !_anotherAction)
        {
            _anim.SetTrigger("Skill1");
        }
        if (_skillCooltime2 < 0f)
        {
            //Debug.Log("Skill2 Cooltime finished!");
            _hornAttackCheck = true;
            _hornPoint = new Vector3(_player.transform.position.x, 0f, _player.transform.position.z);
            var vector = _hornPoint - transform.position;
            _hornPoint += _hornPoint + vector;
        }

        if (_origin == true)
        {
            _origin = false;
        }
    }
    
    private void IdleState()
    {
        if (_anotherAction)
        {
            _anotherAction = false;
        }
    }

    private void WalkState()
    {
        transform.position = Vector3.MoveTowards(transform.position, _walkPoint, _moveSpeed / 2f * Time.deltaTime);
        Quaternion lookRotation = Quaternion.LookRotation(_walkPoint - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            lookRotation, Time.deltaTime * _rotAnglePerSecond);
        if (Vector3.Distance(transform.position, _walkPoint) < 3f)
        {
            _search = true;
        }
    }

    private void SearchingState()
    {
        _search = false;
        float x, z;
        x = UnityEngine.Random.Range(-5f, 5f);
        z = UnityEngine.Random.Range(-5f, 5f);
        _walkPoint = transform.position + new Vector3(x, 0, z);
    }

    private void WalkOriginPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, _originVector, _moveSpeed / 2f * Time.deltaTime);
        Quaternion lookRotation = Quaternion.LookRotation(_originVector - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            lookRotation, Time.deltaTime * _rotAnglePerSecond);
        if (Vector3.Distance(transform.position, _originVector) < 2f)
        {
            _search = true;
        }
    }

    protected override void ChaseState()
    {
        base.ChaseState();
        if (_currentBaseState.fullPathHash != _deadState1) MoveState();
    }

    protected override void AttackState1()
    {
        base.AttackState1();
    }

    #region Skill
    // 울부짖기
    private void SkillState1()
    {
        if (_myState != EState.Skill1)
        {
            _myState = EState.Skill1;
            _skillCooltime1 = _originSkillCooltime1;
        }
        if (_lookAtPlayer)
        {
            _lookAtPlayer = false;
        }
    }

    private void SkillOneDamageCaculate()
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, 5f);
        InTriggerUnit = new List<GameObject>();
        foreach (Collider h in objects)
        {
            GameObject g = h.gameObject;
            bool SameExist = false;
            foreach (GameObject a in InTriggerUnit)
            {
                if (a == g)
                    SameExist = true;
            }
            if (SameExist)
                continue;
            else
            {
                InTriggerUnit.Add(g);
                if (g.tag == "Player")
                {
                    CPlayerPara p = g.GetComponent<CPlayerPara>();
                    p.DamegedRegardDefence((_myPara._attackMin + _myPara._attackMax) * 2);
                }
            }
        }
    }

    // 돌진
    private void SkillState2()
    {
        if (_myState != EState.Skill2)
        {
            _myState = EState.Skill2;
            _skillCooltime2 = _originSkillCooltime2;
            _hornAttackCheck = false;
        }
        if (_lookAtPlayer)
        {
            _lookAtPlayer = false;
        }
        if (!_anotherAction)
        {
            _anotherAction = true;
        }
        Vector3 yZeroPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        transform.position = Vector3.MoveTowards(yZeroPosition, _hornPoint, _moveSpeed * Time.deltaTime);
        Quaternion lookRotation = Quaternion.LookRotation(_hornPoint - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            lookRotation, Time.deltaTime * _rotAnglePerSecond);
    }

    private void HornAttackCheck()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (IsTargetInSight(_hornAttackAngle, _players[i].transform) 
                && IsInAttackDistance(_hornAttackDistance, _players[i].transform))
            {
                _playerPara = _players[i].GetComponent<CPlayerPara>();
                AttackCalculate(_playerPara);
            }
        }
    }

    #endregion
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "WALL" && _origin == false)
        {
            _origin = true;
        }
    }

    protected override void Update()
    {
        DebugState();
        _anim.SetInteger("Hp", _myPara._curHp);
        _anim.SetBool("Hit", _getHit);
        _anim.SetBool("RandomWalk", _randomWalk);
        _anim.SetBool("Search", _search);
        _anim.SetBool("ToOrigin", _origin);
        _anim.SetBool("Skill2", _hornAttackCheck);
        if (_getHit) _getHit = false;
        base.Update();
    }
}