﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRoomInRoomPopUpController : CEventRoomPopUpController
{
    private GameObject[] _players;
    private Vector3 skillElitePos = new Vector3(36, 1, 127);
    private Vector3 itemElitePos = new Vector3(-40, 1, 127);
    private Vector3 Normal1Pos = new Vector3(36, 1, 0);
    private Vector3 Nomral2Pos = new Vector3(-40, 1, 0);

    // Start is called before the first frame update
    public override void Start()
    {    
        base.Start();
        _players = GameObject.FindGameObjectsWithTag("Player");
    }

    public override void ChooseButton(int choose)
    {
        Vector3 pos = new Vector3();

        if (choose == 4)
            ClickCancel();
        else
        {
            switch (choose)
            {
                case 0:
                    pos = skillElitePos;
                    break;
                case 1:
                    pos = itemElitePos;
                    break;
                case 2:
                    pos = Normal1Pos;
                    break;
                case 3:
                    pos = Nomral2Pos;
                    break;
            }
        }

        MovetoRoom(pos);

        CGlobal.popUpCancel = true;
    }

    public void MovetoRoom (Vector3 pos)
    {
        Transform ParentTransform = _players[0].transform; //최상위 오브젝트 찾기 -> 캐릭터 옮기기
        CPlayerCommand.instance.Teleport(0, pos);
        CPlayerCommand.instance.Teleport(1, pos + new Vector3(0, 0, 4));
        CPlayerCommand.instance.Teleport(2, pos + new Vector3(4, 0, 0));
        CPlayerCommand.instance.Teleport(3, pos + new Vector3(4, 0, 4));

        ParentTransform.position = pos;
    }

    public void ClickSkillElite() 
    {
        Transform ParentTransform = _players[0].transform; //최상위 오브젝트 찾기 -> 캐릭터 옮기기
        CPlayerCommand.instance.Teleport(0, skillElitePos);
        CPlayerCommand.instance.Teleport(1, skillElitePos + new Vector3(0, 0, 4));
        CPlayerCommand.instance.Teleport(2, skillElitePos + new Vector3(4, 0, 0));
        CPlayerCommand.instance.Teleport(3, skillElitePos + new Vector3(4, 0, 4));

        ParentTransform.position = skillElitePos;
    }

    
    public void ClickItemElite() { }
    public void ClickFirstNormal() { }
    public void ClickSecondeNormal() { }


    public void ClickCancel() { }
}