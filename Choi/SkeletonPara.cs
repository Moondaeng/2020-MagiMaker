using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkeletonPara : CharacterPara
{
    public string name;
    public int exp { get; set; }
    public int rewardEnemyey { get; set; }
    public Image hpBar;
    public override void InitPara()
    {
        name = "Skeleton";
        level = 1;
        maxHp = 50;
        curHp = maxHp;
        attackMin = 5;
        attackMax = 10;
        defense = 1;
        eLevel = 0;
        eType = EElementType.earth;

        exp = 10;
        rewardEnemyey = Random.Range(10, 31);

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