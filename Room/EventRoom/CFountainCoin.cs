using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFountainCoin : CFountainItemTrigger
{
    [Tooltip("골드 액수")]
    public int gold = 5;
    public override void GetReward()
    {
        _playerPara.Inventory.Gold += gold;
        Object.Destroy(gameObject);
    }
}
