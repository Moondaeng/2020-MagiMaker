#include <Windows.h>
#include <map>

#include "Packet.h"
#include "RingBuffer.h"
#include "define.h"

#include "game.h"


std::map<DWORD, Player *> g_playerList;

Player *CreatePlayer(Session *session)
{
	Player *player = new Player();
	player->session = session;
	player->sessionID = session->sessionID;
	
	
	player->x = DEFALUT_X;
	player->y = DEFALUT_Y;

	//�÷��̾� ����Ʈ�� ���
	g_playerList.insert(std::make_pair(player->sessionID, player));
	

	return player;
}