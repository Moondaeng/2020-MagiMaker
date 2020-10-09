using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [System.Serializable]
    public class CConsumable : CItem
    {
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
