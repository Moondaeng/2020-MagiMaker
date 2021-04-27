using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFountainPopUpController : CNPCPopUpController
{
    private GameObject _fountain;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _fountain = GameObject.Find("Fountain");
    }

    public override void ChooseButton(int choose)
    {
        switch (choose)
        {
            case 0:
                ActivateFountain();
                break;
            case 1:
                ClickCancel();
                break;
        }

        CGlobal.popUpCancel = true;
    }

    public void ActivateFountain()
    {
        CGlobal.isEvent = true;
        CCreateMap.instance.NotifyPortal();

        _fountain.GetComponent<CActivateFountain>().SendMessage("ActivateFountain");
    }

    void ClickCancel()
    {

    }
}
