using System;
using UnityEngine;
using UnityEngine.UI;

public class CElementObtainViewer : MonoBehaviour
{
    public static CElementObtainViewer instance;

    private static readonly int MAX_CONTANING_ELEMENT = 4;

    [SerializeField, EnumNamedArray(typeof(CPlayerSkill.ESkillElement))]
    private Sprite[] _elementImageArr = new Sprite[Enum.GetValues(typeof(CPlayerSkill.ESkillElement)).Length];
    private Transform[] _elementPanel = new Transform[MAX_CONTANING_ELEMENT];
    private TMPro.TMP_Text _changeElementText;

    public Action closeWindowCallback;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < _elementPanel.Length; i++)
        {
            _elementPanel[i] = transform.Find($"Element{i + 1}");
        }
        _changeElementText = transform.Find("ChangeText").GetComponent<TMPro.TMP_Text>();
    }

    public void DrawViewer(CPlayerSkill playerSkill, bool isMainElement, CPlayerSkill.ESkillElement element)
    {
        SetChangeElementText(isMainElement);

        for (int i = 0; i < playerSkill.GetElementContainSize(isMainElement); i++)
        {
            SetElementImage(playerSkill.GetElementNumber(isMainElement, i), _elementPanel[i].Find("Image").GetComponent<Image>());
            var button = _elementPanel[i].Find("ChangeButton").GetComponent<Button>();
            button.interactable = true;
            int slotNumber = i;
            button.onClick.AddListener(() => ChangeElement(playerSkill, isMainElement, element, slotNumber));
        }
        for (int i = playerSkill.GetElementContainSize(isMainElement); i < MAX_CONTANING_ELEMENT; i++)
        {
            _elementPanel[i].Find("ChangeButton").GetComponent<Button>().interactable = false;
        }


    }

    private void SetChangeElementText(bool isMainElement)
    {
        _changeElementText.text = isMainElement ? "주원소" : "부원소";
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

    private void ChangeElement(CPlayerSkill playerSkill, bool isMainElement, CPlayerSkill.ESkillElement element, int slot)
    {
        if (isMainElement)
        {
            playerSkill.SetMainElement(slot, element);
        }
        else
        {
            playerSkill.SetSubElement(slot, element);
        }

        closeWindowCallback?.Invoke();
    }
}
