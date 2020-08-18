using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Network
{
    /*
     * 인게임에서의 패킷 변환을 담당하는 클래스
     * 네트워크에 연결된 경우 이벤트가 
     * 해석된 내용은 Commander로 실행시킨다
     */
    public class CPacketInterpreter
    {
        private const int _setCharacterInfo = 410;
        private const int _moveStart = 411;
        private const int _moveStop = 412;
        private const int _moveCorrection = 413;
        private const int _actionCommand = 510;
        private const int _GetItem = 610;
        private const int _UsePortal = 611;
        private const int _PortalAccept = 612;
        private const int _PortalTeleport = 613;
        private const int _UsePortalPopup = 614;

        private static CLogComponent _logger = new CLogComponent(ELogType.Network);
        private Network.CTcpClient _tcpManager;
        private CPlayerCommand playerCommander;

        public CPacketInterpreter(Network.CTcpClient tcpManger)
        {
            _tcpManager = tcpManger;
            playerCommander = GameObject.Find("GameManager").GetComponent<CPlayerCommand>();
        }

        public void PacketInterpret(byte[] data)
        {
            // 헤더 읽기
            CPacket packet = new CPacket(data);
            packet.ReadHeader(out byte payloadSize, out short messageType);
            _logger.Log("Header : payloadSize = {0}, messageType = {1}", payloadSize, messageType);

            switch((int)messageType)
            {
                case _setCharacterInfo:
                    InterpretSetCharacter(packet);
                    break;
                case _moveStart:
                    InterpretMoveStart(packet);
                    break;
                case _moveStop:
                    InterpretMoveStop(packet);
                    break;
                case _moveCorrection:
                    InterpretMoveCorrection(packet);
                    break;
                case _actionCommand:
                    InterpretActionCommand(packet);
                    break;
            }
        }

        #region Send Message
        public void SendCharacterInfoRequest()
        {
            var message = CPacketFactory.CreateCharacterInfoPacket();

            _tcpManager.Send(message.data);
        }

        public void SendMoveStart(Vector3 now, Vector3 dest)
        {
            var message = CPacketFactory.CreateMoveStartPacket(now, dest);

            _tcpManager.Send(message.data);
        }

        public void SendMoveStop(Vector3 now)
        {
            var message = CPacketFactory.CreateMoveStopPacket(now);

            _tcpManager.Send(message.data);
        }
        public void SendActionStart(int actionNumber, Vector3 now, Vector3 dest)
        {
            var message = CPacketFactory.CreateActionStartPacket(actionNumber, now, dest);

            _tcpManager.Send(message.data);
        }
        #endregion

        #region Interpret Packet
        private void InterpretSetCharacter(CPacket packet)
        {
            Int32 MyId = packet.ReadInt32();

            _logger.Log($"Set Character : my id - {MyId}");

            //Commander
            playerCommander.SetMyCharacter((int)MyId);
        }

        private void InterpretMoveStart(CPacket packet)
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

            _logger.Log("Move Start - id{0} move ({1},{2},{3}) to ({4},{5},{6})", 
                id, now.x, now.y, now.z, dest.x, dest.y, dest.z);

            //Commander
            playerCommander.Move(id, dest);
        }

        private void InterpretMoveStop(CPacket packet)
        {
            Int32 id;
            Vector3 now;

            id = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();

            _logger.Log("Move Stop - id{0} ({1},{2})", id, now.x, now.y, now.z);
        }

        private void InterpretMoveCorrection(CPacket packet)
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

            _logger.Log("Move Correction - id{0} move ({1},{2},{3}) to ({4},{5},{6})",
                id, now.x, now.y, now.z, dest.x, dest.y, dest.z);
        }

        private void InterpretActionCommand(CPacket packet)
        {

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

            _logger.Log("Action Start - id{0} actionNumber{1} move ({2},{3},{4}) to ({5},{6},{7})",
                id, actionNumber, now.x, now.y, now.z, dest.x, dest.y, dest.z);

            playerCommander.UseSkill((int)id, (int)actionNumber, now, dest);
        }

        private void InterpretGetItem(CPacket packet)
        {
            Int32 id;

            Debug.Log("Get Item");

            id = packet.ReadInt32();

            GameObject.Destroy(GameObject.FindGameObjectWithTag("ITEM"));

            //playerCommander.UseSkill((int)id, (int)actionNumber, now, dest);
        }

        private void InterpretUsePortal(CPacket packet)
        {
            Int32 id;

            Debug.Log("Use Portal");

            id = packet.ReadInt32();

            //playerCommander.UseSkill((int)id, (int)actionNumber, now, dest);
        }

        private void InterpretPortalAccept(CPacket packet)
        {
            Int32 id;

            Debug.Log("Get Item");

            id = packet.ReadInt32();
            
        }

        private void InterpretPortalTeleport(CPacket packet)
        {
            Int32 id;

            Debug.Log("Get Item");

            id = packet.ReadInt32();
            
        }
        #endregion
    }
}
