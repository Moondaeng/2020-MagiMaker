using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class CUseEffectExplain
{
    #region 정의된 사용 효과 관련
    public static Dictionary<int, string> DefinedUseEffectNameDict
        = new Dictionary<int, string>();

    #endregion

    private static readonly Dictionary<EBuffAbility, string> AbilityNameDict
            = new Dictionary<EBuffAbility, string>
        {
            {EBuffAbility.Attack, "공격력"},
            {EBuffAbility.Defence, "방어력"},
            {EBuffAbility.AttackSpeed, "공격속도"},
            {EBuffAbility.MoveSpeed, "이동속도"},
        };

    public static string CreateUseEffectText(CUseEffect useEffect)
    {
        if (useEffect == null)
        {
            return "";
        }

        StringBuilder sb = new StringBuilder();
        sb.Append(CreateInstantEffectText(useEffect.instantEffect))
            .Append(CreatePersistEffectText(useEffect.persistEffect))
            .Append(CreateConditionalEffectText(useEffect.conditionalEffect));
        return sb.ToString();
    }

    private static string CreateInstantEffectText(CUseEffect.InstantEffect instantEffect)
    {
        string hpChangeText;
        if (!String.IsNullOrEmpty(hpChangeText = CreateHpChangeText(instantEffect.hpChange)))
        {
            return "즉시 " + hpChangeText + "\n";
        }
        else
        {
            return "";
        }
    }

    private static string CreatePersistEffectText(CUseEffect.PersistEffect persist)
    {
        if (!persist.IsValid())
        {
            return "";
        }

        StringBuilder sb = new StringBuilder();

        sb.Append(persist.time + "초 동안 ");

        // 틱 데미지(할)
        if (persist.HasTickHpChange())
        {
            sb.Append(persist.TickPeriod + "초 마다 ")
                .Append(CreateHpChangeText(persist.TickHpChange));
        }

        // 능력치 강화
        foreach (var ability in persist.changeAbilities)
        {
            if (!ability.IsValid())
            {
                continue;
            }

            if (!AbilityNameDict.TryGetValue(ability.ability, out string abilityName))
            {
                abilityName = "??";
            }
            string increasePerStackStr = ability.increasePerStack != 1 ? ability.increasePerStack.ToString() : "";
            sb.Append(abilityName + " " + ability.increaseBase + "+" + increasePerStackStr + "n% 만큼 ");
            sb.AppendLine(ability.isBuff ? "증가" : "감소");
        }

        sb.Append("최대 " + persist.maxStack + "중첩");

        CreateStackAccumulateEffectText(persist.stackAccumulateEffect);

        return sb.ToString() + "\n";
    }

    private static string CreateConditionalEffectText(CUseEffect.ConditionalEffect conditional)
    {
        if (conditional == null || conditional.conditionEffectId == 0)
        {
            return "";
        }

        StringBuilder sb = new StringBuilder();

        if (!DefinedUseEffectNameDict.TryGetValue(conditional.conditionEffectId, out var effectName))
        {
            effectName = conditional.conditionEffectId.ToString();
        }

        sb.Append(effectName + "상태인 경우 ");
        sb.Append(CreateUseEffectText(conditional.effect));
        if(conditional.isRelationStack)
        {
            sb.AppendLine("중첩된 횟수만큼 " + conditional.stackBonusRate + "% 효과 증폭");
        }

        return sb.ToString() + "\n";
    }

    private static string CreateStackAccumulateEffectText(CUseEffect.StackAccumulateEffect stackAccumulateEffect)
    {
        if(stackAccumulateEffect.threshold == 0)
        {
            return "";
        }

        StringBuilder sb = new StringBuilder();

        sb.Append(stackAccumulateEffect.threshold);
        sb.Append("번 중첩 시 다음 효과 발생");
        sb.Append(CreateUseEffectText(stackAccumulateEffect.effect));

        return sb.ToString();
    }

    private static string CreateHpChangeText(CUseEffect.HpChange hpChange)
    {
        if (!hpChange.IsValid())
        {
            Debug.Log("Hp Change not settinged");
            return "";
        }
        
        string healDamageText = hpChange.isHeal ? "회복" : "데미지";
        string trueDamageText = (!hpChange.isHeal && hpChange.isTrueDamage) ? "방어 무시 " : "";

        string amountText = "";
        #region 체력 텍스트 만드는 부분
        if (hpChange.fixedAmount > 0)
        {
            amountText += hpChange.fixedAmount;
        }
        if (!String.IsNullOrEmpty(amountText))
        {
            amountText += " + ";
        }
        if (hpChange.enhancePercentRate > 0)
        {
            amountText += "공격력의 " + hpChange.enhancePercentRate + "%";
        }
        if (!String.IsNullOrEmpty(amountText))
        {
            amountText += " + ";
        }
        if (hpChange.ratioPercent > 0)
        {
            switch (hpChange.ratioType)
            {
                case CUseEffect.HpChange.EHpRatioType.Current:
                    amountText += "현재 체력의 " + hpChange.ratioPercent + "%";
                    break;
                case CUseEffect.HpChange.EHpRatioType.Max:
                    amountText += "최대 체력의 " + hpChange.ratioPercent + "%";
                    break;
                case CUseEffect.HpChange.EHpRatioType.Lost:
                    amountText += "잃은 체력의 " + hpChange.ratioPercent + "%";
                    break;
                default:
                    break;
            }
        }
        #endregion

        string totalText = amountText + "만큼 " + trueDamageText + healDamageText;

        return totalText;
    }
}

