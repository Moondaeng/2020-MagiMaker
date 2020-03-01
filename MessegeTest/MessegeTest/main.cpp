#pragma comment(lib,"ws2_32")
#define _CRT_SECURE_NO_WARNINGS
#include <WinSock2.h>
#include <ws2tcpip.h>
#include <iostream>
#include <process.h>
#include <ctime>
#define PORT 6000

#define CS_MOVE_START 100
#define CS_MOVE_STOP 101

#define SC_MOVE_START 110
#define SC_MOVE_STOP 111
#define SC_MOVE_SYNC 112

#define SC_CREATE_PLAYER 210
#define SC_CREATE_OTHER_PLAYER 211
#define SC_REMOVE_PLAYER 212

struct Header
{
	BYTE header_code;
	BYTE payLoad_size;
	BYTE type;
	BYTE temp;
	INT checkSum;
};

struct MyInfo
{
	INT ID;
	FLOAT x;
	FLOAT y;
};

unsigned int WINAPI RecvThread(LPVOID lpParam);

MyInfo info;

int main()
{
	WSADATA wsa;
	SOCKET sock;
	HANDLE hThread;

	srand(time(NULL));
	int retval;

	char buf[512];

	//소켓 통신 부
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
		return 1;

	SOCKADDR_IN serveraddr;
	ZeroMemory(&serveraddr, sizeof(serveraddr));

	serveraddr.sin_family = AF_INET;
	inet_pton(AF_INET, "127.0.0.1", &serveraddr.sin_addr.s_addr);
	serveraddr.sin_port = htons(PORT);

	sock = socket(AF_INET, SOCK_STREAM, 0);

	retval = connect(sock, (SOCKADDR *)&serveraddr, sizeof(serveraddr));

	if (retval == SOCKET_ERROR)
	{
		printf("error\n");
		return 0;
	}

	Header header;
	DWORD threadID;
	int num;
	header.header_code = 0x89;

	hThread = (HANDLE)_beginthreadex(NULL, 0, RecvThread, (VOID *)sock, 0, (unsigned int *)&threadID);
	printf("%d %d\n",sizeof(float),sizeof(FLOAT));
	while (1)
	{
		//system("cls");
		printf("move start 1//move stop 2\n");
		//printf(">\n");
		scanf("%d",&num);
		

		switch (num)
		{
		case 1:
			
			header.type = CS_MOVE_START;
			header.payLoad_size= sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT) + sizeof(FLOAT) + sizeof(FLOAT);
			memcpy(buf, &header, sizeof(Header));
			//memcpy(buf + sizeof(Header), &info.ID, sizeof(info.ID));
			memcpy(buf + sizeof(Header), &info.x, sizeof(info.x));
			memcpy(buf + sizeof(Header) + sizeof(FLOAT), &info.y, sizeof(info.y));

			info.x = ((float)rand() / (float)(RAND_MAX)) * 10.0;
			info.y = ((float)rand() / (float)(RAND_MAX)) * 10.0;

			memcpy(buf + sizeof(Header) + sizeof(FLOAT) + sizeof(FLOAT), &info.x, sizeof(info.x));
			memcpy(buf + sizeof(Header) + sizeof(FLOAT) + sizeof(FLOAT) + sizeof(FLOAT), &info.y, sizeof(info.y));
			//memcpy(buf + sizeof(Header) + sizeof(FLOAT) + sizeof(FLOAT), &info.x, sizeof(info.x));
			//memcpy(buf + sizeof(Header) + sizeof(FLOAT) + sizeof(FLOAT) + sizeof(FLOAT), &info.y, sizeof(info.y));
			send(sock, buf, header.payLoad_size + sizeof(Header), 0);
			break;
		case 2:
			header.type = CS_MOVE_STOP;
			header.payLoad_size = sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT);
			memcpy(buf, &header, sizeof(Header));
		//	memcpy(buf + sizeof(Header), &info.ID, sizeof(info.ID));
			memcpy(buf + sizeof(Header), &info.x, sizeof(info.x));
			memcpy(buf + sizeof(Header) + sizeof(FLOAT), &info.y, sizeof(info.y));
			send(sock, buf, header.payLoad_size + sizeof(Header), 0);
			break;
		default:
			break;
		}

		
	}
}


unsigned int WINAPI RecvThread(LPVOID lpParam)
{
	SOCKET sock = (SOCKET)lpParam;
	Header *header;
	char buf[1000];
	int recvLen;
	int cur = 0;

	printf("recvThread\n");
	while (1)
	{
		cur = 0;
		recvLen = recv(sock, buf, 1000, 0);
		
		while (recvLen>0)
		{
			if (recvLen-cur > sizeof(Header))
			{
				header = (Header *)(buf + cur);
				cur += sizeof(Header);
				switch (header->type)
				{
				case SC_CREATE_PLAYER:
					printf("create plyaer\n");
					printf("ID:%d nowX:%f nowY:%f\n",
						(INT)*(buf+cur), 
						(FLOAT)*(buf + cur+sizeof(INT)), 
						(FLOAT)*(buf + cur + sizeof(INT) + sizeof(FLOAT)));
					info.ID= (INT)*(buf + cur);
					info.x = (FLOAT)*(buf + cur + sizeof(INT));
					info.y = (FLOAT)*(buf + cur + sizeof(INT) + sizeof(FLOAT));
					cur += sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT);
					break;
				case SC_CREATE_OTHER_PLAYER:
					printf("create other plyaer\n");
					printf("ID:%d nowX:%f nowY:%f\n", 
						(INT)*(buf + cur), 
						(FLOAT)*(buf + cur + sizeof(INT)), 
						(FLOAT)*(buf + cur + sizeof(INT) + sizeof(FLOAT)));
					cur += sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT);
					break;
				case SC_REMOVE_PLAYER:
					printf("remove player\n");
					printf("ID:%d\n", (INT)*(buf + cur));
					cur += sizeof(INT);
					break;
				case SC_MOVE_START:
					printf("move start\n");
					printf("ID:%d nowX:%f nowY:%f destX:%f destY:%f\n",
						(INT)*(buf + cur),
						(FLOAT)*(buf + cur + sizeof(INT)),
						(FLOAT)*(buf + cur + sizeof(INT) + sizeof(FLOAT)),
						(FLOAT)*(buf + cur + sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT)),
						(FLOAT)*(buf + cur + sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT) + sizeof(FLOAT)));
					cur += sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT) + sizeof(FLOAT) + sizeof(FLOAT);
					break;
				case SC_MOVE_STOP:
					printf("move stop\n");
					printf("ID:%d nowX:%f nowY:%f\n",
						(INT)*(buf + cur),
						(FLOAT)*(buf + cur + sizeof(INT)),
						(FLOAT)*(buf + cur + sizeof(INT) + sizeof(FLOAT)));
					cur += sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT);
					break;
				case SC_MOVE_SYNC:
					printf("move sync\n");
					printf("ID:%d nowX:%f nowY:%f\n",
						(INT)*(buf + cur),
						(FLOAT)*(buf + cur + sizeof(INT)),
						(FLOAT)*(buf + cur + sizeof(INT) + sizeof(FLOAT)));
					cur += sizeof(INT) + sizeof(FLOAT) + sizeof(FLOAT);
					break;
				}	
			}
			else
				break;
		}
	}
}