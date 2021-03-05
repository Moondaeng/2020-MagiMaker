using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CStoreController : MonoBehaviour
{
    public static CStoreController instance;

    [System.NonSerialized]
    public GameObject _confirmPurchaseIntentionPopup; //구매 재확인 팝업

    private Transform _slotRoot;
    private List<CSlotController> _slots;

    private int _positionX; //아이템 선택 위치
    private int _positionY;
    private int _currentSlot; //현재 슬롯
    private int _lastSlot;  //이전 슬롯

    private GameObject _notEnoughGoldPopUp; //골드 부족합니다 팝업  
    private EArrow _arrow;

    private GameObject _player;
    private CPlayerPara _playerPara;

    enum EArrow
    {
        _left,
        _right,
        _up,
    }
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;

        _slotRoot = transform.Find("SlotGroup");
        _slots = new List<CSlotController>();

        for (int i = 0; i < _slotRoot.childCount - 2; i++)
        {
            Debug.Log("get item");
            var slot = _slotRoot.GetChild(i).GetComponent<CSlotController>();
            slot.SetItem(CItemDropTable.instance.DropRandomItem(CCreateMap.instance.GetStageNumber(), CConstants.EQUIP_ITEM_TYPE));

            _slots.Add(slot);
        }

        for (int i = _slotRoot.childCount - 2; i < _slotRoot.childCount; i++)
        {
            var slot = _slotRoot.GetChild(i).GetComponent<CSlotController>();
            slot.SetItem(CItemDropTable.instance.DropRandomItem(CCreateMap.instance.GetStageNumber(), CConstants.CONSUM_ITEM_TYPE));

            _slots.Add(slot);
        }

        _positionX = 0;
        _positionY = 0;
        _lastSlot = 0;
        _arrow = EArrow._right;

        _notEnoughGoldPopUp = GameObject.Find("NotEnoughGoldPopUp").transform.Find("Canvas").gameObject;
        _confirmPurchaseIntentionPopup = GameObject.Find("ConfirmPurchaseIntentionPopUp");
        _confirmPurchaseIntentionPopup.SetActive(false);

        _player = CController.instance.player;
        _playerPara = _player.GetComponent<CPlayerPara>();

        //debug용 추후 삭제
        _playerPara.Inventory.Gold += 500;

        ShowInventoryGold();
    }
    private void Update()
    {
        ChangeSlot(); //키보드 입력 시 슬롯 변경
        WriteInformation(); //아이템 선택 시 정보창 띄워주기
        BuyItem(); //엔터키 입력시 아이템 구매
    }

    private void ShowInventoryGold()
    {
        int inventoryGold = _playerPara.Inventory.Gold;
        var GoldTMP = GameObject.Find("LeftGold").GetComponent<TMPro.TextMeshProUGUI>();
        GoldTMP.text = GoldTMP.text.Substring(0, 6); //남은 골드 텍스트 남겨두고 뒤에 숫자 자르기
        GoldTMP.text += inventoryGold;
    }

    private void ConfirmPurchaseIntention(GameObject slot)
    {
        //커서 옮기기도 처리 필요함

        //선택한 슬롯의 아이템 이미지와 가격 옮기기
        _confirmPurchaseIntentionPopup.SetActive(true);
        CEventRoomNpcClick.instance._stackPopUp.Push(_confirmPurchaseIntentionPopup);

        string guidanceMessage;

        GameObject buyingSlot = GameObject.Find("BuyingSlot");

        Sprite itemImage = slot.transform.Find("Item").GetComponent<Image>().sprite;
        buyingSlot.transform.Find("Item").GetComponent<Image>().sprite = itemImage;

        string itemPrice = slot.transform.Find("Price").GetComponent<TMPro.TextMeshProUGUI>().text;
        buyingSlot.transform.Find("Price").GetComponent<TMPro.TextMeshProUGUI>().text = itemPrice;

        var itemComponent = slot.GetComponent<CSlotController>().item.GetComponent<CItemComponent>();
        var item = itemComponent.Item;
        //var equip = itemComponent.Item as Item.CEquip;
        guidanceMessage = "'" + item.ItemName + "' 아이템을 구매하시겠습니까?";
        _confirmPurchaseIntentionPopup.transform.GetChild(0).Find("GuidanceMessage").GetComponent<TMPro.TextMeshProUGUI>().text = guidanceMessage;
    }

    private void WriteInformation()
    {
        var itemText = GameObject.Find("StoreItemNameText").GetComponent<TMPro.TextMeshProUGUI>();
        var Extext = GameObject.Find("StoreItemExplanationText").GetComponent<TMPro.TextMeshProUGUI>(); //텍스트 컴포넌트

        if (_slots[_currentSlot].item.GetComponent<CEquipComponent>() != null) //장비템
        {
            var itemComponent = _slots[_currentSlot].item.GetComponent<CEquipComponent>();
            var equip = itemComponent.Item as Item.CEquip;
            itemText.text = equip.ItemName;
            Extext.text = Item.CEquipExplainText.CreateExplainText(equip);
        }
        else //소비템
        {
            var itemComponent = _slots[_currentSlot].item.GetComponent<CConsumableComponent>();
            var consumable = itemComponent.Item as Item.CConsumable;
            itemText.text = consumable.ItemName;
            Extext.text = Item.CConsumableExplainText.CreateExplainText(consumable);
        }
    }

    private bool CheckSoldOut()
    {
        for (int i = 0; i < _slotRoot.childCount; i++) //켜진게 하나라도 있으면 품절아님
            if (_slots[_currentSlot].transform.Find("Item").gameObject.activeSelf == true)
                return false;

        for (int i = 0; i < _slots.Count; i++) //슬롯 전부 꺼버림
            _slots[i].gameObject.SetActive(false);

        return true;
    }

    private void ChangeSlot()
    {
        if (CheckSoldOut())
            return; //상점 품절 체크

        if (_confirmPurchaseIntentionPopup.activeSelf == true) //구매 재확인 팝업 켜져있을경우
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                Image yesButton = _confirmPurchaseIntentionPopup.transform.GetChild(0).Find("Yes").GetComponent<Image>();
                Image noButton = _confirmPurchaseIntentionPopup.transform.GetChild(0).Find("No").GetComponent<Image>();

                if (yesButton.enabled == true)
                {
                    yesButton.enabled = false;
                    noButton.enabled = true;
                }
                else
                {
                    yesButton.enabled = true;
                    noButton.enabled = false;
                }
            }
            return;   
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_positionX > 0)
                _positionX--;
            else
                _positionX = CConstants.SLOT_COLOUMN - 1;

            _arrow = EArrow._left;

            _currentSlot = _positionX + _positionY * CConstants.SLOT_COLOUMN;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_positionX < CConstants.SLOT_COLOUMN - 1)
                _positionX++;
            else
                _positionX = 0;

            _arrow = EArrow._right;

            _currentSlot = _positionX + _positionY * CConstants.SLOT_COLOUMN;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _positionY = (_positionY == 0 ? 1 : 0);

            _arrow = EArrow._up;

            _currentSlot = _positionX + _positionY * CConstants.SLOT_COLOUMN;
        }

        if (_slots[_currentSlot].transform.Find("Item").gameObject.activeSelf == false) //상점에 존재하는 상품이 아닌경우 커서 위치 바꿔야함
        {
            switch (_arrow)
            {
                case EArrow._left:
                    while (_slots[_currentSlot].transform.Find("Item").gameObject.activeSelf == false)
                    {
                        if (_positionX <= 0)
                            _positionX += CConstants.SLOT_COLOUMN - 1;
                        else
                            _positionX--;

                        _currentSlot = _positionX + _positionY * CConstants.SLOT_COLOUMN;
                    }
                    break;

                case EArrow._right:
                    while (_slots[_currentSlot].transform.Find("Item").gameObject.activeSelf == false)
                    {
                        if (_positionX >= CConstants.SLOT_COLOUMN - 1)
                            _positionX -= CConstants.SLOT_COLOUMN - 1;
                        else
                            _positionX++;

                        _currentSlot = _positionX + _positionY * CConstants.SLOT_COLOUMN;
                    }
                    break;

                case EArrow._up:
                    while (_slots[_currentSlot].transform.Find("Item").gameObject.activeSelf == false)
                    {
                        _positionY = (_positionY == 0 ? 1 : 0);

                        _currentSlot = _positionX + _positionY * CConstants.SLOT_COLOUMN;
                    }
                    break;
            }
        }

        _slots[_lastSlot].transform.Find("Image").gameObject.SetActive(false);
        _slots[_currentSlot].transform.Find("Image").gameObject.SetActive(true);
        _lastSlot = _currentSlot;
    }

    private void BuyItem()
    {
        GameObject slot = _slots[_currentSlot].gameObject;

        if (_confirmPurchaseIntentionPopup.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.Return)) //구매 재확인 팝업에서 예 아니오 중 하나 선택
            {
                Image yesButton = _confirmPurchaseIntentionPopup.transform.GetChild(0).Find("Yes").GetComponent<Image>();
                Image noButton = _confirmPurchaseIntentionPopup.transform.GetChild(0).Find("No").GetComponent<Image>();

                if (yesButton.enabled == true)
                {
                    //예 버튼 누른경우. 아이템 구매 그대로 쓰고 false
                    if(!MoveItemToInventory(slot.GetComponent<CSlotController>().item.GetComponent<CItemComponent>())) //아이템 인벤토리로 이동
                    { //이동 실패한 경우(아이템칸 부족) 추후 구현...
                        Debug.Log("item not buying");
                    }

                    UseGoldInInventory(slot.transform.Find("Price").GetComponent<TMPro.TextMeshProUGUI>().text); //인벤토리에서 골드 사용

                    RemoveItemNMoveCursor(slot); //아이템 상점에서 빼고 커서 이동

                    ShowInventoryGold();

                    _confirmPurchaseIntentionPopup.SetActive(false);
                    CEventRoomNpcClick.instance._stackPopUp.Pop();
                }
                else if (noButton.enabled == true)
                {
                    _confirmPurchaseIntentionPopup.SetActive(false);
                    CEventRoomNpcClick.instance._stackPopUp.Pop();
                }            
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return)) //엔터키 입력 시 아이템 구매 확인 팝업
        {
            if (_notEnoughGoldPopUp.activeSelf) //골드 부족함 팝업이 켜져 있는 경우 엔터키 다시 누르면 꺼지게 하기
            {
                _notEnoughGoldPopUp.SetActive(false);
                CEventRoomNpcClick.instance._stackPopUp.Pop();
                return;
            }
            else
                if (!CheckEnoughGold(slot))
            {
                _notEnoughGoldPopUp.SetActive(true);
                CEventRoomNpcClick.instance._stackPopUp.Push(_notEnoughGoldPopUp); //골드 부족함 팝업 띄우기
                return;
            }

            ConfirmPurchaseIntention(slot);       
        }
    }

    private bool CheckEnoughGold(GameObject slot)
    {
        //인벤토리 골드와 상점판매가 비교후 리턴
        int inventoryGold = _playerPara.Inventory.Gold;
        string shopPrice = slot.transform.Find("Price").GetComponent<TMPro.TextMeshProUGUI>().text;

        if (inventoryGold >= int.Parse(shopPrice))
            return true;

        return false;
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

    private void UseGoldInInventory(string shopPrice)
    { 
        _playerPara.Inventory.Gold -= int.Parse(shopPrice);
    }

    private void RemoveItemNMoveCursor(GameObject slot)
    {
        for (int i = 0; i < slot.transform.childCount; i++)
            slot.transform.GetChild(i).gameObject.SetActive(false);

        //아이템 구매시 커서 이동
        for (int i = 0; i < _slots.Count; i++) //리스트 가장 앞쪽부터 확인해서 커서 옮김
            if (_slots[i].transform.Find("Item").gameObject.activeSelf == true)
            {
                _currentSlot = i;
                _positionX = i % CConstants.SLOT_COLOUMN;
                _positionY = i / CConstants.SLOT_COLOUMN;

                _slots[_lastSlot].transform.Find("Image").gameObject.SetActive(false);
                _slots[_currentSlot].transform.Find("Image").gameObject.SetActive(true);
                _lastSlot = _currentSlot;
                break;
            }
    }
}
