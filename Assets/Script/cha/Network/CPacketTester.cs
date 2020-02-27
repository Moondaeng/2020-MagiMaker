using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    public class CPacketTester
    {
        public static void TestMoveStartCmdPacket(CPacket packet)
        {
            Console.WriteLine("Test - Move Start Command");

            byte messageSize = packet.data[CPacket.payLoadSizePos];
            byte messageType = packet.data[CPacket.messageTypePos];
            Console.WriteLine("header : {0}, {1}", (int)messageSize, (int)messageType);

            float now_x = packet.GetFloat(8);
            float now_y = packet.GetFloat(12);
            float dest_x = packet.GetFloat(16);
            float dest_y = packet.GetFloat(20);
            Console.WriteLine("message : {0}, {1}, {2}, {3}", now_x, now_y, dest_x, dest_y);
        }

        public static void PackingFloatTester()
        {
            Console.WriteLine("Packing Float Test");
            Single test = 1.23f;
            CPacket packet = new CPacket(8);
            packet.Insert(test, 0);
            var get = packet.GetFloat(0);
            Console.WriteLine(test == get ? "Equal" : "Not Equal");
            Console.WriteLine("get = {0}", get);
        }
    }
}
