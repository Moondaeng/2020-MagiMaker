#pragma comment(lib,"ws2_32")
#include <WinSock2.h>
#include <iostream>
#include <map>

#include "Packet.h"
#include "RingBuffer.h"
#include "define.h"
#include "PacketProc.h"

#include "network.h"

#include "Protocol.h"

SOCKET g_listenSock;

std::map<DWORD, Session *> g_sessionList;


BOOL InitNetwork()
{
	SOCKADDR_IN addr;
	ZeroMemory(&addr, sizeof(addr));
	addr.sin_family = AF_INET;
	addr.sin_addr.s_addr = htonl(INADDR_ANY);
	addr.sin_port = htons(PORT);

	WSADATA wsa;
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
		return false;

	g_listenSock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

	if (g_listenSock == INVALID_SOCKET)
	{
		return false;
	}

	printf("socket make\n");

	if (bind(g_listenSock, (SOCKADDR*)&addr, sizeof(addr)) == SOCKET_ERROR)
	{
		return false;
	}

	printf("bind socket\n");
	if (listen(g_listenSock, SOMAXCONN) == SOCKET_ERROR)
	{
		return false;
	}
	printf("listen socket\n");

	u_long on = 1;

	//non block socket set
	if (ioctlsocket(g_listenSock, FIONBIO, &on) == SOCKET_ERROR)
	{
		return false;
	}

	return true;
}

BOOL NetworkProcess()
{
	DWORD SessionIDTable[FD_SETSIZE];
	SOCKET SessionSockTable[FD_SETSIZE];
	int sockCount = 0;

	FD_SET ReadSet;
	FD_SET WriteSet;

	Session *session;

	//monitorUnit.MonitorNetwork(START);

	FD_ZERO(&ReadSet);
	FD_ZERO(&WriteSet);
	memset(SessionIDTable, 0, sizeof(DWORD) * FD_SETSIZE);
	memset(SessionSockTable, INVALID_SOCKET, sizeof(SOCKET) * FD_SETSIZE);

	//listen socket 등록
	FD_SET(g_listenSock, &ReadSet);

	SessionIDTable[sockCount] = 0;
	SessionSockTable[sockCount] = g_listenSock;
	sockCount++;

	for (auto iter = g_sessionList.begin(); iter != g_sessionList.end();)
	{
		session = iter->second;
		iter++;

		SessionIDTable[sockCount] = session->sessionID;
		SessionSockTable[sockCount] = session->sock;

		//FD set 등록
		FD_SET(session->sock, &ReadSet);

		if (session->SendQ.GetUseSize() > 0)
			FD_SET(session->sock, &WriteSet);
		sockCount++;

		//select 가득 찼을 경우 select 호출
		if (FD_SETSIZE <= sockCount)
		{
			//select
			CallSelect(SessionIDTable, SessionSockTable, &ReadSet, &WriteSet, sockCount);

			FD_ZERO(&ReadSet);
			FD_ZERO(&WriteSet);
			memset(SessionIDTable, 0, sizeof(DWORD) * FD_SETSIZE);
			memset(SessionSockTable, INVALID_SOCKET, sizeof(SOCKET) * FD_SETSIZE);
			sockCount = 0;
		}
	}

	if (sockCount > 0)
		CallSelect(SessionIDTable, SessionSockTable, &ReadSet, &WriteSet, sockCount);

	//monitorUnit.MonitorNetwork(END);
	return true;
}

BOOL CallSelect(DWORD *IDTable, SOCKET *sockTable, FD_SET *ReadSet, FD_SET *WriteSet, int cnt)
{
	timeval Time;
	int result;

	Time.tv_sec = 0;
	Time.tv_usec = 0;

	result = select(0, ReadSet, WriteSet, NULL, &Time);

	//select 감지 시
	if (result > 0)
	{
		for (int i = 0; i < cnt; i++)
		{
			if (sockTable[i] == INVALID_SOCKET)
				continue;

			//write 체크
			if (FD_ISSET(sockTable[i], WriteSet))
			{
				//send
				ProcSend(IDTable[i]);
			}

			//read 체크
			if (FD_ISSET(sockTable[i], ReadSet))
			{
				if (sockTable[i] == g_listenSock)
				{
					//리슨 소켓 반응
					//연결 요청
					ProcAccept();
				}
				else
				{
					//recv
					if (!ProcRecv(IDTable[i]))
					{
						//disconnect
						DisconnectSession(g_sessionList.find(IDTable[i])->second);
					}
				}
			}
		}
	}
	else if (result == SOCKET_ERROR)
	{
		//오류 발생
		return false;
	}

	return true;
}

