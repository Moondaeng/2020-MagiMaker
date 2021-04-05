using System;
using UnityEngine;
using UnityEngine.UI;

public class CElementWindow : MonoBehaviour
{
    private static readonly int MAX_CONTANING_ELEMENT = 4;

    [System.Serializable]
    class ElementInfo
    {
        public Transform mainElementPanel;
        public Transform[] subElementPanels = new Transform[3];
    }

    [SerializeField, EnumNamedArray(typeof(CPlayerSkill.ESkillElement))]
    private Sprite[] _elementImageArr = new Sprite[Enum.GetValues(typeof(CPlayerSkill.ESkillElement)).Length];
    private Transform[] mainElementPanels = new Transform[2];
    private Transform[,] subElementPanels = new Transform[2, 3];

    public Action closeWindowCallback;

    private void Awake()
    {
        // 주원소 개수만큼 패널 초기화
        for (int i = 0; i < 2; i++)
        {
            Debug.Log($"ElementPanel{i}/MainElementPanel");
            mainElementPanels[i] = transform.Find($"ElementPanel{i}").Find("MainElementPanel");
            for (int j = 0; j < 3; j++)
            {
                subElementPanels[i, j] = transform.Find($"ElementPanel{i}").Find($"SubElementPanel{j}");
            }
        }
    }

    public void DrawPlayerElement(CPlayerSkill playerSkill)
    {
        for (int i = 0; i < 2; i++)
        {
            SetElementImage(
                playerSkill.GetElementNumber(true, i, 0),
                mainElementPanels[i].Find("Image").GetComponent<Image>()
            );
            for (int j = 0; j < 3; j++)
            {
                SetElementImage(
                    playerSkill.GetElementNumber(false, i, j),
                    subElementPanels[i, j].Find("Image").GetComponent<Image>()
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
                mainElementPanels[i].Find("Button").GetComponent<Button>().interactable = true;
                var slotNumber = i;
                ChangeElement(playerSkill, isMainElement, element, slotNumber, 0);
                for (int j = 0; j < 3; j++)
                {
                    subElementPanels[i, j].Find("Button").GetComponent<Button>().interactable = false;
                }
            }
            else
            {
                mainElementPanels[i].Find("Button").GetComponent<Button>().interactable = false;
                for (int j = 0; j < 3; j++)
                {
                    subElementPanels[i, j].Find("Button").GetComponent<Button>().interactable = true;
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
