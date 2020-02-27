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
	//서버의 tick time을 1ms로 수정(기본은 15ms 정도)
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