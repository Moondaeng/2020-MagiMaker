using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
using UnityEngine;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3

namespace Network
{
    public static class CPacketFactory
    {
<<<<<<< HEAD
        private const int _cmdShutdownCode = 99;
        private const int _cmdMoveStartCode = 100;
        private const int _cmdMoveStopCode = 101;

        public static CPacket CreateMoveStartPacket(float now_x, float now_y, float dest_x, float dest_y)
=======
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
            CharacterInfoRequest = 400,
            MoveStart = 401,
            MoveStop = 402,
            ActionStart = 500
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
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        {
            byte messageSize = 16;

            CPacket packet = new CPacket((int)messageSize);

<<<<<<< HEAD
            packet.WriteHeader(messageSize, _cmdMoveStartCode);
            packet.Write(now_x).Write(now_y).Write(dest_x).Write(dest_y);
=======
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
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3

            return packet;
        }

<<<<<<< HEAD
        public static CPacket CreateMoveStopPacket(float now_x, float now_y)
=======
        public static CPacket CreateGuestQuitPacket(int rid, int slot)
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        {
            byte messageSize = 8;

            CPacket packet = new CPacket((int)messageSize);

<<<<<<< HEAD
            packet.WriteHeader(messageSize, _cmdMoveStopCode);
            packet.Write(now_x).Write(now_y);

            return packet;
        }
=======
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
        #endregion
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3

        public static CPacket CreateShutdownPacket()
        {
            byte messageSize = 0;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, _cmdShutdownCode);

            return packet;
        }
    }
}
