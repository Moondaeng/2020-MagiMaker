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

    private readonly int mainElementContainSize = 3;
    private readonly int subElementContainSize = 4;
    private const int elementTotalNumber = 6;

    private int[] mainElement;
    private int[] subElement;

    private int _selectedElementNum;

    public MainElementLearnEvent mainElementLearnEvent = new MainElementLearnEvent();
    public SubElementLearnEvent subElementLearnEvent = new SubElementLearnEvent();
    public ElementSelectEvent elementSelectEvent = new ElementSelectEvent();

    protected override void Awake()
    {
        base.Awake();
        _selectedElementNum = -1;

        mainElement = new int[mainElementContainSize];
        subElement = new int[subElementContainSize];
        for (int slot = 0; slot < mainElement.Length; slot++)
        {
            mainElement[slot] = -1;
        }
        for (int slot = 0; slot < subElement.Length; slot++)
        {
            subElement[slot] = -1;
        }
    }

    // Start is called before the first frame update
    protected void Start()
    {
        // 주원소, 부원소 배우기
        SetMainElement(0, ESkillElement.Fire);
        SetMainElement(1, ESkillElement.Water);
        SetSubElement(0, ESkillElement.Water);
        SetSubElement(1, ESkillElement.Earth);
        SetSubElement(2, ESkillElement.Wind);
        SetSubElement(3, ESkillElement.Light);
    }

    public int GetElementContainSize(bool isMainElement)
    {
        return isMainElement ? mainElementContainSize : subElementContainSize;
    }

    public int GetElementNumber(bool isMainElement, int slotNumber)
    {
        if (slotNumber < 0 || slotNumber > GetElementContainSize(isMainElement))
        {
            return (int)ESkillElement.None;
        }

        if (isMainElement)
        {
            return mainElement[slotNumber];
        }
        else
        {
            return subElement[slotNumber];
        }
    }

    public int GetElementNumber(bool isMainElement, int mainSlotNumber, int subSlotNumber)
    {
        return 0;
    }

    public int GetRegisterNumber(int mainElementIndex, int subElementIndex)
    {
        if(subElementIndex == -1)
        {
            if (-1 == mainElement[mainElementIndex])
            {
                return -1;
            }
            return mainElement[mainElementIndex] * (elementTotalNumber + 1);
        }

        if (-1 == mainElement[mainElementIndex] || -1 == subElement[subElementIndex])
        {
            return -1;
        }
        return mainElement[mainElementIndex] * (elementTotalNumber + 1) + subElement[subElementIndex];
    }

    // 주 원소 획득 / 교체
    public void SetMainElement(int slot, ESkillElement element)
    {
        if (slot < 0 || slot >= mainElementContainSize) return;

        mainElement[slot] = (int)element;
        mainElementLearnEvent.Invoke(slot, (int)element);
    }

    // 부 원소 획득 / 교체
    public void SetSubElement(int slot, ESkillElement element)
    {
        if (slot < 0 || slot >= subElementContainSize) return;

        subElement[slot] = (int)element;
        subElementLearnEvent.Invoke(slot, (int)element);
    }

    public void SetSubElement(int mainSlotNumber, int subSlotNumber, ESkillElement element)
    {

    }

    public override void SkillSelect(int index)
    {
        if (index < 0)
        {
            Debug.Log("Skill Select Error");
            return;
        }

        if (_selectedElementNum == -1 || _selectedSkillNum != -1)
        {
            if (index >= mainElementContainSize)
            {
                Debug.Log("Element index valid Error");
                return;
            }
            
            if(-1 == (_selectedElementNum = mainElement[index]))
            {
                Debug.Log("Element Select Error");
                return;
            }
            elementSelectEvent.Invoke(index);
            _selectedSkillNum = -1;
        }
        else
        {
            _selectedSkillNum = index;
        }
        skillSelectEvent.Invoke(_selectedSkillNum);
    }

    public override void UseSkillToPosition(Vector3 targetPos)
    {
        if (_selectedElementNum == -1)
        {
            Debug.Log("Skill Not Selected");
            return;
        }

        _selectedSkillNum = GetRegisterNumber(_selectedElementNum, _selectedSkillNum);

        // 스킬 모션 선택 가능하게 해당 클래스에서 지원 필요
        GetComponent<CCntl>().Skill();
        base.UseSkillToPosition(targetPos);
        
        _selectedElementNum = -1;
        elementSelectEvent.Invoke(_selectedElementNum);
    }

    // 아이템에 의한 스킬 대체
    public void ReplaceSkill(int skillNumber)
    {

    }
}
