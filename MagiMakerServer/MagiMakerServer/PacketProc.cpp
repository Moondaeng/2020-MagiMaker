#include <Windows.h>
#include <map>

#include "Packet.h"
#include "RingBuffer.h"
#include "define.h"

#include "MakePacket.h"
#include "PacketProc.h"
#include "game.h"
#include "network.h"

BOOL ConnectSession(Session *session)
{
	Player *player = CreatePlayer(session);
	Packet p;

	g_sessionList.insert(std::make_pair(session->sessionID, session));
	//SendUnicast(session,p);
	MakePacketCreatePlayer(p, player->sessionID, player->x, player->y);

	SendPacketUnicast(session, p);

	MakePacketCreateOtherPlayer(p, player->sessionID, player->x, player->y);

	SendPacketBroadcast(session, p, FALSE);

	for (auto iter = g_playerList.begin(); iter != g_playerList.end(); iter++)
	{
		if ((*iter).second == player)
			continue;
		MakePacketCreateOtherPlayer(p, (*iter).second->sessionID, (*iter).second->x, (*iter).second->y);
		SendPacketUnicast(session, p);
	}

	return true;
}

BOOL DisconnectSession(Session *session)
{
	Player *player;
	Packet p;

	if (session == NULL)
		return false;

	g_sessionList.erase(session->sessionID);
	player = g_playerList.find(session->sessionID)->second;

	if (player != NULL)
	{
		MakePacketRemovePlayer(p, player->sessionID);
		
		SendPacketBroadcast(session,p);

		g_playerList.erase(session->sessionID);
		delete player;
	}
	closesocket(session->sock);

	//_LOG(dfLOG_LEVEL_ALWAYS, L"#Disconnect Session # SessionID : %d\n", session->sessionID);

	delete session;



	return true;
}

BOOL MoveStart(Session *session, Packet &p)
{
	//INT id;
	FLOAT nowX;
	FLOAT nowY;
	FLOAT destX;
	FLOAT destY;
	Player *player;

	Packet sendPacket;

	p >> nowX >> nowY >> destX >> destY;

	//player찾기

	MakePacketMoveStart(sendPacket, session->sessionID, nowX, nowY, destX, destY);
	
	SendPacketBroadcast(session,sendPacket,FALSE);

	return TRUE;
}
BOOL MoveStop(Session *session, Packet &p)
{
	//INT id;
	FLOAT nowX;
	FLOAT nowY;
	FLOAT destX;
	FLOAT destY;
	Player *player;

	Packet sendPacket;

	p >> nowX >> nowY >> destX >> destY;

	//player찾기

	MakePacketMoveStop(sendPacket, session->sessionID, nowX, nowY);

	SendPacketBroadcast(session,sendPacket,FALSE);

	return TRUE;
}