using System;
using UnityEngine;

[DisallowMultipleComponent]
public class CWindowFacade : MonoBehaviour
{
    public static CWindowFacade instance;

    public Action<bool> SetControlLockCallback;

    [SerializeField] private CInventoryWindow _inventoryWindow;
    [SerializeField] private CMenuWindow _menuWindow;

    private bool _isOtherWindowMode = false;
    private GameObject _activedWindow = null;

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

        if (Input.GetKeyDown(KeyCode.Tab) && (_activedWindow == null))
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

        if (_activedWindow == null)
        {
            OpenMenu();
        }
        else
        {
            _activedWindow.SetActive(false);
            _activedWindow = null;
            SetControlLockCallback(false);
        }
    }

    private void OpenMenu()
    {
        _activedWindow = _menuWindow.gameObject;
        _activedWindow.SetActive(true);
        SetControlLockCallback(true);
    }

    public void OpenInventory(CInventory inventory)
    {
        _activedWindow = _inventoryWindow.gameObject;
        _activedWindow.SetActive(true);
        SetControlLockCallback(true);
        _inventoryWindow.OpenInventory(inventory);
    }

    public void SetOtherWindowMode(bool isOtherWindow)
    {
        _isOtherWindowMode = isOtherWindow;
        SetControlLockCallback(isOtherWindow);
    }
}