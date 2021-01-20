using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CUseEffectExplain
{
    private static readonly Dictionary<CUseEffect.EDefinedPersistEffect, string> DefinedEffectNameDict
            = new Dictionary<CUseEffect.EDefinedPersistEffect, string>
        {
            {CUseEffect.EDefinedPersistEffect.Wet, "첨습"},
            {CUseEffect.EDefinedPersistEffect.Curse, "저주"},
        };

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
        StringBuilder sb = new StringBuilder();
        string instantEffectExplain = CreateInstantEffectText(useEffect.instantEffect);
        if(instantEffectExplain != "")
        {
            sb.AppendLine(instantEffectExplain);
        }
        string persistEffectExplain = CreatePersistEffectText(useEffect.persistEffect);
        if (persistEffectExplain != "")
        {
            sb.AppendLine(persistEffectExplain);
        }
        string conditionalEffectExplain = CreateConditionalEffectText(useEffect.conditionalEffect);
        if (conditionalEffectExplain != "")
        {
            sb.AppendLine(conditionalEffectExplain);
        }
        return sb.ToString();
    }

    private static string CreateInstantEffectText(CUseEffect.InstantEffect instantEffect)
    {
        StringBuilder sb = new StringBuilder();

        // 체력 변화 관련
        string hpChangeText = instantEffect.hpChangeAmount > 0 ? "회복" : "데미지";
        if(instantEffect.hpChangeAmount != 0)
        {
            sb.Append("즉시 ");
            sb.Append(Math.Abs(instantEffect.hpChangeAmount));
            sb.Append("만큼 " + hpChangeText);
        }

        return sb.ToString();
    }

    private static string CreatePersistEffectText(CUseEffect.PersistEffect persist)
    {
        if (persist == null || persist.time <= 0)
        {
            return "";
        }

        StringBuilder sb = new StringBuilder();

        sb.Append(persist.time + "초 동안 ");
        // 지정된 효과이면 지정된 효과로 기술
        if(persist.isUseDefined)
        {
            if (!DefinedEffectNameDict.TryGetValue(persist.definedEffect, out string definedEffectName))
            {
                definedEffectName = "??";
            }
            sb.AppendLine(definedEffectName);
        }
        // 커스텀 효과이면 풀어서 기술
        else
        {
            sb.AppendLine("다음 효과 발생");
            sb.Append(CreatePersistCustomEffectText(persist.customInfo));
        }

        return sb.ToString();
    }

    private static string CreatePersistCustomEffectText(CUseEffect.PersistCustomEffect custom)
    {
        if(custom == null)
        {
            return "";
        }

        StringBuilder sb = new StringBuilder();

        if(custom.TickPeriod != 0 && custom.TickHpChangeAmount != 0)
        {
            sb.Append(custom.TickPeriod + "초 마다");
            sb.Append(Math.Abs(custom.TickHpChangeAmount) + "만큼 ");
            sb.AppendLine(custom.TickHpChangeAmount > 0 ? "힐" : "데미지");
        }

        if(custom.changeAbilities.Count > 0)
        {
            foreach(var ability in custom.changeAbilities)
            {
                if(ability.increaseBase == 0 && ability.increasePerStack == 0)
                {
                    continue;
                }

                if (!AbilityNameDict.TryGetValue(ability.ability, out string abilityName))
                {
                    abilityName = "??";
                }
                string increasePerStackStr = ability.increasePerStack != 1 ? ability.increasePerStack.ToString() : "" ;
                sb.Append(abilityName + " " + ability.increaseBase + "+" + increasePerStackStr + "n% 만큼 ");
                sb.AppendLine(ability.isBuff ? "증가" : "감소");
            }
        }

        sb.Append("최대 " + custom.maxStack + "중첩");

        CreateStackAccumulateEffectText(custom.stackAccumulateEffect);

        return sb.ToString();
    }

    private static string CreateConditionalEffectText(CUseEffect.ConditionalEffect conditional)
    {
        if (conditional == null || conditional.conditionEffectId == 0)
        {
            return "";
        }

        StringBuilder sb = new StringBuilder();

        // 지정된 효과의 경우 이름을 받아서 발동
        sb.Append(conditional.conditionEffectId + "상태인 경우 ");
        sb.Append(CreateInstantEffectText(conditional.instantEffect));
        if(conditional.isRelationStack)
        {
            sb.AppendLine("중첩된 횟수만큼 " + conditional.stackBonusRate + "% 효과 증폭");
        }

        return sb.ToString();
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
        sb.Append(CreateInstantEffectText(stackAccumulateEffect.instantEffect));
        sb.Append(CreatePersistEffectText(stackAccumulateEffect.persistEffect));

        return sb.ToString();
    }

    public static string CreateUseEffectListText(List<UseEffectWithChance> useEffects)
    {
        StringBuilder sb = new StringBuilder();
        
        return sb.ToString();
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
public class CUseEffect
{
    public enum EUseStyle
    {
        Instant, UseObject, Random
    }


    // 첨습, 저주 등 특별히 지정된 효과
    public enum EDefinedPersistEffect
    {
        Wet,
        Curse
    }

    public static Dictionary<EDefinedPersistEffect, PersistCustomEffect> DefinedPersistEffectDict
        = new Dictionary<EDefinedPersistEffect, PersistCustomEffect>
        {
            { 
                EDefinedPersistEffect.Wet, new PersistCustomEffect(101, 0, 0, 
                    new List<ChangeAbilityInfo>{ 
                        new ChangeAbilityInfo(EBuffAbility.MoveSpeed, false, 10, 1),
                }, 1) 
            },
        };

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
    }

    // 즉발 효과
    [System.Serializable]
    public class InstantEffect
    {
        [Tooltip("체력 감소/회복 수치")]
        public int hpChangeAmount;
        public bool isCleanse;

        public InstantEffect MultiplyPersant(float persant)
        {
            hpChangeAmount = (int)((float)hpChangeAmount * (1.00 + (persant * 0.01)));
            return this;
        }
    }

    // 지속 효과
    [System.Serializable]
    public class PersistEffect
    {
        public float time;

        // 첨습, 저주 등 특정 효과 선택
        // 특정 효과 사용을 지정한다면 효과를 설정할 필요 없어보임 -> hide customInfo
        public bool isUseDefined;
        public EDefinedPersistEffect definedEffect;
        public PersistCustomEffect customInfo;

        // 스택 표기
        [Range(1, 10)]
        public int increaseStack;
    }

    // 정보 지정용
    [System.Serializable]
    public class PersistCustomEffect
    {
        // 특정 효과를 선택하지 않는다면 커스터마이징 필요
        // 타이머 상에 등록될 번호 - 등록 번호가 같으면 같은 번호의 효과 삭제 후 적용
        [Tooltip("타이머 상에 등록될 번호")]
        public int id;

        [Tooltip("체력 감소/회복 수치")]
        public int TickHpChangeAmount;
        [Tooltip("틱 발동 주기")]
        public float TickPeriod;

        // 스턴 등 ON / OFF 형식의 CC
        // public ECCEffect cc;

        // 능력치 변화
        public List<ChangeAbilityInfo> changeAbilities;

        // 그 외 특수 기능은 나중에 추가하는거로 (sendMessage 등 이용)
        [Tooltip("최대 스택")]
        public int maxStack;
        [Tooltip("스택 추가 효과")]
        public StackAccumulateEffect stackAccumulateEffect;

        public PersistCustomEffect(int id, int dotAmount, float dotPeriod, List<ChangeAbilityInfo> changeAbilityList, int maxStack)
        {
            this.id = id;
            this.TickHpChangeAmount = dotAmount;
            this.TickPeriod = dotPeriod;
            this.changeAbilities = changeAbilityList;
            this.maxStack = maxStack;
        }
    }

    [System.Serializable]
    public class StackAccumulateEffect
    {
        [Tooltip("스택 최소 발동 수치")]
        public int threshold;
        [Tooltip("발동 수치 추가량")]
        public int thresholdAdd;

        public InstantEffect instantEffect;
        public PersistEffect persistEffect;
    }

    [System.Serializable]
    public class ConditionalEffect
    {
        public int conditionEffectId;

        public bool isRelationStack; // 스택과 상관 있는지
        public float stackBonusRate; // 보너스 비율

        // 발동 효과 지정
        public InstantEffect instantEffect;
    }

    //public EUseStyle useStyle;

    //public List<UseEffectWithChance> randomEffects;

    //public GameObject useEffectObject;

    public InstantEffect instantEffect;
    public PersistEffect persistEffect;
    public ConditionalEffect conditionalEffect;
}

