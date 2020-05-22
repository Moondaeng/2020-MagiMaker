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

        public readonly List<User> others;
        public readonly bool isHost;
        public readonly int roomid;

        public JoinRoom(int roomid, int[] slot, List<User> others, bool isHost)
        {
            this.roomid = roomid;
            Slot = slot;
            this.others = others;
            this.isHost = isHost;
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
            ThisRoom.others.Add(newGuest);
        }

        public static void DeleteUser(int slotNum)
        {
            foreach(var user in ThisRoom.others)
            {
                Debug.Log($"CClientInfo - Delete User : id {user.id}, clear {user.clear}, slot {user.Slot}");
                if(user.Slot == slotNum)
                {
                    ThisRoom.others.Remove(user);
                    return;
                }
            }
        }
    }

    public static User ThisUser { get; set; }
    public static JoinRoom ThisRoom { get; set; }
}