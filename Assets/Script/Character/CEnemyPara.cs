using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CEnemyPara : CharacterPara
{
    public string name;
    public Image hpBar;
    public GameObject selection;
    public ParticleSystem[] hitEffect;

    public override void InitPara()
    {
        JsonConvert.instance.loadToMonster(name, this);
        isStunned = false;
        isDead = false;
        curHp = maxHp;
        HideSelection();
        for(int i = 0; i < hitEffect.Length; i++)
            hitEffect[i].Stop();

        InitHpBarSize();
    }

    void InitHpBarSize()
    {
        hpBar.rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    protected override void UpdateAfterReceiveAttack()
    {
        base.UpdateAfterReceiveAttack();
        if(curHp == 0)
        {
            HideSelection();
        }
        hpBar.rectTransform.localScale = new Vector3(curHp / (float)maxHp, 1f, 1f);
    }

    public void HideSelection()
    {
        selection.SetActive(false);
    }

    public void ShowSelection()
    {
        selection.SetActive(true);
    }
    
    // 타격 이펙트 함수
    public void ShowHitEffect()
    {
        hitEffect[0].Play();
    }
}