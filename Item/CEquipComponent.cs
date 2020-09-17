using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [System.Serializable]
    public class CEquip : CItem
    {
        [System.Serializable]
        public class EquipEffect
        {
            /// <summary>
            /// 발동 효과
            /// </summary>
            public enum EType
            {
                Heal
            }

            /// <summary>
            /// 패시브 효과 발동 조건
            /// </summary>
            public enum ECondition
            {
                Teleport,
                UseSkill,
                KillMonster
            }

            public EType EffectType;
            public ECondition EffectCondition;
            public float arg1;
            public float arg2;
            public float arg3;
            public float arg4;
        }

        [System.Serializable]
        public class EquipEffectWithChance
        {
            public EquipEffect equipEffect;
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
                Def,
                Speed,
                SkillCoolTime,
                DamageReduceRate,
                SkillRange
            }

            public EAbility equipEffect;
            public int value;
        }

        public List<EquipAbility> equipAbilities;

        public int As;     // 공격속도
        public int Atk;    // 공격력
        public int MaxHp;           // 최대 체력
        public int HpRegen;   // 초당 체력 리젠
        public int Def;           // 방어력
        public int Spd;       // 이동속도
        public int _skillCoolTime;   // 쿨타임
        public int _damageTakenRate; // 받는 피해율
        public int _skillRange;      // 스킬 범위

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
    public Item.CEquip equipStat;

    private void Awake()
    {
        if(equipStat == null)
        {
            equipStat = new Item.CEquip("", 0, null);
        }
    }

    public override void CallItemUI()
    {

    }
}
