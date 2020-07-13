using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * 게임 이벤트 관리 컴포넌트
 * 
 */
namespace NEvent
{
    [System.Serializable]
    public class MoveStart : UnityEvent<Vector3, Vector3> { }
    [System.Serializable]
    public class MoveStop : UnityEvent<Vector3> { }
    [System.Serializable]
    public class ActionStart : UnityEvent<int, Vector3, Vector3> { }
}

public class CGameEvent : MonoBehaviour
{
    public NEvent.MoveStart PlayerMoveStartEvent;
    public NEvent.MoveStop PlayerMoveStopEvent;
    public NEvent.ActionStart PlayerActionEvent;

    public UnityEvent<Vector3> PlayerAttackEvent;

    private Network.CTcpClient _tcpManager;
    private Network.CPacketInterpreter _inGameInterpreter;

    private CPlayerCommand _playerCommand;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        PlayerMoveStartEvent = new NEvent.MoveStart();
        PlayerMoveStopEvent = new NEvent.MoveStop();
    }

    private void Start()
    {
        GameObject tcpObject = GameObject.Find("Network");
        if(tcpObject != null)
        {
            _tcpManager = tcpObject.GetComponent<Network.CTcpClient>();
        }
        GameObject commanderObject = GameObject.Find("GameManager");
        if(commanderObject != null)
        {
            _playerCommand = commanderObject.GetComponent<CPlayerCommand>();
        }

        // 연결되면 패킷 받을거 설정
        if (_tcpManager != null && _tcpManager.IsConnect == true)
        {
            Debug.Log("Network Connected");
            _inGameInterpreter = new Network.CPacketInterpreter(_tcpManager);
            _tcpManager.SetPacketInterpret(_inGameInterpreter.PacketInterpret);
            PlayerMoveStartEvent.AddListener(_inGameInterpreter.SendMoveStart);
            PlayerMoveStopEvent.AddListener(_inGameInterpreter.SendMoveStop);
            PlayerActionEvent.AddListener(_inGameInterpreter.SendActionStart);

            // 캐릭터 설정
            Debug.Log($"Set Character : Send Message");
            _playerCommand.SetActivePlayers(CClientInfo.PlayerCount);
            _inGameInterpreter.SendCharacterInfoRequest();
        }
        // 싱글 플레이 시에 일부 동작들은 서버에 거치지 않고 동작해야 함
        else if(_playerCommand != null)
        {
            Debug.Log("Network not Connected");
            _playerCommand.SetMyCharacter(0);
        }
    }

    public void PlayerMoveStart(Vector3 a, Vector3 b) => PlayerMoveStartEvent?.Invoke(a, b);
    public void PlayerMoveStop(Vector3 pos) => PlayerMoveStopEvent?.Invoke(pos);
    public void PlayerAttack(Vector3 pos) => PlayerAttackEvent?.Invoke(pos);
    public void PlayerAction(int actionNumber, Vector3 now, Vector3 dest) => PlayerActionEvent?.Invoke(actionNumber, now, dest);
}
