using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{

    [System.Serializable]
    public class CConsumable : CItem
    {
        [System.Serializable]
        public class UseEffect
        {
            /// <summary>
            /// 발동 효과
            /// </summary>
            public enum EType
            {
                Heal
            }

            public EType EffectType;
            public float arg1;
            public float arg2;
            public float arg3;
            public float arg4;
        }

        [System.Serializable]
        public class UseEffectWithChance
        {
            [Tooltip("소비 아이템 사용 효과")]
            public UseEffect useEffect;
            [Tooltip("효과 발동 확률")]
            [Range(0f, 1f)] public float Chance;
        }

        public List<UseEffectWithChance> UseEffectList;

        public CConsumable(string _itemName, int _itemCode, Sprite _itemImage)
            : base(_itemName, _itemCode, _itemImage)
        {

        }
    }
}

[RequireComponent(typeof(Rigidbody))]
public class CConsumableComponent : CItemComponent
{
    public Item.CConsumable ConsumableStat;

    private void Awake()
    {
        if (ConsumableStat == null)
        {
            ConsumableStat = new Item.CConsumable("", 0, null);
        }
    }

    /// <summary>
    /// 드랍된 아이템 주울 때 정보창 보여주기
    /// </summary>
    public override void CallItemUI()
    {

    }
}
