using System.Collections.Generic;
using UnityEngine;

public class CPlayerManager : MonoBehaviour
{
    private class PlayerInfo
    {
        public int id;
        public GameObject pObject;

        public PlayerInfo(int id, GameObject playerObject)
        {
            this.id = id;
            pObject = playerObject;
        }
    }

    private int _playerCount = 0;
    private List<PlayerInfo> _playerList = new List<PlayerInfo>();

    public static CPlayerManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        DetectAllPlayer();
    }

    private void DetectAllPlayer()
    {
        for (int i = 1; i < 5; i++)
        {
            GameObject temp;
            if ((temp = GameObject.Find("Player" + i.ToString())) != null)
            {
                _playerList.Add(new PlayerInfo(i, temp));
            }
        }
    }

    public List<GameObject> GetPlayerObjects()
    {
        List<GameObject> temp = new List<GameObject>();
        for (int i = 0; i < _playerList.Count; i++)
        {
            temp.Add(_playerList[i].pObject);
        }

        return temp;
    }

    public List<Vector3> GetPlayerPosition()
    {
        List<Vector3> temp = new List<Vector3>();
        for (int i = 0; i < _playerList.Count; i++)
        {
            temp.Add(_playerList[i].pObject.transform.position);
        }

        return temp;
    }

    public int GetPlayerCount()
    {
        return _playerList.Count;
    }
    // Update is called once per frame


}