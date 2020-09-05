using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COldGravestoneButtonController : MonoBehaviour
{
    GameObject _oldGravestone;
    GameObject _popUp;
    private void Start()
    {
        _oldGravestone = GameObject.Find("OldGravestone");
        _popUp = gameObject.transform.parent.gameObject;
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
