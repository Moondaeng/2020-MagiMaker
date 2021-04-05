using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalManager : MonoBehaviour
{
    private GameObject[] player;
    private GameObject PortalAcceptParent;
    private GameObject FadeController;

    public static CPortalManager instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        player = GameObject.FindGameObjectsWithTag("Player");
        PortalAcceptParent = GameObject.Find("PortalPopUp");
        FadeController = GameObject.Find("FadeController");
        PortalAcceptParent.transform.Find("PortalAccept").gameObject.SetActive(false);
        PortalAcceptParent.transform.Find("WaitingForOtherPlayer").gameObject.SetActive(false);
    }

    public void MoveToNextRoom()
    {
        FadeController.transform.Find("FadeCanvas").gameObject.SetActive(true);
        CFadeInOut.instance.PlayFadeFlow(); //다음 방 넘어갈 때, 페이드 아웃 방 생성 이후 페이드 인
        StartCoroutine(RefreshWorld());
    }

    public IEnumerator RefreshWorld()
    {
        yield return new WaitForSeconds(1.0f);

        CCreateMap.instance.DestroyRoom();//오브젝트 삭제

        yield return new WaitForSeconds(1.0f); //삭제 후 잠시 대기(삭제되는 오브젝트 참조하는 경우가 생겼음)

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
        CCreateMap.instance.CreateRoom(CCreateMap.instance.GetRooms(), CCreateMap.instance.getRoomCount(), whichPortalSelect); //유저가 플레이 할 방 생성
        CCreateMap.instance.RoomFlagCtrl(CCreateMap.instance.GetRooms()[CCreateMap.instance.getRoomCount(), whichPortalSelect].RoomType); //유저가 선택한 방 타입 저장

        Transform ParentTransform = player[0].transform; //최상위 오브젝트 찾기 -> 캐릭터 옮기기
        CPlayerCommand.instance.Teleport(0, new Vector3(0, 1, 0));
        CPlayerCommand.instance.Teleport(1, new Vector3(0, 1, 4));
        CPlayerCommand.instance.Teleport(2, new Vector3(4, 1, 0));
        CPlayerCommand.instance.Teleport(3, new Vector3(4, 1, 4));

        ParentTransform.position = new Vector3(0, 1, 0);

        if (CGlobal.isHost)
        {
            CCreateMap.instance.CreateStage(); //현재 방 이후의 방들 맵으로 생성
            CCreateMap.instance.SendRoomArr(); //현재 방 이후의 방들 피어들에게 전송
            
            //CCreateMap.instance.MakePortalText(CCreateMap.instance._roomCount, CCreateMap.instance._roomArr); //포탈 따로 생성
            // Recieve
        }
    }
}
