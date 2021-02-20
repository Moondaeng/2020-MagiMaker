using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSkullButtonController : CEventRoomPopUpController
{
    GameObject _skull;
    public override void Start()
    {
        _skull = GameObject.Find("Skull");
    }

    public override void ChooseButton(int choose)
    {
        switch (choose)
        {
            case 0:
                ClickRandomItem();
                break;
            case 1:
                ClickRandomMinorElement();
                break;
            case 2:
                ClickCancel();
                break;
        }

        CGlobal.popUpCancel = true;
    }

    public void ClickRandomItem()
    {
        GameObject item = CItemDropTable.instance.DropRandomItem(CCreateMap.instance.GetStageNumber(), CConstants.EQUIP_ITEM_TYPE);
        item = Instantiate(item, _skull.transform.position, _skull.transform.rotation);
        item.SetActive(true);

        Debug.Log("Lose Max HP");
        Destroy(_skull);
    }
    
    public void ClickRandomMinorElement()
    {
        Debug.Log("Get Element!");
        Debug.Log("Lose Max HP");
        Destroy(_skull);
    }

    public void ClickCancel()
    {
    }
}
