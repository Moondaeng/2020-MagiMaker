using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonPara : CharacterPara
{
    public string name;
    public int exp { get; set; }
    public int rewardMonsterey { get; set; }
    public override void InitPara()
    {
        name = "Skeleton";
        level = 1;
        maxHp = 50;
        curHp = maxHp;
        attackMin = 2;
        attackMax = 5;
        defense = 1;

        exp = 10;
        rewardMonsterey = Random.Range(10, 31);

        isDead = false;


    }

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();
    }
}