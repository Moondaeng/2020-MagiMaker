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
    private EArrow _arrow;
    private bool allBuyCheckFlag; //상점 아이템 다팔렸는지 확인
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
            slot.SetItem(CItemDropTable.instance.DropRandomItem(CCreateMap.instance.createStageNumber));

            _slots.Add(slot);
        }

        _positionX = 0;
        _positionY = 0;
        _lastSlot = 0;
        _arrow = EArrow._right;
        allBuyCheckFlag = false;
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

            itemText.text = itemComponent.equipStat.ItemName;
            
            if(itemComponent.equipStat.As != 0)
            {
                sumExtext += "공격속도 : " + itemComponent.equipStat.As.ToString() + "\n";
            }

            if (itemComponent.equipStat.Atk != 0)
            {
                sumExtext += "공격력 : " + itemComponent.equipStat.Atk.ToString() + "\n";
            }

            if (itemComponent.equipStat.Def != 0)
            {
                sumExtext += "방어력 : " + itemComponent.equipStat.Def.ToString() + "\n";
            }

            if (itemComponent.equipStat.MaxHp != 0)
            {
                sumExtext += "최대 체력 : " + itemComponent.equipStat.MaxHp.ToString() + "\n";
            }

            if (itemComponent.equipStat.HpRegen != 0)
            {
                sumExtext += "초당 체력회복량 : " + itemComponent.equipStat.HpRegen.ToString() + "\n";
            }

            if (itemComponent.equipStat.Spd != 0)
            {
                sumExtext += "이동속도 : " + itemComponent.equipStat.Spd.ToString() + "\n";
            }

            if (itemComponent.equipStat._skillCoolTime != 0)
            {
                sumExtext += "스킬쿨타임 감소율 : " + itemComponent.equipStat._skillCoolTime.ToString() + "%" + "\n";
            }

            if (itemComponent.equipStat._damageTakenRate != 0)
            {
                sumExtext += "받는 피해량 감소율 : " + itemComponent.equipStat._damageTakenRate.ToString() + "%" + "\n";
            }

            if (itemComponent.equipStat._skillRange != 0)
            {
                sumExtext += "스킬 사거리 증가율 : " + itemComponent.equipStat._skillRange.ToString() + "%" + "\n";
            }

            Extext.text = sumExtext;
        }
        else //소비템
        {
            var itemComponent = _slots[_currentSlot].item.GetComponent<CConsumableComponent>();
        }
    }

    private void CheckSoldOut()
    {
        if (allBuyCheckFlag == true) //상점 품목 전부 구매 시
        {
            for (int i = 0; i < _slots.Count; i++) //슬롯 전부 꺼버림
            {
                _slots[i].gameObject.SetActive(false);
            }

            return;
        }
    }

    private void ChangeSlot()
    {
        CheckSoldOut(); //상점 품절 체크

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
            Debug.Log("Buy Item"); //인벤토리에 넣는 코드 구현해야함

            GameObject slot = _slots[_currentSlot].gameObject;
            for (int i = 0; i < slot.transform.childCount; i++)
                slot.transform.GetChild(i).gameObject.SetActive(false);

            //아이템 구매시 커서 이동
            allBuyCheckFlag = true;
            for (int i = 0; i < _slots.Count; i++) //리스트 가장 앞쪽부터 확인해서 커서 옮김
                if (_slots[i].transform.FindChild("Item").gameObject.activeSelf == true)
                {
                    _currentSlot = i;
                    _positionX = i % CConstants.SLOT_COLOUMN;
                    _positionY = i / CConstants.SLOT_COLOUMN;
                    allBuyCheckFlag = false;

                    _slots[_lastSlot].transform.FindChild("Image").gameObject.SetActive(false);
                    _slots[_currentSlot].transform.FindChild("Image").gameObject.SetActive(true);
                    _lastSlot = _currentSlot;
                    break;
                }
        }
    }
}
