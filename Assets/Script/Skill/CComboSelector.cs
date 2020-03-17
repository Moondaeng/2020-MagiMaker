using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // 콤보 완성이 되면 스킬이 나간다라는 정보가 있어야함
    // 더불어, 초기 상태로 회귀해야 함
    public void Combo(int index)
    {
        // 이상한 케이스 인풋 차단
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
        }

        // 스킬 선택
        Select(index);

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
                break;
            default:
                break;
        }
    }

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
    }

    // 키 입력을 받으면 현재 상태에 따라 함수 실행
    private void Select(int index)
    {
        switch (_currentState)
        {
            case ComboState.cClass:
                SelectClass(index);
                break;
            case ComboState.cSubject:
                SelectSubject(index);
                break;
            case ComboState.cTarget:
                SelectTarget(index);
                break;
            default:
                break;
        }
    }

    // 각 성질 선택
    // 이후 성질 선택 상황 변화
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

}
