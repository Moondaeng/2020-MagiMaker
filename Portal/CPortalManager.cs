using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortalManager : MonoBehaviour
{
    public Vector3 position;
    GameObject[] player;
    GameObject PortalAcceptParent;
    GameObject FadeController;

    private void Start()
    {
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

        Transform ParentTransform = player[0].transform; //최상위 오브젝트 찾기 -> 캐릭터 옮기기

        ParentTransform.position = new Vector3(0, 1, 0);

        CCreateMap.instance.DestroyRoom();//오브젝트 삭제
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
        CCreateMap.instance.RoomFlagCtrl(CCreateMap.instance.GetRooms()[CCreateMap.instance.getRoomCount(), whichPortalSelect].RoomType); //유저가 선택한 방 타입이 다음 방에 중복으로 나올 확률 감소
        CCreateMap.instance.CreateStage(); //현재 방 이후의 방들 맵으로 생성
        CGlobal.roomCount++; //방 생성시 카운트 증가
        CGlobal.isClear = false; //포탈을 사용해서 새로운 방으로 왔으므로 방은 클리어되지 않은 상태
        CCreateMap.instance.NotifyPortal(); //플래그를 이용한 옵저버 패턴, 포탈 삭제하기
    }
}
