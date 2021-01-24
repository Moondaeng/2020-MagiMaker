using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CUseEffectExplain
{
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

        if (persist.TickPeriod != 0 && persist.TickHpChangeAmount != 0)
        {
            sb.Append(persist.TickPeriod + "초 마다");
            sb.Append(Math.Abs(persist.TickHpChangeAmount) + "만큼 ");
            sb.AppendLine(persist.TickHpChangeAmount > 0 ? "힐" : "데미지");
        }

        if (persist.changeAbilities.Count > 0)
        {
            foreach (var ability in persist.changeAbilities)
            {
                if (ability.increaseBase == 0 && ability.increasePerStack == 0)
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
        }

        sb.Append("최대 " + persist.maxStack + "중첩");

        CreateStackAccumulateEffectText(persist.stackAccumulateEffect);

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
        sb.Append(CreateUseEffectText(conditional.effect));
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
        sb.Append(CreateUseEffectText(stackAccumulateEffect.effect));

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

        // 특수 기능은 따로 클래스를 파서 만듦

        // 스택 표기
        [Range(1, 10)]
        public int increaseStack;
        [Tooltip("최대 스택")]
        public int maxStack;
        [Tooltip("스택 추가 효과")]
        public StackAccumulateEffect stackAccumulateEffect;
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
    }

    public InstantEffect instantEffect;
    public PersistEffect persistEffect;
    public ConditionalEffect conditionalEffect;

    public override void TakeUseEffect(CharacterPara cPara)
    {
        cPara.TakeUseEffect(this);
    }
}