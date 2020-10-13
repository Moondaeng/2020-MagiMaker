using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventRoomPortal : CPortal
{
    private Vector3 _presentPosition;
    private Transform _portalMom;

    public override void OpenNClosePortal()
    {
        Debug.Log("before move position");

        if (CGlobal.isEvent) //이벤트 진행중이므로 포탈 없애야함.
        {
            _portalMom = transform.parent;
            _presentPosition = _portalMom.position; //현재 위치 저장해두기
            _portalMom.position = new Vector3(1000, 1000, 1000); //저 멀리 던져두기

            Debug.Log("move position");
        }
        else //이벤트 진행 종료
        {
            _portalMom.position = _presentPosition; //다시 가져오기
        }    

    }
}
