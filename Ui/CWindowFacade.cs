using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CWindowFacade : MonoBehaviour
{
    public static CWindowFacade instance;

    public Action<bool> SetControlLockCallback;

    [SerializeField] private CInventoryWindow _inventoryWindow;
    [SerializeField] private CMenuWindow _menuWindow;
    [SerializeField] private CHelpWindow _helpWindow;
    [SerializeField] private CElementWindow _elementWindow;

    private bool _isOtherWindowMode = false;
    private Stack<GameObject> _activedWindowStack = new Stack<GameObject>();

    private GameObject _windowTarget = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Backspace))
#else
        if (Input.GetKeyDown(KeyCode.Escape))
#endif
        {
            CloseWindow();
        }

        if (Input.GetKeyDown(KeyCode.Tab) && _activedWindowStack.Count == 0)
        {
            OpenInventory(_windowTarget.GetComponent<CPlayerPara>().Inventory);
        }
    }

    // 추적 대상(player) 지정
    public void SetTarget(GameObject player)
    {
        _windowTarget = player;
    }

    public void CloseWindow()
    {
        if (_isOtherWindowMode)
        {
            return;
        }

        if (_activedWindowStack.Count == 0)
        {
            OpenMenu();
        }
        else
        {
            var deactivateWindow = _activedWindowStack.Pop();
            deactivateWindow.SetActive(false);
            if (_activedWindowStack.Count == 0)
            {
                SetControlLockCallback(false);
            }
        }
    }

    private void OpenMenu()
    {
        PushActiveWindow(_menuWindow.gameObject);
    }

    public void OpenInventory(CInventory inventory)
    {
        PushActiveWindow(_inventoryWindow.gameObject);
        _inventoryWindow.OpenInventory(inventory);
    }

    public void OpenHelp()
    {
        PushActiveWindow(_helpWindow.gameObject);
    }

    public void OpenObtainingElement(CPlayerSkill playerSkill, bool isMainElement, CPlayerSkill.ESkillElement element)
    {
        PushActiveWindow(_elementWindow.gameObject);
        _elementWindow.closeWindowCallback = CloseWindow;
        _elementWindow.DrawPlayerElement(playerSkill);
        _elementWindow.SetChangingElementState(playerSkill, isMainElement, element);
    }

    public void SetOtherWindowMode(bool isOtherWindow)
    {
        _isOtherWindowMode = isOtherWindow;
        SetControlLockCallback(isOtherWindow);
    }

    private void PushActiveWindow(GameObject windowObject)
    {
        if (windowObject == null)
        {
            Debug.Log("can't push null window object");
            return;
        }

        _activedWindowStack.Push(windowObject);
        windowObject.SetActive(true);
        SetControlLockCallback(true);
    }
}