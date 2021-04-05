using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CItemEliteRoomPortal : CPortal
{
    private Vector3 _presentPosition;
    private Vector3 _treasurePresentPosition;
    private Transform _portalGroup;
    private Transform _treasure;

    public override void OpenNClosePortal()
    {
        Debug.Log("before move position");

        _portalGroup = transform.parent.parent;
        _treasure = GameObject.FindGameObjectWithTag("NPC").transform;

        if (!CGlobal.isClear) //클리어 전에는 포탈 없애야함.
        {
            if (_portalGroup.position == new Vector3(1000, 1000, 1000)) //중복해서 호출되는 경우 방지
                return;

            _presentPosition = _portalGroup.position; //현재 위치 저장해두기
            _treasurePresentPosition = _treasure.position;
            _portalGroup.position = new Vector3(1000, 1000, 1000); //저 멀리 던져두기
            _treasure.position = new Vector3(1000, 1000, 1000);

            Debug.Log("move position");
        }
        else // 클리어
        {
            Debug.Log("return position");
            _portalGroup.position = _presentPosition; //다시 가져오기
            _treasure.position = _treasurePresentPosition;
        }

    }
}
