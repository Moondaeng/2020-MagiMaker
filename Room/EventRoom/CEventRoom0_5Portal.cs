using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventRoom0_5Portal : MonoBehaviour
{
    private GameObject _npc;
    private GameObject _npcPopUp;
    private Vector3 _presentPosition;
    // Start is called before the first frame update
    void Start()
    {
        _npc = GameObject.Find("NPC");
        _npcPopUp = CEventRoomNpcClick.instance._popUp;
        _presentPosition = transform.position;
        transform.position = new Vector3(1000, 1000, 1000);
    }

    public void setPresentPosition()
    {
        transform.position = _presentPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.transform.position = _npc.transform.position + new Vector3(1, 0, 0);
            CRoomInRoomPopUpController.instance.SetText();
            Destroy(_npc);
            Destroy(_npcPopUp);
            GameObject.Find("EventRoom0_5Reward").transform.GetChild(1).gameObject.SetActive(true);
            GameObject.Find("EventRoom0_5Reward").transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
