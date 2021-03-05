using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFountainElement : CFountainItemTrigger
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public override void GetReward()
    {
        Destroy(gameObject);
    }
}
