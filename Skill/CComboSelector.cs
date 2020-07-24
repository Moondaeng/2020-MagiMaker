﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // 콤보 완성이 되면 스킬이 나간다라는 정보가 있어야함
    // 더불어, 초기 상태로 회귀해야 함
    public void Combo(int index)
    {
        // 이상한 케이스 인풋 차단
        if (index < 0) return;
        if (_currentState == ComboState.Main || _currentState == ComboState.Sub)
        {
            if (index >= mainElementContainSize) return;
        }
        else
        {
            if (index >= subElementContainSize) return;
        }

        // 스킬 선택
        Select(index);

        // 현재까지 선택한 콤보 UI로 보여주기
        DrawCurrentState();
    }

    // 스킬 성질 선택 상태를 그림
    public void DrawCurrentState()
    {
        
    }

    // 조합 마법 발사
    public int EndCombo()
    {
        int currentSelectedSkillNumber = -1;

        if (_currentState == ComboState.Sub)
        {
            currentSelectedSkillNumber = mainElement[selectedMainElement];
        }
        else if (_currentState == ComboState.Select)
        {
            currentSelectedSkillNumber = elementTotalNumber
                + mainElement[selectedMainElement] * elementTotalNumber
                + subElement[selectedSubElement];
        }
        _currentState = ComboState.Main;
        return currentSelectedSkillNumber;
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
                    mainElement[mainElementSlotNumber]);
        for (int i = 0; i < subElementContainSize; i++)
        {
            if (subElement[i] != -1)
            {
                _skillUIManager.RegisterSkillUi(mainElementSlotNumber, i+1,
                    (mainElement[mainElementSlotNumber] + 1) * elementTotalNumber + subElement[i]);
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
                    (mainElement[i] + 1) * elementTotalNumber + subElement[subElementSlotNumber]);
            }
        }
    }
}
