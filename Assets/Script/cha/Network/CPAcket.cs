using System;
using System.Collections.Generic;

namespace Network
{
    /*
     * 직렬화 버퍼 : 설명은 지영이가 만든 버퍼를 참고할 것
     * C#에 맞게 최대한 사용하기 편한 방식으로 변환
     * (operator를 의도대로 변경할 수 없으므로 Read(), Write()로 대체함)
     * Packet은 헤더(8byte)와 메세지(가변 크기)를 가짐
     */
    public class CPacket
    {
        private const byte _header_code = 0x86;
        private const int headerSize = 8;

        public const int payLoadSizePos = 1;
        public const int messageTypePos = 2;

        public byte[] data;
        private int _front;
        private int _rear;

        public CPacket(int messageSize)
        {
            data = new byte[headerSize + messageSize];
            _front = 0;
            _rear = 0;
        }

        public CPacket(byte[] packet)
        {
            data = packet;
            _front = 0;
            _rear = packet.Length;
        }

        // 패킷 파괴
        public void Release()
        {
            _front = 0;
            _rear = 0;
        }

        // 패킷 청소
        public void Clear()
        {

        }

        // 메세지 헤더 작성
        public void WriteHeader(byte payLoad_size, byte type)
        {
            byte temp = 0x00;
            Int32 checkSum = -1;
            Write(_header_code).Write(payLoad_size).Write(type).Write(temp).Write(checkSum);
            Console.WriteLine("make header");
        }

        // 메세지 헤더 읽기
        public void ReadHeader(out byte payloadSize, out byte messageType)
        {
            payloadSize = data[payLoadSizePos];
            messageType = data[messageTypePos];
            MoveReadPos(headerSize);
        }

        // 패킷 byte에 추가
        public CPacket Write(bool arg)
        {
            if (CanWrite(sizeof(bool)))
            {
                byte[] buf = System.BitConverter.GetBytes(arg);
                Buffer.BlockCopy(buf, 0, data, _rear, buf.Length);
                MoveWritePos(sizeof(bool));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Write Error");
            }

            return this;
        }

        public CPacket Write(byte arg)
        {
            if (CanWrite(sizeof(byte)))
            {
                data[_rear] = arg;
                MoveWritePos(sizeof(byte));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Write Error - Byte");
            }

            return this;
        }

        public CPacket Write(char arg)
        {
            if (CanWrite(sizeof(char)))
            {
                byte[] buf = System.BitConverter.GetBytes(arg);
                Buffer.BlockCopy(buf, 0, data, _rear, buf.Length);
                MoveWritePos(sizeof(char));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Write Error");
            }

            return this;
        }

        public CPacket Write(Int16 arg)
        {
            if (CanWrite(sizeof(Int16)))
            {
                byte[] buf = System.BitConverter.GetBytes(arg);
                Buffer.BlockCopy(buf, 0, data, _rear, buf.Length);
                MoveWritePos(sizeof(Int16));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Write Error");
            }

            return this;
        }

        public CPacket Write(Int32 arg)
        {
            if (CanWrite(sizeof(Int32)))
            {
                byte[] buf = System.BitConverter.GetBytes(arg);
                Buffer.BlockCopy(buf, 0, data, _rear, buf.Length);
                MoveWritePos(sizeof(Int32));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Write Error");
            }

            return this;
        }

        public CPacket Write(Int64 arg)
        {
            if (CanWrite(sizeof(Int64)))
            {
                byte[] buf = System.BitConverter.GetBytes(arg);
                Buffer.BlockCopy(buf, 0, data, _rear, buf.Length);
                MoveWritePos(sizeof(Int64));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Write Error");
            }

            return this;
        }

        public CPacket Write(Single arg)
        {
            if (CanWrite(sizeof(Single)))
            {
                byte[] buf = System.BitConverter.GetBytes(arg);
                Buffer.BlockCopy(buf, 0, data, _rear, buf.Length);
                MoveWritePos(sizeof(Single));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Write Error");
            }

            return this;
        }

        public CPacket Write(Double arg)
        {
            if (CanWrite(sizeof(Double)))
            {
                byte[] buf = System.BitConverter.GetBytes(arg);
                Buffer.BlockCopy(buf, 0, data, _rear, buf.Length);
                MoveWritePos(sizeof(Double));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Write Error");
            }

            return this;
        }

        public Boolean ReadBoolean()
        {
            Boolean ret = false;
            if (CanRead(sizeof(Boolean)))
            {
                ret = System.BitConverter.ToBoolean(data, _front);
                MoveReadPos(sizeof(Boolean));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Read Error");
            }
            return ret;
        }

        public Char ReadChar()
        {
            Char ret = '\0';
            if (CanRead(sizeof(Char)))
            {
                ret = System.BitConverter.ToChar(data, _front);
                MoveReadPos(sizeof(Char));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Read Error");
            }
            return ret;
        }

