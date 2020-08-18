using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

<<<<<<< HEAD
public class CComboSelector
{
    enum ComboClass
    {
        Attack, Defence, Support
    }

    enum ComboSubject
    {
        Power, Area, Multi, Utility
    }

    enum ComboTarget
    {
        Self, Enemy, Forward
    }

    enum ComboState
    {
        cClass, cSubject, cTarget, cSelect
    }

    public delegate void SkillCall(int skillNumber);

    public SkillCall ReturnSkill;

    private static Array _comboClassArray = Enum.GetValues(typeof(ComboClass));
    private static Array _comboSubjectArray = Enum.GetValues(typeof(ComboSubject));
    private static Array _comboTargetArray = Enum.GetValues(typeof(ComboTarget));

    private static CLogComponent _logger = new CLogComponent(ELogType.Skill);

    private const int _comboRegisteredNumberStart = 4;

    public const int skillCount = 36;
    private const int comboClassSize = 12;
    private const int comboSubjectSize = 3;
    private const int comboTargetSize = 1;

    private CSkillTimer _timer;
    private CSkillUIManager _skillUIManager;

    private UInt64 _learnedSkillList;
    private ComboState _currentState;
    private ComboClass _currentClass;
    private ComboSubject _currentSubject;
    private ComboTarget _currentTarget;

    public CComboSelector()
    {
        _skillUIManager = GameObject.Find("SkillScript").GetComponent<CSkillUIManager>();
        _timer = GameObject.Find("SkillScript").GetComponent<CSkillTimer>();
        _learnedSkillList = 0;
        _currentState = ComboState.cClass;
    }

    public void LearnSkill(int learnSkillNumber)
    {
        _learnedSkillList |= (UInt64)1 << learnSkillNumber;
    }
=======
public class CSkillSelector
{
    enum ComboState
    {
        Main, Sub, Select
    }

    public enum SkillElement
    {
        Fire, Water, Earth, Wind, Light, Dark
    }
    
    private readonly int mainElementContainSize = 3;
    private readonly int subElementContainSize = 4;
    private const int elementTotalNumber = 6;

    private int[] mainElement;
    private int[] subElement;

    private int selectedMainElement = 0;
    private int selectedSubElement = 0;
    
    private CSkillUIManager _skillUIManager;
    
    private ComboState _currentState;

    public CSkillSelector()
    {
        //_skillUIManager = GameObject.Find("UiScript").GetComponent<CSkillUIManager>();
        _skillUIManager = CSkillUIManager.instance;
        _currentState = ComboState.Main;

        mainElement = new int[mainElementContainSize];
        subElement = new int[subElementContainSize];

        for(int slot = 0; slot < mainElement.Length; slot++)
        {
            mainElement[slot] = -1;
        }
        for (int slot = 0; slot < subElement.Length; slot++)
        {
            subElement[slot] = -1;
        }
    }

    // 주 원소 획득 / 교체
    public void SetMainElement(int slot, SkillElement element)
    {
        if (slot < 0 || slot >= mainElementContainSize) return;

        mainElement[slot] = (int)element;

        SyncMainElementWithUi(slot);
    }

    // 부 원소 획득 / 교체
    public void SetSubElement(int slot, SkillElement element)
    {
        if (slot < 0 || slot >= subElementContainSize) return;

        subElement[slot] = (int)element;
        
        SyncSubElementWithUi(slot);
    }

    #region Second

    public void ChangeElement()
    {
        do
        {
            selectedMainElement = (selectedMainElement + 1) % mainElementContainSize;
        } while (mainElement[selectedMainElement] == -1);
        _skillUIManager.ShowSelectElement(selectedMainElement);
    }

    // 임시 2안 - 주원소는 따로 선택된 상태에서 스킬을 사용
    public int PickElementSkill(int index)
    {
        int currentSelectedSkillNumber = -1;

        if(index == 0)
        {
            currentSelectedSkillNumber = mainElement[selectedMainElement];
        }
        else
        {
            currentSelectedSkillNumber = elementTotalNumber
                    + mainElement[selectedMainElement] * elementTotalNumber
                    + subElement[index - 1];
        }

        return currentSelectedSkillNumber;
    }
    #endregion
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3

