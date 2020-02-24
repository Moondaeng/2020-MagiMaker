using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPara : CharacterPara
{
    public string name { get; set; }
    public int curExp { get; set; }
    public int expToNextLevel { get; set; }
    public int Monsterey { get; set; }

    public override void InitPara()
    {
        name = "hong";
        level = 1;
        maxHp = 100;
        curHp = maxHp;
        maxMp = 10;
        curMp = maxMp;
        attackMin = 5;
        attackMax = 8;
        defense = 1;

        curExp = 0;
        expToNextLevel = 100 * level;
        Monsterey = 0;

        isDead = false;
    }

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();
    }
}