        public Int16 ReadInt16()
        {
            Int16 ret = 0;
            if (CanRead(sizeof(Int16)))
            {
                ret = System.BitConverter.ToInt16(data, _front);
                MoveReadPos(sizeof(Int16));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Read Error");
            }
            return ret;
        }

        public Int32 ReadInt32()
        {
            Int32 ret = 0;
            if (CanRead(sizeof(Int32)))
            {
                ret = System.BitConverter.ToInt32(data, _front);
                MoveReadPos(sizeof(Int32));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Read Error");
            }
            return ret;
        }

        public Int64 ReadInt64()
        {
            Int64 ret = 0;
            if (CanRead(sizeof(Int64)))
            {
                ret = System.BitConverter.ToInt64(data, _front);
                MoveReadPos(sizeof(Int64));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Read Error");
            }
            return ret;
        }

        public Single ReadSingle()
        {
            Single ret = 0;
            if (CanRead(sizeof(Single)))
            {
                ret = System.BitConverter.ToSingle(data, _front);
                MoveReadPos(sizeof(Single));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Read Error");
            }
            return ret;
        }

        public Double ReadDouble()
        {
            Double ret = 0;
            if (CanRead(sizeof(Double)))
            {
                ret = System.BitConverter.ToDouble(data, _front);
                MoveReadPos(sizeof(Double));
            }
            else
            {
                // 디버그 코드
                Console.WriteLine("Packet Read Error");
            }
            return ret;
        }

        // 패킷 byte[] 특정 위치에 arg를 박음
        public void Insert(Boolean arg, int offset)
        {
            byte[] buf = System.BitConverter.GetBytes(arg);
            Buffer.BlockCopy(buf, 0, data, offset, buf.Length);
        }

        public void Insert(Char arg, int offset)
        {
            byte[] buf = System.BitConverter.GetBytes(arg);
            Buffer.BlockCopy(buf, 0, data, offset, buf.Length);
        }

        public void Insert(Int16 arg, int offset)
        {
            byte[] buf = System.BitConverter.GetBytes(arg);
            Buffer.BlockCopy(buf, 0, data, offset, buf.Length);
        }

        public void Insert(Int32 arg, int offset)
        {
            byte[] buf = System.BitConverter.GetBytes(arg);
            Buffer.BlockCopy(buf, 0, data, offset, buf.Length);
        }

        public void Insert(Int64 arg, int offset)
        {
            byte[] buf = System.BitConverter.GetBytes(arg);
            Buffer.BlockCopy(buf, 0, data, offset, buf.Length);
        }

        public void Insert(Single arg, int offset)
        {
            byte[] buf = System.BitConverter.GetBytes(arg);
            Buffer.BlockCopy(buf, 0, data, offset, buf.Length);
        }

        public void Insert(Double arg, int offset)
        {
            byte[] buf = System.BitConverter.GetBytes(arg);
            Buffer.BlockCopy(buf, 0, data, offset, buf.Length);
        }

        // Mashaling으로 만든 byte[] 배열을 직접 넣을 때
        public void Insert(byte[] arg, int offset)
        {
            Buffer.BlockCopy(arg, 0, data, offset, arg.Length);
        }

        public Boolean GetBoolean(int offset)
        {
            return System.BitConverter.ToBoolean(data, offset);
        }

        public Char GetChar(int offset)
        {
            return System.BitConverter.ToChar(data, offset);
        }

        public Int16 GetInt16(int offset)
        {
            return System.BitConverter.ToInt16(data, offset);
        }

        public Int32 GetInt32(int offset)
        {
            return System.BitConverter.ToInt32(data, offset);
        }

        public Int64 GetInt64(int offset)
        {
            return System.BitConverter.ToInt64(data, offset);
        }

        public Single GetFloat(int offset)
        {
            return System.BitConverter.ToSingle(data, offset);
        }

        public Double GetDouble(int offset)
        {
            return System.BitConverter.ToDouble(data, offset);
        }

        // 온전히 쓸 수 있는지 판단
        private bool CanWrite(int iSize)
        {
            return _rear + iSize <= data.Length;
        }

        // 데이터를 넣으면 _rear 이동
        // CanWrite()로 검증된 상태에서만 사용할 것
        private void MoveWritePos(int iSize)
        {
            _rear += iSize;
        }

        // 온전히 읽을 수 있는지 판단
        private bool CanRead(int iSize)
        {
            return _front + iSize <= _rear;
        }

        // 데이터를 넣으면 _rear 이동
        // CanWrite()로 검증된 상태에서만 사용할 것
        private void MoveReadPos(int iSize)
        {
            _front += iSize;
        }
    }
}