public enum EBuffAbility
{
    Attack,
    Defence,
    MoveSpeed,
    AttackSpeed
}

// CUseEffect(클래스명 고민 중) 개선안 
[System.Serializable]
public class CUseEffect : CUseEffectHandle
{
    [System.Serializable]
    public class ChangeAbilityInfo
    {
        public EBuffAbility ability;
        public bool isBuff;
        public float increaseBase;
        public float increasePerStack;

        public ChangeAbilityInfo(EBuffAbility _ability, bool _isBuff, float _increaseBase, float _increasePerStack)
        {
            ability = _ability;
            isBuff = _isBuff;
            increaseBase = _increaseBase;
            increasePerStack = _increasePerStack;
        }

        public bool IsValid()
        {
            return increaseBase != 0 || increasePerStack != 0;
        }
    }

    // 체력 감소 / 회복 수치
    [System.Serializable]
    public class HpChange
    {
        public enum EHpRatioType
        {
            Current,
            Max,
            Lost
        }

        [Tooltip("힐 / 데미지 구분")]
        public bool isHeal;
        [Tooltip("데미지용 : 방어무시인가")]
        public bool isTrueDamage;
        [Tooltip("능력치에 영향받지 않는 수치")]
        public int fixedAmount;
        [Tooltip("능력치에 영향받는 수치(퍼센트)")]
        public int enhancePercentRate;
        [Tooltip("현재 체력 비례 타입인지 최대 체력 비례 타입인지")]
        public EHpRatioType ratioType;
        [Tooltip("체력 비례량"), Range(0, 100)]
        public int ratioPercent;

        public HpChange MultiplyPersant(float percent)
        {
            fixedAmount = (int)((float)fixedAmount * (1.00 + (percent * 0.01)));
            enhancePercentRate = (int)((float)enhancePercentRate * (1.00 + (percent * 0.01)));
            ratioPercent = (int)((float)ratioPercent * (1.00 + (percent * 0.01)));
            return this;
        }

        public void Enhance(int enhancePercent)
        {
            Debug.Log($"enhancePercentRate = {enhancePercentRate}");
            enhancePercentRate = (enhancePercentRate * enhancePercent) / 100;
            Debug.Log($"enhancePercentRate = {enhancePercentRate}");
        }

        public bool IsValid()
        {
            if (fixedAmount != 0 || enhancePercentRate != 0 || ratioPercent != 0)
            {
                return true;
            }
            return false;
        }

        public int RatioToAmount(int hpValue)
        {
            return ratioPercent * hpValue / 100;
        }

