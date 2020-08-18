using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsePortal : MonoBehaviour
{
    public Vector3 position;
    GameObject[] player;
    GameObject PortalAcceptParent;
    GameObject FadeController;
//=======
//    public GameObject PortalPopup;

//    private CPlayerCommand _playerCommand;

//    private void Awake()
//    {
//        _playerCommand = GameObject.Find("GameManager").GetComponent<CPlayerCommand>();
//    }
//>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3

    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        PortalAcceptParent = GameObject.Find("PortalPopUp");
        FadeController = GameObject.Find("FadeController");
        PortalAcceptParent.transform.FindChild("PortalAccept").gameObject.SetActive(false);
        PortalAcceptParent.transform.FindChild("WaitingForOtherPlayer").gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            PortalAcceptParent.transform.FindChild("PortalAccept").gameObject.SetActive(true);
            FadeController.transform.FindChild("FadeCanvas").gameObject.SetActive(false);
            CWaitingForAccept.instance._portal = gameObject;
        }
    }

    public void MoveToNextRoom()
    {
        FadeController.transform.FindChild("FadeCanvas").gameObject.SetActive(true);
        CFadeInOut.instance.PlayFadeFlow(); //다음 방 넘어갈 때, 페이드 아웃 방 생성 이후 페이드 인

        StartCoroutine(RefreshWorld());
    }

    public IEnumerator RefreshWorld()
    {
        yield return new WaitForSeconds(1.0f);

        Transform ParentTransform = player[0].transform; //최상위 오브젝트 찾기 -> 캐릭터 옮기기
        while (true)
        {
            if (ParentTransform.parent == null)
                break;
            else
                ParentTransform = ParentTransform.parent;
        }
        ParentTransform.position = new Vector3(0, 1, 0);

        CCreateMap.instance.map.DestroyRoom();//오브젝트 삭제
        //방 배치
        int whichPortalSelect = 0;

        switch(gameObject.tag)
        {
            case "LEFT_PORTAL":
                whichPortalSelect = 0;
                break;
            case "PORTAL":
                whichPortalSelect = 1;
                break;
            case "RIGHT_PORTAL":
                whichPortalSelect = 2;
                break;
        }
        CCreateMap.instance.map.CreateRoom(CCreateMap.instance.map.GetRooms(), CCreateMap.instance.map.getRoomCount(), whichPortalSelect); //유저가 플레이 할 방 생성
        CCreateMap.instance.map.RoomFlagCtrl(CCreateMap.instance.map.GetRooms()[CCreateMap.instance.map.getRoomCount(), whichPortalSelect].RoomType); //유저가 선택한 방 타입이 다음 방에 중복으로 나올 확률 감소
        CCreateMap.instance.map.CreateStage(); //현재 방 이후의 방들 맵으로 생성
        CGlobal.roomCount++; //방 생성시 카운트 증가
        CGlobal.isClear = false; //포탈을 사용해서 새로운 방으로 왔으므로 방은 클리어되지 않은 상태
    }
//=======
//        if (coll.tag == "Player")
//        {
//            ActivePortalPopup();

//        }
//    }

//    public void ActivePortalPopup()
//    {
//        Debug.Log("************************************");
//        PortalPopup.SetActive(true);
//        PortalPopup.transform.Find("PortalAccept").gameObject.SetActive(true);
//        PortalPopup.transform.Find("WaitingForOtherPlayer").gameObject.SetActive(true);
//        PortalPopup.GetComponent<CWaitingForAccept>().SetWaitingPlayer();
//        //CWaitingForAccept.instance._portal = gameObject;
//    }

//    public void MoveToNextRoom()
//    {
//        if (_playerCommand == null)
//        {
//            _playerCommand = GameObject.Find("GameManager").GetComponent<CPlayerCommand>();
//        }
//        for (int player = 0; player < _playerCommand.activePlayersCount; player++)
//        {
//            position = position + Vector3.right * player * 8;
//            _playerCommand.Teleport(player, position);
//        }
//    }

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
