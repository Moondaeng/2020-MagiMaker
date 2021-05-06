using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRewardChoiceEventRoom0_5 : CRewardChoicePopupController
{
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
        CEventRoomNpcClick.instance.ChangePopUp(gameObject);
    }
}
