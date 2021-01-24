using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [System.Serializable]
    public class CTestConsumable : CItem
    {
        public List<CUseEffectHandle> testEffects;

        public CTestConsumable(string _itemName, int _itemCode, Sprite _itemImage)
            : base(_itemName, _itemCode, _itemImage)
        {

        }
    }
}

[RequireComponent(typeof(Rigidbody))]
public class TestConsumableComponent : CItemComponent
{
    [SerializeField]
    private Item.CTestConsumable ConsumableStat;

    private void Awake()
    {
        if (ConsumableStat == null)
        {
            ConsumableStat = new Item.CTestConsumable("", 0, null);
        }

        Item = ConsumableStat;
    }
}
