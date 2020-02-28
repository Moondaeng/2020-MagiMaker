#pragma comment(lib, "winmm.lib")
#include <Windows.h>
#include <tchar.h>
#include <iostream>
#include <map>

#include "Packet.h"
#include "RingBuffer.h"
#include "define.h"

#include "network.h"

int _tmain()
{
	//������ tick time�� 1ms�� ����(�⺻�� 15ms ����)
	timeBeginPeriod(1);
	
	if (!InitNetwork())
	{
		printf("listen socket setting error\n");
		return -1;
	}

	while (1)
	{
		NetworkProcess();
	}
}