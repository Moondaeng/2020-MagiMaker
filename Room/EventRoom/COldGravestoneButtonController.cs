using System.Collections;
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

        GameObject[] portalMom = GameObject.FindGameObjectsWithTag("PORTAL_MOM"); //포탈들 대기
        foreach (GameObject obj in portalMom)
            obj.SetActive(false);

        //추후에 몹 다잡으면 포탈 다시 돌려주는 코드 있어야함
        foreach (GameObject obj in portalMom) //추후 삭제할 코드
            obj.SetActive(true);

        _popUp.SetActive(false);
        Destroy(_oldGravestone);
    }

    public void ClickCancel()
    {
        _popUp.SetActive(false);
    }
}
