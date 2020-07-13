using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUserMelee : MonoBehaviour
{
    Collider _myCol;
    Collider _obj;
    Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _myCol = GetComponent<Collider>();
        _anim = transform.root.GetComponent<Animator>();
    }

    public void SendAttackEnemy()
    {
        transform.root.gameObject.SendMessage("AttackCalculate", _obj);
    }

    IEnumerator CheckAnimationState()
    {
        while (!_anim.GetCurrentAnimatorStateInfo(0)
        .IsName("Attack"))
        {
            //전환 중일 때 실행되는 부분
            _myCol.enabled = false;
            yield return null;
        }
        while (_anim.GetCurrentAnimatorStateInfo(0)
        .normalizedTime < 0.8f)
        {
            //애니메이션 재생 중 실행되는 부분
            _myCol.enabled = true;
            yield return null;
        }
        _myCol.enabled = false;
        yield break;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.transform.root.tag == "Monster"
            && _anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            _obj = col;
            SendAttackEnemy();
            
        }
    }
    private void Update()
    {
        StartCoroutine("CheckAnimationState");
    }
}