        public HpChange Copy(HpChange origin)
        {
            isHeal = origin.isHeal;
            isTrueDamage = origin.isTrueDamage;
            fixedAmount = origin.fixedAmount;
            enhancePercentRate = origin.enhancePercentRate;
            ratioType = origin.ratioType;
            ratioPercent = origin.ratioPercent;
            return this;
        }

        public static HpChange Clone(HpChange origin)
        {
            return new HpChange
            {
                isHeal = origin.isHeal,
                isTrueDamage = origin.isTrueDamage,
                fixedAmount = origin.fixedAmount,
                enhancePercentRate = origin.enhancePercentRate,
                ratioType = origin.ratioType,
                ratioPercent = origin.ratioPercent
            };
        }
    }

    // 즉발 효과
    [System.Serializable]
    public class InstantEffect
    {
        [Tooltip("체력 감소/회복 수치")]
        public HpChange hpChange;
        public bool isCleanse;

        public InstantEffect MultiplyPersant(float persant)
        {
            hpChange.MultiplyPersant(persant);
            return this;
        }

        public InstantEffect EnhanceByUserStat(CharacterPara userStatus)
        {
            hpChange.Enhance(userStatus.TotalAttackMax);
            return this;
        }

        public InstantEffect Copy(InstantEffect origin)
        {
            hpChange.Copy(origin.hpChange);
            isCleanse = origin.isCleanse;
            return this;
        }

        public static InstantEffect Clone(InstantEffect origin)
        {
            return new InstantEffect
            {
                hpChange = HpChange.Clone(origin.hpChange),
                isCleanse = origin.isCleanse
            };
        }
    }

    // 지속 효과
    [System.Serializable]
    public class PersistEffect
    {
        public float time;
        // 특정 효과를 선택하지 않는다면 커스터마이징 필요
        // 타이머 상에 등록될 번호 - 등록 번호가 같으면 같은 번호의 효과 삭제 후 적용
        [Tooltip("타이머 상에 등록될 번호")]
        public int id;

        [Tooltip("체력 감소/회복 수치")]
        public HpChange TickHpChange;
        [Tooltip("틱 발동 주기")]
        public float TickPeriod;

        // 스턴 등 ON / OFF 형식의 CC
        // public ECCEffect cc;

        // 능력치 변화
        public List<ChangeAbilityInfo> changeAbilities;

        // 특수 기능은 따로 클래스를 파서 만듦

        // 스택 표기
        [Range(1, 10)]
        public int increaseStack;
        [Tooltip("최대 스택")]
        public int maxStack;
        [Tooltip("스택 추가 효과")]
        public StackAccumulateEffect stackAccumulateEffect;

        public bool IsValid()
        {
            return id != 0 && time > 0;
        }

        public bool HasTickHpChange()
        {
            return TickPeriod != 0 && TickHpChange.IsValid();
        }

        public PersistEffect EnhanceByUserStat(CharacterPara userStatus)
        {
            TickHpChange.Enhance(userStatus.TotalAttackMax);
            return this;
        }

        public PersistEffect Copy(PersistEffect origin)
        {
            time = origin.time;
            id = origin.id;
            TickHpChange.Copy(origin.TickHpChange);
            TickPeriod = origin.TickPeriod;
            changeAbilities = origin.changeAbilities;
            increaseStack = origin.increaseStack;
            maxStack = origin.maxStack;
            stackAccumulateEffect.Copy(origin.stackAccumulateEffect);
            return this;
        }

        public static PersistEffect Clone(PersistEffect origin)
        {
            return new PersistEffect
            {
                time = origin.time,
                id = origin.id,
                TickHpChange = HpChange.Clone(origin.TickHpChange),
                TickPeriod = origin.TickPeriod,
                // 주의 - 능력치 버프는 강화되는 일 없으리라 생각되므로 얕은 복사 사용
                changeAbilities = origin.changeAbilities,
                increaseStack = origin.increaseStack,
                maxStack = origin.maxStack,
                stackAccumulateEffect = StackAccumulateEffect.Clone(origin.stackAccumulateEffect),
            };
        }
    }

