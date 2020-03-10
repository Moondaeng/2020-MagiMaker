using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSkeletonPara : CharacterPara
{
    public string name;
    public int rewardMoney { get; set; }
    public Image hpBar;
    public override void InitPara()
    {
        name = "Skeleton";
        maxHp = 50;
        curHp = maxHp;
        attackMin = 5;
        attackMax = 10;
        defense = 1;
        eLevel = 0;
        eType = EElementType.earth;
        rewardMoney = Random.Range(10, 31);
        isStunned = false;
        isDead = false;

        InitHpBarSize();
    }

    void InitHpBarSize()
    {
        hpBar.rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();

        hpBar.rectTransform.localScale = new Vector3((float)curHp / (float)maxHp, 1f, 1f);
    }
}