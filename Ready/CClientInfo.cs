using System.Collections.Generic;
using UnityEngine;

public class CClientInfo : MonoBehaviour
{
    public class User
    {
        public static readonly int USER_DATA_SIZE = 24;

        public readonly int uid;
        // id 최대 16자
        public readonly string id;
        public int clear;
        public int SlotNumber;

        public User(string id, int clear, int slot)
        {
            this.uid = 0;
            this.id = id;
            this.clear = clear;
            SlotNumber = slot;
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
            SlotNumber = slot;
        }

        public void UpdateUserStat(int clear)
        {
            this.clear = clear;
        }
    }

    // 방에 들어갈 때 필요한 정보
    public static class JoinRoom
    {
        public static readonly int MAX_PLAYER_COUNT = 4;

        public enum ESlotState
        {
            Open = 0,
            User = 1,
        }

        // C# Auto 속성
        public static ESlotState[] Slots 
        {
            get { return _slots; }
        }
        public static bool IsHost
        {
            get { return _isHost; }
            set { _isHost = value; }
        }
        public static List<User> Others
        {
            get { return _others; }
        }
        public static int RoomID { get; private set; }

        // 디버그 시 씬에서 작업할 때 사용
        private static bool _isHost = true;
        private static ESlotState[] _slots = new ESlotState[MAX_PLAYER_COUNT];
        private static List<User> _others = new List<User>();

        public static void CreateRoom(int rid)
        {
            ClearRoomData();

            // 다른 사람 정보
            Others.Clear();

            // 내 정보 및 방 정보
            Slots[0] = ESlotState.User;
            IsHost = true;
            RoomID = rid;

            ThisUser.SlotNumber = 0;
        }

        public static void JoinToRoom(int rid, Network.CPacket packet, int payloadSize)
        {
            int myslot = packet.ReadInt32();
            int ucnt = packet.ReadInt32();
            Debug.LogFormat("ucnt : ", ucnt);

            ClearRoomData();

            // 다른 사람 정보
            int userCount = payloadSize / 24;
            for (int i = 0; i < userCount; i++)
            {
                string id = packet.ReadString(16);
                var slot = packet.ReadInt32();
                int clear = packet.ReadInt32();
                Others.Add(new User(id, clear, slot));
                Slots[slot] = ESlotState.User;
                Debug.Log("CClientInfo - JoinRoom : A Slot is " + slot + " and id " + id);
            }

            // 내 정보 및 방 정보
            ThisUser.SlotNumber = myslot;
            Slots[myslot] = ESlotState.User;
            IsHost = false;
            RoomID = rid;
        }

        public static void AddUser(int slotNum, User newGuest)
        {
            newGuest.SlotNumber = slotNum;
            Others.Add(newGuest);
            Slots[slotNum] = ESlotState.User;
        }

        public static void DeleteUser(int slotNum)
        {
            Others.RemoveAll(user => user.SlotNumber == slotNum);
            Debug.Log($"CClientInfo - Delete User {slotNum}");
            Slots[slotNum] = ESlotState.Open;
        }

        private static void ClearRoomData()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i] = ESlotState.Open;
            }
            Others.Clear();
        }
    }

    public static bool IsSinglePlay()
    {
        int playerCount = 1 + JoinRoom.Others.Count;
        return playerCount == 1 ? true : false;
    }

    public static User ThisUser = new User(0, "SinglePlayer", 0);
    public static int PlayerCount { get; set; }
    public static readonly bool IsDebugMode = true;
    public static readonly bool IsSingleDebugMode = true;
}