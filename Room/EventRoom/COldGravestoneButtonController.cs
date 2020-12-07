﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class COldGravestoneButtonController : MonoBehaviour
{
    private GameObject _oldGravestone;
    private GameObject _popUp;
    private int _choose; //0이면 첫번째 선택 1이면 2번째 선택
    private Color _color;
    private void Start()
    {
        _oldGravestone = GameObject.Find("OldGravestone");
        _popUp = gameObject;
        _choose = 0;

        _popUp.transform.GetChild(0).GetComponent<Image>().color = Color.white; //1번째 선택 처음에 되있음. 하이라이트 = 흰색
        _popUp.transform.GetChild(1).GetComponent<Image>().color = Color.grey;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) //위 방향키
        {
            _choose = 0;

            _popUp.transform.GetChild(0).GetComponent<Image>().color = Color.white; //1번째 선택 하이라이트 = 흰색
            _popUp.transform.GetChild(1).GetComponent<Image>().color = Color.grey;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) //아래 방향키
        {
            _choose = 1;

            _popUp.transform.GetChild(0).GetComponent<Image>().color = Color.grey; 
            _popUp.transform.GetChild(1).GetComponent<Image>().color = Color.white; //2번째 선택 하이라이트 = 흰색
        }

        if (Input.GetKeyDown(KeyCode.Return)) //엔터 입력시
        {
            switch(_choose)
            {
                case 0:
                    ClickRandomItem();
                    break;
                case 1:
                    ClickCancel();
                    break;
            }
        }
    }
    public void ClickRandomItem()
    {
        Debug.Log("Get Item!");
        Debug.Log("Summon Enemies");

        CGlobal.isEvent = true; //적이 소환됬으므로 포탈 대기 상태
        CCreateMap.instance.NotifyPortal(); //플래그 바뀐 상태 방송하기

        _popUp.SetActive(false);
        Destroy(_oldGravestone);

        CGlobal.isEvent = false; //몹을 다 잡아서 이벤트 끝난 경우
        CCreateMap.instance.NotifyPortal();
    }

    public void ClickCancel()
    {
        _popUp.SetActive(false);
    }
}
