#include <Windows.h>
#include <map>
#include <iostream>

#include "Packet.h"
#include "RingBuffer.h"
#include "define.h"

#include "Protocol.h"

#include "MakePacket.h"


void MakePacketCreatePlayer(Packet &p, INT id, FLOAT nowX, FLOAT nowY)
{
	Header header;
	header.header_code = HEADER_CODE;
	header.type = SC_CREATE_PLAYER;
	header.payLoad_size = sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT);
	printf("make create player %d %f %f\n", id, nowX, nowY);
	p.Clear();
	p.PutData((char *)&header, sizeof(header));
	p << id << nowX << nowY;
}

void MakePacketCreateOtherPlayer(Packet &p, INT id, FLOAT nowX, FLOAT nowY)
{
	Header header;
	header.header_code = HEADER_CODE;
	header.type = SC_CREATE_OTHER_PLAYER;
	header.payLoad_size = sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT);
	printf("make create other player %d %f %f\n", id, nowX, nowY);
	p.Clear();
	p.PutData((char *)&header, sizeof(header));
	p << id << nowX << nowY;
}


void MakePacketRemovePlayer(Packet &p, INT id)
{
	Header header;
	header.header_code = HEADER_CODE;
	header.type = SC_REMOVE_PLAYER;
	header.payLoad_size = sizeof(INT);
	printf("make remove player %d\n",id);
	p.Clear();
	p.PutData((char *)&header, sizeof(header));
	p << id;
}

void MakePacketMoveStart(Packet &p, INT id, FLOAT nowX, FLOAT nowY, FLOAT destX,FLOAT destY)
{
	Header header;
	header.header_code = HEADER_CODE;
	header.type = SC_MOVE_START;
	header.payLoad_size = sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT) + sizeof(FLOAT) + sizeof(FLOAT);
	printf("make move start %d %f %f %f %f\n", id, nowX, nowY,destX,destY);
	p.Clear();
	p.PutData((char *)&header, sizeof(header));
	p << id << nowX << nowY << destX << destY;
}

void MakePacketMoveStop(Packet &p, INT id, FLOAT nowX, FLOAT nowY)
{
	Header header;
	header.header_code = HEADER_CODE;
	header.type = SC_MOVE_STOP;
	header.payLoad_size = sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT);
	printf("make move stop %d %f %f\n", id, nowX, nowY);
	p.Clear();
	p.PutData((char *)&header, sizeof(header));
	p << id << nowX << nowY;
}



void MakePacketSync(Packet &p, INT id, FLOAT nowX, FLOAT nowY)
{
	Header header;
	header.header_code = HEADER_CODE;
	header.type = SC_MOVE_SYNC;
	header.payLoad_size = sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT);
	printf("make move sync %d\n", id);
	p.Clear();
	p.PutData((char *)&header, sizeof(header));
	p << id << nowX << nowY;
}
