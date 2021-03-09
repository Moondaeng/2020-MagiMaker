using UnityEngine;
using UnityEngine.Events;

namespace NEvent
{
    [System.Serializable]
    public class MoveStart : UnityEvent<Vector3, Vector3> { }

    [System.Serializable]
    public class MoveStop : UnityEvent<Vector3> { }

    [System.Serializable]
    public class ActionStart : UnityEvent<int, Vector3, Vector3> { }
}

[DisallowMultipleComponent]
public class CGameEvent : MonoBehaviour
{
    public class ChangingMoneyEvent : UnityEvent<int> { }


    public NEvent.MoveStart PlayerMoveStartEvent;
    public NEvent.MoveStop PlayerMoveStopEvent;
    public NEvent.ActionStart PlayerActionEvent;

    public UnityEvent<Vector3> PlayerAttackEvent;

    private Network.CTcpClient _tcpManager;
    private Network.CPacketInterpreter _inGameInterpreter;

    public ChangingMoneyEvent EarnMoneyEvent = new ChangingMoneyEvent();
    public ChangingMoneyEvent LoseMoneyEvent = new ChangingMoneyEvent();

    public static CGameEvent instance;

    private CPlayerCommand _playerCommand;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        PlayerMoveStartEvent = new NEvent.MoveStart();
        PlayerMoveStopEvent = new NEvent.MoveStop();
    }

    private void Start()
    {
        _tcpManager = Network.CTcpClient.instance;
        _playerCommand = CPlayerCommand.instance;

        if (_tcpManager != null && _tcpManager.IsConnect == true)
        {
            Debug.Log("Network Connected");
            // 연결되면 패킷 받을거 설정
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
        else if (_playerCommand != null)
        {
            Debug.Log("Network not Connected");
            //_playerCommand.SetMyCharacter(0);
            EarnMoneyEvent.AddListener(_playerCommand.EarnMoneyAllCharacter);
        }
    }

    public void PlayerMoveStart(Vector3 a, Vector3 b) => PlayerMoveStartEvent?.Invoke(a, b);

    public void PlayerMoveStop(Vector3 pos) => PlayerMoveStopEvent?.Invoke(pos);

    public void PlayerAttack(Vector3 pos) => PlayerAttackEvent?.Invoke(pos);

    public void PlayerAction(int actionNumber, Vector3 now, Vector3 dest) => PlayerActionEvent?.Invoke(actionNumber, now, dest);
}