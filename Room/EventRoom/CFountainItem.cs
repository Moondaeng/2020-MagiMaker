using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFountainItem : CFountainItemTrigger
{
    private GameObject _item;
    public override void Start()
    {
        base.Start();
        _item = CItemDropTable.instance.DropRandomItem(CCreateMap.instance.GetStageNumber(), CConstants.EQUIP_ITEM_TYPE);
    }
    public override void GetReward()
    {
        if(!MoveItemToInventory(_item.GetComponent<CEquipComponent>()))
            Debug.LogError("MoveItemFail");

        Object.Destroy(gameObject);
    }

    private bool MoveItemToInventory(CItemComponent itemComponent)
    {
        bool check = false;

        if (itemComponent != null)
        {
            if (itemComponent.Item is Item.CEquip) //장비템
                check = _playerPara.Inventory.AddEquip(itemComponent.Item as Item.CEquip);

            else if (itemComponent.Item is Item.CConsumable) //소비템
                check = _playerPara.Inventory.AddConsumableItem(itemComponent.Item as Item.CConsumable);
        }

        return check;
    }
}
