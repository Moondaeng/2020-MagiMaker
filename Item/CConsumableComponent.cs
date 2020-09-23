using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{

    [System.Serializable]
    public class CConsumable : CItem
    {
        [System.Serializable]
        public class UseEffectWithChance
        {
            [Tooltip("소비 아이템 사용 효과")]
            public CUseEffect useEffect;
            [Tooltip("소비 아이템 사용 시 생성되는 오브젝트(투사체 등)")]
            public GameObject useEffectObject;
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
    [SerializeField]
    private Item.CConsumable ConsumableStat;

    private void Awake()
    {
        if (ConsumableStat == null)
        {
            ConsumableStat = new Item.CConsumable("", 0, null);
        }

        Item = ConsumableStat;
    }
}
