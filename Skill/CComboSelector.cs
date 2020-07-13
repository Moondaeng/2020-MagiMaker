using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CComboSelector
{
    enum ComboState
    {
        Main, Sub, Select
    }

    public delegate void SkillCall(int skillNumber);

    public SkillCall ReturnSkill;

    private static CLogComponent _logger = new CLogComponent(ELogType.Skill);
    
    private const int elementContainSize = 3;
    private const int elementTotalNumber = 6;

    private int[] mainElement = new int[elementContainSize];
    private int[] subElement = new int[elementContainSize];

    private int selectedMainElement = 0;
    private int selectedSubElement = 0;

    private CSkillTimer _timer;
    private CSkillUIManager _skillUIManager;
    
    private ComboState _currentState;

    public CComboSelector(GameObject player)
    {
        //_skillUIManager = GameObject.Find("UiScript").GetComponent<CSkillUIManager>();
        _skillUIManager = CSkillUIManager.instance;
        //_timer = player.GetComponent<CSkillTimer>();
        _currentState = ComboState.Main;

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
    public void SetMainElement(int slot, int elementNumber)
    {
        if (slot < 0 || slot >= elementContainSize) return;
        if (elementNumber < 0 || elementNumber >= elementTotalNumber) return;

        mainElement[slot] = elementNumber;
    }

    // 부 원소 획득 / 교체
    public void SetSubElement(int slot, int elementNumber)
    {
        if (slot < 0 || slot >= elementContainSize) return;
        if (elementNumber < 0 || elementNumber >= elementTotalNumber) return;

        subElement[slot] = elementNumber;
    }

    // 콤보 완성이 되면 스킬이 나간다라는 정보가 있어야함
    // 더불어, 초기 상태로 회귀해야 함
    public void Combo(int index)
    {
        // 이상한 케이스 인풋 차단
        if (index < 0 || index >= elementContainSize) return;

        // 스킬 선택
        Select(index);

        // 선택 상황 출력
        //_logger.Log(ToStringCurrentState());

        // 현재까지 선택한 콤보 UI로 보여주기
        DrawCurrentState();
        DrawSelectableSkill();
    }

    // 스킬 성질 선택 상태를 그림
    public void DrawCurrentState()
    {
        
    }

    // 선택할 수 있는 스킬 그리기 요청
    public void DrawSelectableSkill()
    {

    }

    // 조합 마법 발사
    public void EndCombo()
    {
        if (_currentState == ComboState.Main) return;
        
        _currentState = ComboState.Main;
        if(_currentState == ComboState.Sub)
        {
            ReturnSkill(mainElement[selectedMainElement]);
        }
        else if(_currentState == ComboState.Select)
        {
            ReturnSkill(elementTotalNumber 
                + mainElement[selectedMainElement] * elementTotalNumber 
                + subElement[selectedSubElement]);
        }
    }

    // 키 입력을 받으면 현재 상태에 따라 함수 실행
    private void Select(int index)
    {
        switch (_currentState)
        {
            case ComboState.Main:
                SelectMain(index);
                break;
            case ComboState.Sub:
                SelectSub(index);
                break;
            case ComboState.Select:
                SelectMain(index);
                break;
            default:
                break;
        }
    }

    // 각 성질 선택
    // 이후 성질 선택 상황 변화
    private void SelectMain(int index)
    {
        // 주 원소 안 배운 경우
        if (mainElement[index] == -1)
        {
            return;
        }

        selectedMainElement = index;
        _currentState = ComboState.Sub;
    }

    private void SelectSub(int index)
    {
        // 부 원소 안 배운 경우
        if (subElement[index] == -1)
        {
            return;
        }

        selectedSubElement = index;
        _currentState = ComboState.Select;
    }
}
