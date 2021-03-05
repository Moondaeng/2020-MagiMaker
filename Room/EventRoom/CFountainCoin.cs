using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFountainCoin : CFountainItemTrigger
{
    public override void GetReward()
    {
        _playerPara.Inventory.Gold += 5;
        Object.Destroy(gameObject);
    }
}
