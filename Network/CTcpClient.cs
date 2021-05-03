﻿using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
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
        public const int Shutdown = 950;
        public static readonly int CONNECT_END = 954;

        public delegate void PacketInterpret(byte[] buffer);

        public static CTcpClient instance;

        // The port number for the remote device.  
        public Int32 port = 95200;
        public string ipString = "127.0.0.1";
        public ConcurrentQueue<byte[]> tcpBuffer = new ConcurrentQueue<byte[]>();
        public bool IsConnect
        {
            get
            {
                if (_client == null)
                {
                    return false;
                }
                return _client.Connected;
            }
        }


        private static Socket _client;
        private PacketInterpret _interpretFunc = null;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (IsConnect && !tcpBuffer.IsEmpty)
            {
                tcpBuffer.TryDequeue(out byte[] data);

                if (CheckShutdown(data))
                {
                    EndClient();
                }
                else
                {
                    _interpretFunc?.Invoke(data);
                }
            }
        }

        private void OnApplicationQuit()
        {
            SendShutdown();
            if (IsConnect) EndClient();
        }

        public void StartClient()
        {
            // Connect to a remote device.  
            try
            {
                //_client.set
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

        // 실제로 끄는 동작
        public void EndClient()
        {
            Debug.Log("Shutdown");
            // Release the socket
            DeletePacketInterpret();
            while (tcpBuffer.TryDequeue(out byte[] data)) ;
            Debug.Log("Clean");
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
            _client.Dispose();
            _client = null;
        }

        public void Send(byte[] data)
        {
            // Begin sending the data to the remote device.  
            try
            {
                _client.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), _client);
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

                Debug.LogFormat("Socket connected to {0}", client.RemoteEndPoint.ToString());

                Receive(client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Receive(Socket client)
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

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                byte[] data = (byte[])state.buffer.Clone();
                // (확인 중) 엔디안 변환s
                //Array.Reverse(data);

                // 수신 큐에 넣기
                tcpBuffer.Enqueue(data);

                // 다시 받을 준비
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        private bool CheckShutdown(byte[] data)
        {
            const int messageTypePos = 2;

            var messageCode = System.BitConverter.ToInt16(data, messageTypePos);

            if (messageCode == Shutdown || messageCode == CONNECT_END)
                return true;

            return false;
        }

        // 서버에 종료 요청
        public void SendShutdown()
        {
            if (!IsConnect) return;

            Debug.Log("Send Shutdown Message");
            var packet = CPacketFactory.CreateShutdownPacket();
            Send(packet.data);
        }
    }
}