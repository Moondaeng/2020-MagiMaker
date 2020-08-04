using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class CCntl : MonoBehaviour 
{
    [SerializeField] public float _jumpPower = 12f;
    [Range(1f, 4f)] [SerializeField] float _gravityMultiplier = 3.5f;
    [SerializeField] float _groundCheckDistance = 0.5f;

    // 추후에 공이속 추가되면 사용할 배수
    //[SerializeField] float _animSpeedMultiplier = 1f;
    //[SerializeField] float _moveSpeedMultiplier = 1f;
    
    Animator _animator;
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
    bool _crouchCheck;
    bool _rollCheck;
    bool _isGrounded;
    bool _jump;
    bool _crouch;
    bool _roll;

    // 상수
    const float _halfF = .5f;
    const float _seatingTime = .2f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsule = GetComponent<CapsuleCollider>();
        _attack = GetComponentInChildren<BoxCollider>();
        _capsuleHeight = _capsule.height;
        _capsuleCenter = _capsule.center;
        _origGroundCheckDistance = _groundCheckDistance;
    }
    
    public IEnumerator COStunPause(float pauseTime)
    {
        yield return new WaitForSeconds(pauseTime);
    }

    private void Update()
    {
        z = Input.GetAxisRaw("Horizontal");
		x = -(Input.GetAxisRaw("Vertical"));
        _inputVec = new Vector3(x, 0, z);
        
        CheckGroundStatus();
        // 법선임 hitcast 할 때 씀
        _inputVec = Vector3.ProjectOnPlane(_inputVec, _groundNormal);

        // 공격 모션 키 입력
        if (Input.GetButtonDown("Fire1"))
        {
            if ((_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")
                || _animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                && _isGrounded)
            {
                _animator.SetTrigger("Attack");
                StartCoroutine(COStunPause(.6f));
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))    _roll = true;
        else                            _roll = false;

        // 앉아다니기 모션 키 입력
        if (Input.GetKey(KeyCode.C))    _crouch = true;
        else                                _crouch = false;

        // 점프 모션 키 입력
        if (Input.GetKeyDown(KeyCode.Space))    _jump = true;
        else                                    _jump = false;
        
        ScaleCapsuleForCrouching();
        ScaleCapsuleForRolling();
        PreventStandingInLowHeadroom();
        if (_isGrounded)        HandleGroundedMovement();
        else                    HandleAirborneMovement();

        UpdateMovement();
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

    // 앉을 때 캡슐 조절
    void ScaleCapsuleForCrouching()
    {
        if (_crouch)
        {
            if (_crouchCheck)
            {
                return;
            }
            _capsule.height = _capsule.height / 2f;
            _capsule.center = _capsule.center / 2f;
            _crouchCheck = true;
        }
        else
        {
            Ray crouchRay = new Ray(_rigidbody.position + Vector3.up *
                _capsule.radius * 0.5f, Vector3.up);
            float crouchRayLength = _capsuleHeight - _capsule.radius * 0.5f;
            if (Physics.SphereCast(crouchRay, _capsule.radius * 0.5f,
                crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                _crouchCheck = true;
                return;
            }
            _capsule.height = _capsuleHeight;
            _capsule.center = _capsuleCenter;
            _crouchCheck = false;
        }
    }

    // 추후 개발
    void ScaleCapsuleForRolling()
    {
        if (_roll)
        {
            if (_rollCheck)
            {
                return;
            }
            _rollCheck = true;
        }
        else
        {
            _rollCheck = false;
        }
    }

    void ScaleTrasformForRolling()
    {
        if(_rollCheck)
        {
            // 구를때 콜라이더가 따로 놀고 자세히 보면 미묘하게 뒤로감 이걸 처리하려고함
        }
    }

    // 낮은 곳에서 앉은 키 때도 앉음
    void PreventStandingInLowHeadroom()
    {
        // prevent standing up in crouch-only zones
        if (!_crouchCheck)
        {
            Ray crouchRay = new Ray(_rigidbody.position + Vector3.up *
                _capsule.radius * _halfF, Vector3.up);
            float crouchRayLength = _capsuleHeight - _capsule.radius * _halfF;

            if (Physics.SphereCast(crouchRay, _capsule.radius * _halfF,
                crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                _crouchCheck = true;
            }
        }
    }

    void HandleGroundedMovement()
    {
        // 점프 조건 1. 앉아 있지 않기 2. Idle 상태 3. 뛰는 상태
        if (_jump && !_crouch && (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")
            || _animator.GetCurrentAnimatorStateInfo(0).IsName("Run")))
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
        // 웅크리기 체크
        _animator.SetBool("Crouch", _crouchCheck);
        // 땅바닥에 있는가? -> 점프 체크
        _animator.SetBool("OnGround", _isGrounded);
        // 구르기 체크
        _animator.SetBool("Roll", _rollCheck);

        // 이동 키를 눌렀을 경우 체크
        if (x != 0 || z != 0)   _animator.SetBool("Moving", true);
        else                    _animator.SetBool("Moving", false);

        // 땅에 붙어 있지 않으면 y축의 속도를 잼 -> 가속도를 재는 것
        if (!_isGrounded)       _animator.SetFloat("Jump", _rigidbody.velocity.y);
        RotateTowardMovementDirection();
        GetCameraRelativeMovement();
    }

    // 구형보간 
    void RotateTowardMovementDirection()
    {
        if (_inputVec != Vector3.zero && (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")
            || _animator.GetCurrentAnimatorStateInfo(0).IsName("Run")
            || _animator.GetCurrentAnimatorStateInfo(0).IsName("Crouch")))
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
}