using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class CMultiDoll : MonoBehaviour
{
    [SerializeField] public float _jumpPower = 12f;
    [Range(1f, 4f)] [SerializeField] float _gravityMultiplier = 3.5f;
    [SerializeField] float _groundCheckDistance = 0.5f;
    private CEventTester _test;
    private Animator _animator;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _test = GetComponent<CEventTester>();
        _test.Attack.AddListener(Attack);
        _test.Jump.AddListener(Jump);
    }

    // 초안 
    /*멀티 플레이어의 키 입력값을 event에 추가시켜서 밑의 함수들을 리스너로 쓰기.
      보간하는 간격은 난 모르겟음...
      Bool 값은 유지되는 상태, Trigger값은 단발성 상태*/

    public void MoveTo(Vector3 targetPos)
    {
        transform.rotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.position = Vector3.MoveTowards(transform.position,
                             targetPos, 1 * Time.deltaTime);

        //(transform.position - targetPos).magnitude / 

        Move(0, 0);
        CancelInvoke("MoveExit");   // 중복 호출 시 걷다가 멈추는 현상 방지
        Invoke("MoveExit", 0.15f);
    }

    public void RollTo(Vector3 targetPos)
    {
        transform.rotation = Quaternion.LookRotation(targetPos - transform.position);
        Roll();
    }

    public void Move(float x, float z)
    {
        // x, z 값을 받아와서 lerp 하기

        _animator.SetBool("Moving", true);
    }

    public void MoveExit()
    {
        // lerp 끝날 시 호출

        _animator.SetBool("Moving", false);
    }

    public void Attack()
    {
        _animator.SetTrigger("Attack");
    }
    
    public void Skill()
    {
        _animator.SetTrigger("Skill1");
    }

    public void Jump()
    {
        _animator.SetTrigger("Jump");
    }

    public void Roll()
    {
        _animator.SetTrigger("Roll");
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
}
