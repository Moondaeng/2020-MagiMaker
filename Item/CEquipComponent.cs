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

    [System.Serializable]
    public class CEquip : CItem
    {
        [System.Serializable]
        public class EquipEffectWithChance
        {
            public CUseEffect useEffect;
            [Range(0f, 1f)] public float Chance;
        }

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
        public List<EquipEffectWithChance> EquipEffectList;

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
