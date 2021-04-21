using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRewardChoicePopupController : CNPCPopUpController
{
    private int _userSelectRoom = 0; //0 엘리트 1 보스
    private int _gold;
    private Dictionary<int, string> _itemGrade;
    private bool isUsed = false;
    CPlayerPara _playerPara;
    private bool[] isBoss = { false, false };
    private string[] _elementName = { "부원소", "부원소" };

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if (CCreateMap.instance.userSelectRoom() == CCreateMap.ERoomType._elite) //유저가 보스 엘리트 중 어떤 방에 들어온 것인지 확인.
        {
            _userSelectRoom = 0;
            isBoss[_userSelectRoom] = false;
            _elementName[_userSelectRoom] = "부원소";
        }
        else if (CCreateMap.instance.userSelectRoom() == CCreateMap.ERoomType._boss)
        {
            _userSelectRoom = 1;
            isBoss[_userSelectRoom] = true;
            _elementName[_userSelectRoom] = "주원소";
        }

        _playerPara = CController.instance.player.GetComponent<CPlayerPara>(); //인벤토리를 위한 플레이어 파라 정의       

        _itemGrade = new Dictionary<int, string>();
        _itemGrade.Add(0, "일반"); _itemGrade.Add(1, "특별"); //아이템 등급들 저장
        _itemGrade.Add(2, "희귀"); _itemGrade.Add(3, "유일");

        MakeText();
    }

    private void MakeText()
    {
        TMPro.TextMeshProUGUI goldText = _popUp.transform.GetChild(2).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>(); //골드 보상 결정
        _gold = 300 + (100 * _userSelectRoom) + (CCreateMap.instance._stageNumber + 1) * UnityEngine.Random.Range(50, 100);
        goldText.text = _gold.ToString() + goldText.text;

        TMPro.TextMeshProUGUI itemText = _popUp.transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>(); //아이템 보상 텍스트
        itemText.text = itemText.text + " (" + _itemGrade[_userSelectRoom] + " ~ " + _itemGrade[_userSelectRoom + 2] + ")";

        TMPro.TextMeshProUGUI elementText = _popUp.transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>(); //원소 보상 텍스트
        elementText.text = "무작위 " + _elementName[_userSelectRoom] + " 획득";
    }

    public override void ChooseButton(int choose)
    {
        if(isUsed) //한 번 보상 받았으면 끝
        {
            CGlobal.popUpCancel = true;
            return;
        }

        switch (choose) //아이템 엘리트 기준으로 짜고 어떻게 변경할지 생각 ㄱ
        {
            case 0: //부원소 획득
                GetElemental();
                break;
            case 1: //무작위 아이템 획득
                GetItem();
                break;
            case 2: //골드 획득
                _playerPara.Inventory.Gold += _gold;
                break;
            case 3: //취소
                CGlobal.popUpCancel = true;
                return;
        }

        isUsed = true;
        CGlobal.popUpCancel = true;
    }

    private void GetElemental()
    {
        CElementObtainViewer.instance.OpenViewer(CController.instance.player.GetComponent<CPlayerSkill>(), isBoss[_userSelectRoom], (CPlayerSkill.ESkillElement)
            Random.Range(0, 5));
    }

    private void GetItem()
    {
        //확률 50 30 20
        int random = Random.Range(0, 100);

        GameObject item;
        if (random < 50)
            item = CItemManager.instance.PopRandomItemByGrade((CItemManager.EItemGrade)_userSelectRoom, CConstants.EQUIP_ITEM_TYPE);
        else if (random < 80)
            item = CItemManager.instance.PopRandomItemByGrade((CItemManager.EItemGrade)_userSelectRoom + 1, CConstants.EQUIP_ITEM_TYPE);
        else
            item = CItemManager.instance.PopRandomItemByGrade((CItemManager.EItemGrade)_userSelectRoom + 2, CConstants.EQUIP_ITEM_TYPE);

        MoveItemToInventory(item.GetComponent<CItemComponent>());
    }

    private bool MoveItemToInventory(CItemComponent itemComponent)
    {
        bool check = false;

        if (itemComponent != null)
        {
            if (itemComponent.Item is Item.CEquip) //장비템
                check = _playerPara.Inventory.AddEquip(itemComponent.Item as Item.CEquip);
        }

        return check;
    }
}
