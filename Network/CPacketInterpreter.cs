using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    /*
     * 인게임에서의 패킷 변환을 담당하는 클래스
     * 네트워크에 연결된 경우 이벤트가 
     * 해석된 내용은 Commander로 실행시킨다
     */
    public static class CPacketInterpreter
    {
        private delegate void PacketInterpretImpl(CPacket packet);

        private static Dictionary<int, PacketInterpretImpl> _packetInterpretDict = new Dictionary<int, PacketInterpretImpl>()
        {
            // 캐릭터 움직임 관련
            [450] = InterpretSetCharacter,
            [451] = InterpretMoveStart,
            [452] = InterpretMoveStop,
            [453] = InterpretActionCommand,
            // 포탈, 맵
            [651] = InterpretPortalAccept,
            [652] = InterpretUsePortal,
            [653] = InterpretCreateRooms,
            // 시스템
            [951] = InterpretQuitGame,
            [952] = InterpretQuitGame,
        };

        public static void PacketInterpret(byte[] data)
        {
            // 헤더 읽기
            CPacket packet = new CPacket(data);
            packet.ReadHeader(out byte payloadSize, out short messageType);
            Debug.Log($"Header : payloadSize = {payloadSize}, messageType = {messageType}");

            if (_packetInterpretDict.TryGetValue((int)messageType, out var interpretFunc))
            {
                interpretFunc(packet);
            }
            else
            {
                Debug.Log($"Unknown Message Type : {messageType}");
            }
        }

        #region Send Message
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

            var message = CPacketFactory.CreateRoomsInfo(roomsIntArr);

            CTcpClient.instance.Send(message.data);
        }
        #endregion

        #region Interpret Packet
        private static void InterpretSetCharacter(CPacket packet)
        {
            Int32 MyId = packet.ReadInt32();

            Debug.Log($"Set Character : my id - {MyId}");

            CPlayerCommand.instance.SetMyCharacter((int)MyId);
        }

        private static void InterpretMoveStart(CPacket packet)
        {
            Int32 id;
            Vector3 now, dest;
            
            id = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();
            dest.x = packet.ReadSingle();
            dest.y = packet.ReadSingle();
            dest.z = packet.ReadSingle();

            Debug.LogFormat("Move Start - id{0} move ({1},{2},{3}) to ({4},{5},{6})", 
                id, now.x, now.y, now.z, dest.x, dest.y, dest.z);

            CPlayerCommand.instance.Move(id, dest);
        }

        private static void InterpretMoveStop(CPacket packet)
        {
            Int32 id;
            Vector3 now;

            id = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();

            Debug.LogFormat("Move Stop - id{0} ({1},{2})", id, now.x, now.y, now.z);
        }

        private static void InterpretMoveCorrection(CPacket packet)
        {
            Int32 id;
            Vector3 now, dest;

            id = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();
            dest.x = packet.ReadSingle();
            dest.y = packet.ReadSingle();
            dest.z = packet.ReadSingle();

            Debug.LogFormat("Move Correction - id{0} move ({1},{2},{3}) to ({4},{5},{6})",
                id, now.x, now.y, now.z, dest.x, dest.y, dest.z);
        }

        private static void InterpretActionCommand(CPacket packet)
        {
            const int ATTACK = 0;
            const int JUMP = 1;
            const int ROLL = 2;
            const int USE_SKILL = 3;

            Int32 id;
            Int32 actionNumber;
            Vector3 now, dest;

            Debug.Log("action Command");

            id = packet.ReadInt32();
            actionNumber = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();
            dest.x = packet.ReadSingle();
            dest.y = packet.ReadSingle();
            dest.z = packet.ReadSingle();

            Debug.LogFormat("Action Start - id{0} actionNumber{1} move ({2},{3},{4}) to ({5},{6},{7})",
                id, actionNumber, now.x, now.y, now.z, dest.x, dest.y, dest.z);

            if (actionNumber == ATTACK)
            {
                CPlayerCommand.instance.Attack(id, now, dest);
            }
            else if (actionNumber == JUMP)
            {
                CPlayerCommand.instance.Jump(id, now, dest);
            }
            else if (actionNumber == ROLL)
            {
                CPlayerCommand.instance.Roll(id, now, dest);
            }
            else if (actionNumber >= USE_SKILL && actionNumber < USE_SKILL + 42)
            {
                Debug.Log($"player {id} use skill {actionNumber - USE_SKILL}");
                CPlayerCommand.instance.UseSkill((int)id, (int)actionNumber - USE_SKILL, now, dest);
            }
        }

        private static void InterpretGetItem(CPacket packet)
        {
            Int32 id;

            Debug.Log("Get Item");

            id = packet.ReadInt32();

            GameObject.Destroy(GameObject.FindGameObjectWithTag("ITEM"));

            //playerCommander.UseSkill((int)id, (int)actionNumber, now, dest);
        }

        private static void InterpretUsePortal(CPacket packet)
        {
            Int32 id;

            Debug.Log("Use Portal");

            id = packet.ReadInt32();

            CWaitingForAccept.instance.SetActivePortalPopup(true);
            //playerCommander.UseSkill((int)id, (int)actionNumber, now, dest);
        }

        private static void InterpretPortalAccept(CPacket packet)
        {
            Int32 id;
            Int32 accept;

            Debug.Log("Get Item");

            id = packet.ReadInt32();
            accept = packet.ReadInt32();

            if (accept == 0)
            {
                CWaitingForAccept.instance.SetPortalUseSelect(id, CWaitingForAccept.EAccept._accept);
            }
            else if (accept == 1)
            {
                CWaitingForAccept.instance.SetPortalUseSelect(id, CWaitingForAccept.EAccept._cancle);
            }
        }

        private static void InterpretPortalTeleport(CPacket packet)
        {
            Int32 id;

            Debug.Log("Get Item");

            id = packet.ReadInt32();
            
        }

        private static void InterpretCreateRooms(CPacket packet)
        {
            CRoom[,] rooms = new CRoom[CConstants.ROOM_PER_STAGE, CConstants.MAX_ROAD];
            for (int i = 0; i < CConstants.ROOM_PER_STAGE; i++)
            {
                for (int j = 0; j < CConstants.MAX_ROAD; j++)
                {
                    rooms[i, j].RoomType = (CGlobal.ERoomType)packet.ReadInt32();
                }
            }

            CCreateMap.instance.ReceiveRoomArr(rooms);
        }

        private static void InterpretReturnLobby(CPacket packet)
        {
            CTcpClient.instance.DeletePacketInterpret();
            SceneManager.LoadScene("Lobby");
        }

        private static void InterpretQuitGame(CPacket packet)
        {
            CGameEvent.instance.QuitPlayer(packet.ReadInt32());
        }
        #endregion
    }
}
