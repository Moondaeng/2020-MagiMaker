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

        int slotCount = _slotRoot.childCount;

        for (int i = 0; i < slotCount; i++)
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
        _arrow = EArrow._right;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_positionX > 0)
                _positionX--;
            else
                _positionX = 3;

            _arrow = EArrow._left;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_positionX < 3)
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

        if(_slots[_positionX + _positionY * 4].item.transform.FindChild("Item").gameObject.activeSelf == true) //상점에 존재하는 상품인지 확인
        {
            
        }
        else
        {

        }
    }
}
