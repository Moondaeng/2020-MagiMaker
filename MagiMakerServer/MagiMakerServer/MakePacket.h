#ifndef __MAKE_PACKET__
#define __MAKE_PACKET__

void MakePacketCreatePlayer(Packet &p, INT id, FLOAT nowX, FLOAT nowY);
void MakePacketCreateOtherPlayer(Packet &p, INT id, FLOAT nowX, FLOAT nowY);
void MakePacketRemovePlayer(Packet &p, INT id);

void MakePacketMoveStart(Packet &p, INT id, FLOAT nowX, FLOAT nowY, FLOAT destX, FLOAT destY);
void MakePacketMoveStop(Packet &p, INT id, FLOAT nowX, FLOAT nowY);
void MakePacketSync(Packet &p, INT id, FLOAT nowX, FLOAT nowY);

#endif // !__MAKE_PACKET__


