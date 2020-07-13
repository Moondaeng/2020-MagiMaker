using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsePortal : MonoBehaviour
{
    public Vector3 position;
    GameObject[] player;
    public GameObject PortalPopup;

    private CPlayerCommand _playerCommand;

    private void Awake()
    {
        _playerCommand = GameObject.Find("GameManager").GetComponent<CPlayerCommand>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        PortalPopup.transform.Find("PortalAccept").gameObject.SetActive(false);
        PortalPopup.transform.Find("WaitingForOtherPlayer").gameObject.SetActive(false);
        PortalPopup.SetActive(false);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            ActivePortalPopup();

        }
    }

    public void ActivePortalPopup()
    {
        Debug.Log("************************************");
        PortalPopup.SetActive(true);
        PortalPopup.transform.Find("PortalAccept").gameObject.SetActive(true);
        PortalPopup.transform.Find("WaitingForOtherPlayer").gameObject.SetActive(true);
        PortalPopup.GetComponent<CWaitingForAccept>().SetWaitingPlayer();
        //CWaitingForAccept.instance._portal = gameObject;
    }

    public void MoveToNextRoom()
    {
        if (_playerCommand == null)
        {
            _playerCommand = GameObject.Find("GameManager").GetComponent<CPlayerCommand>();
        }
        for (int player = 0; player < _playerCommand.activePlayersCount; player++)
        {
            position = position + Vector3.right * player * 8;
            _playerCommand.Teleport(player, position);
        }
    }

    //public void MoveToNextRoom()
    //{
    //    Transform ParentTransform = player[0].transform;
    //    while (true)
    //    {
    //        if (ParentTransform.parent == null)
    //            break;
    //        else
    //            ParentTransform = ParentTransform.parent;
    //    }

    //    Debug.Log("Position = " + ParentTransform.position);

    //    CGlobal.roomCount++;
    //    switch (this.tag)
    //    {
    //        case "PORTAL":
    //            ParentTransform.position = new Vector3(0, 1, CConstants.ROOM_DISTANCE_Z * CGlobal.roomCount - CConstants.PORTAL_DISTANCE_Z);
    //            break;

    //        case "RIGHT_PORTAL":
    //            ParentTransform.position = new Vector3(CConstants.PORTAL_DISTANCE_X, 1, CConstants.ROOM_DISTANCE_Z * CGlobal.roomCount - CConstants.PORTAL_DISTANCE_Z);
    //            break;

    //        case "LEFT_PORTAL":
    //            ParentTransform.position = new Vector3(-CConstants.PORTAL_DISTANCE_X, 1, CConstants.ROOM_DISTANCE_Z * CGlobal.roomCount - CConstants.PORTAL_DISTANCE_Z);
    //            break;

    //    }

    //    CGlobal.usePortal = true; //포탈 사용 시 다음 맵 생성
    //    CGlobal.isClear = false; //포탈을 사용해서 새로운 방으로 왔으므로 방은 클리어되지 않은 상태
    //}
}
