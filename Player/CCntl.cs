using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class CCntl : MonoBehaviour
{
    #region Properties
    [SerializeField] public float _jumpPower = 12f;
    [Range(1f, 4f)] [SerializeField] float _gravityMultiplier = 3.5f;
    [SerializeField] float _groundCheckDistance = 0.5f;
    // 발, 팔 콜라이더 왼쪽 팔 오른쪽 팔
    [SerializeField] public SphereCollider[] _handCollider;
    CPlayerMeleecheker AttackTrailR, AttackTrailL;

    // 추후에 공이속 추가되면 사용할 배수
    //[SerializeField] float _animSpeedMultiplier = 1f;
    //[SerializeField] float _moveSpeedMultiplier = 1f;

    private Animator _animator;
    private AnimatorStateInfo _currentBaseState;
    private CPlayerPara _myPara;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsule;
    private BoxCollider _attack;
    private CPlayerSkill _mySkill;
    private float z, x;
    private float _rotationSpeed = 360f;

    // 초기 체크값을 저장할 변수
    private float _origGroundCheckDistance;
    private float _capsuleHeight;
    private Vector3 _capsuleCenter;

    // 입력 위치값 조정
    private Vector3 _inputVec;
    private Vector3 _targetDirection;

    // 법선 벡터 (
    private Vector3 _groundNormal;

    // 애니메이션 상태값
    private bool _isGrounded;
    private bool _isJumping;
    private bool _jump;
    private bool _isAttackInputed;
    private bool _isJumpInputed;
    private bool _isConcentrated;
    private int _skillActionNumber;

    // 애니메이션 상태값 상태이상
    private bool _knockBack;
    private float _downTime;
    public bool _getHit;
    private bool _stun;

    // 상수
    private const float _halfF = .5f;
    const float _seatingTime = .2f;
    Coroutine CO;
    Coroutine COSkill;
    
    private int indexer;

    float _exitTime = 0.8f;
    // 어느 손에서 나갈 것인가?
    public struct SkillStartPoint
    {
        // false : 왼손 true : 오른손
        public bool handPoint;
        public Vector3 _skillStartPoint;
    }

    public SkillStartPoint _skillHand;
    [System.NonSerialized]
    public UnityEvent SkillExitEvent = new UnityEvent();
    public UnityEvent AttackEvent = new UnityEvent();
    public UnityEvent JumpEvent = new UnityEvent();
    public UnityEvent JumpEndEvent = new UnityEvent();
    public UnityEvent RollEvent = new UnityEvent();
    #endregion

    #region __start__
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _myPara = GetComponent<CPlayerPara>();
        _capsule = GetComponent<CapsuleCollider>();
        _attack = GetComponentInChildren<BoxCollider>();
        _mySkill = GetComponent<CPlayerSkill>();
        _animator.SetFloat("Jump", -4f);
        _capsuleHeight = _capsule.height;
        _capsuleCenter = _capsule.center;
        _origGroundCheckDistance = _groundCheckDistance;
        _myPara.deadEvent.AddListener(Dead);
        _skillActionNumber = 0;
        AttackTrailR = transform.GetChild(2).GetComponent<CPlayerMeleecheker>();
        AttackTrailL = transform.GetChild(3).GetComponent<CPlayerMeleecheker>();
    }
    // 애니메이터에서 사용함. 
    public void AttackDisabledRightCollider()
    {
        AttackTrailR.DiscardList();
    }
    public void AttackDisabledLeftCollider()
    {
        AttackTrailL.DiscardList();
    }
    #endregion

    #region 코루틴 모음집
    IEnumerator COPause(float pauseTime)
    {
        yield return new WaitForSeconds(pauseTime);
    }

    IEnumerator COStun(float pauseTime)
    {
        _stun = true;
        _animator.SetTrigger("StunTrigger");
        yield return new WaitForSeconds(pauseTime);
        ExitStun();
    }

    IEnumerator COExitConcentration(float pauseTime)
    {
        _isConcentrated = true;
        yield return new WaitForSeconds(pauseTime);
        _isConcentrated = false;
    }

    IEnumerator COPushingForSeconds(float level, Vector3 Direction)
    {
        int i = 1;
        while (true)
        {
            yield return new WaitForSeconds(.02f);
            _rigidbody.AddForce(Direction * 30.0f * level / i, ForceMode.VelocityChange);
            if (i > 10) break;
            i++;
        }
    }
    IEnumerator COSlowForSeconds(float level)
    {
        int i = 0;
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (i < level) _myPara._runAnimationMultiply = .5f;
            else
            {
                _myPara._runAnimationMultiply = 1f;
                break;
            }
            i++;
        }
    }
    #endregion

    #region CController Use these function
    public void Move(float inputX, float inputZ)
    {
        z = inputZ;
        x = inputX;
        _inputVec = new Vector3(x, 0, z);
    }

    public void Attack()
    {
        _animator.SetTrigger("Attack");
        _isAttackInputed = true;
    }

    public void ComboIndexer(int index)
    {
        indexer = index;
    }

    public void Skill()
    {
        if ((_currentBaseState.IsName("Idle") || _currentBaseState.IsName("Run")) || !_isConcentrated)
        {
            int layerMask = (1 << LayerMask.NameToLayer("Monster")) + (1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("Default"));
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                //print("I'm looking at " + hit.transform.name);
                //GetComponent<CController>().hit = hit;
            }
            if (indexer == 2)
            {
                _skillActionNumber = 1;
                StartCoroutine(COExitConcentration(5f));
            }
            else
            {
                _skillActionNumber = 0;
            }
            _animator.SetTrigger("SkillTrigger");
            COSkill = StartCoroutine(COPause(.8f));
        }
        else if (_isConcentrated)
        {
            StopCoroutine(COSkill);
            _isConcentrated = false;
            SkillExitEvent.Invoke();
        }
    }

    public void Jump()
    {
        if (_rigidbody.velocity.y < 0.5f || _rigidbody.velocity.y > -0.5f)
            _isJumpInputed = true;
    }

    public void Roll()
    {
        _animator.SetTrigger("Roll");
        GetStateFreeFromDamage();
    }

    public void Dead()
    {
        gameObject.name = "deadBody";
    }
    #endregion


    #region 업데이트
    private void Update()
    {
        _currentBaseState = _animator.GetCurrentAnimatorStateInfo(0);
        
        _inputVec = new Vector3(x, 0, z);

        CheckGroundStatus();
        // 법선임 hitcast 할 때 씀
        _inputVec = Vector3.ProjectOnPlane(_inputVec, _groundNormal);

        // 점프 모션 키 입력
        if (_isJumpInputed)
        {
            _jump = true;
            _isJumpInputed = false;
        }
        else
        {
            _jump = false;
        }

        if (_currentBaseState.IsName("Attack1") ||  _currentBaseState.IsName("Skill1") || _currentBaseState.IsName("KnockBack"))
        {
            TurnToCameraRelative();
        }

        if (_currentBaseState.IsName("Idle"))
        {
            _isAttackInputed = false;
        }

        if (_isGrounded) HandleGroundedMovement();
        else HandleAirborneMovement();

        UpdateMovement();
    }

    void HandleGroundedMovement()
    {
        // 점프 조건 1. 앉아 있지 않기 2. Idle 상태 3. 뛰는 상태
        if (_jump && (_currentBaseState.IsName("Idle") || _currentBaseState.IsName("Run")))
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _jumpPower,
                _rigidbody.velocity.z);
            JumpEvent?.Invoke();
            _isGrounded = false;
            _animator.applyRootMotion = false;
            _groundCheckDistance = 0.5f;
        }
    }

    void HandleAirborneMovement()
    {
        // 인스펙터상으로 추가한 중력보정 곱
        Vector3 extraGravityForce = (Physics.gravity * _gravityMultiplier) - Physics.gravity;
        _rigidbody.AddForce(extraGravityForce);

        _groundCheckDistance = _rigidbody.velocity.y < 0 ? _origGroundCheckDistance : 0.01f;
    }

    // 애니메이터에 기입할 정보들을 갱신
    void UpdateMovement()
    {
        Vector3 motion = _inputVec;
        // 만약에 대각선으로 움직이면 움직임 총량을 0.7로 바꿈
        motion *= (Mathf.Abs(_inputVec.x) == 1 && Mathf.Abs(_inputVec.z) == 1) ? 0.7f : 1;

        // 이동방향 
        _animator.SetFloat("Input X", z);
        _animator.SetFloat("Input Z", -(x));
        // 땅바닥에 있는가? -> 점프 체크
        _animator.SetBool("OnGround", _isGrounded);

        _animator.SetBool("KnockBack", _knockBack);
        _animator.SetBool("Stun", _stun);
        _animator.SetInteger("SkillList", _skillActionNumber);
        _animator.SetBool("Skill2", _isConcentrated);
        _animator.SetBool("IsAttackInputed", _isAttackInputed);
        // 이동 키를 눌렀을 경우 체크
        if (x != 0 || z != 0) _animator.SetBool("Moving", true);
        else _animator.SetBool("Moving", false);

        // 땅에 붙어 있지 않으면 y축의 속도를 잼 -> 가속도를 재는 것
        if (!_isGrounded) _animator.SetFloat("Jump", _rigidbody.velocity.y);
        RotateTowardMovementDirection();
        GetCameraRelativeMovement();
        CrowdControlAnimation();
    }

    // RaycastHit을 이용한 땅에 붙어있는지 체크
    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f),
            Vector3.down, out hitInfo, _groundCheckDistance))
        {
            _groundNormal = hitInfo.normal;
            if (_isJumping)
            {
                _isJumping = false;
                JumpEndEvent?.Invoke();
            }
            _isGrounded = true;
            _animator.applyRootMotion = true;
        }
        else
        {
            _isGrounded = false;
            _isJumping = true;
            _groundNormal = hitInfo.normal;
            _animator.applyRootMotion = false;
        }
    }

    // 구형보간 
    void RotateTowardMovementDirection()
    {
        if (_inputVec != Vector3.zero && (_currentBaseState.IsName("Idle") || _currentBaseState.IsName("Run")))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(_targetDirection), Time.deltaTime * _rotationSpeed);
        }
    }

    // 캐릭터가 바라보는 방향으로 돌려버림
    void GetCameraRelativeMovement()
    {
        Transform cameraTransform = Camera.main.transform;

        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        _targetDirection = h * right + v * forward;
    }

    public void TurnToCameraRelative()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Default");
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            transform.LookAt(hit.point - new Vector3(0f, hit.point.y, 0f));
        }
    }

    void CrowdControlAnimation()
    {
        if (_currentBaseState.IsName("KnockBack"))
        {
            _knockBack = false;
        }
    }
    #endregion

    // 점프할 발 체크 추후에 여기에 사운드 추가
    // 애니메이션 placeholder에 들어가있는 함수
    private void FootR()
    {
        if (_isGrounded)
        {
            _animator.SetFloat("JumpLeg", 1f);
        }
    }

    private void FootL()
    {
        if (_isGrounded)
        {
            _animator.SetFloat("JumpLeg", -1f);
        }
    }

    private void Hit()
    {
        //Debug.Log(_attack.transform.position);
    }

    private void SetLeftStartPoint()
    {
        _skillHand.handPoint = false;
        _skillHand._skillStartPoint = _handCollider[0].gameObject.transform.position;

    }
    private void SetRightStartPoint()
    {
        _skillHand.handPoint = false;
        _skillHand._skillStartPoint = _handCollider[1].gameObject.transform.position;
    }

    private void BeginEffect()
    {

    }

    private void AttackStart()
    {
        AttackEvent?.Invoke();
    }

    private void RollStart()
    {
        RollEvent?.Invoke();
    }

    // 애니메이션이 스크립트의 효과를 받아서 움직이면 여기서 처리해줌.
    public void OnAnimatorMove()
    {
        if (_isGrounded && Time.deltaTime > 0)
        {
            Vector3 v = _animator.deltaPosition / Time.deltaTime;

            v.y = _rigidbody.velocity.y;
            _rigidbody.velocity = v;
        }
    }

    #region 상태이상 관리
    public void CCController(string type, float level)
    {
        if (_myPara._invincibility) return;
        switch (type)
        {
            case "KnockBack":
                // 30프레임을 기준으로 입력한 레벨만큼 배수를 해서 초단위로 변경함
                level = (1 / 30f) / level;
                _knockBack = true;
                GetStateFreeFromDamage();
                _animator.SetFloat("KnockBackTime", level);
                break;
            case "Gethit":
                break;
            case "Stun":
                StartCoroutine(COStun(level * 2.5f));
                break;
            case "Slow":
                StartCoroutine(COSlowForSeconds(level));
                break;
            case null:
                break;
        }
    }

    public void CCController(string type, float level, Vector3 vec)
    {
        if (_myPara._invincibility) return;
        switch (type)
        {
            case "Push":
                StartCoroutine(COPushingForSeconds(level, vec));
                break;
            case null:
                break;
        }
    }

    // 무적 판정을 넣어줌.
    // Animation 상에서 실행시키는 Offinvincibility가 알아서 check 해제함
    void GetStateFreeFromDamage()
    {
        _myPara._invincibility = true;
        _myPara._invincibilityChecker = true;
    }

    public void ExitStun()
    {
        _stun = false;
        StopCoroutine(CO);
    }

    #endregion
}
