using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    public static class CConsumableExplainText
    {
        public static string CreateExplainText(CConsumable consumable)
        {
            if (consumable.UseEffectList.Count == 0)
            {
                return "";
            }
            else
            {
                return "사용 시 " + CUseEffectHandleExplain.CreateUseEffectListText(consumable.UseEffectList);
            }
        }

        
    }

    [System.Serializable]
    public class CConsumable : CItem
    {
        public List<CUseEffectHandle> UseEffectList;

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
