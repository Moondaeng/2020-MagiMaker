using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CPlayerInventoryWindow : MonoBehaviour
{
    private const int EQUIP_BUTTON_COUNT = 12;
    private const int CONSUMABLE_BUTTON_COUNT = 3;

    private enum EItemType
    {
        None,
        Equip,
        Consumable
    }

    public static CPlayerInventoryWindow instance;

    #region 아이템 버리기 처리
    // 버리기 찾기
    [SerializeField] GraphicRaycaster m_Raycaster;
    [SerializeField] private EventSystem m_EventSystem;
    PointerEventData m_PointerEventData;

    private EItemType dropItemType;
    private int dropItemNumber;

    // 버리기 그리기
    private bool isDroped = false;
    [SerializeField] private float DropWaitTime = 1.0f;
    [SerializeField] GameObject DropItemCursor;
    [SerializeField] Image DropItemGauge;
    #endregion

    private EItemType _currentExplainItemType;
    private int _currentExplainItemNumber;
    [SerializeField] private GameObject _itemExplainPanel;

    private GameObject[] _equipBtnObjectArr = new GameObject[EQUIP_BUTTON_COUNT];
    private GameObject[] _consumableBtnObjectArr = new GameObject[CONSUMABLE_BUTTON_COUNT];

    private CInventory _inventory;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for(int i = 0; i < EQUIP_BUTTON_COUNT; i++)
        {
            _equipBtnObjectArr[i] = transform.Find("ItemSelectPanel/EquipSelectBtn" + i).gameObject;
            if(_equipBtnObjectArr[i] == null)
            {
                Debug.Log($"Can't Find {gameObject.name}'s Chile Object EquipSelectBtn{i}");
            }
            else
            {
                int btnNumber = i;
                _equipBtnObjectArr[i].GetComponent<Button>().onClick.AddListener(() => DrawItemInformation(EItemType.Equip, btnNumber));
            }
        }
        for (int i = 0; i < CONSUMABLE_BUTTON_COUNT; i++)
        {
            _consumableBtnObjectArr[i] = transform.Find("ItemSelectPanel/ConsumableSelectBtn" + i).gameObject;
            if (_consumableBtnObjectArr[i] == null)
            {
                Debug.Log($"Can't Find {gameObject.name}'s Chile Object ConsumableSelectBtn{i}");
            }
            else
            {
                int btnNumber = i;
                _consumableBtnObjectArr[i].GetComponent<Button>().onClick.AddListener(() => DrawItemInformation(EItemType.Consumable, btnNumber));
            }
        }
    }

    void Update()
    {
        // 버리기 처리
        if (Input.GetMouseButtonDown(1))
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);

            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            Vector3 dropItemPos = results[0].gameObject.transform.position;
            string hitItem = results[0].gameObject.name;
            
            if (hitItem.Contains("EquipSelectBtn"))
            {
                dropItemType = EItemType.Equip;
                dropItemNumber = int.Parse(hitItem.Substring("EquipSelectBtn".Length, hitItem.Length - "EquipSelectBtn".Length));
                if (_inventory.EquipItems.Count <= dropItemNumber || _inventory.EquipItems[dropItemNumber] == null)
                {
                    dropItemType = EItemType.None;
                }
            }
            else if (hitItem.Contains("ConsumableSelectBtn"))
            {
                dropItemType = EItemType.Consumable;
                dropItemNumber = int.Parse(hitItem.Substring("ConsumableSelectBtn".Length, hitItem.Length - "ConsumableSelectBtn".Length));
                if (_inventory.ConsumableItems.Count <= dropItemNumber || _inventory.ConsumableItems[dropItemNumber] == null)
                {
                    dropItemType = EItemType.None;
                }
            }
            else
            {
                dropItemType = EItemType.None;
            }

            if (dropItemType != EItemType.None)
            {
                DropItemCursor.SetActive(true);
                DropItemCursor.transform.position = dropItemPos;
            }
        }

        if (Input.GetMouseButton(1) && dropItemType != EItemType.None && !isDroped)
        {
            if (DropItemGauge.fillAmount >= 1)
            {
                CancelDrawingDrop();
                // 버리기 행동
                DropItem();
                DrawItemSelectPanel(_inventory);
            }
            else
            {
                DropItemGauge.fillAmount += Time.deltaTime / DropWaitTime;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            CancelDrawingDrop();
        }
    }

    private void CancelDrawingDrop()
    {
        isDroped = false;
        DropItemGauge.fillAmount = 0;
        DropItemCursor.SetActive(false);
    }

    private void DropItem()
    {
        if (dropItemType == EItemType.Equip)
        {
            if (_inventory.DeleteEquipItem(dropItemNumber))
            {
                if (_currentExplainItemType == dropItemType && _currentExplainItemNumber == dropItemNumber)
                {
                    InitItemInformation();
                }
            }
            else
            {
                Debug.Log("귀속 아이템은 버릴 수 없습니다");
            }
        }
        else if (dropItemType == EItemType.Consumable)
        {
            Debug.Log("Can't drop consumable");
        }
    }

    private void OnDisable()
    {
        // 버리기 게이지 초기화
        CancelDrawingDrop();

        // 아이템 정보 초기화

    }

    public void OpenInventory(CInventory inventory)
    {
        Debug.Log("Open inventory");
        DrawItemSelectPanel(inventory);
        InitItemInformation();
    }

    private void DrawItemSelectPanel(CInventory inventory)
    {
        _inventory = inventory;

        for(int i = 0; i < EQUIP_BUTTON_COUNT; i++)
        {
            if (i < inventory.EquipItems.Count)
            {
                _equipBtnObjectArr[i].gameObject.GetComponent<Image>().sprite = _inventory.EquipItems[i].ItemImage;
                _equipBtnObjectArr[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                _equipBtnObjectArr[i].gameObject.GetComponent<Image>().sprite = null;
                _equipBtnObjectArr[i].GetComponent<Button>().interactable = false;
            }
        }
        for (int i = 0; i < CONSUMABLE_BUTTON_COUNT; i++)
        {
            if (i < inventory.ConsumableItems.Count)
            {
                _consumableBtnObjectArr[i].gameObject.GetComponent<Image>().sprite = _inventory.ConsumableItems[i].consumable.ItemImage;
                _consumableBtnObjectArr[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                _consumableBtnObjectArr[i].gameObject.GetComponent<Image>().sprite = null;
                _consumableBtnObjectArr[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    private void InitItemInformation()
    {
        Text itemName = _itemExplainPanel.transform.Find("ItemName").GetComponent<Text>();
        Text itemGrade = _itemExplainPanel.transform.Find("ItemGrade").GetComponent<Text>();
        Text itemExplain = _itemExplainPanel.transform.Find("ItemExplain").GetComponent<Text>();
        Image itemImage = _itemExplainPanel.transform.Find("Image").GetComponent<Image>();

        itemName.text = "";
        itemGrade.text = "";
        itemExplain.text = "";
        itemImage.sprite = null;

        _currentExplainItemType = EItemType.None;
        _currentExplainItemNumber = -1;
    }

    private void DrawItemInformation(EItemType type, int buttonNumber)
    {
        if (_inventory == null)
        {
            Debug.Log("_inventory is null");
            return;
        }

        _currentExplainItemType = type;
        _currentExplainItemNumber = buttonNumber;

        Text itemName = _itemExplainPanel.transform.Find("ItemName").GetComponent<Text>();
        Text itemGrade = _itemExplainPanel.transform.Find("ItemGrade").GetComponent<Text>();
        Text itemExplain = _itemExplainPanel.transform.Find("ItemExplain").GetComponent<Text>();
        Image itemImage = _itemExplainPanel.transform.Find("Image").GetComponent<Image>();

        if (type == EItemType.Equip)
        {
            itemName.text = _inventory.EquipItems[buttonNumber].ItemName;
            itemGrade.text = Item.CItemExplainText.ItemCodeToGrade(_inventory.EquipItems[buttonNumber].ItemCode);
            itemExplain.text = Item.CEquipExplainText.CreateExplainText(_inventory.EquipItems[buttonNumber]);
            itemImage.sprite = _inventory.EquipItems[buttonNumber].ItemImage;
        }
        else if(type == EItemType.Consumable)
        {
            itemName.text = _inventory.ConsumableItems[buttonNumber].consumable.ItemName;
            itemGrade.text = Item.CItemExplainText.ItemCodeToGrade(_inventory.ConsumableItems[buttonNumber].consumable.ItemCode);
            itemExplain.text = Item.CConsumableExplainText.CreateExplainText(_inventory.ConsumableItems[buttonNumber].consumable);
            itemImage.sprite = _inventory.ConsumableItems[buttonNumber].consumable.ItemImage;
        }
    }
}
