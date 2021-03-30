using System;
using UnityEngine;
using UnityEngine.UI;

public class CElementObtainViewer : MonoBehaviour
{
    public static CElementObtainViewer instance;

    private static readonly int MAX_CONTANING_ELEMENT = 4;

    [System.Serializable]
    class ElementInfo
    {
        public Transform mainElementPanel;
        public Transform[] subElementPanels = new Transform[3];
    }

    [Serializable]
    class ChangingElement
    {
        Image image;
        CPlayerSkill.ESkillElement retaining;
    }

    [SerializeField, EnumNamedArray(typeof(CPlayerSkill.ESkillElement))]
    private Sprite[] _elementImageArr = new Sprite[Enum.GetValues(typeof(CPlayerSkill.ESkillElement)).Length];
    private ElementInfo[] _elementInfos = new ElementInfo[2];
    private ChangingElement changingInfo;

    public Action closeWindowCallback;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        Debug.Log(_elementInfos[0] + " ** elementalinfos");
        Debug.Log(_elementInfos[1] + " ** elementalinfos");

        // 주원소 개수만큼 패널 초기화
        for (int i = 0; i < 2; i++)
        {        
            _elementInfos[i].mainElementPanel = transform.Find($"ElementPanel{i}/MainElementPanel");
            for (int j = 0; j < 3; j++)
            {
                _elementInfos[i].mainElementPanel = transform.Find($"ElementPanel{i}/SubElementPanel{j}");
            }
        }
    }

    public void DrawPlayerElement(CPlayerSkill playerSkill)
    {
        for (int i = 0; i < 2; i++)
        {
            SetElementImage(
                playerSkill.GetElementNumber(true, i, 0),
                _elementInfos[i].mainElementPanel.Find("Image").GetComponent<Image>()
            );
            for (int j = 0; j < 3; j++)
            {
                SetElementImage(
                    playerSkill.GetElementNumber(false, i, j),
                    _elementInfos[i].subElementPanels[j].Find("Image").GetComponent<Image>()
                );
            }
        }
    }

    public void SetChangingElementState(CPlayerSkill playerSkill, bool isMainElement, CPlayerSkill.ESkillElement element)
    {
        for (int i = 0; i < 2; i++)
        {
            if (isMainElement)
            {
                _elementInfos[i].mainElementPanel.Find("Button").GetComponent<Button>().interactable = true;
                var slotNumber = i;
                ChangeElement(playerSkill, isMainElement, element, slotNumber, 0);
                for (int j = 0; j < 3; j++)
                {
                    _elementInfos[i].subElementPanels[j].Find("Button").GetComponent<Button>().interactable = false;
                }
            }
            else
            {
                _elementInfos[i].mainElementPanel.Find("Button").GetComponent<Button>().interactable = false;
                for (int j = 0; j < 3; j++)
                {
                    _elementInfos[i].subElementPanels[j].Find("Button").GetComponent<Button>().interactable = true;
                    var mainSlotNumber = i;
                    var subSlotNumber = j;
                    ChangeElement(playerSkill, isMainElement, element, mainSlotNumber, subSlotNumber);
                }
            }
        }
    }

    private void SetElementImage(int element, Image image)
    {
        if (element == (int)CPlayerSkill.ESkillElement.None)
        {
            // 원소 없는 경우 이미지 매칭
            element = _elementImageArr.Length - 1;
        }
        image.sprite = _elementImageArr[element];
    }

    private void ChangeElement(CPlayerSkill playerSkill, bool isMainElement, CPlayerSkill.ESkillElement element, int mainSlot, int subSlot)
    {
        if (isMainElement)
        {
            playerSkill.SetMainElement(mainSlot, element);
        }
        else
        {
            playerSkill.SetSubElement(mainSlot, subSlot, element);
        }

        closeWindowCallback?.Invoke();
    }
}
