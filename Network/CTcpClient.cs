using System;
<<<<<<< HEAD
=======
using System.Collections.Concurrent;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Network
{
    // State object for receiving data from remote device.  
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
    }

    public sealed class CTcpClient : MonoBehaviour
    {
<<<<<<< HEAD
        private static CLogComponent logger;
        private static CGameEvent _gameEvent;

        // The port number for the remote device.  
        public Int32 port = 9000;
        public string ipString = "127.0.0.1";
        
        private Socket _client;
        private bool _isConnected = false;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone = new ManualResetEvent(false);

        private void Start()
        {
            logger = new CLogComponent(ELogType.Network);
            _gameEvent = GameObject.Find("GameEvent").GetComponent<CGameEvent>();
            StartClient();
=======
        private static CLogComponent _logger;
        private static CGameEvent _gameEvent;

        public const int Shutdown = 910;

        public delegate void PacketInterpret(byte[] buffer);

        // The port number for the remote device.  
        public Int32 port = 9000;
        public string ipString = "127.0.0.1";
        public ConcurrentQueue<byte[]> tcpBuffer = new ConcurrentQueue<byte[]>();
        public bool IsConnect { get; private set; } = false;
        public int RoomID = -1;

        private Socket _client;
        private PacketInterpret _interpretFunc = null;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _logger = new CLogComponent(ELogType.Network);
        }

        private void Update()
        {
            if(IsConnect && !tcpBuffer.IsEmpty)
            {
                tcpBuffer.TryDequeue(out byte[] data);

                if(CheckShutdown(data))
                {
                    EndClient();
                }
                else
                {
                    _interpretFunc?.Invoke(data);
                }
            }
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        }

        private void OnApplicationQuit()
        {
            SendShutdown();
<<<<<<< HEAD
            if(_isConnected) EndClient();
=======
            if(IsConnect) EndClient();
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        }

        public void StartClient()
        {
            // Connect to a remote device.  
            try
            {
                IPAddress ipAddress = IPAddress.Parse(ipString);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                _client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                _client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), _client);

                // 연결 완료까지 기다림
                //connectDone.WaitOne();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

<<<<<<< HEAD
        public void EndClient()
        {
            // Release the socket
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
            _isConnected = false;
=======
        // 실제로 끄는 동작
        public void EndClient()
        {
            Debug.Log("Shutdown");
            // Release the socket
            IsConnect = false;
            DeletePacketInterpret();
            while (tcpBuffer.TryDequeue(out byte[] data)) ;
            Debug.Log("Clean");
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
            _client.Dispose();
            _client = null;
        }

        public void Send(Socket client, byte[] data)
        {
            // Begin sending the data to the remote device.  
            try
            {
                //client.BeginSend(data, 0, data.Length, 0, null, client);
                client.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), client);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public void SetPacketInterpret(PacketInterpret func)
        {
            _interpretFunc = func;
        }

        public void DeletePacketInterpret()
        {
            _interpretFunc = null;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        }

        // 연결 시 수행할 내용
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);
                
<<<<<<< HEAD
                logger.Log("Socket connected to {0}", client.RemoteEndPoint.ToString());
                _isConnected = true;

                _gameEvent.PlayerMoveStopEvent += SendMoveStop;

                // Signal that the connection has been made.  
                //connectDone.Set();

                Receive(_client);
=======
                Debug.LogFormat("Socket connected to {0}", client.RemoteEndPoint.ToString());
                IsConnect = true;

                Receive(client);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

<<<<<<< HEAD
        private static void Receive(Socket client)
=======
        private void Receive(Socket client)
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

<<<<<<< HEAD
        private static void ReceiveCallback(IAsyncResult ar)
=======
        private void ReceiveCallback(IAsyncResult ar)
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

<<<<<<< HEAD
                // 패킷 해석 및 명령 수행
                CPacketInterpreter.PacketInterpret(state.buffer);

                // 다시 받을 준비
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
=======
                byte[] data = (byte[])state.buffer.Clone();
                // (확인 중) 엔디안 변환s
                //Array.Reverse(data);

                // 수신 큐에 넣기
                tcpBuffer.Enqueue(data);

                // 다시 받을 준비
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

<<<<<<< HEAD
        private static void Send(Socket client, byte[] data)
        {
            // Begin sending the data to the remote device.  
            client.BeginSend(data, 0, data.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
=======
        private void SendCallback(IAsyncResult ar)
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Debug.LogFormat("Sent {0} bytes to server.", bytesSent);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

<<<<<<< HEAD
        // 메세지 보내기
        public bool SendMoveStart(float nX, float nY, float dX, float dY)
        {
            if (!_isConnected) return false;

            Console.WriteLine("Send Move Start : {0} {1} {2} {3}", nX, nY, dX, dY);
            var packet = Network.CPacketFactory.CreateMoveStartPacket(nX, nY, dX, dY);
            Send(_client, packet.data);
            return true;
        }

        public bool SendMoveStop(float nX, float nY)
        {
            if (!_isConnected) return false;

            Console.WriteLine("Send Move Start : {0} {1}", nX, nY);
            var packet = Network.CPacketFactory.CreateMoveStopPacket(nX, nY);
            Send(_client, packet.data);
            return true;
        }

        private void SendMoveStop(object sender, Tuple<float, float> data)
        {
            Console.WriteLine("Send Move Start : {0} {1}", data.Item1, data.Item2);
            var packet = Network.CPacketFactory.CreateMoveStopPacket(data.Item1, data.Item2);
            Send(_client, packet.data);
        }

        public bool SendShutdown()
        {
            if (!_isConnected) return false;

            Console.WriteLine("Send Shutdown Message");
            var packet = Network.CPacketFactory.CreateShutdownPacket();
            Send(_client, packet.data);
            return true;
=======
        private bool CheckShutdown(byte[] data)
        {
            const int messageTypePos = 2;

            var messageCode = System.BitConverter.ToInt16(data, messageTypePos);

            if (messageCode == Shutdown)
                return true;

            return false;
        }

        public void Send(byte[] data)
        {
            Send(_client, data);
        }

        // 서버에 종료 요청
        public void SendShutdown()
        {
            if (!IsConnect) return;

            Debug.Log("Send Shutdown Message");
            var packet = Network.CPacketFactory.CreateShutdownPacket();
            Send(_client, packet.data);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        }
    }
}