BOOL ProcAccept()
{
	Session *session;
	SOCKET sock;
	SOCKADDR_IN addr;
	int addrLen = sizeof(addr);
	static DWORD sessionID = 1;

	sock = accept(g_listenSock, (sockaddr *)&addr, &addrLen);

	//accept 할 수 없을 경우 while을 빠져나감
	if (sock == INVALID_SOCKET)
	{
		return false;
	}

	//_LOG(dfLOG_LEVEL_DEBUG, L"ProcAccept: sessionID(%d)\n", sessionID);
	session = new Session();
	session->sock = sock;
	session->sessionID = sessionID;
	sessionID++;
	//session->recvPacketCount = 0;
	//session->recvTime = timeGetTime();

	//세션 추가
	
	ConnectSession(session);


	return true;
}

BOOL ProcRecv(DWORD sID)
{
	int recvSize;
	PROCRESULT result;
	Session *session = g_sessionList.find(sID)->second;

	//없는 세션
	if (session == NULL)
		return false;

	recvSize = recv(session->sock, session->RecvQ.GetWritePos(), session->RecvQ.DirectEnqueueSize(), 0);
	//monitorUnit.MonitorRecv();
	//_LOG(dfLOG_LEVEL_DEBUG, L"ProcRecv");
	//_LOG(dfLOG_LEVEL_DEBUG, L"SessionID: %d\n",sID);
	//socket 에러 또는 recvQ가 가득찼을 경우
	if (recvSize == SOCKET_ERROR || recvSize == 0)
	{
		return false;
	}

	//session->recvTime = timeGetTime();
	//session->recvPacketCount++;

	//수신한 데이터 처리
	if (recvSize > 0)
	{
		session->RecvQ.MoveWritePos(recvSize);

		while (1)
		{
			result = CompleteRecvPacket(session);

			if (result == NONE)
				break;
			else if (result == FAIL)
			{
				//_LOG(dfLOG_LEVEL_ERROR, L"PRError SessionID: %d", session->sessionID);
				return false;
			}
		}
	}

	return true;
}
BOOL ProcSend(DWORD sID)
{
	Session *session = g_sessionList.find(sID)->second;
	int sendSize;
	int result;

	if (session == NULL)
		return false;

	//_LOG(dfLOG_LEVEL_DEBUG, L"ProcSend");
	//_LOG(dfLOG_LEVEL_DEBUG, L"SessionID: %d\n", sID);

	sendSize = session->SendQ.DirectDequeueSize();

	if (sendSize <= 0)
		return true;

	result = send(session->sock, session->SendQ.GetReadPos(), sendSize, 0);
	//monitorUnit.MonitorSend();

	if (result == SOCKET_ERROR)
	{
		if (WSAGetLastError() == WSAEWOULDBLOCK)
		{
			//wouldblock 로그

			return true;
		}

		return false;
	}

	session->SendQ.MoveReadPos(result);
	return true;
}

PROCRESULT CompleteRecvPacket(Session *session)
{

	Header header;
	int recvQSize = session->RecvQ.GetUseSize();
	Packet payload;
	BYTE endCode;
	//메시지 완성 x
	if (sizeof(header) > recvQSize)
		return NONE;

	session->RecvQ.Peek((char *)&header, sizeof(Header));

	//패킷코드 불 일치 시 실패
	//if (header.header_code != HEADER_CODE)
	//	return FAIL;

	//메시지 완성 x
	if (recvQSize < header.payLoad_size + sizeof(header))
		return NONE;

	session->RecvQ.MoveReadPos(sizeof(Header));

	if (session->RecvQ.Dequeue(payload, header.payLoad_size) != header.payLoad_size)
		return FAIL;

	//END CODE 체크
	//if (session->RecvQ.Dequeue((char *)&endCode, sizeof(BYTE)) != sizeof(BYTE))
	//	return FAIL;
	//if (endCode != dfNETWORK_PACKET_END)
	//	return FAIL;

	//monitorUnit.MonitorRecvPacket();

	if (!PacketProc(session, header.type, payload))
		return FAIL;

	return SUCCESS;
}




BOOL PacketProc(Session *session, BYTE type, Packet &p)
{
	switch (type)
	{
	case CS_MOVE_START:
		MoveStart(session, p);
		break;
	case CS_MOVE_STOP:
		MoveStop(session, p);
		break;
	default:
		break;
	}

	return true;
}

BOOL SendPacketUnicast(Session *session, Packet &p)
{
	if (session == NULL)
	{
		return false;
	}

	if (session->SendQ.GetFreeSize() < p.GetDataSize())
	{
		return false;
	}

	session->SendQ.Enqueue(p);

	return true;
}

BOOL SendPacketBroadcast(Session *session,Packet &p,BOOL sendMe)
{
	for (auto iter = g_sessionList.begin(); iter != g_sessionList.end(); iter++)
	{
		if (sendMe || session->sessionID != iter->second->sessionID)
		{
			SendPacketUnicast(iter->second, p);
		}
	}

	return TRUE;
}