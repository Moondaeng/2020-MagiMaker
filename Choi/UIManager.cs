using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // 언제 어디서나 쉽게 접금할수 있도록 하기위해 만든 정적변수
    public static UIManager instance;

    public Text playerName;
    public Text playerMoney;
    public Image playerHPBar;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void UpdatePlayerUI(PlayerPara playerPara)
    {
        playerName.text = playerPara.name;
       // playerMoney.text = "Coin : " + playerPara.money.ToString();
        playerHPBar.rectTransform.localScale =
            new Vector3((float)playerPara.curHp / (float)playerPara.maxHp, 1f, 1f);

    }
    void Update()
    {

    }
}

