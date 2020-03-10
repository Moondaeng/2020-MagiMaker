using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CPlayerPara : CharacterPara
{
    public string name { get; set; }
    public int curExp { get; set; }
    public int expToNextLevel { get; set; }
    public int money { get; set; }

    public override void InitPara()
    {
        name = "hong";
        maxHp = 1000;
        curHp = maxHp;
        attackMin = 50;
        attackMax = 80;
        defense = 30;
        eLevel = 0;
        eType = EElementType.none;
        money = 0;
        isAnotherAction = false;
        isStunned = false;
        isDead = false;
        CUIManager.instance.UpdatePlayerUI(this);
    }
    

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();

        CUIManager.instance.UpdatePlayerUI(this);
    }
}