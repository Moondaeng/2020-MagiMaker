﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemyMelee : MonoBehaviour
{
    Collider _myCol;
    Animator _anim;
    // Start is called before the first frame update
    void Start()
    {
        _myCol = GetComponent<Collider>();
        _anim = transform.root.GetComponent<Animator>();
        Debug.Log(_myCol);
        Debug.Log(_anim);
    }

    public void SendAttackEnemy()
    {
        transform.root.gameObject.SendMessage("AttackCalculate");
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" 
            && _anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            SendAttackEnemy();
        }
    }
}
