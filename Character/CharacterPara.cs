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
public class CharacterPara : MonoBehaviour
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
    public int CurrentHp
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

    #region UseEffect 변수
    protected const float DoTCheckTime = 0.1f;

    protected class DamageOfTime
    {
        public static readonly float TimeBonus = 0.2f;

        public int id;
        public CUseEffect.HpChange Amount;
        public float CurrentTime;
        public float Period;

        public DamageOfTime(int id, CUseEffect.HpChange amount, float currentTime, float period)
        {
            this.id = id;
            Amount = amount;
            CurrentTime = currentTime;
            Period = period;
        }
    }

    [System.Serializable]
    protected class PersistStack
    {
        public int id;
        public int operatedStack;

        public PersistStack(int id, int operatedStack)
        {
            this.id = id;
            this.operatedStack = operatedStack;
        }
    }

    protected float[] _buffCoef = new float[Enum.GetValues(typeof(EBuffAbility)).Length];
    protected float[] _debuffCoef = new float[Enum.GetValues(typeof(EBuffAbility)).Length];
    protected List<DamageOfTime> _DoTList = new List<DamageOfTime>();
    protected List<PersistStack> _PersistStackList = new List<PersistStack>();
    
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

    #region UseEffect
    public void TakeUseEffectHandleList(List<CUseEffectHandle> effects)
    {
        foreach(var effect in effects)
        {
            effect.TakeUseEffect(this);
        }
    }

    public virtual void TakeUseEffect(CUseEffect effect)
    {
        if (effect == null)
        {
            return;
        }

        ApplyInstantEffect(effect.instantEffect);
        ApplyConditionalEffect(effect.conditionalEffect);
        ApplyPersistEffect(effect.persistEffect);
    }

    protected void ApplyInstantEffect(CUseEffect.InstantEffect instantEffect)
    {
        // 데미지 효과 지정
        ApplyHpChange(instantEffect.hpChange);
            
        // 그 외 효과 지정
    }

    protected void ApplyConditionalEffect(CUseEffect.ConditionalEffect conditionalEffect)
    {
        if(!conditionalEffect.IsValid())
        {
            return;
        }

        int effectStack = _buffTimer.GetBuffStack(conditionalEffect.conditionEffectId);
        if (effectStack >= 1)
        {
            // 스택 관련 옵션 지정 - 클래스라 값이 누적되는지 확인 필요
            if (conditionalEffect.isRelationStack)
            {
                conditionalEffect.effect.instantEffect.MultiplyPersant(effectStack * conditionalEffect.stackBonusRate);
            }

            TakeUseEffect(conditionalEffect.effect);
        }
    }

    // 지속 효과 적용 방안
    // 버그 발생 확인 필요 : lambda로 캡쳐한 변수가 시간이 지나면 달라질 수 있음
    // 해결 방안 2 : Timer에는 id만 보내고 실제 내용은 CharacterPara에 LinkedList 만들어서 관리하기
    protected void ApplyPersistEffect(CUseEffect.PersistEffect persistEffect)
    {
        // 초기화 값이 들어가는거 방지용
        if (persistEffect.IsValid())
        {
            _buffTimer.Register(persistEffect.id, persistEffect.time, persistEffect.maxStack, persistEffect.increaseStack,
                (int buffStack) => StartPersistEffect(persistEffect, buffStack),
                (int buffStack) => EndPersistEffect(persistEffect, buffStack));
        }
    }

    // 지속 효과 시작
    protected void StartPersistEffect(CUseEffect.PersistEffect persistEffect, int stack)
    {
        // 지속 데미지(힐) 관련은 CCharacterPara에서 Update or Coroutine으로 관리
        // Coroutine 사용 이유 : 체력 변화는 오직 CCharacterPara 안에서만 일어나는 일
        // 체력 변화 대상 관리 등 복잡한 연산은 Timer에서 수행
        if(persistEffect.TickHpChange.IsValid() && _DoTList.Find(x => x.id == persistEffect.id) == null)
        {
            _DoTList.Add(new DamageOfTime(persistEffect.id, persistEffect.TickHpChange, DamageOfTime.TimeBonus, persistEffect.TickPeriod));
        }

        // CC 관련

        // 능력치 변화 관련
        foreach (var changeAbility in persistEffect.changeAbilities)
        {
            StartChangeAbility(changeAbility, stack);
        }

        // 스택 추가 효과 관련
        StackAccumalateEffect(persistEffect.id, persistEffect.stackAccumulateEffect);
    }

    // 지속 효과 끝
    protected void EndPersistEffect(CUseEffect.PersistEffect persistEffect, int stack)
    {
        // 지속 데미지(힐) 관련은 CCharacterPara에서 Update or Coroutine으로 관리
        // Coroutine 사용 이유 : 체력 변화는 오직 CCharacterPara 안에서만 일어나는 일
        // 체력 변화 대상 관리 등 복잡한 연산은 Timer에서 수행
        _DoTList.Remove(_DoTList.Find(x => x.id == persistEffect.id));

        // CC 관련

        // 능력치 변화 관련 
        foreach (var changeAbility in persistEffect.changeAbilities)
        {
            EndChangeAbility(changeAbility, stack);
        }

        // 스택 효과 관련
        if(_buffTimer.GetBuffStack(persistEffect.id) <= 0)
        {
            _PersistStackList.Remove(_PersistStackList.Find(x => x.id == persistEffect.id));
        }
    }

    // 체력 변화 처리함수
    protected void ApplyHpChange(CUseEffect.HpChange hpChange)
    {
        if (hpChange.isHeal)
        {
            ApplyHeal(hpChange);
        }
        else
        {
            ApplyDamage(hpChange);
        }
    }

    protected void ApplyHeal(CUseEffect.HpChange hpChange)
    {
        int ratioToAmount;
        switch (hpChange.ratioType)
        {
            case CUseEffect.HpChange.EHpRatioType.Current:
                Debug.LogWarning("힐에 현재 체력 비례 적용 불가");
                ratioToAmount = hpChange.RatioToAmount(CurrentHp);
                break;
            case CUseEffect.HpChange.EHpRatioType.Max:
                ratioToAmount = hpChange.RatioToAmount(TotalMaxHp);
                break;
            case CUseEffect.HpChange.EHpRatioType.Lost:
                ratioToAmount = hpChange.RatioToAmount(TotalMaxHp - CurrentHp);
                break;
            default:
                Debug.LogWarning("Not Defined Ratio Type");
                ratioToAmount = 0;
                break;
        }
        Heal(hpChange.fixedAmount + hpChange.enhancePercentRate + ratioToAmount);
    }

    protected void ApplyDamage(CUseEffect.HpChange hpChange)
    {
        int ratioToAmount;
        switch (hpChange.ratioType)
        {
            case CUseEffect.HpChange.EHpRatioType.Current:
                ratioToAmount = hpChange.RatioToAmount(CurrentHp);
                break;
            case CUseEffect.HpChange.EHpRatioType.Max:
                ratioToAmount = hpChange.RatioToAmount(TotalMaxHp);
                break;
            case CUseEffect.HpChange.EHpRatioType.Lost:
                ratioToAmount = hpChange.RatioToAmount(TotalMaxHp - CurrentHp);
                break;
            default:
                Debug.LogWarning("Not Defined Ratio Type");
                ratioToAmount = 0;
                break;
        }

        int totalDamage = hpChange.fixedAmount + hpChange.enhancePercentRate + ratioToAmount;
        if (hpChange.isTrueDamage)
        {
            DamagedDisregardDefence(totalDamage);
        }
        else
        {
            DamegedRegardDefence(totalDamage);
        }
    }

    // 틱당 체력 변화 코루틴
    protected IEnumerator GetDamageOfTime()
    {
        while(true)
        {
            foreach(var DoT in _DoTList)
            {
                // 부동 소수점 계산으로 틱 계산이 불안정 - 틱을 손해볼 수 있음. 개선 필요
                if (DoT.CurrentTime >= DoT.Period)
                {
                    ApplyHpChange(DoT.Amount);
                    DoT.CurrentTime -= DoT.Period;
                }

                DoT.CurrentTime += DoTCheckTime;
            }
            yield return new WaitForSeconds(DoTCheckTime);
        }
    }

    protected void StartChangeAbility(CUseEffect.ChangeAbilityInfo changeAbility, int stack)
    {
        if (changeAbility.isBuff)
        {
            _buffCoef[(int)changeAbility.ability] += (changeAbility.increaseBase + changeAbility.increasePerStack * stack) * 0.01f;
        }
        else
        {
            _debuffCoef[(int)changeAbility.ability] *= 1.00f + (changeAbility.increaseBase + changeAbility.increasePerStack * stack) * 0.01f;
        }
    }

    protected void EndChangeAbility(CUseEffect.ChangeAbilityInfo changeAbility, int stack)
    {
        if (changeAbility.isBuff)
        {
            _buffCoef[(int)changeAbility.ability] -= (changeAbility.increaseBase + changeAbility.increasePerStack * stack) * 0.01f;
        }
        else
        {
            _debuffCoef[(int)changeAbility.ability] /= 1.00f + (changeAbility.increaseBase + changeAbility.increasePerStack * stack) * 0.01f;
        }
    }

    protected void StackAccumalateEffect(int id, CUseEffect.StackAccumulateEffect stackAccumEffect)
    {
        if(stackAccumEffect.threshold == 0)
        {
            return;
        }

        int effectStack = _buffTimer.GetBuffStack(id);
        if (effectStack >= stackAccumEffect.threshold)
        {
            PersistStack persistStack;
            // 이미 효과가 발동됐는지 확인
            if ((persistStack = _PersistStackList.Find(x => x.id == id)) != null
                && persistStack.operatedStack >= effectStack)
            {
                return;
            }

            // 발동되지 않은 경우 다음 한계치 설정하기
            // 발동했을 때의 스택 계산 : effectStack > operateStack + n * limit인 operateStack + n * limit의 최대치
            // ex) 6에서 발동하는데 처음 발동한 스택이 12인 경우, 그리고 다음 발동 스택이 18인 경우 이전 발동 스택은 12로 계산되며 6은 발동하면 안 됨
            int previousOperatedStack = stackAccumEffect.threshold;
            while (previousOperatedStack + stackAccumEffect.thresholdAdd <= effectStack)
            {
                previousOperatedStack += stackAccumEffect.thresholdAdd;
            }
            if (persistStack == null)
            {
                _PersistStackList.Add(new PersistStack(id, previousOperatedStack));
            }
            else
            {
                persistStack.operatedStack = previousOperatedStack;
            }

            TakeUseEffect(stackAccumEffect.effect);
        }
    }
    #endregion

    // 방어력 계산식: 1000 / (950 + 10*방어력)
    public void DamegedRegardDefence(int enemyAttack)
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