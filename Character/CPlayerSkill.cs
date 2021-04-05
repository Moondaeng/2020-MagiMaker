using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainElementLearnEvent : UnityEvent<int, int> { }
public class SubElementLearnEvent : UnityEvent<int, int> { }
public class ElementSelectEvent : UnityEvent<int> { }

/*
 * 플레이어 캐릭터의 스킬을 관리하는 클래스
 * 
 */
public class CPlayerSkill : CCharacterSkill
{
    [System.Serializable]
    public enum ESkillElement
    {
        Fire, Water, Earth, Wind, Light, Dark, None = -1
    }

    private static readonly int mainElementContainSize = 3;
    private static readonly int subElementContainSize = 4;
    private static readonly int ELEMENT_lENGTH = System.Enum.GetValues(typeof(ESkillElement)).Length;

    private ESkillElement[] mainElement = new ESkillElement[mainElementContainSize];
    private ESkillElement[,] subElement = new ESkillElement[mainElementContainSize, subElementContainSize];

    private ESkillElement _selectedElement;

    public MainElementLearnEvent mainElementLearnEvent = new MainElementLearnEvent();
    public SubElementLearnEvent subElementLearnEvent = new SubElementLearnEvent();
    public ElementSelectEvent elementSelectEvent = new ElementSelectEvent();

    protected override void Awake()
    {
        base.Awake();
        _selectedElement = ESkillElement.None;

        for (int mainElementSlot = 0; mainElementSlot < mainElement.Length; mainElementSlot++)
        {
            mainElement[mainElementSlot] = ESkillElement.None;
            for (int subElementSlot = 0; subElementSlot < subElementContainSize; subElementSlot++)
            {
                subElement[mainElementSlot, subElementSlot] = ESkillElement.None;
            }
        }
    }

    // Start is called before the first frame update
    protected void Start()
    {
        // 주원소, 부원소 배우기
        SetMainElement(0, ESkillElement.Fire);
        SetMainElement(1, ESkillElement.Water);
        SetSubElement(0, 0, ESkillElement.Fire);
        SetSubElement(0, 1, ESkillElement.Water);
        SetSubElement(0, 2, ESkillElement.Earth);
    }

    public int GetElementContainSize(bool isMainElement)
    {
        return isMainElement ? mainElementContainSize : subElementContainSize;
    }

    public int GetElementNumber(bool isMainElement, int mainSlot, int subSlot)
    {
        if (mainSlot < 0 || mainSlot >= mainElementContainSize
            || subSlot < 0 || subSlot >= subElementContainSize)
        {
            return (int)ESkillElement.None;
        }

        if (isMainElement)
        {
            return (int)mainElement[mainSlot];
        }
        else
        {
            return (int)subElement[mainSlot, subSlot];
        }
    }

    public int GetRegisterNumber(int mainElementIndex, int subElementIndex)
    {
        if (mainElement[mainElementIndex] == ESkillElement.None)
        {
            return NOT_SELECTED;
        }

        if (subElementIndex == NOT_SELECTED)
        {
            return (int)mainElement[mainElementIndex] * ELEMENT_lENGTH;
        }

        if (subElement[mainElementIndex, subElementIndex] == ESkillElement.None)
        {
            return NOT_SELECTED;
        }

        return (int)mainElement[mainElementIndex] * ELEMENT_lENGTH + (int)subElement[mainElementIndex, subElementIndex] + 1;
    }

    public override void SkillSelect(int index)
    {
        if (index < 0)
        {
            Debug.Log("Skill Select Error");
            return;
        }

        if (_selectedElement == ESkillElement.None || _selectedSkillNum != NOT_SELECTED)
        {
            if (index >= mainElementContainSize)
            {
                Debug.Log("Element index valid Error");
                return;
            }
            
            if ((_selectedElement = mainElement[index]) == ESkillElement.None)
            {
                Debug.Log("Element Select Error");
                return;
            }
            elementSelectEvent.Invoke(index);
            _selectedSkillNum = NOT_SELECTED;
        }
        else
        {
            _selectedSkillNum = index;
        }
        skillSelectEvent.Invoke(_selectedSkillNum);
    }

    public override void UseSkillToPosition(Vector3 targetPos)
    {
        if (_selectedElement == ESkillElement.None)
        {
            Debug.Log("Skill Not Selected");
            return;
        }

        _selectedSkillNum = GetRegisterNumber((int)_selectedElement, _selectedSkillNum);

        // 스킬 모션 선택 가능하게 해당 클래스에서 지원 필요
        GetComponent<CCntl>().Skill();
        base.UseSkillToPosition(targetPos);
        
        _selectedElement = ESkillElement.None;
        elementSelectEvent.Invoke((int)_selectedElement);
    }

    #region 원소 획득 / 교체
    // 주 원소 획득 / 교체
    public void SetMainElement(int slot, ESkillElement element)
    {
        if (slot < 0 || slot >= mainElementContainSize) return;

        mainElement[slot] = element;
        mainElementLearnEvent.Invoke(slot, (int)element);
    }

    // 부 원소 획득 / 교체
    public void SetSubElement(int mainSlot, int subSlot, ESkillElement element)
    {
        if (mainSlot < 0 || mainSlot >= mainElementContainSize
            || subSlot < 0 || subSlot >= subElementContainSize)
        {
            return;
        }

        subElement[mainSlot, subSlot] = element;
        subElementLearnEvent.Invoke(subSlot, (int)element);
    }
    #endregion
}
