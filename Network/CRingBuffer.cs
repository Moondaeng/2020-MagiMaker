using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public class CRingBuffer
    {
        public const int ringBufferDefaultSize = 10000;
        
        private int _front;
        private int _rear;
        private byte[] _buf;

        public CRingBuffer(int iBufferSize = ringBufferDefaultSize)
        {
            _front = 0;
            _rear = 0;
            _buf = new byte[iBufferSize];
        }

        public void Resize()
        {

        }

        // 버퍼 크기 구하기
        public int GetBufferSize()
        {
            return _buf.Length;
        }

        // 사용한 용량 구하기
        public int GetUseSize()
        {
            return (_rear - _front + _buf.Length + 1) % (_buf.Length + 1);
        }

        // 남은 용량 구하기
        public int GetFreeSize()
        {
            return _buf.Length - GetUseSize();
        }

        // 한 번에 쓸 수 있는 길이
        // 원형 큐 구조상 버퍼 끝단에 있는 데이터는 끝 -> 처음으로 돌아감
        // 두 번에 끊어져서 쓰는 경우는 제외
        public int DirectEnqueueSize()
        {
            if(_front > _rear)
            {
                return _front - _rear - 1;
            }
            else
            {
                // 이 부분 질문해볼 것
                if(_front == 0)
                {
                    return _buf.Length - _rear;
                }
                else
                {
                    return _buf.Length + 1 - _rear;
                }
            }
        }

        // 한 번에 읽을 수 있는 길이
        public int DirectDequeueSize()
        {
            if(_rear > _front)
            {
                return _rear - _front;
            }
            else
            {
                return _buf.Length + 1 - _front;
            }
        }

        // 버퍼에 삽입
        public int Enqueue(byte[] chpData, int iSize)
        {
            // 쓸 수 있는 양 확인
            int temp = GetFreeSize();
            if(temp < iSize)
            {
                iSize = temp;
            }

            // 버퍼에 쓰기
            temp = DirectEnqueueSize();
            // 한 번에 버퍼에 쓸 수 있는가?
            if (temp < iSize)
            {
                // 두 번에 걸쳐서 버퍼에 쓰기
                _Enqueue(chpData, 0, temp);
                _Enqueue(chpData, temp, iSize - temp);
            }
            else
            {
                _Enqueue(chpData, 0, iSize);
            }

            return iSize;
        }

        // 패킷에 해당하는 버퍼 삽입
        // 패킷의 구조를 더 생각해보고 개선할 예정
        public int Enqueue(CPacket packet)
        {
            // 쓸 수 있는 양 확인
            int temp = GetFreeSize();
            int size = packet.data.Length;
            if (temp < size)
            {
                size = temp;
            }

            if (0 >= size)
                return 0;

            temp = DirectEnqueueSize();
            // 한 번에 버퍼에 쓸 수 있는가?
            if (temp < size)
            {
                // 두 번에 걸쳐서 버퍼에 쓰기
                _Enqueue(packet.data, 0, temp);
                _Enqueue(packet.data, temp, size - temp);
            }
            else
            {
                _Enqueue(packet.data, 0, size);
            }

            return size;
        }

        // 버퍼에서 삭제
        public int Dequeue(byte[] chpData, int iSize)
        {
            int temp = GetUseSize();

            if(temp < iSize)
            {
                iSize = temp;
            }

            if (0 >= iSize)
                return 0;

            temp = DirectDequeueSize();
            if(temp < iSize)
            {
                _Dequeue(chpData, 0, temp);
                _Dequeue(chpData, temp, iSize - temp);
            }
            else
            {
                _Dequeue(chpData, 0, iSize);
            }

            return iSize;
        }

        // 패킷의 구조를 더 생각해보고 개선할 예정
        public int Dequeue(CPacket packet, int iSize)
        {
            int temp = GetUseSize();
            int freeSize = packet.data.Length;

            if (temp < iSize)
            {
                iSize = temp;
            }

            temp = DirectDequeueSize();
            if (temp < iSize)
            {
                _Dequeue(packet.data, 0, temp);
                _Dequeue(packet.data, temp, iSize - temp);
            }
            else
            {
                _Dequeue(packet.data, 0, iSize);
            }

            return iSize;
        }

        // 버퍼에 있는 내용을 data에 복제
        public int Peek(byte[] chpData, int iSize)
        {
            int temp = GetUseSize();

            if(temp < iSize)
            {
                iSize = temp;
            }

            if (0 >= iSize)
                return 0;

            temp = DirectDequeueSize();
            if(temp < iSize)
            {
                Buffer.BlockCopy(_buf, _front, chpData, 0, temp);
                Buffer.BlockCopy(_buf, 0, chpData, temp, iSize - temp);
            }
            else
            {
                Buffer.BlockCopy(_buf, _front, chpData, 0, temp);
            }

            return iSize;
        }

        private void Initial()
        {

        }

        private bool IsFull()
        {
            return (_rear + 1) % (_buf.Length + 1) == _front;
        } 

        private bool IsEmpty()
        {
            return _rear == _front;
        }

        // 버퍼에 쓰기(내부 함수)
        private void _Enqueue(byte[] data, int dataOffset, int enqueueSize)
        {
            Buffer.BlockCopy(data, dataOffset, _buf, _rear, enqueueSize);
            _rear += enqueueSize;
            _rear %= _buf.Length + 1;
        }

        // 버퍼에서 가져오기(내부 함수)
        private void _Dequeue(byte[] data, int dataOffset, int dequeueSize)
        {
            Buffer.BlockCopy(_buf, _front, data, dataOffset, dequeueSize);
            _front += dequeueSize;
            _rear %= _buf.Length + 1;
        }
    }
}
