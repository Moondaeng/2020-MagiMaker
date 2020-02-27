#ifndef __NETWORK__
#define __NETWORK__

//packe process ���
enum PROCRESULT { SUCCESS = 0, NONE, FAIL };

//��Ʈ��ũ ���� ����� listen�� ���� �ʱ�ȭ
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

