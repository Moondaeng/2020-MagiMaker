using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGoblinPara : CharacterPara
{
    public string name;
    public int exp { get; set; }
    public int rewardEnemyey { get; set; }
    public Image hpBar;
    public override void InitPara()
    {
        name = "Wind Goblin";
        /*
        maxHp = 20;
        curHp = maxHp;
        attackMin = 3;
        attackMax = 5;
        defense = 0;
        eLevel = 0;
        eType = EElementType.wind;

        exp = 10;
        rewardEnemyey = Random.Range(10, 31);

        isDead = false;
        */

        InitHpBarSize();
    }

    void InitHpBarSize()
    {
        hpBar.rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();

        hpBar.rectTransform.localScale = new Vector3((float)_curHp / (float)_maxHp, 1f, 1f);
    }
}