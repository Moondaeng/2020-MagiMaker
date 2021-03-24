using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Net.Sockets;

public class CClientInfo : MonoBehaviour
{
    public class User
    {
        public readonly int uid;
        public readonly string id;
        public int clear;
        public int Slot { get; set; }

        public User(string id, int clear, int slot)
        {
            this.uid = 0;
            this.id = id;
            this.clear = clear;
            Slot = slot;
        }

        public User(int uid, string id, int clear)
        {
            this.uid = uid;
            this.id = id;
            this.clear = clear;
        }

        public User(int uid, string id, int clear, int slot)
        {
            this.uid = uid;
            this.id = id;
            this.clear = clear;
            Slot = slot;
        }

        public void UpdateUserStat(int clear)
        {
            this.clear = clear;
        }
    }

    // 방에 들어갈 때 필요한 정보
    public class JoinRoom
    {
        // C# Auto 속성
        public int[] Slot { get; set; }
        public bool IsHost { get; private set; }
        public List<User> Others { get; private set; }
        public int RoomID { get; private set; }

        public JoinRoom(int roomid, int[] slot, List<User> others, bool isHost)
        {
            RoomID = roomid;
            Slot = slot;
            Others = others;
            IsHost = isHost;
        }

        // 방 정보를 얻음
        public static JoinRoom GetRoomInfo(int rid, Network.CPacket packet, int payloadSize)
        {
            int myslot = packet.ReadInt32();
            int ucnt = packet.ReadInt32();
            Debug.LogFormat("ucnt : ", ucnt);
            List<User> others = new List<User>();
            
            int[] slots = new int[4];
            int slot;

            int userCount = payloadSize / 24;

            for(int i = 0; i < userCount; i++)
            {
                string id = packet.ReadString(16);
                slot = packet.ReadInt32();
                int clear = packet.ReadInt32();
                others.Add(new User(id, clear, slot));
                Debug.Log("CClientInfo - JoinRoom : A Slot is " + slot + " and id " + id);
            }

            ThisUser.Slot = myslot;

            return new JoinRoom(rid, slots, others, false);
        }

        public static void UpdateRoom(int slotNum, User newGuest)
        {
            newGuest.Slot = slotNum;
            ThisRoom.Others.Add(newGuest);
        }

        public static void DeleteUser(int slotNum)
        {
            foreach(var user in ThisRoom.Others)
            {
                Debug.Log($"CClientInfo - Delete User : id {user.id}, clear {user.clear}, slot {user.Slot}");
                if(user.Slot == slotNum)
                {
                    ThisRoom.Others.Remove(user);
                    return;
                }
            }
        }
    }

    public static void CreateRoom(int rid)
    {
        CClientInfo.ThisRoom = new CClientInfo.JoinRoom(rid, new int[4] { 0, -1, -1, -1 }, new List<CClientInfo.User>(), true);
        CClientInfo.ThisUser.Slot = 0;
    }

    public static bool IsSinglePlay()
    {
        int playerCount = 0;
        for (int i = 0; i < ThisRoom.Slot.Length; i++)
        {
            if (ThisRoom.Slot[i] != -1)
            {
                ++playerCount;
            }
        }
        return playerCount == 1 ? true : false;
    }

    public static User ThisUser { get; set; }
    public static JoinRoom ThisRoom { get; set; }
    public static int PlayerCount { get; set; }
    public static readonly bool IsDebugMode = true;
    public static readonly bool IsSingleDebugMode = true;
}