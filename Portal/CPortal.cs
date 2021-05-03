using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CPortal : MonoBehaviour
{
    protected GameObject PortalAcceptParent;
    protected GameObject FadeController;

    protected virtual void Start()
    {
        PortalAcceptParent = GameObject.Find("PortalPopUp");
        FadeController = GameObject.Find("FadeController");
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            FadeController.transform.Find("FadeCanvas").gameObject.SetActive(false);
            CPortalManager.instance.PortalPopup.SetActive(true);
            CPortalManager.instance.SelectedPortalStr = gameObject.tag;
            Network.CNetworkEvent.instance.UsePortalEvent?.Invoke();
        }
    }

    public abstract void OpenNClosePortal();
}
