using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Network
{
    /*
     * 인게임에서의 패킷 변환을 담당하는 클래스
     * 네트워크에 연결된 경우 이벤트가 
     * 해석된 내용은 Commander로 실행시킨다
     */
    public class CPacketInterpreter
    {
        private const int _setCharacterInfo = 410;
        private const int _moveStart = 411;
        private const int _moveStop = 412;
        private const int _moveCorrection = 413;
        private const int _actionCommand = 510;
        private const int _GetItem = 610;
        private const int _PortalAccept = 611;
        private const int _UsePortal = 612;
        private const int _PortalTeleport = 613;
        private const int _UsePortalPopup = 614;
        private const int _monsterIdle = 711;
        private const int _monsterTrace = 712;
        private const int _monsterAttack = 713;
        private const int _monsterSkill = 714;

        private Network.CTcpClient _tcpManager;
        private CPlayerCommand playerCommander;
        private CMonsterManager monsterManager;

        public CPacketInterpreter(Network.CTcpClient tcpManger)
        {
            _tcpManager = tcpManger;
            playerCommander = CPlayerCommand.instance;
            monsterManager = CMonsterManager.instance;
        }

        public void PacketInterpret(byte[] data)
        {
            // 헤더 읽기
            CPacket packet = new CPacket(data);
            packet.ReadHeader(out byte payloadSize, out short messageType);
            Debug.Log($"Header : payloadSize = {payloadSize}, messageType = {messageType}");

            switch((int)messageType)
            {
                case _setCharacterInfo:
                    InterpretSetCharacter(packet);
                    break;
                case _moveStart:
                    InterpretMoveStart(packet);
                    break;
                case _moveStop:
                    InterpretMoveStop(packet);
                    break;
                case _moveCorrection:
                    InterpretMoveCorrection(packet);
                    break;
                case _actionCommand:
                    InterpretActionCommand(packet);
                    break;
                case _UsePortal:
                    InterpretUsePortal(packet);
                    break;
                case _PortalAccept:
                    InterpretPortalAccept(packet);
                    break;
                case _PortalTeleport:
                    InterpretPortalTeleport(packet);
                    break;
                case _monsterIdle:
                    InterpretMonsterIdle(packet);
                    break;
                case _monsterTrace:
                    InterpretMonsterTrace(packet);
                    break;
                case _monsterAttack:
                    InterpretMonsterAttack(packet);
                    break;
                case _monsterSkill:
                    InterpretMonsterSkill(packet);
                    break;
                
            }
        }

        #region Send Message
        public void SendCharacterInfoRequest()
        {
            var message = CPacketFactory.CreateCharacterInfoPacket();

            _tcpManager.Send(message.data);
        }

        public void SendMoveStart(Vector3 now, Vector3 dest)
        {
            var message = CPacketFactory.CreateMoveStartPacket(now, dest);

            _tcpManager.Send(message.data);
        }

        public void SendMoveStop(Vector3 now)
        {
            var message = CPacketFactory.CreateMoveStopPacket(now);

            _tcpManager.Send(message.data);
        }

        public void SendMonsterIdle(int monsterId)
        {
            var message = CPacketFactory.CreateMonsterIdlePacket(monsterId);

            _tcpManager.Send(message.data);
        }
        public void SendMonsterTrace(int monsterId, int playerId)
        {
            var message = CPacketFactory.CreateMonsterTracePacket(monsterId, playerId);

            _tcpManager.Send(message.data);
        }
        public void SendMonsterAttack(int monsterId, int playerId)
        {
            var message = CPacketFactory.CreateMonsterAttackPacket(monsterId, playerId);

            _tcpManager.Send(message.data);
        }
        public void SendMonsterSkill(int monsterId, int skillNum)
        {
            var message = CPacketFactory.CreateMonsterSkillPacket(monsterId, skillNum);

            _tcpManager.Send(message.data);
        }

        public void SendMonsterSkill(int monsterId, int skillNum, int playerId)
        {
            var message = CPacketFactory.CreateMonsterSkillPacket(monsterId, skillNum, playerId);

            _tcpManager.Send(message.data);
        }

        public void SendActionStart(int actionNumber, Vector3 now, Vector3 dest)
        {
            var message = CPacketFactory.CreateActionStartPacket(actionNumber, now, dest);

            _tcpManager.Send(message.data);
        }
        #endregion

        #region Interpret Packet
        private void InterpretSetCharacter(CPacket packet)
        {
            Int32 MyId = packet.ReadInt32();

            Debug.Log($"Set Character : my id - {MyId}");

            //Commander
            playerCommander.SetMyCharacter((int)MyId);
        }

        private void InterpretMoveStart(CPacket packet)
        {
            Int32 id;
            Vector3 now, dest;
            
            id = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();
            dest.x = packet.ReadSingle();
            dest.y = packet.ReadSingle();
            dest.z = packet.ReadSingle();

            Debug.LogFormat("Move Start - id{0} move ({1},{2},{3}) to ({4},{5},{6})", 
                id, now.x, now.y, now.z, dest.x, dest.y, dest.z);

            //Commander
            playerCommander.Move(id, dest);
        }

        private void InterpretMoveStop(CPacket packet)
        {
            Int32 id;
            Vector3 now;

            id = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();

            Debug.LogFormat("Move Stop - id{0} ({1},{2})", id, now.x, now.y, now.z);
        }

        private void InterpretMoveCorrection(CPacket packet)
        {
            Int32 id;
            Vector3 now, dest;

            id = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();
            dest.x = packet.ReadSingle();
            dest.y = packet.ReadSingle();
            dest.z = packet.ReadSingle();

            Debug.LogFormat("Move Correction - id{0} move ({1},{2},{3}) to ({4},{5},{6})",
                id, now.x, now.y, now.z, dest.x, dest.y, dest.z);
        }

        private void InterpretActionCommand(CPacket packet)
        {

            Int32 id;
            Int32 actionNumber;
            Vector3 now, dest;

            Debug.Log("action Command");

            id = packet.ReadInt32();
            actionNumber = packet.ReadInt32();
            now.x = packet.ReadSingle();
            now.y = packet.ReadSingle();
            now.z = packet.ReadSingle();
            dest.x = packet.ReadSingle();
            dest.y = packet.ReadSingle();
            dest.z = packet.ReadSingle();

            Debug.LogFormat("Action Start - id{0} actionNumber{1} move ({2},{3},{4}) to ({5},{6},{7})",
                id, actionNumber, now.x, now.y, now.z, dest.x, dest.y, dest.z);

            playerCommander.UseSkill((int)id, (int)actionNumber, now, dest);
        }

        private void InterpretGetItem(CPacket packet)
        {
            Int32 id;

            Debug.Log("Get Item");

            id = packet.ReadInt32();

            GameObject.Destroy(GameObject.FindGameObjectWithTag("ITEM"));

            //playerCommander.UseSkill((int)id, (int)actionNumber, now, dest);
        }

        private void InterpretUsePortal(CPacket packet)
        {
            Int32 id;

            Debug.Log("Use Portal");

            id = packet.ReadInt32();

            //CWaitingForAccept.instance.SetActivePortalPopup(true);
            //playerCommander.UseSkill((int)id, (int)actionNumber, now, dest);
        }

        private void InterpretPortalAccept(CPacket packet)
        {
            Int32 id;
            Int32 accept;

            Debug.Log("Get Item");

            id = packet.ReadInt32();
            accept = packet.ReadInt32();

            if (accept == 0)
            {
                //CWaitingForAccept.instance.SetPortalUseSelect(id, CWaitingForAccept.EAccept._accept);
            }
            else if (accept == 1)
            {
                //CWaitingForAccept.instance.SetPortalUseSelect(id, CWaitingForAccept.EAccept._cancle);
            }
        }

        private void InterpretPortalTeleport(CPacket packet)
        {
            Int32 id;

            Debug.Log("Get Item");

            id = packet.ReadInt32();
            
        }

        /// <summary>
        /// 몬스터 패턴 당 다른 종류의 패킷 송수신
        /// </summary>
        
        private void InterpretMonsterIdle(CPacket packet)
        {
            Int32 id;

            id = packet.ReadInt32();

            Debug.Log(id + " Idle state");

            monsterManager.DecideMonsterPattern(id, CMonsterManager._idleState);
        }

        private void InterpretMonsterTrace(CPacket packet)
        {
            Int32 id;
            Int32 id2;

            id = packet.ReadInt32();
            id2 = packet.ReadInt32();

            Debug.Log(id + " Trace state to " + id2);

            monsterManager.DecideMonsterPattern(id, id2, CMonsterManager._traceState);
        }
        /// <summary>
        /// 몬스터 
        /// </summary>
        private void InterpretMonsterAttack(CPacket packet)
        {
            Int32 id;

            id = packet.ReadInt32();

            Debug.Log(id + " Idle state");

            monsterManager.DecideMonsterPattern(id, CMonsterManager._attackState1);

        }
        /// <summary>
        /// id : 해당 몬스터
        /// id2 : 스킬 번호
        /// id3 : 대상 플레이어 (없는 경우 있음)
        /// </summary>
        private void InterpretMonsterSkill(CPacket packet)
        {
            Int32 id;
            Int32 id2;
            Int32 id3;

            id = packet.ReadInt32();
            id2 = packet.ReadInt32();
            id3 = packet.ReadInt32();

            if (id3 == -2)
            {
                switch(id2)
                {
                    case 1:
                        monsterManager.DecideMonsterPattern(id, CMonsterManager._skillState1);
                        break;
                    case 2:
                        monsterManager.DecideMonsterPattern(id, CMonsterManager._skillState2);
                        break;
                    case 3:
                        monsterManager.DecideMonsterPattern(id, CMonsterManager._skillState3);
                        break;
                    default:
                        Console.WriteLine("Packet Error");
                        break;
                }
            }
            else
            {
                switch (id2)
                {
                    case 1:
                        monsterManager.DecideMonsterPattern(id, CMonsterManager._skillState1, id3);
                        break;
                    case 2:
                        monsterManager.DecideMonsterPattern(id, CMonsterManager._skillState2, id3);
                        break;
                    case 3:
                        monsterManager.DecideMonsterPattern(id, CMonsterManager._skillState3, id3);
                        break;
                    default:
                        Console.WriteLine("Packet Error");
                        break;
                }
            }


        }
        #endregion
    }
}
