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
        PortalAcceptParent = GameObject.Find("PortalPopUp");
        PortalAcceptParent.transform.FindChild("PortalAccept").gameObject.SetActive(false);
        PortalAcceptParent.transform.FindChild("WaitingForOtherPlayer").gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider coll)
    {

        if (coll.tag == "Player")
        {
            Debug.Log("************************************");
            PortalAcceptParent.transform.FindChild("PortalAccept").gameObject.SetActive(true);
            CWaitingForAccept.instance._portal = gameObject;
        }
    }

    public void MoveToNextRoom()
    {
        CFadeInOut.instance.PlayFadeFlow(); //다음 방 넘어갈 때, 페이드 아웃 방 생성 이후 페이드 인

        Transform ParentTransform = player[0].transform; //최상위 오브젝트 찾기
        while (true)
        {
            if (ParentTransform.parent == null)
                break;
            else
                ParentTransform = ParentTransform.parent;
        }

        CGlobal.roomCount++;

        ParentTransform.position = new Vector3(0, 0, 0);
        //오브젝트 삭제
        //방 배치
        CCreateMap.instance.map.CreateStage(); //포탈 사용 시 맵 생성
        CCreateMap.instance.map.CreateRoom(CCreateMap.instance.map.GetRooms(), CCreateMap.instance.map.getRoomCount());
        CGlobal.isClear = false; //포탈을 사용해서 새로운 방으로 왔으므로 방은 클리어되지 않은 상태
    }
}
