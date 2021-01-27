using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CPortal : MonoBehaviour
{
    GameObject PortalAcceptParent;
    GameObject FadeController;

    protected virtual void Start()
    {
        PortalAcceptParent = GameObject.Find("PortalPopUp");
        FadeController = GameObject.Find("FadeController");
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            PortalAcceptParent.transform.Find("PortalAccept").gameObject.SetActive(true);
            FadeController.transform.Find("FadeCanvas").gameObject.SetActive(false);
            CWaitingForAccept.instance._portal = gameObject;
        }
    }

    public abstract void OpenNClosePortal();
}
