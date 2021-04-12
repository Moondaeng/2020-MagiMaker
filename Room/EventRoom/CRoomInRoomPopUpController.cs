using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRoomInRoomPopUpController : CNPCPopUpController
{
    private GameObject[] _players;
    private Vector3 skillElitePos = new Vector3(36, 1, 127);
    private Vector3 itemElitePos = new Vector3(-40, 1, 127);
    private Vector3 Normal1Pos = new Vector3(-40, 1, 0);
    private Vector3 Nomral2Pos = new Vector3(36, 1, 0);

    public GameObject normal1Text;
    public GameObject normal2Text;
    public GameObject elite1Text;
    public GameObject elite2Text;

    static public CRoomInRoomPopUpController instance = null;

    // Start is called before the first frame update
    public override void Start()
    {    
        base.Start();
        _players = GameObject.FindGameObjectsWithTag("Player");

        if (instance == null)
            instance = this;
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

            MovetoRoom(pos);
        }     

        CGlobal.popUpCancel = true;
    }

    public void SetText()
    {
        normal1Text.SetActive(true);
        normal2Text.SetActive(true);
        elite1Text.SetActive(true);
        elite2Text.SetActive(true);
    }

    public void OffText()
    {
        normal1Text.SetActive(false);
        normal2Text.SetActive(false);
        elite1Text.SetActive(false);
        elite2Text.SetActive(false);
    }

    public void MovetoRoom (Vector3 pos)
    {
        Transform ParentTransform = _players[0].transform; //최상위 오브젝트 찾기 -> 캐릭터 옮기기
        CPlayerCommand.instance.Teleport(0, pos);
        CPlayerCommand.instance.Teleport(1, pos + new Vector3(0, 0, 4));
        CPlayerCommand.instance.Teleport(2, pos + new Vector3(4, 0, 0));
        CPlayerCommand.instance.Teleport(3, pos + new Vector3(4, 0, 4));

        ParentTransform.position = pos;

        OffText();
    }

    public void ClickCancel() { }
}
