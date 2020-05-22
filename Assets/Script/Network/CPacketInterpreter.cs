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
    public static class CPacketInterpreter
    {
        private const int _characterCreateMy    = 110;
        private const int _characterCreateOther = 111;
        private const int _characterDelete      = 112;
        private const int _moveStart = 210;
        private const int _moveStop = 211;
        private const int _moveCorrection = 212;

        private static CLogComponent _logger = new CLogComponent(ELogType.Network);

        public static void PacketInterpret(byte[] data)
        {
            // 헤더 읽기
            CPacket packet = new CPacket(data);
            packet.ReadHeader(out byte payloadSize, out short messageType);
            _logger.Log("Header : payloadSize = {0}, messageType = {1}", payloadSize, messageType);

            switch((int)messageType)
            {
                case _moveStart:
                    InterpretMoveStart(packet);
                    break;
                case _moveStop:
                    InterpretMoveStop(packet);
                    break;
                case _moveCorrection:
                    InterpretMoveCorrection(packet);
                    break;
            }
        }

        public static void Send()
        {

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

            _logger.Log("Move Start - id{0} move ({1},{2},{3}) to ({4},{5},{6})", 
                id, now.x, now.y, now.z, dest.x, dest.y, dest.z);

            //Commander

        }

        private static void InterpretMoveStop(CPacket packet)
        {
            Int32 id;
            float nX, nY;
            Vector3 now;

            id = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();

            _logger.Log("Move Stop - id{0} ({1},{2})", id, now.x, now.y, now.z);
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

            _logger.Log("Move Correction - id{0} move ({1},{2},{3}) to ({4},{5},{6})",
                id, now.x, now.y, now.z, dest.x, dest.y, dest.z);
        }
    }
}
