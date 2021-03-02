using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CDragonBossFSM : CEnemyFSM
{
    private static float _glideSpeed;
    private Vector3 _newDirection;
    private Vector3 _glidingPoint;
    private Vector3 _newGlidingPoint;

    private bool _nomoreGliding = false;
    private bool _sleeping = true;
    private bool _defend = false;
    private bool _beforeUsingSkill = false;
    private bool _startForward = false;
    private bool _startGlide = false;
    private static int _defendState;
    private static int _takeOff;
    private static int _flyFloat;
    private static int _flyFloat1;
    private static int _flyFloat2;
    private static int _flyForward;
    private static int _flyFireball;
    private static int _flyGlide;
    private static int _land;
    TextMeshPro _text;
    //private List<string> _skill1 = new List<string>();
    
    public sealed class DragonEState : EState
    {
        public static readonly DragonEState Defend = new DragonEState("Defend");
        public static readonly DragonEState TakeOff = new DragonEState("TakeOff");
        public static readonly DragonEState FlyFloat = new DragonEState("FlyFloat");
        public static readonly DragonEState FlyForward = new DragonEState("FlyForward");
        public static readonly DragonEState FlyGlide = new DragonEState("FlyGlide");
        public static readonly DragonEState FlyFireballShoot = new DragonEState("FlyFireballShoot");
        public static readonly DragonEState Land = new DragonEState("Land");

        private DragonEState(string value)
          : base(value)
        {
        }
    }

    protected override void InitStat()
    {
        _moveSpeed = 5f;
        _glideSpeed = 25f;
        _attackDistance = 6f;
        _attackAngle = 5f;

        _text = transform.GetChild(2).GetComponent<TextMeshPro>();
        _myBossPara.deadEvent.AddListener(CallDeadEvent);
        _myBossPara.hitEvent.AddListener(CallHitEvent);
        _myBossPara.SkillUsingHPEvent.AddListener(SkillState3);
        _myBossPara.DefenceUsingEvent.AddListener(DefendState);
        
        _defendState = Animator.StringToHash("Base Layer.Defend");
        _takeOff = Animator.StringToHash("Base Layer.TakeOff");
        _flyFloat = Animator.StringToHash("Base Layer.Skill3Detail.FlyFloat");
        _flyFloat1 = Animator.StringToHash("Base Layer.Skill3Detail.FlyFloat1");
        _flyFloat2 = Animator.StringToHash("Base Layer.Skill3Detail.FlyFloat2");
        _flyFireball = Animator.StringToHash("Base Layer.FlyFireballShoot"); ;
        _flyGlide = Animator.StringToHash("Base Layer.FlyGlide");
        _land = Animator.StringToHash("Base Layer.Land");
        
        _cooltime = 1f;
    }
    

    #region State
    protected override void UpdateState()
    {
        if (_actionStart)
        {
            _skillCoolTime[0] -= Time.deltaTime;
            _skillCoolDown1 = true;
            _skillCoolTime[1] -= Time.deltaTime;
            _skillCoolDown2 = true;
        }
        if (_currentBaseState.fullPathHash == _idleState) IdleState();
        else if (_currentBaseState.fullPathHash == _runState) ChaseState();
        else if (_currentBaseState.fullPathHash == _attackState1) AttackState1();
        else if (_currentBaseState.fullPathHash == _skillState1) SkillState1();
        else if (_currentBaseState.fullPathHash == _skillState2) SkillState2();
        else if (_currentBaseState.fullPathHash == _skillState3) SkillState3();
        else if (_currentBaseState.fullPathHash == _defendState) DefendState();
        else if (_currentBaseState.fullPathHash == _waitState) AttackWaitState();
        else if (_currentBaseState.fullPathHash == _deadState) DeadState();
        else if (_currentBaseState.fullPathHash == _takeOff) TakeOffState();
        else if (_currentBaseState.fullPathHash == _flyFloat) FlyFloatState();
        else if (_currentBaseState.fullPathHash == _flyFloat1) FlyFloatState();
        else if (_currentBaseState.fullPathHash == _flyFloat2) FlyFloatState();
        else if (_currentBaseState.fullPathHash == _flyGlide) FlyGlideState();
        else if (_currentBaseState.fullPathHash == _flyFireball) FlyFireballState();
        else if (_currentBaseState.fullPathHash == _land) LandState();

        if (_skillCoolTime[0] < 0f && !_anotherAction)
        {
            _anim.SetTrigger("Skill1");
            _skillCoolDown1 = false;
        }
        if (_skillCoolTime[1] < 0f && !_anotherAction)
        {
            _anim.SetTrigger("Skill2");
            _skillCoolDown2 = false;
        }
    }
    
    private new void IdleState()
    {
        if (_anotherAction)
        {
            _anotherAction = false;
        }
    }

    protected override void ChaseState()
    {
        base.ChaseState();
        if (_currentBaseState.fullPathHash != _deadState) MoveState();
    }

    protected override void AttackState1()
    {
        base.AttackState1();
    }

    #region Skill
    // 꼬리 공격
    private void SkillState1()
    {
        if (_myState != EState.Skill1)
        {
            _myState = EState.Skill1;
            _skillCoolTime[0] = _originSkillCoolTime[0];
        }
        _lookAtPlayer = false;
    }

    // 몬스터 파이어볼
    private void SkillState2()
    {
        if (_myState != EState.Skill2)
        {
            _myState = EState.Skill2;
            _skillCoolTime[1] = _originSkillCoolTime[1];
        }
        if (!_lookAtPlayer)
        {
            _lookAtPlayer = true;
        }
    }

    // 방어 후 울부짖음
    /*
     피가 일정 이하로 떨어지면 방어를 하는데 이 때, 5초후 울부짖고 플레이어 스턴
         */
    private void SkillState3()
    {
        if (_myState != EState.Skill3)
        {
            _myState = EState.Skill3;
            _anim.SetTrigger("Skill3");
            BoxCollider b_tmp = GetComponent<BoxCollider>();
            b_tmp.enabled = false;
            Rigidbody r_tmp = GetComponent<Rigidbody>();
            r_tmp.useGravity = false;
            r_tmp.isKinematic = true;
        }
        _beforeUsingSkill = true;
    }

    private void TakeOffState()
    {
        if (_myState != DragonEState.TakeOff)
        {
            _myState = DragonEState.TakeOff;
        }
        if (_lookAtPlayer)
        {
            _lookAtPlayer = false;
        }
        if (!_anotherAction)
        {
            _anotherAction = true;
        }
    }

    private void FlyFloatState()
    {
        if (_myState != DragonEState.FlyFloat)
        {
            _myState = DragonEState.FlyFloat;
            
            _glidingPoint = new Vector3(_player.transform.position.x, 0f, _player.transform.position.z);

            var vector = _glidingPoint - transform.position;
            _newGlidingPoint = _glidingPoint + vector;
            //Debug.Log(transform.position);
            //Debug.Log(_glidingPoint);
            //Debug.Log(_newGlidingPoint);

            var quaternion = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        }
    }

    public void StartingGlide()
    {
        _startGlide = true;
        _anim.SetBool("Glide", _startGlide);
    }

    public void FlyGlideState()
    {
        if (_myState != DragonEState.FlyGlide)
        {
            _myState = DragonEState.FlyGlide;
        }
        if (!_lookAtPlayer)
        {
            _lookAtPlayer = false;
        }
        
        if (!_nomoreGliding)
        {
            //Debug.Log("distance of gliding point : " + Vector3.Distance(transform.position, _glidingPoint));
            transform.position = Vector3.MoveTowards(transform.position, _glidingPoint - new Vector3(0f, 2f, 0f), _glideSpeed * Time.deltaTime);
            Quaternion lookRotation = Quaternion.LookRotation(_glidingPoint - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                lookRotation, Time.deltaTime * _rotAnglePerSecond);
            if (Vector3.Distance(transform.position, _glidingPoint) <= 6f)
            {
                _nomoreGliding = true;
            }
        }
        else if (_nomoreGliding)
        {
            //Debug.Log("distance of new gliding point : " + Vector3.Distance(transform.position, _newGlidingPoint));
            transform.position = Vector3.MoveTowards(transform.position, _newGlidingPoint, _glideSpeed * Time.deltaTime);
            Quaternion lookRotation = Quaternion.LookRotation(_newGlidingPoint - _glidingPoint);
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                lookRotation, Time.deltaTime * _rotAnglePerSecond);
        }

        if (Vector3.Distance(_newGlidingPoint, transform.position) < 3f)
        {
            _startGlide = false;
            _anim.SetBool("Glide", _startGlide);
        }

    }

    public void FlyFireballState()
    {
        if (_myState != DragonEState.FlyFireballShoot)
        {
            _myState = DragonEState.FlyFireballShoot;
        }
    }

    public void LandState()
    { 
        if (_myState != DragonEState.Land)
        {
            _myState = DragonEState.Land;
            BoxCollider b_tmp = GetComponent<BoxCollider>();
            b_tmp.enabled = true;
            Rigidbody r_tmp = GetComponent<Rigidbody>();
            r_tmp.useGravity = true;
            r_tmp.isKinematic = false;
        }
    }
    
    IEnumerator DefencingOnPlayerAttack()
    {
        yield return new WaitForSeconds(5f);
        _defend = false;
    }

    public void DefendState()
    {
        // 11-27 가드이벤트 처리 해야함
        _myState = DragonEState.Defend;
        Debug.Log("가드 이벤트 보내기 후");
        _defend = true;
    }

    #endregion
    #endregion

    protected override void DebugState()
    {
        _text.text = _myState.ToString();
        base.DebugState();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_myState == DragonEState.FlyGlide)
        {
            if (other.tag == "Player")
            {
                _nomoreGliding = true;
                Debug.Log("fuck");
                CPlayerPara otherPara = other.GetComponent<CPlayerPara>();
                CCntl otherCntl = other.GetComponent<CCntl>();
                otherPara.DamegedRegardDefence(_myBossPara._attackMax * 2);
                otherCntl.CCController("KnockBack", 30f);
            }
        }
    }

    protected override void Update()
    {
        DebugState();
        _anim.SetBool("CoolDown", _coolDown);
        _anim.SetInteger("Hp", _myBossPara.CurrentHp);
        _anim.SetBool("Defend", _defend);
        _anim.SetBool("Hit", _getHit);
        if (_getHit) _getHit = false;
        if (_defend) _defend = false;
        base.Update();
    }
}