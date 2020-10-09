using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    /// <summary>
    /// 효과 발동 이벤트 조건
    /// </summary>
    public enum EEquipEvent
    {
        None,
        Always,
        UseSkill,
        KillMonster
    }

    /// <summary>
    /// 이벤트 발동 횟수 계산 방식
    /// </summary>
    public enum EEquipEventCountOption
    {
        Accumulate,
        Each_Over,
        Each_Below
    }

    /// <summary>
    /// 장비 강화 능력치
    /// </summary>
    public enum EEquipAbility
    {
        Attack,
        AttackSpeed,
        MaxHp,
        HpRegen,
        Defence,
        Speed,
        SkillCoolTime,
        DamageReduceRate,
        SkillRange
    }

    /// <summary>
    /// 능력치 설명 텍스트 클래스
    /// </summary>
    public static class CEquipExplainText
    {
        public static readonly Dictionary<EEquipAbility, string> EquipAbilityExplainDict 
            = new Dictionary<EEquipAbility, string>
        {
            {EEquipAbility.Attack, "공격력"},
            {EEquipAbility.AttackSpeed, "공격속도"},
            {EEquipAbility.MaxHp, "최대 체력"},
            {EEquipAbility.HpRegen, "체력 재생"},
            {EEquipAbility.Defence, "방어력"},
            {EEquipAbility.Speed, "이동속도"},
            {EEquipAbility.SkillCoolTime, "스킬 쿨다운 감소"},
            {EEquipAbility.DamageReduceRate, "받는 데미지 감소"},
            {EEquipAbility.SkillRange, "스킬 사거리"},
        };

        /// <summary>
        /// 이벤트 발동 조건에 따른 설명 텍스트 : ex) 한 번에 [골드] 1500 이상 [획득 시]
        /// []에 해당하는 부분을 각각 설명으로 취급
        /// 일부 설명은 생략될 수 있음 : ex) [스킬] 3[회] [사용 시] / 한 번에 [골드] 1500[] 이상 [획득 시]
        /// Tuple 아이템 순서 : 설명1(string) / 설명2(string) / 설명3(string)
        /// </summary>
        public static readonly Dictionary<EEquipEvent, Tuple<string, string, string>> EquipEventExplainDict 
            = new Dictionary<EEquipEvent, Tuple<string, string, string>>
        {
            {EEquipEvent.None, new Tuple<string, string, string>("없음", "", "") },
            {EEquipEvent.Always, new Tuple<string, string, string>("항상", "", "") },
            {EEquipEvent.UseSkill, new Tuple<string, string, string>("스킬", "회", "사용") },
            {EEquipEvent.KillMonster, new Tuple<string, string, string>("몬스터", "회", "처치") },
        };

        /// <summary>
        /// 이벤트 발동 추가 조건에 따른 설명 : ex) [한 번에] 골드 1500 [이상] 획득 시
        /// Tuple 아이템 순서 : 내용1(string) / 내용2(string)
        /// </summary>
        public static readonly Dictionary<EEquipEventCountOption, Tuple<string, string>> EquipEventCountOptionExplainDict 
            = new Dictionary<EEquipEventCountOption, Tuple<string, string>>
        {
            {EEquipEventCountOption.Accumulate, new Tuple<string, string>("", "")},
            {EEquipEventCountOption.Each_Below, new Tuple<string, string>("한 번에 ", " 이하")},
            {EEquipEventCountOption.Each_Over, new Tuple<string, string>("한 번에 ", " 이상")},
        };

        public static string CreateAbilityText(List<CEquip.EquipAbility> equipAbilities)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var ability in equipAbilities)
            {
                if (!EquipAbilityExplainDict.TryGetValue(ability.equipEffect, out string abilityText))
                {
                    abilityText = ability.equipEffect.ToString();
                    Debug.Log($"Warning : Item.EEquipAbility's {abilityText} not initialized");
                }
                sb.AppendLine(abilityText + " : " + ability.value);
            }
            return sb.ToString();
        }

        public static string CreatePassiveText(
            EEquipEvent passiveCondition, int passiveCount, EEquipEventCountOption passiveCountOption, List<UseEffectWithChance> useEffects)
        {
            if (passiveCondition == Item.EEquipEvent.None)
            {
                return "";
            }
            
            if (!EquipEventExplainDict.TryGetValue(passiveCondition, out var passiveExplain))
            {
                Debug.Log($"Warning : Item.EEquipEvent's {passiveCondition.ToString()} Explain not initialized");
                return passiveCondition.ToString() + " Error";
            }

            if (!EquipEventCountOptionExplainDict.TryGetValue(passiveCountOption, out var passiveOptionExplain))
            {
                Debug.Log($"Warning : Item.EEquipEventCountOption's {passiveCountOption.ToString()} Explain not initialized");
                return passiveCountOption.ToString() + " Error";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(passiveOptionExplain.Item1)
                .Append(passiveExplain.Item1 + " ")
                .Append(passiveCount + passiveExplain.Item2 + " ")
                .Append(passiveOptionExplain.Item2)
                .Append(passiveExplain.Item3 + " ")
                .Append("시 효과 발동\n");

            sb.Append(CUseEffectExplain.CreateUseEffectListText(useEffects));

            return sb.ToString();
        }

        public static string CreateUpgradeText(EEquipEvent upgradeCondition, int upgradeRequireCount, List<CEquip.EquipAbility> upgradeAbilities)
        {
            if (upgradeCondition == EEquipEvent.None)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            if (!EquipEventExplainDict.TryGetValue(upgradeCondition, out var upgradeExplain))
            {
                Debug.Log($"Warning : Item.EEquipEvent's {upgradeCondition.ToString()} Explain not initialized");
                return upgradeCondition.ToString() + " Error";
            }
            sb.Append(upgradeExplain.Item1 + " ")
                .Append(upgradeRequireCount + upgradeExplain.Item2 + " ")
                .AppendLine(upgradeExplain.Item3 + " 충족 시 다음 능력치 상승")
                .AppendLine(CreateAbilityText(upgradeAbilities));
            return sb.ToString();
        }
    }

    [System.Serializable]
    public class CEquip : CItem
    {
        [System.Serializable]
        public class EquipAbility
        {

            public EEquipAbility equipEffect;
            public int value;
        }

        public List<EquipAbility> equipAbilities;

        [Tooltip("패시브 발동 조건")]
        public EEquipEvent PassiveCondition;
        [Tooltip("패시브 발동 조건 적용 방식")]
        public EEquipEventCountOption PassiveConditionOption;
        [Tooltip("패시브 발동 조건 횟수")]
        public int PassiveUseCount;
        [Tooltip("현재 패시브 발동 조건 횟수"), HideInInspector]
        public int passiveCurrentCount;
        public List<UseEffectWithChance> EquipEffectList;

        [Tooltip("성장 조건")]
        public EEquipEvent UpgradeCondition;
        [Tooltip("성장 조건 횟수")]
        public int UpgradeCount;
        [Tooltip("현재 성장 조건 횟수"), HideInInspector]
        public int UpgradeCurrentCount;
        [Tooltip("성장 시 능력치")]
        public List<EquipAbility> upgradeAbilities;

        public CEquip(string _itemName, int _itemCode, Sprite _itemImage)
            : base(_itemName, _itemCode, _itemImage)
        {

        }
    }
}

[RequireComponent(typeof(Rigidbody))]
public class CEquipComponent : CItemComponent
{
    [SerializeField]
    private Item.CEquip equipStat;

    private void Awake()
    {
        if (equipStat == null)
        {
            equipStat = new Item.CEquip("", 0, null);
        }

        Item = equipStat;
    }
}
