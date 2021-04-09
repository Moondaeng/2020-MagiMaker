using System;
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

namespace Network
{

    [DisallowMultipleComponent]
    public class CNetworkEvent : MonoBehaviour
    {
        public class ChangingMoneyEvent : UnityEvent<int> { }

        public NEvent.MoveStart PlayerMoveStartEvent = new NEvent.MoveStart();
        public NEvent.MoveStop PlayerMoveStopEvent = new NEvent.MoveStop();
        public NEvent.ActionStart PlayerActionEvent;

        public UnityEvent<Vector3> PlayerAttackEvent;

        private Network.CTcpClient _tcpManager;

        public ChangingMoneyEvent EarnMoneyEvent = new ChangingMoneyEvent();
        public ChangingMoneyEvent LoseMoneyEvent = new ChangingMoneyEvent();

        public Action UsePortalEvent;
        public Action<int> PortalVoteEvent;

        public static CNetworkEvent instance;

        private CPlayerCommand _playerCommand;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            _tcpManager = Network.CTcpClient.instance;
            _playerCommand = CPlayerCommand.instance;

            //if (_tcpManager?.IsConnect == true && !CClientInfo.IsSinglePlay())
            if (_tcpManager?.IsConnect == true)
            {
                _tcpManager.SetPacketInterpret(Network.CPacketInterpreter.PacketInterpret);

                Debug.Log("Multiplay Mode");
                AddNetworkCode();
                InitMultiplay();
            }
            else
            {
                AddSingleplayCode();
                InitSinglePlay();
            }
        }

        public void QuitPlayer(int playerNumber)
        {
            CPlayerCommand.instance.DeactivatePlayer(playerNumber);

            if (CPlayerCommand.instance.ActivatedPlayersCount == 1)
            {
                RemoveNetworkCode();
            }
        }

        public void PlayerMoveStart(Vector3 now, Vector3 dest) => PlayerMoveStartEvent?.Invoke(now, dest);

        public void PlayerMoveStop(Vector3 pos) => PlayerMoveStopEvent?.Invoke(pos);

        public void PlayerAttack(Vector3 pos) => PlayerAttackEvent?.Invoke(pos);

        public void PlayerAction(int actionNumber, Vector3 now, Vector3 dest) => PlayerActionEvent?.Invoke(actionNumber, now, dest);

        private void InitMultiplay()
        {
            // 캐릭터 설정
            Debug.Log($"Set Character : Send Message");
            _playerCommand.SetActivePlayers(CClientInfo.PlayerCount);
            SendLodingFinish();
            CPacketInterpreter.SendCharacterInfoRequest();
        }

        private void InitSinglePlay()
        {
            Debug.Log("Singleplay Mode");
            _playerCommand.SetActivePlayers(1);
            _playerCommand.SetMyCharacter(0);
            CCreateMap.instance.CreateStage();
        }

        private void AddNetworkCode()
        {
            // 플레이어 움직임
            PlayerMoveStartEvent.AddListener(Network.CPacketInterpreter.SendMoveStart);
            PlayerMoveStopEvent.AddListener(Network.CPacketInterpreter.SendMoveStop);
            PlayerActionEvent.AddListener(Network.CPacketInterpreter.SendActionStart);

            // 포탈 관련
            UsePortalEvent += Network.CPacketInterpreter.SendUsePortal;
            PortalVoteEvent += Network.CPacketInterpreter.SendPortalVote;

            if (CClientInfo.JoinRoom.IsHost)
            {
                Debug.Log("I'm Host");
                AddNetworkHostCode();
            }
        }

        private void RemoveNetworkCode()
        {
            // 플레이어 움직임
            PlayerMoveStartEvent.RemoveListener(Network.CPacketInterpreter.SendMoveStart);
            PlayerMoveStopEvent.RemoveListener(Network.CPacketInterpreter.SendMoveStop);
            PlayerActionEvent.RemoveListener(Network.CPacketInterpreter.SendActionStart);

            // 포탈 관련
            UsePortalEvent -= Network.CPacketInterpreter.SendUsePortal;
            PortalVoteEvent -= Network.CPacketInterpreter.SendPortalVote;
        }

        private void AddNetworkHostCode()
        {
            // 몬스터 패턴 코드
            // 돈 획득
            // 방 생성
            CCreateMap.instance.CreateRooms.AddListener(CPacketInterpreter.SendRoomsInfo);
        }

        private void AddSingleplayCode()
        {
            // 몬스터 패턴 바로 적용하는 코드
            // 돈 획득 바로하는 코드
            //EarnMoneyEvent.AddListener(_playerCommand.EarnMoneyAllCharacter);
            // 방 생성 바로하는 코드
            CCreateMap.instance.CreateRooms.AddListener(CCreateMap.instance.ReceiveRoomArr);
        }

        private void SucceedHost()
        {

        }

        #region Packet Send
        private void SendLodingFinish()
        {
            Debug.Log("Send Loading Finish Message");

            var packet = CPacketFactory.CreateFinishLoading(CClientInfo.JoinRoom.IsHost, CClientInfo.PlayerCount);

            CTcpClient.instance.Send(packet.data);
        }

        #endregion
    }
}
