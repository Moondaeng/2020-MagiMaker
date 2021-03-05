using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBossRoomPortal : CPortal
{
    private Vector3 _presentPosition;
    private Transform _portalMom;
    [SerializeField] private GameObject _clearUi;

    public override void OpenNClosePortal()
    {
        Debug.Log("before move position");

        _portalMom = transform.parent;

        if (!CGlobal.isClear) //클리어 전에는 포탈 없애야함.
        {
            if (_portalMom.position == new Vector3(1000, 1000, 1000)) //중복해서 호출되는 경우 방지
                return;

            _presentPosition = _portalMom.position; //현재 위치 저장해두기
            _portalMom.position = new Vector3(1000, 1000, 1000); //저 멀리 던져두기

            Debug.Log("move position");
        }
        else // 클리어
        {
            Debug.Log("return position");
            _portalMom.position = _presentPosition; //다시 가져오기
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            Debug.Log("Game Clear");
            _clearUi.SetActive(true);
            CWindowFacade.instance.SetOtherWindowMode(true);
        }
    }
}
