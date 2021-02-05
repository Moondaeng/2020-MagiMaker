using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CMenuWindow : MonoBehaviour
{
    public static CMenuWindow instance;

    public Button QuitToLobbyBtn;
    public Button ReturnToGameBtn;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        QuitToLobbyBtn.onClick.AddListener(QuitToLobby);

        gameObject.SetActive(false);
    }

    private void QuitToLobby()
    {
        Debug.Log("Quit to lobby");
    }
}
