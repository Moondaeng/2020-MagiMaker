using System;
using UnityEngine;
using UnityEngine.UI;

public class CElementObtainViewer : MonoBehaviour
{
    public static CElementObtainViewer instance;

    private static readonly int MAX_CONTANING_ELEMENT = 4;

    [SerializeField, EnumNamedArray(typeof(CPlayerSkill.ESkillElement))]
    private Sprite[] _elementImageArr = new Sprite[Enum.GetValues(typeof(CPlayerSkill.ESkillElement)).Length];
    private Transform[] mainElementPanels = new Transform[2];
    private Transform[,] subElementPanels = new Transform[2, 3];

    // 획득했음에도 사용하지 않으면 팝업 나오는 용
    private CPlayerSkill.ESkillElement _currentObtainingElement;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // 주원소 개수만큼 패널 초기화
        for (int i = 0; i < 2; i++)
        {
            mainElementPanels[i] = transform.Find($"ElementPanel{i}").Find("MainElementPanel");
            for (int j = 0; j < 3; j++)
            {
                subElementPanels[i, j] = transform.Find($"ElementPanel{i}").Find($"SubElementPanel{j}");
            }
        }

        transform.Find("CancelButton").GetComponent<Button>().onClick.AddListener(CancelChangeElement);
    }

    public void OpenViewer(CPlayerSkill playerSkill, bool isMainElement, CPlayerSkill.ESkillElement element)
    {
        SetActiveViewer(true);
        DrawPlayerElement(playerSkill);
        DrawObtainedElement(isMainElement, element);
        SetObtainChangeButton(playerSkill, isMainElement, element);
    }

    private void DrawPlayerElement(CPlayerSkill playerSkill)
    {
        for (int i = 0; i < 2; i++)
        {
            SetElementImage(
                playerSkill.GetElementNumber(true, i, 0),
                mainElementPanels[i].Find("ElementImage").GetComponent<Image>()
            );
            for (int j = 0; j < 3; j++)
            {
                SetElementImage(
                    playerSkill.GetElementNumber(false, i, j),
                    subElementPanels[i, j].Find("ElementImage").GetComponent<Image>()
                );
            }
        }
    }

    private void DrawObtainedElement(bool isMainElement, CPlayerSkill.ESkillElement element)
    {
        var panel = transform.Find("ObtainedElementPanel");
        panel.Find("ElementImage").GetComponent<Image>().sprite = _elementImageArr[(int)element];
        panel.Find("MainElementText").GetComponent<TMPro.TMP_Text>().text = isMainElement ? "주원소" : "부원소";
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

    private void SetObtainChangeButton(CPlayerSkill playerSkill, bool isMainElement, CPlayerSkill.ESkillElement element)
    {
        for (int i = 0; i < 2; i++)
        {
            var mainElementButton = mainElementPanels[i].Find("ObtainChangeButton").GetComponent<Button>();
            var mainSlotNumber = i;
            mainElementButton.onClick.AddListener(() => ChangeElement(playerSkill, isMainElement, element, mainSlotNumber, 0));
            mainElementButton.interactable = isMainElement ? true : false;
            for (int j = 0; j < 3; j++)
            {
                var subElementButton = subElementPanels[i, j].Find("ObtainChangeButton").GetComponent<Button>();
                var subSlotNumber = j;
                subElementButton.onClick.AddListener(() => ChangeElement(playerSkill, isMainElement, element, mainSlotNumber, subSlotNumber));
                subElementButton.interactable = !isMainElement ? true : false;
            }
        }
    }

    private void ChangeElement(CPlayerSkill playerSkill, bool isMainElement, CPlayerSkill.ESkillElement element, int mainSlot, int subSlot)
    {
        Debug.Log("ChangeElement function");

        if (isMainElement)
        {
            playerSkill.SetMainElement(mainSlot, element);
        }
        else
        {
            playerSkill.SetSubElement(mainSlot, subSlot, element);
        }
        SetActiveViewer(false);
    }

    private void CancelChangeElement()
    {
        SetActiveViewer(false);
    }

    private void SetActiveViewer(bool isOpen)
    {
        gameObject.SetActive(isOpen);
        CController.instance.SetControlLock(isOpen);
    }
}