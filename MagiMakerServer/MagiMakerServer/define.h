#pragma once

#define PORT 6000

#define HEADER_CODE 0x89

#define DEFALUT_X 100
#define DEFALUT_Y 100

struct Session
{
	SOCKET sock;
	DWORD sessionID;

	RingBuffer SendQ;
	RingBuffer RecvQ;
};

struct Player
{
	Session *session;
	DWORD sessionID;

	FLOAT x;
	FLOAT y;
};

struct Header
{
	BYTE header_code;
	BYTE payLoad_size;
	BYTE type;
	BYTE temp;
	INT checkSum;
};

extern std::map<DWORD, Session *> g_sessionList;
extern std::map<DWORD, Player *> g_playerList;