using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class CTcpClientNetwork : MonoBehaviour
{
    delegate void Command(byte[] packet);

    // Tcp 연결 포트(고정값은 수정 예정)
    private const Int32 port = 13000;

    ConcurrentQueue<byte[]> tcpBuffer = new ConcurrentQueue<byte[]>();
    Thread tcpReceiveThread;
    TcpClient tcpClient;
    NetworkStream tcpNetworkStream;
    const Int32 tcpPacketByteSize = 24;

    void TcpConnect(String server)
    {
        try
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer 
            // connected to the same address as specified by the server, port
            // combination.
            tcpClient = new TcpClient(server, port);
            tcpNetworkStream = tcpClient.GetStream();
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
    }

    // TCP 송신 코루틴 / 수신 스레드 생성
    void MakeTcpRecieveThread()
    {
        tcpReceiveThread = new Thread(TcpReceiveMessage);
        tcpReceiveThread.IsBackground = true;
        tcpReceiveThread.Start();
    }

    // 서버로부터 오는 TCP 메세지를 수신해 큐에 저장한다
    void TcpReceiveMessage()
    {
        while (true)
        {
            byte[] tcpReceivePacket = new byte[tcpPacketByteSize];
            // 해당 코드를 tcpClient class에 맞게 대체
            //tcpSocket.Receive(tcpReceivePacket, 0, tcpReceivePacket.Length, SocketFlags.None);
            Int32 bytes = tcpNetworkStream.Read(tcpReceivePacket, 0, tcpReceivePacket.Length);

            Debug.LogFormat("Receive n bytes - {0}", bytes);

            // 수신 큐에 넣기
            tcpBuffer.Enqueue((byte[])tcpReceivePacket.Clone());

            // 패킷 초기화
            System.Array.Clear(tcpReceivePacket, 0, tcpReceivePacket.Length);
        }
    }

    //private Int32 ReadCommandCode(byte[] data)
    //{
    //    return;
    //}

    // TCP 수신 큐에 읽는 정보를 읽고 해석해 실행한다
    IEnumerator TcpReadMessage()
    {
        byte[] readMessage;
        int messageCode;

        //Debug.Log("Read Message Coroutine Start");

        while (true)
        {
            yield return null;
            if (tcpBuffer.IsEmpty == false)
            {
                tcpBuffer.TryDequeue(out readMessage);
                //Debug.LogFormat("Dequeue n bytes - {0}", readMessage.Length);

                // 메세지 코드 해석
                messageCode = BitConverter.ToInt32(readMessage, 0);
                //Debug.LogFormat("Message Code - {0}", messageCode);

                // 구조 변경
                //for(var dic in messageDictionary)
                //{
                //    if (dic.key == messageCode)
                //    {
                //        dic.Value();
                //    }
                //}

                switch (messageCode)
                {
                    //case SMapInfoCode:
                    //    MapInfo receiveMapInfo = ByteToStruct<MapInfo>(readMessage);
                    //    Instantiate(obstacleList[receiveMapInfo.objNum],
                    //        new Vector3(receiveMapInfo.x, receiveMapInfo.y, receiveMapInfo.z), Quaternion.identity);
                    //    break;
                    //case SUserInfoCode:
                    //    UserInfo receiveUserInfo = ByteToStruct<UserInfo>(readMessage);
                    //    receiveUIArr.Add(receiveUserInfo);
                    //    break;
                    //case SAttackMessageCode:
                    //    AttackInfo receiveAttackInfo = ByteToStruct<AttackInfo>(readMessage);
                    //    DamageToCharacter(receiveAttackInfo);
                    //    break;
                    //case SGetItemCode:
                    //    // 해당 구조체 변환
                    //    // GetItem();
                    //    break;
                    //case SUdpConnectCode:
                    //    Int32 id = BitConverter.ToInt32(readMessage, sizeof(Int32));

                    //    Debug.LogFormat("readMessage : {0}", id);
                    //    if (id == -1)
                    //    {
                    //        Debug.Log("Udp Connect Message");
                    //        StartCoroutine("UdpConnectMessage");
                    //    }
                    //    else
                    //    {
                    //        settingCharacterID(id);
                    //        SettingCharacterPositionAll();
                    //    }
                    //    break;
                    //case SRespawnCode:
                    //    RespawnInfo receiveRespawnInfo = ByteToStruct<RespawnInfo>(readMessage);
                    //    GameObject deathCharacter = FindCharacterByID(receiveRespawnInfo.uidNum);
                    //    CharacterExtraInfomation deathCEInfo = FindCEInfoByID(receiveRespawnInfo.uidNum);
                    //    Debug.LogFormat("Receive Message : uid - {0}, x - {1}, y - {2}, z - {3}",
                    //        receiveRespawnInfo.uidNum, receiveRespawnInfo.x, receiveRespawnInfo.y, receiveRespawnInfo.z);
                    //    deathCharacter.SetActive(true);
                    //    deathCEInfo.currentHP = CharacterExtraInfomation.maxHP;
                    //    SettingCharacterPosition(receiveRespawnInfo.uidNum, receiveRespawnInfo.x, receiveRespawnInfo.y, receiveRespawnInfo.z);
                    //    break;
                    //case SGameEndCode:
                    //    Int32 winner = BitConverter.ToInt32(readMessage, sizeof(Int32));
                    //    Debug.LogFormat("Winner - {0}", winner);
                    //    // Game End Function
                    //    break;
                }
            }
        }
    }

    void TcpClose()
    {
        tcpReceiveThread.Abort();
        tcpNetworkStream.Close();
        tcpClient.Close();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
