using UnityEngine;

public class CTutorialMapCreator : CCreateMap
{
    protected override void LoadSpecialRoom()
    {
        startRoom = Resources.Load("Room/StartRoomTuto") as GameObject;
        bossRoom = Resources.Load("Room/BossRoomTuto") as GameObject;
        shopRoom = Resources.Load("Room/0/ShopRoom0") as GameObject;
    }

    public override void CreateStage()
    {
        MakeNextRoomTypeInfos(_roomCount + 1);
    }

    protected override void MakeNextRoomTypeInfos(int roomCount)
    {
        Debug.Log("Tutorial");

        if (_roomCount > 5) //방 전부 생성됬을경우 대기
            return;

        if (_roomCount == 0)
        {
            if (_explicitRoomList.Count != 0)
            {
                GetExplicitRoomInList(0);
            }
            else
            {
                nextRoomTypeArr[0] = ERoomType._start;
                Debug.Log("Create Start Room");
                InstantiateRoom(nextRoomTypeArr[0]); //시작방 생성
            }
        }

        if (_roomCount == 1)
        {
            nextRoomTypeArr[0] = ERoomType._elite; //일반방 넣기
        }

        if (_roomCount == 2)
        {
            nextRoomTypeArr[0] = ERoomType._event; //이벤트방 넣기
        }

        if (_roomCount == 3)
        {
            nextRoomTypeArr[0] = ERoomType._normal; //아이템 엘리트방 넣기
        }

        if (_roomCount == 4)
        {
            nextRoomTypeArr[0] = ERoomType._shop; //상점방 넣기
        }

        if (_roomCount == 6 - 1)
        {
            nextRoomTypeArr[0] = ERoomType._boss; //보스방 따로 넣기
        }

        MakePortalText();
    }
}
