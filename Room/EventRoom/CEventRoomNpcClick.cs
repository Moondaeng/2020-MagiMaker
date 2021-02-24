using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventRoomNpcClick : MonoBehaviour
{
    private GameObject _eventRoom;
    private GameObject _popUp;
    public static CEventRoomNpcClick instance = null;
    public Stack<GameObject> _stackPopUp;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;

        instance._stackPopUp = new Stack<GameObject>();

        _eventRoom = gameObject.transform.parent.gameObject;
        _popUp = _eventRoom.transform.Find("NPCPopUp").gameObject;
        _popUp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Backspace))
#else
        if (Input.GetKeyDown(KeyCode.Escape))
#endif
        {
            CanclePopUp();
        }
    }

    public void CanclePopUp()
    {
        if (instance._stackPopUp.Count != 0)
        {
            GameObject popUp = instance._stackPopUp.Pop();
            popUp.SetActive(false);
        }

        if (instance._stackPopUp.Count == 0)
        {
            CGlobal.useNPC = false;
            CWindowFacade.instance.SetOtherWindowMode(false);
        }
    }

    public void UseNPC()
    {
        _popUp.SetActive(true);
        instance._stackPopUp.Push(_popUp);
        CGlobal.useNPC = true;
        CWindowFacade.instance.SetOtherWindowMode(true);
    }
}
