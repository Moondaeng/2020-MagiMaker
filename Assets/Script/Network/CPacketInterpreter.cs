using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Network
{
    // 패킷의 내용을 해석하는 클래스
    // 해석된 내용은 Commander로 실행시킨다
    public static class CPacketInterpreter
    {
        private const int _moveStart            = 110;
        private const int _moveStop             = 111;
        private const int _moveCorrection       = 112;
        private const int _characterCreateMy    = 210;
        private const int _characterCreateOther = 211;
        private const int _characterDelete      = 212;

        private static CLogComponent logger = new CLogComponent(ELogType.Network);

        public static void PacketInterpret(byte[] data)
        {
            // 헤더 읽기
            byte payloadSize, messageType;
            CPacket packet = new CPacket(data);
            packet.ReadHeader(out payloadSize, out messageType);
            logger.Log("Header : payloadSize = {0}, messageType = {1}", payloadSize, messageType);

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

        private static void InterpretMoveStart(CPacket packet)
        {
            Int32 id;
            float nX, nY, dX, dY;

            id = packet.ReadInt32();
            nX = packet.ReadSingle();
            nY = packet.ReadSingle();
            dX = packet.ReadSingle();
            dY = packet.ReadSingle();

            logger.Log("Move Start - id{0} move ({1},{2}) to ({3},{4})", id, nX, nY, dX, dY);

            //Commander
        }

        private static void InterpretMoveStop(CPacket packet)
        {
            Int32 id;
            float nX, nY;

            id = packet.ReadInt32();
            nX = packet.ReadSingle();
            nY = packet.ReadSingle();

            logger.Log("Move Stop - id{0} ({1},{2})", id, nX, nY);
        }

        private static void InterpretMoveCorrection(CPacket packet)
        {
            Int32 id;
            float nX, nY;

            id = packet.ReadInt32();
            nX = packet.ReadSingle();
            nY = packet.ReadSingle();

            logger.Log("Move Stop - id{0} ({1},{2})", id, nX, nY);
        }
    }
}
