using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class CCntl : MonoBehaviour
{
    #region Properties
    [SerializeField] public float _jumpPower = 12f;
    [Range(1f, 4f)] [SerializeField] float _gravityMultiplier = 3.5f;
    [SerializeField] float _groundCheckDistance = 0.5f;

    // 추후에 공이속 추가되면 사용할 배수
    //[SerializeField] float _animSpeedMultiplier = 1f;
    //[SerializeField] float _moveSpeedMultiplier = 1f;

    Animator _animator;
    AnimatorStateInfo _currentBaseState;
    CPlayerPara _myPara;
    CStunExitCommand _myStun;
    Rigidbody _rigidbody;
    CapsuleCollider _capsule;
    BoxCollider _attack;
    float z, x;
    float _rotationSpeed = 30;

    // 초기 체크값을 저장할 변수
    float _origGroundCheckDistance;
    float _capsuleHeight;
    Vector3 _capsuleCenter;

    // 입력 위치값 조정
    Vector3 _inputVec;
    Vector3 _targetDirection;

    // 법선 벡터 (
    Vector3 _groundNormal;

    // 애니메이션 상태값
    bool _isGrounded;
    bool _jump;
    bool _roll;
    private bool _isJumpInputed;
    private bool _isRollInputed;

    // 애니메이션 상태값 상태이상
    bool _knockBack;
    float _downTime;
    public bool _getHit;
    bool _stun;

    // 상수
    const float _halfF = .5f;
    const float _seatingTime = .2f;
    Coroutine CO;

    static int _rollState;
    static int _wakeUpState;
    #endregion
    
    #region __start__
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _myPara = GetComponent<CPlayerPara>();
        _myStun = GetComponent<CStunExitCommand>();
        _capsule = GetComponent<CapsuleCollider>();
        _attack = GetComponentInChildren<BoxCollider>();
        _animator.SetFloat("Jump", -4f);
        _capsuleHeight = _capsule.height;
        _capsuleCenter = _capsule.center;
        _origGroundCheckDistance = _groundCheckDistance;
        _rollState = Animator.StringToHash("Base Layer.Roll");
        _wakeUpState = Animator.StringToHash("Base Layer.WakeUp");
    }
    #endregion

    #region 코루틴 모음집
    public IEnumerator COStunPause(float pauseTime)
    {
        yield return new WaitForSeconds(pauseTime);
    }

    public IEnumerator COStun(float pauseTime)
    {
        _stun = true;
        _myStun.Start((int)pauseTime * 3);
        _animator.SetTrigger("StunTrigger");
        yield return new WaitForSeconds(pauseTime);
        SendMessage("EndTime");
        ExitStun();
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
        // 공격 모션 키 입력
        if ((_currentBaseState.IsName("Idle") || _currentBaseState.IsName("Run")))
        {
            _animator.SetTrigger("Attack");
            StartCoroutine(COStunPause(.6f));
        }
    }

    public void Skill()
    {
        if ((_currentBaseState.IsName("Idle") || _currentBaseState.IsName("Run")))
        {
            _animator.SetTrigger("SkillTrigger");
            StartCoroutine(COStunPause(.8f));
            int layerMask = (1 << LayerMask.NameToLayer("Monster")) + (1 << LayerMask.NameToLayer("Player"));
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                print("I'm looking at " + hit.transform.name);
                var HitInfo = GetComponent<CController>().hit;
            }
        }
    }

    public void Jump()
    {
        _isJumpInputed = true;
    }

    public void Roll()
    {
        _animator.SetTrigger("Roll");
        GetStateFreeFromDamage();
    }
    #endregion

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
            _isGrounded = true;
            _animator.applyRootMotion = true;
        }
        else
        {
            _isGrounded = false;
            _groundNormal = hitInfo.normal;
            _animator.applyRootMotion = false;
        }
    }

    // 구형보간 
    void RotateTowardMovementDirection()
    {
        if (_inputVec != Vector3.zero && (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")
            || _animator.GetCurrentAnimatorStateInfo(0).IsName("Run")))
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

    void CrowdControlAnimation()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("KnockBack"))
        {
            _knockBack = false;
        }
    }

    // 점프할 발 체크 추후에 여기에 사운드 추가
    // 애니메이션 placeholder에 들어가있는 함수
    void FootR()
    {
        if (_isGrounded)
        {
            _animator.SetFloat("JumpLeg", 1f);
        }
    }

    void FootL()
    {
        if (_isGrounded)
        {
            _animator.SetFloat("JumpLeg", -1f);
        }
    }

    void Hit()
    {
        Debug.Log(_attack.transform.position);
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
                CO = StartCoroutine(COStun(level * 2.5f));
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
