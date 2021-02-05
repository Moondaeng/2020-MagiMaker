using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class COldGravestoneButtonController : MonoBehaviour
{
    private GameObject _oldGravestone;
    private GameObject _popUp;
    private GameObject _monsterGroup;
    private int _choose; //0이면 첫번째 선택 1이면 2번째 선택
    private Color _color;
    private void Start()
    {
        _oldGravestone = GameObject.Find("OldGravestone");
        _popUp = gameObject;
        _choose = 0;

        _popUp.transform.GetChild(0).GetComponent<Image>().color = Color.white; //1번째 선택 처음에 되있음. 하이라이트 = 흰색
        _popUp.transform.GetChild(1).GetComponent<Image>().color = Color.grey;

        _monsterGroup = GameObject.Find("MonsterGroup");
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
            CEventRoomNpcClick.instance.CanclePopUp();
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
        GameObject item = CItemDropTable.instance.DropRandomItem(CCreateMap.instance.GetStageNumber());
        item = Instantiate(item, _oldGravestone.transform.position, _oldGravestone.transform.rotation);
        item.SetActive(true);

        for (int i = 0; i < _monsterGroup.transform.childCount; i++)
            _monsterGroup.transform.GetChild(i).gameObject.SetActive(true);

        CGlobal.isEvent = true; //적이 소환됬으므로 포탈 대기 상태
        CGlobal.useNPC = false; //팝업 꺼지므로 플레이어 이동 안막힘
        CCreateMap.instance.NotifyPortal(); //플래그 바뀐 상태 방송하기

        _popUp.SetActive(false);
        Destroy(_oldGravestone);

        //debug
        GameObject.Find("EventRoom0_0(Clone)").GetComponent<CEventRoomMonsterCheck>().SendMessage("ForceDeadMonster");
    }

    public void ClickCancel()
    {
        _popUp.SetActive(false);
        CGlobal.useNPC = false; //팝업 꺼지므로 플레이어 이동 안막힘
    }
}