    [System.Serializable]
    public class StackAccumulateEffect
    {
        [Tooltip("스택 최소 발동 수치")]
        public int threshold;
        [Tooltip("발동 수치 추가량")]
        public int thresholdAdd;

        [Tooltip("스택 충족 시 발동 효과")]
        public CUseEffect effect;

        public StackAccumulateEffect EnhanceByUserStat(CharacterPara userStatus)
        {
            effect.EnhanceEffectByStat(userStatus);
            return this;
        }

        public StackAccumulateEffect Copy(StackAccumulateEffect origin)
        {
            threshold = origin.threshold;
            thresholdAdd = origin.thresholdAdd;
            effect = origin.effect;
            return this;
        }

        public static StackAccumulateEffect Clone(StackAccumulateEffect origin)
        {
            return new StackAccumulateEffect
            {
                threshold = origin.threshold,
                thresholdAdd = origin.thresholdAdd,
                effect = origin.effect,
            };
        }
    }

    [System.Serializable]
    public class ConditionalEffect
    {
        public int conditionEffectId;

        [Tooltip("스택과 상관 있는지")]
        public bool isRelationStack;
        [Tooltip("스택에 따른 효과 강화 비율")]
        public float stackBonusRate;

        [Tooltip("스택 충족 시 발동 효과")]
        public CUseEffect effect;

        public bool IsValid()
        {
            return conditionEffectId != 0 && effect != null;
        }

        public ConditionalEffect EnhanceByUserStat(CharacterPara userStatus)
        {
            if (IsValid())
            {
                effect.EnhanceEffectByStat(userStatus);
            }
            return this;
        }

        public ConditionalEffect Copy(ConditionalEffect origin)
        {
            conditionEffectId = origin.conditionEffectId;
            isRelationStack = origin.isRelationStack;
            stackBonusRate = origin.stackBonusRate;
            effect = origin.effect;
            return this;
        }

        public static ConditionalEffect Clone(ConditionalEffect origin)
        {
            return new ConditionalEffect
            {
                conditionEffectId = origin.conditionEffectId,
                isRelationStack = origin.isRelationStack,
                stackBonusRate = origin.stackBonusRate,
                effect = origin.effect,
            };
        }
    }

    public bool IsUseEffectName;
    public string EffectName;

    public InstantEffect instantEffect;
    public PersistEffect persistEffect;
    public ConditionalEffect conditionalEffect;

    #region 초기 UseEffect 설정값 저장
    public InstantEffect InstantEffectInitialValue { get; private set; }

    public PersistEffect PersistEffectInitialValue { get; private set; }

    public ConditionalEffect ConditionalEffectInitialValue { get; private set; }

    private void Awake()
    {
        // clone()을 만들어서 초기값을 저장해둠
        InstantEffectInitialValue = InstantEffect.Clone(instantEffect);
        PersistEffectInitialValue = PersistEffect.Clone(persistEffect);
        ConditionalEffectInitialValue = ConditionalEffect.Clone(conditionalEffect);
    }
    #endregion

    public static CUseEffect Clone(CUseEffect origin)
    {
        return new CUseEffect
        {
            IsUseEffectName = origin.IsUseEffectName,
            EffectName = origin.EffectName,
            instantEffect = InstantEffect.Clone(origin.instantEffect),
            persistEffect = PersistEffect.Clone(origin.persistEffect),
            conditionalEffect = ConditionalEffect.Clone(origin.conditionalEffect),
        };
    }

    public override void TakeUseEffect(CharacterPara cPara)
    {
        cPara.TakeUseEffect(this);
    }

    #region 효과 강화
    /// <summary>
    /// 사용자의 능력치에 따라 데미지를 변경한다
    /// </summary>
    /// <param name="userStatus"></param>
    /// 
    public override void EnhanceEffectByStat(CharacterPara userStatus)
    {
        instantEffect.Copy(InstantEffectInitialValue).EnhanceByUserStat(userStatus);
        persistEffect.Copy(PersistEffectInitialValue).EnhanceByUserStat(userStatus);
        conditionalEffect.Copy(ConditionalEffectInitialValue).EnhanceByUserStat(userStatus);
    }
    #endregion
}