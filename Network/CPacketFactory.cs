using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    public static class CPacketFactory
    {
        private const int _cmdShutdownCode = 900;

        enum ELogin
        {
            LoginRequest = 100,
            RegisterRequest = 101,
            LobbyRequest = 102,
        }
        enum ELobby
        {
            RoomCreateRequest = 200,
            RoomJoinRequest = 201,
            RoomListRequest = 202,
            RoomCountRequest = 203,
        }
        enum EReadyRoom
        {
            StartRequest = 301,
            GuestQuitRequest = 302,
            HostQuitRequest = 303,
            LobbyReturnRequest = 304,
            // debug
            BanRequest = 305,
            AddGuestRequest = 306
        }

        enum EInGame
        {
            CharacterInfoRequest = 400,
            MoveStart = 401,
            MoveStop = 402,
            ActionStart = 403,
            JumpStart = 404,
            AttackStart = 405,
            RollStart = 406,

            ReturnLobby = 901,
            FinishLoding = 902,
        }

        enum EMapInfo
        {
            WaitEntering = 603,
            EnterNextRoom = 604,
        }

        enum EDebug
        {
            ChangePlayer = 1000,
            KickPlayer = 1001,
            RoomTypeInfo = 1600,
        }

        #region Create Login Message
        public static CPacket CreateLoginRequest(string id, string pw)
        {
            byte messageSize = 36;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)ELogin.LoginRequest);
            packet.Write(id, 16).Write(pw, 20);

            return packet;
        }

        public static CPacket CreateRegisterRequest(string id, string pw)
        {
            byte messageSize = 36;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)ELogin.RegisterRequest);
            packet.Write(id, 16).Write(pw, 20);

            return packet;
        }

        public static CPacket CreateLobbyRequest()
        {
            byte messageSize = 0;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)ELogin.LobbyRequest);
            Debug.Log("Lobby Request");

            return packet;
        }
        #endregion

        #region Create Lobby Message
        public static CPacket CreateRoomCreateRequest(string id)
        {
            byte messageSize = 16;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)ELobby.RoomCreateRequest);
            packet.Write(id, 16);
            Debug.Log("Room Create Request");

            return packet;
        }

        public static CPacket CreateRoomJoinRequest(int rid, string id)
        {
            byte messageSize = 20;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)ELobby.RoomJoinRequest);
            packet.Write(rid).Write(id, 16);
            Debug.Log("Room Join Request");

            return packet;
        }

        public static CPacket CreateRoomListRequest()
        {
            byte messageSize = 0;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)ELobby.RoomListRequest);
            Debug.Log("Room List Request");

            return packet;
        }

        // 메세지 구성 확인 필요
        public static CPacket CreateRoomCountRequest(int sid)
        {
            byte messageSize = 4;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)ELobby.RoomCountRequest);
            Debug.Log("Room Count Request");

            return packet;
        }
        #endregion

        #region Create ReadyRoom Message
        public static CPacket CreateGameStartPacket(int rid)
        {
            byte messageSize = 4;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EReadyRoom.StartRequest);
            packet.Write(rid);
            Debug.Log("Game Start Request");

            return packet;
        }

        public static CPacket CreateGuestQuitPacket(int rid, int slot)
        {
            byte messageSize = 8;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EReadyRoom.GuestQuitRequest);
            packet.Write(rid).Write(slot);
            Debug.Log("Guest Quit Request");

            return packet;
        }

        public static CPacket CreateHostQuitPacket(int rid)
        {
            byte messageSize = 4;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EReadyRoom.HostQuitRequest);
            packet.Write(rid);
            Debug.Log("Host Quit Request");

            return packet;
        }

        public static CPacket CreateReturnLobbyPacket()
        {
            byte messageSize = 0;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EReadyRoom.LobbyReturnRequest);
            Debug.Log("Return Lobby Request");

            return packet;
        }

        // debug
        public static CPacket CreateBanRequest(int slotNum)
        {
            byte messageSize = 4;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EReadyRoom.BanRequest);
            packet.Write(slotNum);

            return packet;
        }

        // debug
        public static CPacket CreateAddGuestRequest(int slotNum)
        {
            byte messageSize = 4;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EReadyRoom.AddGuestRequest);
            packet.Write(slotNum);

            return packet;
        }
        #endregion

        #region Create InGame Message
        #region Character Movement
        public static CPacket CreateCharacterInfoPacket()
        {
            byte messageSize = 0;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EInGame.CharacterInfoRequest);

            return packet;
        }

        public static CPacket CreateMoveStartPacket(Vector3 now, Vector3 dest)
        {
            byte messageSize = 24;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EInGame.MoveStart);
            packet.Write(now.x).Write(now.y).Write(now.z).Write(dest.x).Write(dest.y).Write(dest.z);

            return packet;
        }

        public static CPacket CreateMoveStopPacket(Vector3 now)
        {
            byte messageSize = 12;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EInGame.MoveStop);
            packet.Write(now.x).Write(now.y).Write(now.z);

            return packet;
        }

        public static CPacket CreateActionStartPacket(int actionNumber, Vector3 now, Vector3 dest)
        {
            byte messageSize = 28;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EInGame.ActionStart);
            packet.Write(actionNumber).Write(now.x).Write(now.y).Write(now.z).Write(dest.x).Write(dest.y).Write(dest.z);

            return packet;
        }

        public static CPacket CreateJumpStartPacket(Vector3 now, float jumpRotate, bool isMoving)
        {
            byte messageSize = 20;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EInGame.JumpStart);
            packet.Write(now.x).Write(now.y).Write(now.z)
                .Write(jumpRotate)
                .Write(isMoving);

            return packet;
        }

        public static CPacket CreateAttackStartPacket(Vector3 now, float attackRotate)
        {
            byte messageSize = 16;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EInGame.AttackStart);
            packet.Write(now.x).Write(now.y).Write(now.z)
                .Write(attackRotate);

            return packet;
        }

        public static CPacket CreateRollStartPacket(Vector3 now, float rollRotate)
        {
            byte messageSize = 16;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EInGame.RollStart);
            packet.Write(now.x).Write(now.y).Write(now.z)
                .Write(rollRotate);

            return packet;
        }
        #endregion

        #region Map Info
        public static CPacket CreatePortalVote(int accept)
        {
            byte messageSize = 4;

            CPacket packet = new CPacket((int)messageSize);

            Debug.Log("Portal Vote");
            packet.WriteHeader(messageSize, (int)601);
            packet.Write(accept);

            return packet;
        }

        public static CPacket CreatePortalPopup()
        {
            byte messageSize = 0;

            Debug.Log("Portal Popup");

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)602);

            return packet;
        }

        public static CPacket CreateWaitEntering()
        {
            byte messageSize = 0;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EMapInfo.WaitEntering);

            return packet;
        }

        public static CPacket CreateEnterNextRoom(int enteringRoomType, int enteringRoomNumber, int[] nextRoomTypeInfos)
        {
            byte messageSize = 20;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EMapInfo.EnterNextRoom);
            packet.Write(enteringRoomType).Write(enteringRoomNumber)
                .Write(nextRoomTypeInfos[0]).Write(nextRoomTypeInfos[1]).Write(nextRoomTypeInfos[2]);

            return packet;
        }

        public static CPacket CreateRoomTypeInfo(int[,] rooms)
        {
            Debug.Log("Create Room Info Packet");

            byte messageSize = 144;

            CPacket packet = new CPacket((int)messageSize);

            //packet.WriteHeader(messageSize, (int)EMapInfo.RoomTypeInfo);
            for (int i = 0; i < CConstants.ROOM_PER_STAGE; i++)
            {
                for (int j = 0; j < CConstants.MAX_ROAD; j++)
                {
                    packet.Write(rooms[i, j]);
                }
            }

            return packet;
        }

        public static CPacket CreateRoomNumberInfo(int[,] roomNumbers)
        {
            Debug.Log("Create Room Number Infomations Packet");

            byte messageSize = 120;

            CPacket packet = new CPacket((int)messageSize);

            //packet.WriteHeader(messageSize, (int)EMapInfo.RoomNumberInfo);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    packet.Write(roomNumbers[i, j]);
                }
            }

            return packet;
        }
        #endregion


        public static CPacket CreateReturnLobby(bool isHost)
        {
            byte messageSize = 4;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EInGame.ReturnLobby);
            packet.Write(isHost);

            return packet;
        }

        public static CPacket CreateFinishLoading(bool isHost, int userCount)
        {
            byte gameUserCount = (byte)userCount;

            byte messageSize = 4;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EInGame.FinishLoding);
            packet.Write(isHost).Write(gameUserCount);

            return packet;
        }

        #endregion

        public static CPacket CreateShutdownPacket()
        {
            byte messageSize = 0;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, _cmdShutdownCode);

            return packet;
        }

        #region debug
        public static CPacket CreateChangePlayer(int playerNumber)
        {
            byte messageSize = 4;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EDebug.ChangePlayer);
            packet.Write(playerNumber);

            return packet;
        }

        public static CPacket CreateKickPlayer(int playerNumber)
        {
            byte messageSize = 4;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EDebug.KickPlayer);
            packet.Write(playerNumber);

            return packet;
        }

        public static CPacket CreateDebugRequsstRoomTypeInfo()
        {
            byte messageSize = 4;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EDebug.RoomTypeInfo);

            return packet;
        }
        #endregion
    }
}