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
    public class UseSkill : UnityEvent<int, Vector3, Vector3> { }
}

namespace Network
{
    [DisallowMultipleComponent]
    public class CNetworkEvent : MonoBehaviour
    {
        public class ChangingMoneyEvent : UnityEvent<int> { }

        public NEvent.MoveStart PlayerMoveStartEvent = new NEvent.MoveStart();
        public NEvent.MoveStop PlayerMoveStopEvent = new NEvent.MoveStop();
        public NEvent.UseSkill PlayerActionEvent;

        public ChangingMoneyEvent EarnMoneyEvent = new ChangingMoneyEvent();
        public ChangingMoneyEvent LoseMoneyEvent = new ChangingMoneyEvent();

        public Action UsePortalEvent;
        public Action<int> PortalVoteEvent;

        public static CNetworkEvent instance;

        private Network.CTcpClient _tcpManager;
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

        public void PlayerAction(int actionNumber, Vector3 now, Vector3 dest) => PlayerActionEvent?.Invoke(actionNumber, now, dest);

        private void InitMultiplay()
        {
            // 캐릭터 설정
            Debug.Log($"Set Character : Send Message");
            _playerCommand.SetActivePlayers(CClientInfo.PlayerCount);
            SendLodingFinish();
            SendCharacterInfoRequest();
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
            var controller = CController.instance;
            // 플레이어 움직임
            PlayerMoveStartEvent.AddListener(SendMoveStart);
            PlayerMoveStopEvent.AddListener(SendMoveStop);
            PlayerActionEvent.AddListener(SendActionStart);
            controller.PlayerPosCorrectionEvent.AddListener(SendMoveStop);
            controller.PlayerJumpEvent.AddListener(SendJumpStart);
            controller.PlayerAttackEvent.AddListener(SendAttackStart);
            controller.PlayerRollEvent.AddListener(SendRollStart);

            // 포탈 관련
            UsePortalEvent += SendUsePortal;
            PortalVoteEvent += SendPortalVote;

            if (CClientInfo.JoinRoom.IsHost)
            {
                Debug.Log("I'm Host");
                AddNetworkHostCode();
            }
        }

        private void RemoveNetworkCode()
        {
            // 플레이어 움직임
            PlayerMoveStartEvent.RemoveListener(SendMoveStart);
            PlayerMoveStopEvent.RemoveListener(SendMoveStop);
            PlayerActionEvent.RemoveListener(SendActionStart);

            // 포탈 관련
            UsePortalEvent -= SendUsePortal;
            PortalVoteEvent -= SendPortalVote;
        }

        private void AddNetworkHostCode()
        {
            // 몬스터 패턴 코드
            // 돈 획득
            // 방 생성
            CCreateMap.instance.CreateRooms.AddListener(SendRoomsInfo);
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

        public static void SendCharacterInfoRequest()
        {
            var message = CPacketFactory.CreateCharacterInfoPacket();

            CTcpClient.instance.Send(message.data);
        }

        public static void SendMoveStart(Vector3 now, Vector3 dest)
        {
            var message = CPacketFactory.CreateMoveStartPacket(now, dest);

            CTcpClient.instance.Send(message.data);
        }

        public static void SendMoveStop(Vector3 now)
        {
            var message = CPacketFactory.CreateMoveStopPacket(now);

            CTcpClient.instance.Send(message.data);
        }

        public static void SendActionStart(int actionNumber, Vector3 now, Vector3 dest)
        {
            var message = CPacketFactory.CreateActionStartPacket(actionNumber, now, dest);

            CTcpClient.instance.Send(message.data);
        }

        private static void SendJumpStart(Vector3 currentPos, float rotate, bool isMoving)
        {
            Debug.Log("send jump");
            var message = CPacketFactory.CreateJumpStartPacket(currentPos, rotate, isMoving);
            CTcpClient.instance.Send(message.data);
        }

        private static void SendAttackStart(Vector3 currentPos, float rotate)
        {
            Debug.Log("send attack");
            var message = CPacketFactory.CreateAttackStartPacket(currentPos, rotate);
            CTcpClient.instance.Send(message.data);
        }

        private static void SendRollStart(Vector3 currentPos, float rotate)
        {
            Debug.Log("send roll");
            var message = CPacketFactory.CreateRollStartPacket(currentPos, rotate);
            CTcpClient.instance.Send(message.data);
        }

        public static void SendUsePortal()
        {
            var packet = CPacketFactory.CreatePortalPopup();

            CTcpClient.instance.Send(packet.data);
        }

        public static void SendPortalVote(int accept)
        {
            var packet = CPacketFactory.CreatePortalVote(accept);

            CTcpClient.instance.Send(packet.data);
        }

        public static void SendRoomsInfo(CRoom[,] roomArr)
        {
            var roomsIntArr = new int[CConstants.ROOM_PER_STAGE, CConstants.MAX_ROAD];

            for (int i = 0; i < CConstants.ROOM_PER_STAGE; i++)
            {
                for (int j = 0; j < CConstants.MAX_ROAD; j++)
                {
                    roomsIntArr[i, j] = (int)roomArr[i, j].RoomType;
                }
            }

            var message = CPacketFactory.CreateRoomTypeInfo(roomsIntArr);

            CTcpClient.instance.Send(message.data);
        }

        private void SendLodingFinish()
        {
            Debug.Log("Send Loading Finish Message");

            var packet = CPacketFactory.CreateFinishLoading(CClientInfo.JoinRoom.IsHost, CClientInfo.PlayerCount);

            CTcpClient.instance.Send(packet.data);
        }

        #endregion
    }
}
