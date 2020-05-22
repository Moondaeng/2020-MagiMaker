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
            BanRequest = 305,
            AddGuestRequest = 306
        }

        enum EInGame
        {
            MoveStart,
            MoveStop
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
        public static CPacket CreateAddGuestRequest()
        {
            byte messageSize = 0;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, (int)EReadyRoom.AddGuestRequest);

            return packet;
        }
        #endregion

        #region Create InGame Message
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
        #endregion

        public static CPacket CreateShutdownPacket()
        {
            byte messageSize = 0;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, _cmdShutdownCode);

            return packet;
        }
    }
}