/// <summary>
/// 확률적으로 효과 사용
/// </summary>
[System.Serializable]
public class UseEffectWithChance
{
    [Tooltip("사용 효과"), SerializeField]
    public CUseEffect useEffect;
    [Tooltip("사용 시 생성하는 오브젝트(투사체 등)"), SerializeField]
    public GameObject useEffectObject;
    [Tooltip("효과 발동 확률"), SerializeField, Range(0f, 1f)]
    public float Chance;
}

[Serializable]
public class UseEffectList
{
    public List<UseEffectWithChance> UseEffects;

    public int SelectRandomEffect()
    {
        if (UseEffects.Count == 0)
        {
            Debug.Log("No Use Effect");
            return -1;
        }
        else if (UseEffects.Count == 1)
        {
            return 0;
        }

        Debug.Log("Get Random Effect");
        float chanceSum = 0f;
        List<float> chanceSumList = new List<float>(UseEffects.Count);
        foreach (var elem in UseEffects)
        {
            chanceSum += elem.Chance;
            chanceSumList.Add(chanceSum);
        }
        Debug.Log("set chance");

        // 임의 효과 선택
        float randomChance = UnityEngine.Random.Range(0f, 1f);
        int idx = 0;
        while (idx < chanceSumList.Count - 1 && randomChance >= chanceSumList[idx])
        {
            idx++;
        }

        return idx;
    }
}