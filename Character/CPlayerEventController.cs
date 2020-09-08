using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerEventController : MonoBehaviour
{
    public void SendAttackEnemy()
    {
        transform.root.gameObject.SendMessage("AttackCalculate");
    }
}