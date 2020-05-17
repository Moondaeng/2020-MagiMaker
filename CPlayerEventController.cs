using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerEventController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    public void SendAttackEnemy()
    {
        transform.root.gameObject.SendMessage("AttackCalculate");
    }

    // Update is called once per frame
    void Update()
    {

    }
}