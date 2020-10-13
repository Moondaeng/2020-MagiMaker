using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStoreController : MonoBehaviour
{
    private Transform _slotRoot;
    private List<CSlotController> _slots;

    private int _positionX; //아이템 선택 위치
    private int _positionY;
    private int _currentSlot; //현재 슬롯
    private int _lastSlot;  //이전 슬롯

    private GameObject _notEnoughGoldPopUp; //골드 부족합니다 팝업
    private EArrow _arrow;
    private static string[] _equipAbilityExplainArr //아이템 능력치 한글명
    = new string[] {
            "공격력",
            "공격속도",
            "최대 체력",
            "초당 체력 회복",
            "방어력",
            "이동속도",
            "스킬 쿨타임 감소율",
            "받는 피해량 감소율",
            "스킬 사거리 증가율"
    };
    enum EArrow
    {
        _left,
        _right,
        _up,
    }
    // Start is called before the first frame update
    void Start()
    {
        _slotRoot = transform.FindChild("SlotGroup");
        _slots = new List<CSlotController>();

        for (int i = 0; i < _slotRoot.childCount; i++)
        {
            var slot = _slotRoot.GetChild(i).GetComponent<CSlotController>();
            slot.SetItem(CItemDropTable.instance.DropRandomItem(CCreateMap.instance.GetStageNumber()));

            _slots.Add(slot);
        }

        _positionX = 0;
        _positionY = 0;
        _lastSlot = 0;
        _arrow = EArrow._right;

        _notEnoughGoldPopUp = GameObject.Find("NotEnoughGoldPopUp");
        _notEnoughGoldPopUp.SetActive(false);
    }
    private void Update()
    {
        ChangeSlot(); //키보드 입력 시 슬롯 변경
        WriteInformation(); //아이템 선택 시 정보창 띄워주기
        BuyItem(); //엔터키 입력시 아이템 구매
    }

    private void WriteInformation()
    {
        var itemText = GameObject.Find("StoreItemNameText").GetComponent<TMPro.TextMeshProUGUI>();
        var Extext = GameObject.Find("StoreItemExplanationText").GetComponent<TMPro.TextMeshProUGUI>(); //텍스트 컴포넌트
        string sumExtext = null;

        if (_slots[_currentSlot].item.GetComponent<CEquipComponent>() != null) //장비템
        {
            var itemComponent = _slots[_currentSlot].item.GetComponent<CEquipComponent>();
            var equip = itemComponent.Item as Item.CEquip;

            itemText.text = equip.ItemName;

            foreach (var ability in equip.equipAbilities)
            {
                sumExtext += _equipAbilityExplainArr[(int)ability.equipEffect] + ability.value;
                if (ability.equipEffect == Item.EEquipAbility.DamageReduceRate || ability.equipEffect == Item.EEquipAbility.SkillCoolTime || ability.equipEffect == Item.EEquipAbility.SkillRange)
                    sumExtext += "%";
                sumExtext = "\n";
            }
            Extext.text = sumExtext;
        }
        else //소비템
        {
            var itemComponent = _slots[_currentSlot].item.GetComponent<CConsumableComponent>();
        }
    }

    private bool CheckSoldOut()
    {
        for (int i = 0; i < _slotRoot.childCount; i++) //켜진게 하나라도 있으면 품절아님
            if (_slots[_currentSlot].transform.FindChild("Item").gameObject.activeSelf == true)
                return false;

        for (int i = 0; i < _slots.Count; i++) //슬롯 전부 꺼버림
            _slots[i].gameObject.SetActive(false);

        return true;
    }

    private void ChangeSlot()
    {
        if (CheckSoldOut())
            return; //상점 품절 체크

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

        if (_slots[_currentSlot].transform.FindChild("Item").gameObject.activeSelf == false) //상점에 존재하는 상품이 아닌경우 커서 위치 바꿔야함
        {
            switch (_arrow)
            {
                case EArrow._left:
                    while (_slots[_currentSlot].transform.FindChild("Item").gameObject.activeSelf == false)
                    {
                        if (_positionX <= 0)
                            _positionX += CConstants.SLOT_COLOUMN - 1;
                        else
                            _positionX--;

                        _currentSlot = _positionX + _positionY * CConstants.SLOT_COLOUMN;
                    }
                    break;

                case EArrow._right:
                    while (_slots[_currentSlot].transform.FindChild("Item").gameObject.activeSelf == false)
                    {
                        if (_positionX >= CConstants.SLOT_COLOUMN - 1)
                            _positionX -= CConstants.SLOT_COLOUMN - 1;
                        else
                            _positionX++;

                        _currentSlot = _positionX + _positionY * CConstants.SLOT_COLOUMN;
                    }
                    break;

                case EArrow._up:
                    while (_slots[_currentSlot].transform.FindChild("Item").gameObject.activeSelf == false)
                    {
                        _positionY = (_positionY == 0 ? 1 : 0);

                        _currentSlot = _positionX + _positionY * CConstants.SLOT_COLOUMN;
                    }
                    break;
            }
        }

        _slots[_lastSlot].transform.FindChild("Image").gameObject.SetActive(false);
        _slots[_currentSlot].transform.FindChild("Image").gameObject.SetActive(true);
        _lastSlot = _currentSlot;
    }

    private void BuyItem()
    {
        if (Input.GetKeyDown(KeyCode.Return)) //엔터키 입력 시 아이템 구매
        {
            GameObject slot = _slots[_currentSlot].gameObject;

            if (_notEnoughGoldPopUp.activeSelf) //골드 부족함 팝업이 켜져 있는 경우 엔터키 다시 누르면 꺼지게 하기
            {
                _notEnoughGoldPopUp.SetActive(false);
            }
            else
                if (!CheckEnoughGold(slot))
            {
                StartCoroutine("DisplayNotEnoughGoldPopUp"); //시간 지나면 꺼지는 골드 부족함 팝업 띄우기
                return;
            }

            MoveItemToInventory(slot.GetComponent<CItemComponent>()); //아이템 인벤토리로 이동

            UseGoldInInventory(slot); //인벤토리에서 골드 사용

            RemoveItemNMoveCursor(slot); //아이템 상점에서 빼고 커서 이동
        }
    }

    IEnumerator DisplayNotEnoughGoldPopUp()
    {
        //팝업 띄우기
        _notEnoughGoldPopUp.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        //팝업 끄기
        _notEnoughGoldPopUp.SetActive(false);
    }

    private bool CheckEnoughGold(GameObject slot)
    {
        //인벤토리 골드와 상점판매가 비교후 리턴
        GameObject player = CController.instance.player;
        var playerPara = player.GetComponent<CPlayerPara>();
        int inventoryGold = playerPara.Inventory.GetGold();
        string shopPrice = slot.transform.FindChild("Price").GetComponent<TMPro.TextMeshProUGUI>().text;

        if (inventoryGold > int.Parse(shopPrice))
            return true;

        return false;
    }

    private void MoveItemToInventory(CItemComponent itemComponent)
    {
        GameObject player = CController.instance.player;
        var playerPara = player.GetComponent<CPlayerPara>();

        if (itemComponent != null)
        {
            if (itemComponent.Item is Item.CEquip) //장비템
                playerPara.Inventory.AddEquip(itemComponent.Item as Item.CEquip);

            else if (itemComponent.Item is Item.CConsumable) //소비템
                playerPara.Inventory.AddConsumableItem(itemComponent.Item as Item.CConsumable);
        }
    }

    private void UseGoldInInventory(GameObject slot)
    {
        string shopPrice = slot.transform.FindChild("Price").GetComponent<TMPro.TextMeshProUGUI>().text;

        GameObject player = CController.instance.player;
        var playerPara = player.GetComponent<CPlayerPara>();
        playerPara.Inventory.SetGold(-int.Parse(shopPrice));
    }

    private void RemoveItemNMoveCursor(GameObject slot)
    {
        for (int i = 0; i < slot.transform.childCount; i++)
            slot.transform.GetChild(i).gameObject.SetActive(false);

        //아이템 구매시 커서 이동
        for (int i = 0; i < _slots.Count; i++) //리스트 가장 앞쪽부터 확인해서 커서 옮김
            if (_slots[i].transform.FindChild("Item").gameObject.activeSelf == true)
            {
                _currentSlot = i;
                _positionX = i % CConstants.SLOT_COLOUMN;
                _positionY = i / CConstants.SLOT_COLOUMN;

                _slots[_lastSlot].transform.FindChild("Image").gameObject.SetActive(false);
                _slots[_currentSlot].transform.FindChild("Image").gameObject.SetActive(true);
                _lastSlot = _currentSlot;
                break;
            }
    }
}
