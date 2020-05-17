using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUIManager : MonoBehaviour
{
    // 언제 어디서나 쉽게 접금할수 있도록 하기위해 만든 정적변수
    public static CUIManager instance;
    
    public Image playerHPBar;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void UpdatePlayerUI(CPlayerPara playerPara)
    {
        playerHPBar.rectTransform.localScale = 
            new Vector3(1f, playerPara._curHp / (float)playerPara._maxHp, 1f);
    }
    void Update()
    {

    }
}

