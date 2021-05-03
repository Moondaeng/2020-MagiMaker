using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

#region 캐릭터 이벤트
[System.Serializable]
public class DamageEvent : UnityEvent<int, int>
{

}

public class HitEvent : UnityEvent<int>
{

}

/// <summary>
/// 체력이 바뀌는 모든 이벤트(데미지, 힐 등)
/// int : 체력 변화량
/// </summary>
public class HpChanageEvent : UnityEvent<int> { }

// UI
public class HpDrawEvent : UnityEvent<int, int> { }
#endregion

[RequireComponent(typeof(CBuffTimer))]
public partial class CharacterPara : MonoBehaviour
{
    #region 총 능력치
    public virtual int TotalAttackMin
    {
        get { return (int)(_attackMin * _buffCoef[(int)EBuffAbility.Attack] * _debuffCoef[(int)EBuffAbility.Attack]); }
    }
    public virtual int TotalAttackMax
    {
        get { return (int)(_attackMax * _buffCoef[(int)EBuffAbility.Attack] * _debuffCoef[(int)EBuffAbility.Attack]); }
    }
    public virtual int TotalDefenece
    {
        get { return (int)(_defense * _buffCoef[(int)EBuffAbility.Defence] * _debuffCoef[(int)EBuffAbility.Defence]); }
    }
    public virtual int TotalMaxHp
    {
        get { return _maxHp; }
        protected set
        {
            var previousMaxHp = TotalMaxHp;
            _maxHp = value;
            if(previousMaxHp < value)
            {
                CurrentHp = (int)(value * ((float)CurrentHp / previousMaxHp));
            }
            else if(value < CurrentHp)
            {
                CurrentHp = value;
            }
        }
    }
    #endregion

    #region 캐릭터 기본 능력치
    public virtual int CurrentHp
    {
        get { return _curHp; }
        protected set 
        {
            if(value < 0)
            {
                _curHp = 0;
            }
            else if(value > TotalMaxHp)
            {
                _curHp = TotalMaxHp;
            }
            else
            {
                _curHp = value;
            }
            hpDrawEvent?.Invoke(CurrentHp, TotalMaxHp);
        }
    }

    [Header("Status")]
    [Tooltip("최대 체력")] [SerializeField] public int _maxHp;
    [HideInInspector] public int _curHp;
    [Tooltip("최소 공격력")] [SerializeField] public int _attackMin;
    [Tooltip("최대 공격력")] [SerializeField] public int _attackMax;
    [Tooltip("방어력")] [SerializeField] public int _defense;
    public bool _isAnotherAction { get; set; }
    public bool _isStunned { get; set; }
    public bool _isDead { get; set; }
    [SerializeField] public int _rewardMoney;
    public int _spawnID { get; set; }
    [Tooltip("히트 애니메이션 출력\n최대체력 비율")] public float _hitGauge;
    #endregion

    

    #region 이벤트
    [System.NonSerialized]
    public UnityEvent hitEvent = new UnityEvent();
    public UnityEvent deadEvent = new UnityEvent();
    [System.NonSerialized]
    public HpChanageEvent damageEvent = new HpChanageEvent();
    [System.NonSerialized]
    public HpChanageEvent healEvent = new HpChanageEvent();

    public HitEvent hitGaugeEvent = new HitEvent();
    [System.NonSerialized]
    public HpDrawEvent hpDrawEvent = new HpDrawEvent();
    #endregion

    protected CBuffTimer _buffTimer;

    public int RandomAttackDamage()
    {
        int _random = UnityEngine.Random.Range(_attackMin, _attackMax);
        return _random;
    }

    protected virtual void Awake()
    {
        _buffTimer = gameObject.GetComponent<CBuffTimer>();
        for (int i = 0; i < Enum.GetValues(typeof(EBuffAbility)).Length; i++)
        {
            _buffCoef[i] = 1.0f;
            _debuffCoef[i] = 1.0f;
        }

        // 파라미터가 다른 이벤트 처리
        InitPara();
    }

    protected virtual void Start()
    {
        StartCoroutine("GetDamageOfTime");
    }

    public virtual void InitPara()
    {
        hitGaugeEvent.AddListener(HitGaugeCalculate);
    }

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
        CurrentHp -= (int)EnemyAttackPower;
        //transform.gameObject.SendMessage("hitEnemyAttack");
        UpdateAfterReceiveAttack();
    }

    // 방어력 계산식: 1000 / (950 + 10*방어력)
    public virtual void DamegedRegardDefence(int enemyAttack)
    {
        int damage = enemyAttack * 1000 / (950 + 10 * TotalDefenece);
        DamagedDisregardDefence(damage);
    }

    public void DamagedDisregardDefence(int enemyAttack)
    {
        CurrentHp -= (int)enemyAttack;
        hitGaugeEvent?.Invoke(enemyAttack);
        UpdateAfterReceiveAttack();
    }

    //캐릭터가 적으로 부터 공격을 받은 뒤에 자동으로 실행될 함수를 가상함수로 만듬
    protected virtual void UpdateAfterReceiveAttack()
    {
        print(name + "'s HP: " + CurrentHp);
        // 체력 관련 이벤트

        if (CurrentHp <= 0 && !_isDead)
        {
            Debug.Log("Dead?");
            CurrentHp = 0;
            _isDead = true;

            deadEvent.Invoke();
        }
    }

    public virtual void HitGaugeCalculate(int attackDamage)
    {
        float result = (attackDamage * 100f) / _maxHp;
        if (result > _hitGauge)
        {
            Debug.Log("Hit Event Before");
            hitEvent.Invoke();
        }
    }

    public virtual void Heal(int amount)
    {
        CurrentHp += amount;
        healEvent?.Invoke(amount);
    }
}