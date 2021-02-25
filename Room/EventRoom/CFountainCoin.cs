using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFountainCoin : CFountainItemTrigger
{
    private CPlayerPara _playerPara;
    public void Start()
    {
        _playerPara = CController.instance.player.GetComponent<CPlayerPara>();
    }
    public override void GetReward()
    {
        _playerPara.Inventory.Gold += 5;
    }
}
