using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventRoomNpcClick : MonoBehaviour
{
    private GameObject _eventRoom;
    public GameObject _popUp;
    public static CEventRoomNpcClick instance = null;
    public Stack<GameObject> _stackPopUp;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;

        instance._stackPopUp = new Stack<GameObject>();

        _eventRoom = gameObject.transform.parent.gameObject;
        _popUp = GameObject.Find("NPCPopUp");
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
            CancelPopUp();
        }
    }

    public void ChangePopUp(GameObject popUp)
    {
        instance._popUp = popUp;
        instance._popUp.SetActive(false);
    }

    public void CancelPopUp()
    {
        Debug.Log($"{instance._stackPopUp.Count}");
        if (instance._stackPopUp.Count != 0)
        {
            GameObject popUp = instance._stackPopUp.Pop();
            popUp.SetActive(false);
        }

        if (instance._stackPopUp.Count == 0)
        {
            CWindowFacade.instance.SetOtherWindowMode(false);
        }
    }

    public void UseNPC()
    {
        _popUp.SetActive(true);
        instance._stackPopUp.Push(_popUp);
        CWindowFacade.instance.SetOtherWindowMode(true);
    }
}
