using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPara : MonoBehaviour
{
    public int level { get; set; }
    public int maxHp { get; set; }
    public int curHp { get; set; }
    public int maxMp { get; set; }
    public int curMp { get; set; }
    public int attackMin { get; set; }
    public int attackMax { get; set; }
    public int defense { get; set; }
    public bool isDead { get; set; }

    void Start()
    {
        InitPara();
    }

    public virtual void InitPara()
    {

    }

    public int GetRandomAttack()
    {
        int randAttack = Random.Range(attackMin, attackMax + 1);
        return randAttack;
    }

    public void SetMonsterAttack(int MonsterAttackPower)
    {
        curHp -= MonsterAttackPower;
        UpdateAfterReceiveAttack();
    }

    //캐릭터가 적으로 부터 공격을 받은 뒤에 자동으로 실행될 함수를 가상함수로 만듬
    protected virtual void UpdateAfterReceiveAttack()
    {
        print(name + "'s HP: " + curHp);
    }

}