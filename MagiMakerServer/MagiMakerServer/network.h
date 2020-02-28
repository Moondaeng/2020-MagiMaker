#ifndef __NETWORK__
#define __NETWORK__

//packe process 결과
enum PROCRESULT { SUCCESS = 0, NONE, FAIL };

//네트워크 최초 실행시 listen을 열고 초기화
BOOL InitNetwork();

//Network Process;
BOOL NetworkProcess();

//select call
BOOL CallSelect(DWORD *IDTable, SOCKET *sockTable, FD_SET *ReadSet, FD_SET *WriteSet, int cnt);

BOOL ProcAccept();
BOOL ProcRecv(DWORD sID);
BOOL ProcSend(DWORD sID);
PROCRESULT CompleteRecvPacket(Session *session);
BOOL DisconnectSession(Session *session);

BOOL PacketProc(Session *session, BYTE type, Packet &p);

BOOL SendPacketUnicast(Session *session,Packet &p);
BOOL SendPacketBroadcast(Session *session, Packet &p, BOOL sendMe = TRUE);

#endif // !__NETWORK__

