using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStoreController : MonoBehaviour
{
    private Transform _slotRoot;
    private List<CSlotController> _slots;
    private int _positionX; //아이템 선택 위치
    private int _positionY;
    private int _currentSlot;
    private int _lastSlot;  //이전 슬롯
    private EArrow _arrow;
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
            //slot.SetItem(CItemDropTable.instance.DropRandomItem(CCreateMap.instance.createStageNumber));

            //debug 임시코드
            GameObject testItem;
            if (i % 2 == 0)
            {
                testItem = GameObject.Find("Item1");
                if (testItem != null)
                    slot.SetItem(testItem);
                else
                {
                    Debug.Log("item is null");
                    continue;
                }
            }
            else
            {
                testItem = GameObject.Find("Item2");
                if (testItem != null)
                    slot.SetItem(testItem);
                else
                {
                    Debug.Log("item is null");
                    continue;
                }
            }
            //임시코드 끝
            _slots.Add(slot);
        }

        _positionX = 0;
        _positionY = 0;
        _lastSlot = 0;
        _arrow = EArrow._right;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_positionX > 0)
                _positionX--;
            else
                _positionX = CConstants.SLOT_COLOUMN - 1;

            _arrow = EArrow._left;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_positionX < CConstants.SLOT_COLOUMN - 1)
                _positionX++;
            else
                _positionX = 0;

            _arrow = EArrow._right;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _positionY = (_positionY == 0 ? 1 : 0);

            _arrow = EArrow._up;
        }

        _currentSlot = _positionX + _positionY * CConstants.SLOT_COLOUMN;

        if (_slots[_currentSlot].gameObject.transform.FindChild("Item").gameObject.activeSelf == false) //상점에 존재하는 상품이 아닌경우 커서 위치 바꿔야함
        {
            switch (_arrow)
            {
                case EArrow._left: //아이템 구매할때 예외처리 따로 필요함 예시) 맨 윗줄 3개 아이템 이미 구매한 상태에서 마지막 남은 1개 구매할 때 -> 아마 구매할때 액티브인 슬롯 포문 돌려서 찾아서 거따 두면 될듯
                    while (_slots[_currentSlot].gameObject.transform.FindChild("Item").gameObject.activeSelf == false)
                    {
                        if (_positionX == 0 || _positionX == 4)
                            _positionX += 3;
                        else
                            _positionX--;

                        _currentSlot = _positionX + _positionY * CConstants.SLOT_COLOUMN;
                    }
                    break;

                case EArrow._right:
                    while (_slots[_currentSlot].gameObject.transform.FindChild("Item").gameObject.activeSelf == false)
                    {
                        if (_positionX == 3 || _positionX == 7)
                            _positionX -= 3;
                        else
                            _positionX++;

                        _currentSlot = _positionX + _positionY * CConstants.SLOT_COLOUMN;
                    }
                    break;

                case EArrow._up:
                    while (_slots[_currentSlot].gameObject.transform.FindChild("Item").gameObject.activeSelf == false)
                    {
                        _positionY = (_positionY == 0 ? 1 : 0);

                        _currentSlot = _positionX + _positionY * CConstants.SLOT_COLOUMN;
                    }
                    break;
            }
        }

        _slots[_lastSlot].gameObject.transform.FindChild("Image").gameObject.SetActive(false);
        _slots[_currentSlot].gameObject.transform.FindChild("Image").gameObject.SetActive(true);
        _lastSlot = _currentSlot;

    }
}
