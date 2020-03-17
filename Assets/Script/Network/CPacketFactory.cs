using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    public static class CPacketFactory
    {
        private const int _cmdShutdownCode = 99;
        private const int _cmdMoveStartCode = 100;
        private const int _cmdMoveStopCode = 101;

        public static CPacket CreateMoveStartPacket(float now_x, float now_y, float dest_x, float dest_y)
        {
            byte messageSize = 16;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, _cmdMoveStartCode);
            packet.Write(now_x).Write(now_y).Write(dest_x).Write(dest_y);

            return packet;
        }

        public static CPacket CreateMoveStopPacket(float now_x, float now_y)
        {
            byte messageSize = 8;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, _cmdMoveStopCode);
            packet.Write(now_x).Write(now_y);

            return packet;
        }

        public static CPacket CreateShutdownPacket()
        {
            byte messageSize = 0;

            CPacket packet = new CPacket((int)messageSize);

            packet.WriteHeader(messageSize, _cmdShutdownCode);

            return packet;
        }
    }
}
