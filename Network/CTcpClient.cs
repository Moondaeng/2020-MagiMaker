using System;
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
        }

        private void OnApplicationQuit()
        {
            SendShutdown();
            if(_isConnected) EndClient();
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

        public void EndClient()
        {
            // Release the socket
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
            _isConnected = false;
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
                
                logger.Log("Socket connected to {0}", client.RemoteEndPoint.ToString());
                _isConnected = true;

                _gameEvent.PlayerMoveStopEvent += SendMoveStop;

                // Signal that the connection has been made.  
                //connectDone.Set();

                Receive(_client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
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

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                // 패킷 해석 및 명령 수행
                CPacketInterpreter.PacketInterpret(state.buffer);

                // 다시 받을 준비
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        private static void Send(Socket client, byte[] data)
        {
            // Begin sending the data to the remote device.  
            client.BeginSend(data, 0, data.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
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
        }
    }
}