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

        if (_tcpManager != null && _tcpManager.IsConnect == true && !CClientInfo.IsSinglePlay())
        {
            Debug.Log("Multiplay Node");
            // 연결되면 패킷 받을거 설정
            _inGameInterpreter = new Network.CPacketInterpreter(_tcpManager);
            AddNetworkCode();

            InitMultiplay();
        }
        // 싱글 플레이 시에 일부 동작들은 서버에 거치지 않고 동작해야 함
        else if (_playerCommand != null)
        {
            EarnMoneyEvent.AddListener(_playerCommand.EarnMoneyAllCharacter);
            InitSinglePlay();
        }
    }

    public void QuitPlayer(int roomSlotNum)
    {
        
    }

    public void PlayerMoveStart(Vector3 a, Vector3 b) => PlayerMoveStartEvent?.Invoke(a, b);

    public void PlayerMoveStop(Vector3 pos) => PlayerMoveStopEvent?.Invoke(pos);

    public void PlayerAttack(Vector3 pos) => PlayerAttackEvent?.Invoke(pos);

    public void PlayerAction(int actionNumber, Vector3 now, Vector3 dest) => PlayerActionEvent?.Invoke(actionNumber, now, dest);

    private void InitMultiplay()
    {
        // 캐릭터 설정
        Debug.Log($"Set Character : Send Message");
        _playerCommand.SetActivePlayers(CClientInfo.PlayerCount);
        _inGameInterpreter.SendCharacterInfoRequest();
    }

    private void InitSinglePlay()
    {
        Debug.Log("Singleplay Mode");
        _playerCommand.SetActivePlayers(1);
        _playerCommand.SetMyCharacter(0);
    }

    private void AddNetworkCode()
    {
        _tcpManager.SetPacketInterpret(_inGameInterpreter.PacketInterpret);
        // 플레이어 움직임
        PlayerMoveStartEvent.AddListener(_inGameInterpreter.SendMoveStart);
        PlayerMoveStopEvent.AddListener(_inGameInterpreter.SendMoveStop);
        PlayerActionEvent.AddListener(_inGameInterpreter.SendActionStart);
    }

    private void RemoveNetworkCode()
    {
        // 플레이어 움직임
        PlayerMoveStartEvent.RemoveListener(_inGameInterpreter.SendMoveStart);
        PlayerMoveStopEvent.RemoveListener(_inGameInterpreter.SendMoveStop);
        PlayerActionEvent.RemoveListener(_inGameInterpreter.SendActionStart);
    }

    private void AddSingleplayCode()
    {
        // 몬스터 패턴 바로 적용하는 코드
        // 돈 획득 바로하는 코드
        // 방 생성 바로하는 코드
    }

    private void SucceedHost()
    {

    }
}