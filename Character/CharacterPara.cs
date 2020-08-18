using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

<<<<<<< HEAD
public class CharacterPara : MonoBehaviour
{
    public int maxHp { get; set; }
    public int curHp { get; set; }
    public int attackMin { get; set; }
    public int attackMax { get; set; }
    public int defense { get; set; }
    public int eLevel { get; set; }
    public EElementType eType {get; set;}
    public bool isAnotherAction { get; set; }
    public bool isStunned { get; set; }
    public bool isDead { get; set; }
    public int rewardMoney { get; set; }

    //석래가 추가한 부분
    public int _attackSpeed { get; set; }
    public int _damageTakeRate { get; set; }
    public int _HPRegenPerSec { get; set; }
    //추가한 부분 끝

    [System.NonSerialized]
    public UnityEvent deadEvent = new UnityEvent();

    protected CTimer _buffTimer;

    public int RandomAttackDamage()
    {
        int random = UnityEngine.Random.Range(attackMin, attackMax);
        return random;
=======
[System.Serializable]
public class DamageEvent : UnityEvent<int, int>
{

}

[RequireComponent(typeof(CBuffTimer))]
public class CharacterPara : MonoBehaviour
{
    public int _maxHp { get; set; }
    public int _curHp { get; set; }
    public int _attackMin { get; set; }
    public int _attackMax { get; set; }
    public int _defense { get; set; }
    public bool _isAnotherAction { get; set; }
    public bool _isStunned { get; set; }
    public bool _isDead { get; set; }
    public int _rewardMoney { get; set; }

    // ((캐릭터 공격력 * 공격력 증가) + 장비 공격력) * 버프로 올라가는 공격력 %
    public virtual int TotalAttackMin
    {
        get { return (int)(_attackMin * buffParameter.AttackCoef * buffParameter.AttackDebuffCoef); }
    }
    public virtual int TotalAttackMax
    {
        get { return (int)(_attackMax * buffParameter.AttackCoef * buffParameter.AttackDebuffCoef); }
    }
    public virtual int TotalDefenece
    {
        get { return (int)(_defense * buffParameter.DefenceCoef * buffParameter.DefenceDebuffCoef); }
    }
    
    [System.NonSerialized]
    public UnityEvent deadEvent = new UnityEvent();
    public DamageEvent damageEvent = new DamageEvent();

    protected CBuffTimer _buffTimer;
    public CBuffPara buffParameter;

    public int RandomAttackDamage()
    {
        int _random = UnityEngine.Random.Range(_attackMin, _attackMax);
        return _random;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    protected void Awake()
    {
<<<<<<< HEAD
        // 변경 예정 : Player 소유의 Buff Timer로
        //_buffTimer = GameObject.Find(transform.name).GetComponent<CBuffTimer>();
        _buffTimer = GameObject.Find("SkillScript").GetComponent<CBuffTimer>();
=======
        _buffTimer = gameObject.GetComponent<CBuffTimer>();
        buffParameter = new CBuffPara(_buffTimer);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    void Start()
    {
        InitPara();
    }

    public virtual void InitPara()
    {

    }

<<<<<<< HEAD
=======
    // 평타 데미지 계산식
    public float GetRandomAttack()
    {
        float randAttack = UnityEngine.Random.Range(_attackMin, _attackMax + 1);
        // 최종 계산식 대충
        randAttack = randAttack - _defense;
        return randAttack;
    }

    public void SetEnemyAttack(float EnemyAttackPower)
    {
        // 데미지를 버림 형식으로 표현
        _curHp -= (int)EnemyAttackPower;
        //transform.gameObject.SendMessage("hitEnemyAttack");
        UpdateAfterReceiveAttack();
    }

    // 방어력 계산식: 1000 / (950 + 10*방어력)
    public void DamegedRegardDefence(int enemyAttack)
    {
        int damage = 1000 / (950 + 10*TotalDefenece);
        DamagedDisregardDefence(damage);
    }

    public void DamagedDisregardDefence(int enemyAttack)
    {
        _curHp -= (int)enemyAttack;
        UpdateAfterReceiveAttack();
    }

    //캐릭터가 적으로 부터 공격을 받은 뒤에 자동으로 실행될 함수를 가상함수로 만듬
    protected virtual void UpdateAfterReceiveAttack()
    {
        print(name + "'s HP: " + _curHp);
        // 체력 관련 이벤트
        damageEvent?.Invoke(_curHp, _maxHp);

        if (_curHp <= 0)
        {
            _curHp = 0;
            _isDead = true;
            deadEvent.Invoke();
        }
    }

    #region ObsoleteCode
    // 기획안에서 없어진 부분
    public int _eLevel { get; set; }
    public EElementType _eType { get; set; }

>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
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
<<<<<<< HEAD
    
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
    
    // 버프 스킬군
    public virtual void BuffAttack(float time, float buffScale)
    {
        Debug.LogFormat("Character Attack buff!");
        StartBuffAttack(buffScale);
        // 버프 효과 끝내기 수치 넣을 땐 필요에 따라 커링을 사용
        Debug.LogFormat("Character Attack buff! : ");
        _buffTimer.Register(1, time, () => EndBuffAttack(buffScale));
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
    
    protected virtual void StartBuffAttack(float buffScale)
    {
        float tempAttackMin = attackMin;
        float tempAttackMax = attackMax;
        tempAttackMin *= buffScale;
        tempAttackMax *= buffScale;
        attackMin = (int)tempAttackMin;
        attackMax = (int)tempAttackMax;
    }

    protected void EndBuffAttack(float buffScale)
    {
        float tempAttackMin = attackMin;
        float tempAttackMax = attackMax;
        tempAttackMin /= buffScale;
        tempAttackMax /= buffScale;
        attackMin = (int)tempAttackMin;
        attackMax = (int)tempAttackMax;
    }
=======

    // 평타 데미지 계산식
    public float GetRandomAttack(EElementType eTypeDefence, EElementType eTypeAttack)
    {
        float randAttack = UnityEngine.Random.Range(_attackMin, _attackMax + 1);
        float Bonus = GetElementTypeBonusSize(_eLevel, eTypeDefence, eTypeAttack);
        // 최종 계산식 대충
        randAttack = (randAttack - _defense) * Bonus;
        return randAttack;
    }
    #endregion
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
}