    // 콤보 완성이 되면 스킬이 나간다라는 정보가 있어야함
    // 더불어, 초기 상태로 회귀해야 함
    public void Combo(int index)
    {
        // 이상한 케이스 인풋 차단
<<<<<<< HEAD
        if(_currentState != ComboState.cSubject && index == 3)
        {
            _logger.Log("wrong case");
            return;
        }

        // 스킬이 선택 가능한지 판단
        if (!IsExistLearnedSkill(BitSelector.GetSequence(GetSkillListSize(),
                GetSkillListPosition() + index * GetSkillListSize())))
        {
            _logger.Log("not learned");
            return;
=======
        if (index < 0) return;
        if (_currentState == ComboState.Main || _currentState == ComboState.Sub)
        {
            if (index >= mainElementContainSize) return;
        }
        else
        {
            if (index >= subElementContainSize) return;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        }

        // 스킬 선택
        Select(index);

<<<<<<< HEAD
        // 선택 상황 출력
        _logger.Log(ToStringCurrentState());

        // 선택이 끝난 상황
        if (_currentState == ComboState.cSelect)
        {
            // 선택한 스킬 리턴 -> 일단은 로그를 찍는거로 하자
            ReturnSkill((int)_currentClass * comboClassSize
                + (int)_currentSubject * comboSubjectSize
                + (int)_currentTarget * comboTargetSize);
            // 콤보 종료
            EndCombo();
        }
    }

    // 스킬 성질 선택 상태를 그림
    // 이 부분도 개선 필요
    public void DrawCurrentState()
    {
        switch(_currentState)
        {
            case ComboState.cSubject:
                _skillUIManager.DrawComboClassState((int)_currentClass);
                break;
            case ComboState.cTarget:
                _skillUIManager.DrawComboSubjectState((int)_currentSubject);
=======
        // 현재까지 선택한 콤보 UI로 보여주기
        DrawCurrentState(index);
    }

    // 스킬 성질 선택 상태를 그림
    public void DrawCurrentState(int index)
    {
        if(_currentState == ComboState.Sub)
        {
            _skillUIManager.ShowSelectElement(index);
        }

        switch(_currentState)
        {
            case ComboState.Sub:
                _skillUIManager.ShowSelectSkill(0);
                break;
            case ComboState.Select:
                _skillUIManager.ShowSelectSkill(index+1);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
                break;
            default:
                break;
        }
    }

<<<<<<< HEAD
    // 선택할 수 있는 스킬 그리기 요청
    public void DrawSelectableSkill()
    {
        // enum 크기 구하기
        List<Type> types = new List<Type>() { typeof(ComboClass), typeof(ComboSubject), typeof(ComboTarget) };
        int enumSize = 0;
        if ((int)_currentState < types.Count)
        {
            enumSize = types[(int)_currentState].GetEnumValues().Length;
        }

        // 선택할 수 있는 키는 보여주고 아닌 건 안 보여주기
        _skillUIManager.ActiveComboOn(true);
        _skillUIManager.DeactivateComboUIAll();
        for (int combo = 0; combo < enumSize; combo++)
        {
            int startPosition = GetSkillListPosition() + combo * GetSkillListSize();

            // 해당 파트의 그림 보여주기
            switch (_currentState)
            {
                case ComboState.cClass:
                    _skillUIManager.SetComboClassImage(combo, combo);
                    break;
                case ComboState.cSubject:
                    _skillUIManager.SetComboSubejectImage(combo, combo);
                    break;
                case ComboState.cTarget:
                    _skillUIManager.SetComboSkillImage(combo, startPosition % 9);
                    break;
                default:
                    break;
            }

            // 해당 파트의 쿨타임이 가장 적은 스킬 보여주기
            if (IsExistLearnedSkill(BitSelector.GetSequence(GetSkillListSize(), startPosition)))
            {
                _skillUIManager.ActivateComboUI(combo);
                _skillUIManager.Preempt(
                    (CSkillUIManager.EUIName)_comboRegisteredNumberStart + combo,
                    _timer.FindMinimumCooldown(FindLearnedComboSkillList(startPosition, GetSkillListSize()))
                    );
            }
        }
    }

    // 배운 스킬만 찾아서 번호 리스트로 만들기
    private List<int> FindLearnedComboSkillList(int startPosition, int searchSize)
    {
        var learnedList = new List<int>();
        UInt64 learnedCopy = _learnedSkillList;

        learnedCopy >>= startPosition;
        for(int i = 0; i < searchSize; i++)
        {
            if((learnedCopy & 1) == 1)
            {
                learnedList.Add(startPosition + i + _comboRegisteredNumberStart);
            }
            learnedCopy >>= 1;
        }

        return learnedList;
    }

    // 콤보 종료
    public void EndCombo()
    {
        _currentState = ComboState.cClass;
        _skillUIManager.DeactivateAll();
    }

    // 현재 배운 콤보 스킬들 목록을 출력한다
    public void PrintLearnedSkillList()
    {
        UInt64 learned = _learnedSkillList;
        for (int learnSkillNumber = 0; learnSkillNumber < skillCount; learnSkillNumber++)
        {
            if ((learned & 1) == 1)
            {
                Console.Write(learnSkillNumber + " ");
            }
            learned >>= 1;
        }
        Console.WriteLine();
=======
    // 조합 마법 발사
    public int EndCombo()
    {
        int currentSelectedSkillNumber = -1;
        int subElementNumber = 0;

        if (_currentState == ComboState.Sub)
        {
            subElementNumber = 0;
        }
        else if (_currentState == ComboState.Select)
        {
            subElementNumber = subElement[selectedSubElement];
        }
        currentSelectedSkillNumber = mainElement[selectedMainElement] * elementTotalNumber + subElementNumber;

        _currentState = ComboState.Main;
        _skillUIManager.ShowSelectSkill(-1);
        return currentSelectedSkillNumber;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    // 키 입력을 받으면 현재 상태에 따라 함수 실행
    private void Select(int index)
    {
        switch (_currentState)
        {
<<<<<<< HEAD
            case ComboState.cClass:
                SelectClass(index);
                break;
            case ComboState.cSubject:
                SelectSubject(index);
                break;
            case ComboState.cTarget:
                SelectTarget(index);
=======
            case ComboState.Main:
                SelectMain(index);
                break;
            case ComboState.Sub:
                SelectSub(index);
                break;
            case ComboState.Select:
                SelectMain(index);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
                break;
            default:
                break;
        }
    }

    // 각 성질 선택
    // 이후 성질 선택 상황 변화
<<<<<<< HEAD
    private void SelectClass(int index)
    {
        _currentClass = (ComboClass)(Enum.GetValues(typeof(ComboClass))).GetValue(index);
        _currentState = ComboState.cSubject;
    }

    private void SelectSubject(int index)
    {
        _currentSubject = (ComboSubject)(Enum.GetValues(typeof(ComboSubject))).GetValue(index);
        _currentState = ComboState.cTarget;
    }

    private void SelectTarget(int index)
    {
        _currentTarget = (ComboTarget)(Enum.GetValues(typeof(ComboTarget))).GetValue(index);
        _currentState = ComboState.cSelect;
    }

    private Boolean IsExistLearnedSkill(UInt64 skillList)
    {
        return (_learnedSkillList & skillList) >= 1;
    }

    private int GetSkillListSize()
    {
        switch (_currentState)
        {
            case ComboState.cClass:
                return comboClassSize;
            case ComboState.cSubject:
                return comboSubjectSize;
            case ComboState.cTarget:
                return comboTargetSize;
            default:
                return 0;
        }
    }

    private int GetSkillListPosition()
    {
        int result = 0;
        if (_currentState == ComboState.cClass) return result;
        result += (int)_currentClass * comboClassSize;
        if (_currentState == ComboState.cSubject) return result;
        result += (int)_currentSubject * comboSubjectSize;
        return result;
    }

    // 현재까지 선택한 콤보 스킬 성질 출력
    private String ToStringCurrentState()
    {
        String currentStateString = "Current Select : ";
        if (_currentState == ComboState.cClass) return currentStateString;
        currentStateString += " " + ToStringCurrentClass();
        if (_currentState == ComboState.cSubject) return currentStateString;
        currentStateString += " " + ToStringCurrentSubject();
        if (_currentState == ComboState.cTarget) return currentStateString;
        currentStateString += " " + ToStringCurrentTarget();
        return currentStateString;
    }

    // 현재 Combo Class를 String으로 표현
    private String ToStringCurrentClass()
    {
        String[] comboClassStringArray =
        {
                "Attack", "Defence", "Support"
            };
        int eTypeIndex;

        for (eTypeIndex = 0; _currentClass != (ComboClass)_comboClassArray.GetValue(eTypeIndex); eTypeIndex++) ;
        return comboClassStringArray[eTypeIndex];
    }

    private String ToStringCurrentSubject()
    {
        String[] comboSubjectStringArray =
        {
                "Power", "Area", "Multi", "Utility"
            };
        int eTypeIndex;

        for (eTypeIndex = 0; _currentSubject != (ComboSubject)_comboSubjectArray.GetValue(eTypeIndex); eTypeIndex++) ;
        return comboSubjectStringArray[eTypeIndex];
    }

    private String ToStringCurrentTarget()
    {
        String[] comboTargetStringArray =
        {
                "Self", "Enemy", "Forward"
            };
        int eTypeIndex;

        for (eTypeIndex = 0; _currentTarget != (ComboTarget)_comboTargetArray.GetValue(eTypeIndex); eTypeIndex++) ;
        return comboTargetStringArray[eTypeIndex];
    }

=======
    private void SelectMain(int index)
    {
        // 주 원소 안 배운 경우
        if (mainElement[index] == -1) return;

        selectedMainElement = index;
        _currentState = ComboState.Sub;
    }

    private void SelectSub(int index)
    {
        // 부 원소 안 배운 경우
        if (subElement[index] == -1) return;

        selectedSubElement = index;
        _currentState = ComboState.Select;
    }
    
    private void SyncMainElementWithUi(int mainElementSlotNumber)
    {
        _skillUIManager.RegisterSkillUi(mainElementSlotNumber, 0,
                    mainElement[mainElementSlotNumber] * elementTotalNumber);
        for (int i = 0; i < subElementContainSize; i++)
        {
            if (subElement[i] != -1)
            {
                _skillUIManager.RegisterSkillUi(mainElementSlotNumber, i+1,
                    mainElement[mainElementSlotNumber] * elementTotalNumber + subElement[i]);
            }
        }
    }
    
    private void SyncSubElementWithUi(int subElementSlotNumber)
    {
        for (int i = 0; i < mainElementContainSize; i++)
        {
            if (mainElement[i] != -1)
            {
                _skillUIManager.RegisterSkillUi(i, subElementSlotNumber+1,
                    mainElement[i] * elementTotalNumber + subElement[subElementSlotNumber]);
            }
        }
    }
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
}
