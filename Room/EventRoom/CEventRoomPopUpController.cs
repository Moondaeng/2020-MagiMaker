using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CEventRoomPopUpController : MonoBehaviour
{
    private GameObject _popUp;
    private int _childCount;
    private int _choose; //0이면 첫번째 선택 1이면 2번째 선택

    // Start is called before the first frame update
    public virtual void Start()
    {
        _popUp = gameObject;

        _childCount = _popUp.transform.childCount;

        _popUp.transform.GetChild(0).GetComponent<Image>().color = Color.white; //1번째 선택 처음에 되있음. 하이라이트 = 흰색
        _choose = 0;

        for (int i = 1; i < _childCount; i++)
            _popUp.transform.GetChild(i).GetComponent<Image>().color = Color.grey;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) //위 방향키
        {
            if (_choose > 0)
                _choose--;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) //아래 방향키
        {
            if (_choose < (_childCount - 1))
                _choose++;
        }

        highlightSet(_choose);

        if (Input.GetKeyDown(KeyCode.Return)) //엔터 입력 시
        {
            ChooseButton(_choose);
          
            if (CGlobal.popUpCancel)
            {
                CGlobal.useNPC = false; //팝업 꺼지므로 플레이어 이동 안막힘
                CGlobal.popUpCancel = false;
                CEventRoomNpcClick.instance.CanclePopUp();
            }
        }
    }

    public virtual void ChooseButton(int choose) { }

    void highlightSet(int choose)
    {
        for (int i = 0; i < _childCount; i++)
            _popUp.transform.GetChild(i).GetComponent<Image>().color = Color.grey;

        _popUp.transform.GetChild(choose).GetComponent<Image>().color = Color.white;
    }
}
