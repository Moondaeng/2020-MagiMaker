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
            [452] = InterpretMoveCorrection,
            [453] = InterpretSkillCommand,
            [454] = InterpretJumpCommand,
            [455] = InterpretAttackCommand,
            [456] = InterpretRollCommand,
            // 포탈, 맵
            [651] = InterpretPortalAccept,
            [652] = InterpretUsePortal,
            [653] = InterpretRoomTypeInfos,
            [654] = InterpretRoomNumberInfos,
            [655] = InterpretEnterNextRoom,
            // 시스템
            [951] = InterpretReturnLobby,
            [952] = InterpretQuitGame,
            [953] = InterpretLoadingAllFinish,
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

        #region Interpret Packet
        #region Character Movement
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

            //Debug.LogFormat("Move Start - id{0} move ({1},{2},{3}) to ({4},{5},{6})", id, now.x, now.y, now.z, dest.x, dest.y, dest.z);

            CPlayerCommand.instance.Move(id, dest);
        }

        private static void InterpretMoveCorrection(CPacket packet)
        {
            Int32 id;
            Vector3 now;

            id = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();

            Debug.LogFormat("Move Stop - id{0} ({1},{2})", id, now.x, now.y, now.z);

            CPlayerCommand.instance.Teleport(id, now);
        }

        private static void InterpretSkillCommand(CPacket packet)
        {
            Int32 id;
            Int32 skillNumber;
            Vector3 now, dest;

            Debug.Log("action Command");

            id = packet.ReadInt32();
            skillNumber = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();
            dest.x = packet.ReadSingle();
            dest.y = packet.ReadSingle();
            dest.z = packet.ReadSingle();

            Debug.Log($"Use Skill - id{id} use skill number {skillNumber} in pos ({now.x},{now.y},{now.z}) to ({dest.x},{dest.y},{dest.z})");

            CPlayerCommand.instance.UseSkill((int)id, skillNumber, now, dest);
        }

        private static void InterpretJumpCommand(CPacket packet)
        {
            Debug.Log("Jump Command");

            Int32 id;
            Vector3 now;
            float rotateY;
            bool isMoving;

            id = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();
            rotateY = packet.ReadSingle();
            isMoving = packet.ReadBoolean();

            Debug.Log($"Jump Start - id{id} ({now.x},{now.y},{now.z}) with rotate ({rotateY})");

            CPlayerCommand.instance.Jump(id, now, rotateY);
        }

        private static void InterpretAttackCommand(CPacket packet)
        {
            Debug.Log("Attack Command");

            Int32 id;
            Vector3 now;
            float rotateY;

            id = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();
            rotateY = packet.ReadSingle();

            Debug.Log($"Attack Start - id{id} ({now.x},{now.y},{now.z}) with rotate ({rotateY})");

            CPlayerCommand.instance.Attack(id, now, rotateY);
        }

        private static void InterpretRollCommand(CPacket packet)
        {
            Debug.Log("Roll Command");

            Int32 id;
            Vector3 now;
            float rotateY;

            id = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();
            rotateY = packet.ReadSingle();

            Debug.Log($"Roll Start - id{id} ({now.x},{now.y},{now.z}) with rotate ({rotateY})");

            CPlayerCommand.instance.Roll(id, now, rotateY);
        }
        #endregion

        #region MapInfo
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

            Debug.Log("Portal Accept");

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

        private static void InterpretEnterNextRoom(CPacket packet)
        {
            Debug.Log("Enter Next Room");

            CPortalManager.instance.MoveToNextRoom();
        }

        private static void InterpretRoomTypeInfos(CPacket packet)
        {
            Debug.Log("Receive Room Type Infomation Packets");

            int[,] rooms = new int[CConstants.ROOM_PER_STAGE, CConstants.MAX_ROAD];

            for (int i = 0; i < CConstants.ROOM_PER_STAGE; i++)
            {
                string roomData = $"rooms type row {i} : ";
                for (int j = 0; j < CConstants.MAX_ROAD; j++)
                {
                    rooms[i, j] = packet.ReadInt32();
                    roomData += rooms[i, j];
                }
                Debug.Log(roomData);
            }

            CCreateMap.instance.ReceiveRoomArr(rooms);
        }

        private static void InterpretRoomNumberInfos(CPacket packet)
        {
            Debug.Log("Receive Room Number Infomation Packets");

            int[,] rooms = new int[3, 10];

            for (int i = 0; i < 3; i++)
            {
                string roomData = $"rooms number row {i} : ";
                for (int j = 0; j < 10; j++)
                {
                    rooms[i, j] = packet.ReadInt32();
                    roomData += rooms[i, j];
                }
                Debug.Log(roomData);
            }

            // Stage Number 추가 필요
            CCreateMap.instance.NonHostRoomEnqueue(rooms, 0);
        }
        #endregion


        #region System
        private static void InterpretReturnLobby(CPacket packet)
        {
            CTcpClient.instance.DeletePacketInterpret();
            SceneManager.LoadScene("Lobby");
        }

        private static void InterpretQuitGame(CPacket packet)
        {
            CNetworkEvent.instance.QuitPlayer(packet.ReadInt32());
        }

        private static void InterpretLoadingAllFinish(CPacket packet)
        {
            Debug.Log("All player Loading finished");
            CWaitingLoadViewer.Instance.FinishLoading();
            if (CClientInfo.JoinRoom.IsHost)
            {
                CCreateMap.instance.CreateStage();

                var sendPacket = CPacketFactory.CreateRoomNumberInfo(CCreateMap.instance.randomRoomArray);
                CTcpClient.instance.Send(sendPacket.data);
            }
        }
        #endregion
        #endregion

        #region Obsolete
        [Obsolete]
        private static void InterpretGetItem(CPacket packet)
        {
            Int32 id;

            Debug.Log("Get Item");

            id = packet.ReadInt32();

            GameObject.Destroy(GameObject.FindGameObjectWithTag("ITEM"));

            //playerCommander.UseSkill((int)id, (int)actionNumber, now, dest);
        }

        [Obsolete]
        private static void InterpretPortalTeleport(CPacket packet)
        {
            Int32 id;

            Debug.Log("Get Item");

            id = packet.ReadInt32();

        }
        #endregion
    }
}
