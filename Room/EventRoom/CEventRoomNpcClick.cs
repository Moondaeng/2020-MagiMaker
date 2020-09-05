using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventRoomNpcClick : MonoBehaviour
{
    private GameObject _eventRoom;
    private GameObject _popUp;
    // Start is called before the first frame update
    void Start()
    {
        _eventRoom = gameObject.transform.parent.gameObject;
        _popUp = _eventRoom.transform.FindChild("PopUp").gameObject;
        _popUp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            ClickNPC();

        if (Input.GetKeyDown(KeyCode.Escape))
            _popUp.SetActive(false);
    }

    void ClickNPC()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        Physics.Raycast(ray, out hit);

        if (hit.collider.gameObject.tag == "NPC")//마우스 가져간 대상이 상인 캐릭터인 경우
        {
            UseNPC();
        }
    }

    void UseNPC()
    {
        _popUp.SetActive(true);
    }
}
