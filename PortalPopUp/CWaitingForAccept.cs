using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CWaitingForAccept : MonoBehaviour
{
    public enum EAccept
    {
        _waiting,
        _accept,
        _cancel
    }
    public List<EAccept> _playerAcceptList;
    public GameObject _portal;
    public GameObject _waitingForOtherPlayer;
    public GameObject _portalPopUp;
    public static CWaitingForAccept instance = null;

    private CPlayerCommand _playerCommand;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("instaceeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
        }
        for (int i = 0; i < _playerAcceptList.Count; i++)
        {
            _playerAcceptList[i] = EAccept._waiting;
        }
        //_portalPopUp = GameObject.Find("PortalPopUp");
        //_waitingForOtherPlayer = _portalPopUp.transform.Find("WaitingForOtherPlayer").gameObject;
        _playerCommand = GameObject.Find("GameManager").GetComponent<CPlayerCommand>();
    }

    public void SetWaitingPlayer()
    {
        if(_playerCommand == null)
        {
            _playerCommand = GameObject.Find("GameManager").GetComponent<CPlayerCommand>();
        }
        for (int player = 0; player < _playerCommand.activePlayersCount; player++)
        {
            Debug.Log($"Waiting Player : {player}");
            _waitingForOtherPlayer.transform.GetChild(player).gameObject.SetActive(true);
        }
    }

    public void SetPlayerAccept(int playerNum, EAccept accept)
    {
        Image image = _waitingForOtherPlayer.transform.GetChild(playerNum).GetComponent<Image>();
        if (accept == EAccept._accept)
        {
            Debug.Log($"Press Accept : {playerNum}");
            _playerAcceptList[playerNum] = EAccept._accept;
            image.sprite = Resources.Load<Sprite>("T_12_ok_") as Sprite;
            CheckAllAccept();
        }
        else if(accept == EAccept._cancel)
        {
            Debug.Log($"Press Cancel : {playerNum}");
            _playerAcceptList[playerNum] = EAccept._cancel;
            image.sprite = Resources.Load<Sprite>("T_12_no_") as Sprite;
            CancelPortal();
        }
    }

    // 모든 사람이 Accept했는지 확인한다
    // 모든 사람이 Accept했다면 Portal 위치로 전부 강제 이동시킨다
    private void CheckAllAccept()
    {
        for (int player = 0; player < _playerCommand.activePlayersCount; player++)
        {
            Debug.Log($"Check Accept");
            if (_playerAcceptList[player] != EAccept._accept)
            {
                return;
            }
        }

        _portalPopUp.SetActive(false);

        UsePortal usePortal = instance._portal.GetComponent<UsePortal>();
        usePortal.MoveToNextRoom();
    }

    private void CancelPortal()
    {
        _portalPopUp.SetActive(false);
    }
}
