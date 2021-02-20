using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COldGravestoneButtonController : CEventRoomPopUpController
{
    private GameObject _oldGravestone;
    private GameObject _monsterGroup;
    
    private Color _color;
    public override void Start()
    {
        base.Start();
        _oldGravestone = GameObject.Find("OldGravestone");     
        _monsterGroup = GameObject.Find("MonsterGroup");
    }

    public override void ChooseButton(int choose)
    {
        switch(choose)
        {
            case 0:
                ClickRandomItem();
                break;
            case 1:
                ClickCancel();
                break;
        }

        CGlobal.popUpCancel = true;
    }

    public void ClickRandomItem()
    {
        GameObject item = CItemDropTable.instance.DropRandomItem(CCreateMap.instance.GetStageNumber(), CConstants.EQUIP_ITEM_TYPE);
        item = Instantiate(item, _oldGravestone.transform.position, _oldGravestone.transform.rotation);
        item.SetActive(true);

        for (int i = 0; i < _monsterGroup.transform.childCount; i++)
            _monsterGroup.transform.GetChild(i).gameObject.SetActive(true);
        CGlobal.isEvent = true; //적이 소환됬으므로 포탈 대기 상태

        CCreateMap.instance.NotifyPortal(); //플래그 바뀐 상태 방송하기

        Destroy(_oldGravestone);

        //debug
        GameObject.Find("EventRoom0_0(Clone)").GetComponent<CEventRoomMonsterCheck>().SendMessage("ForceDeadMonster");
    }

    public void ClickCancel()
    {
    }
}
