using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class CMultiDoll : MonoBehaviour
{
    public enum EAction
    {
        Idle,
        Run,
        Jump,
        Roll,
        Attack1,
        Skill1,
        Stun,
        Knockback,
        Dead = 10,
    }

    #region Properties
    private Rigidbody _rigidbody;
    [SerializeField] public float _jumpPower = 12f;
    [Range(1f, 4f)] [SerializeField] float _gravityMultiplier = 3.5f;
    [SerializeField] float _groundCheckDistance = 0.5f;
    private AnimatorStateInfo _currentBaseState;

    // 법선 벡터 (
    private Vector3 _groundNormal;

    // 애니메이션 상태값
    private bool _isGrounded;
    private bool _jump;

    // Run 보간용
    [SerializeField] float moveSpeed = 5.5f;

    private bool _isActing = false;
    private CEventTester _test;
    private Animator _animator;
    #endregion

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator.SetFloat("Jump", -4f);

        //_test = GetComponent<CEventTester>();
        //_test.Attack.AddListener(Attack);
        //_test.Jump.AddListener(Jump);
    }

    private void Update()
    {
        _animator.SetBool("OnGround", _isGrounded);

        CheckGroundStatus();
        if (_isGrounded) HandleGroundedMovement();
        if (!_isGrounded) _animator.SetFloat("Jump", _rigidbody.velocity.y);
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

    // 초안 
    /*멀티 플레이어의 키 입력값을 event에 추가시켜서 밑의 함수들을 리스너로 쓰기.
      보간하는 간격은 난 모르겟음...
      Bool 값은 유지되는 상태, Trigger값은 단발성 상태*/

    public void MoveTo(Vector3 targetPos)
    {
        if (_isActing)
        {
            return;
        }

        targetPos.y = transform.position.y;
        transform.rotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.position = Vector3.MoveTowards(transform.position,
                             targetPos, 1 * Time.deltaTime);

        _animator.SetInteger("Motion", (int)EAction.Run);
        CancelInvoke("RunEnd"); // 이전 달리기 명령 취소
        Invoke("RunEnd", Vector3.Distance(transform.position, targetPos) / moveSpeed);    // 거리와 move speed를 보고 끄기
    }

    private void RunEnd()
    {
        _animator.SetInteger("Motion", (int)EAction.Idle);
    }

    public void RollTo(Vector3 rotateDirection)
    {
        transform.rotation = Quaternion.Euler(rotateDirection);
        Act(EAction.Roll, 0.6f);
    }

    public void AttackTo(Vector3 rotateDirection)
    {
        transform.rotation = Quaternion.Euler(rotateDirection);
        Act(EAction.Attack1, 0.8f);
    }
    
    public void Skill()
    {
        Act(EAction.Skill1, 1.2f);
    }

    public void JumpTo(Vector3 rotateDirection)
    {
        transform.rotation = Quaternion.Euler(rotateDirection);
        Act(EAction.Jump, 0.6f);
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _jumpPower,
                _rigidbody.velocity.z);
    }

    public void Stun()
    {
        _animator.SetTrigger("StunTrigger");
        _animator.SetBool("Stun", true);
    }

    public void StunExit()
    {
        _animator.SetBool("Stun", false);
    }

    public void Dead()
    {
        _animator.SetTrigger("DeadTrigger");
    }

    private void Act(EAction action, float endTime)
    {
        // 행동 중 이동에 따른 행동 씹힘 방지
        _isActing = true;
        CancelInvoke("RunEnd");

        _animator.SetInteger("Motion", (int)action);
        Invoke("ActEnd", endTime);
    }

    private void ActEnd()
    {
        _isActing = false;
        _animator.SetInteger("Motion", (int)EAction.Idle);
    }

    // 점프할 발 체크 추후에 여기에 사운드 추가
    // 애니메이션 placeholder에 들어가있는 함수
    void FootR()
    {
    }

    void FootL()
    {
    }

    void Hit()
    {
    }

    void AttackDisabledRightCollider()
    {

    }

    void SetRightStartPoint()
    {

    }

    void RollStart()
    {

    }
}
