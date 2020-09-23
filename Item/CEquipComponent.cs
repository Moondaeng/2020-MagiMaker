using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [System.Serializable]
    public class CEquip : CItem
    {
        /// <summary>
        /// 패시브 효과 발동 조건
        /// </summary>
        public enum ECondition
        {
            Teleport,
            UseSkill,
            KillMonster
        }

        [System.Serializable]
        public class EquipEffectWithChance
        {
            public ECondition useEffectCondition;
            public CUseEffect useEffect;
            [Range(0f, 1f)] public float Chance;
        }

        [System.Serializable]
        public class EquipAbility
        {
            public enum EAbility
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

            public EAbility equipEffect;
            public int value;
        }

        public List<EquipAbility> equipAbilities;

        public List<EquipEffectWithChance> EquipEffectList;

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
