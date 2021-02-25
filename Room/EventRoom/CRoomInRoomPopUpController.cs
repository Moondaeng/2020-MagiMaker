using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRoomInRoomPopUpController : CEventRoomPopUpController
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public override void ChooseButton(int choose)
    {
        switch(choose)
        {
            case 0:
                ClickSkillElite();
                break;
            case 1:
                ClickItemElite();
                break;
            case 2:
                ClickFirstNormal();
                break;
            case 3:
                ClickSecondeNormal();
                break;
            case 4:
                ClickCancel();
                break;
        }

        CGlobal.popUpCancel = true;
    }

    public void ClickSkillElite() { }
    public void ClickItemElite() { }
    public void ClickFirstNormal() { }
    public void ClickSecondeNormal() { }


    public void ClickCancel() { }
}
