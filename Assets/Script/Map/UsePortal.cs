using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsePortal : MonoBehaviour
{
    public Vector3 position;
    GameObject[] player;
    GameObject PortalAcceptParent;

    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        PortalAcceptParent = GameObject.Find("PortalAccept").transform.parent.gameObject;
        PortalAcceptParent.transform.Find("PortalAccept").gameObject.SetActive(false);
        PortalAcceptParent.transform.Find("WaitingForOtherPlayer").gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider coll)
    {

        if (coll.tag == "Player")
        {
            Debug.Log("************************************");
            PortalAcceptParent.transform.Find("PortalAccept").gameObject.SetActive(true);
            CWaitingForAccept.instance._portal = gameObject;
        }
    }

    public void MoveToNextRoom()
    {
        Transform ParentTransform = player[0].transform;
        while (true)
        {
            if (ParentTransform.parent == null)
                break;
            else
                ParentTransform = ParentTransform.parent;
        }

        Debug.Log("Position = " + ParentTransform.position);

        CGlobal.roomCount++;
        switch (this.tag)
        {
            case "PORTAL":
                ParentTransform.position = new Vector3(0, 1, CConstants.ROOM_DISTANCE_Z * CGlobal.roomCount - CConstants.PORTAL_DISTANCE_Z);
                break;

            case "RIGHT_PORTAL":
                ParentTransform.position = new Vector3(CConstants.PORTAL_DISTANCE_X, 1, CConstants.ROOM_DISTANCE_Z * CGlobal.roomCount - CConstants.PORTAL_DISTANCE_Z);
                break;

            case "LEFT_PORTAL":
                ParentTransform.position = new Vector3(-CConstants.PORTAL_DISTANCE_X, 1, CConstants.ROOM_DISTANCE_Z * CGlobal.roomCount - CConstants.PORTAL_DISTANCE_Z);
                break;

        }

        CGlobal.usePortal = true; //포탈 사용 시 다음 맵 생성
        CGlobal.isClear = false; //포탈을 사용해서 새로운 방으로 왔으므로 방은 클리어되지 않은 상태
    }
}
