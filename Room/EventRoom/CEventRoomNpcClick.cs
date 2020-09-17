using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventRoomNpcClick : MonoBehaviour
{
    private GameObject _eventRoom;
    private GameObject _popUp;
    public static CEventRoomNpcClick instance = null;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;

        _eventRoom = gameObject.transform.parent.gameObject;
        _popUp = _eventRoom.transform.Find("NPCPopUp").gameObject;
        _popUp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _popUp.SetActive(false);
    }

    public void UseNPC()
    {
        _popUp.SetActive(true);
    }
}
