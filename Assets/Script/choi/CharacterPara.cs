using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

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
    public int eLevel { get; set; }
    public EElementType eType {get; set;}
    public bool isDead { get; set; }

    [System.NonSerialized]
    public UnityEvent deadEvent = new UnityEvent();

    void Start()
    {
        InitPara();
    }

    public virtual void InitPara()
    {

    }

    public const int _elementTypeSize = 4;
    public static List<EElementType> elementTypeList =
           new List<EElementType>
           {
                EElementType.fire,
                EElementType.wind,
                EElementType.water,
                EElementType.earth,
                EElementType.none
           };

    public enum EElementType
    {
        none = -1, fire = 0, wind = 1, water = 2, earth = 3
    }

    public enum EElementTypeBonus
    {
        attackIsNone, defenceIsNone, advantage, disadvantage, noadvantage
    }

    public EElementTypeBonus FindAttacksElementTypeBonus(EElementType elementType, EElementType attacksElementType)
    {
        if (attacksElementType == EElementType.none)
            return EElementTypeBonus.attackIsNone;

        if (elementType == EElementType.none)
            return EElementTypeBonus.defenceIsNone;

        int advantage = ((int)attacksElementType - (int)elementType + _elementTypeSize) % 4;

        if (advantage == 1)
            return EElementTypeBonus.advantage;
        else if (advantage == 3)
            return EElementTypeBonus.disadvantage;
        else
            return EElementTypeBonus.noadvantage;
    }
    public static float[,] _elementTypeBonusArray = new float[,]
        {
            // 각각 공격이 무속성 / 방어가 무속성 / (공격이) 상성상 강함 / (공격이) 상성상 약함 / 무상성
            // 공격의 속성 레벨이 방어의 속성 레벨보다 낮은 경우
            {1.0f, 1.0f, 1.1f, 0.75f, 0.95f },
            // 공격의 속성 레벨이 방어의 속성 레벨과 같은 경우
            {1.0f, 1.05f, 1.2f, 0.8f, 1.00f },
            // 공격의 속성 레벨이 방어의 속성 레벨보다 높은 경우
            {1.0f, 1.1f, 1.3f, 0.85f, 1.05f }
        };

    // 공격자의 속성 레벨, 타입에 따라 추가 데미지 비율을 결정

    public float GetElementTypeBonusSize(int eLevel, EElementType eTypeDefence, EElementType eTypeAttack)
    {
        return _elementTypeBonusArray[(eLevel - eLevel + 1), (int)FindAttacksElementTypeBonus(eTypeDefence, eTypeAttack)];
    }
    
    // 평타 데미지 계산식
    public float GetRandomAttack(EElementType eTypeDefence, EElementType eTypeAttack)
    {
        float randAttack = UnityEngine.Random.Range(attackMin, attackMax + 1);
        float Bonus = GetElementTypeBonusSize(eLevel, eTypeDefence, eTypeAttack);
        // 최종 계산식 대충
        randAttack = (randAttack - defense) * Bonus;
        print(Bonus + "is advantage!!");
        return randAttack;
    }

    public void SetEnemyAttack(float EnemyAttackPower)
    {
        // 데미지를 버림 형식으로 표현
        curHp -= (int)EnemyAttackPower;
        UpdateAfterReceiveAttack();
    }

    //캐릭터가 적으로 부터 공격을 받은 뒤에 자동으로 실행될 함수를 가상함수로 만듬
    protected virtual void UpdateAfterReceiveAttack()
    {
        print(name + "'s HP: " + curHp);

        if (curHp <= 0)
        {
            curHp = 0;
            isDead = true;
            deadEvent.Invoke();
        }
    }
}
