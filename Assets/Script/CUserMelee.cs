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

    public void SendAttackSkeleton()
    {
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.transform.root.tag == "Monster"
            && _anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            _obj = col;
            SendAttackEnemy();
            StartCoroutine("WaitForSec");
        }
    }
}
