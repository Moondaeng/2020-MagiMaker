#ifndef __PACKET_PROC__
#define __PACKET_PROC__


BOOL ConnectSession(Session *session);
BOOL DisconnectSession(Session *session);
BOOL MoveStart(Session *session, Packet &p);
BOOL MoveStop(Session *session, Packet &p);

#endif