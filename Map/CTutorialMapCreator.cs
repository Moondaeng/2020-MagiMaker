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
        Debug.Log("Tutorial");

        int randomRoad; //랜덤한 갈림길 개수
        int selectRoomType; //랜덤으로 방 종류 뽑기
        int roadCount;  //갈림길 숫자

        if (_roomCount > 5) //방 전부 생성됬을경우 대기
            return;

        if (_roomCount == 0)
        {
            if (_explicitRoomList.Count != 0)
            {
                CreateExplicitRoomInList(0);
            }
            else
            {
                _roomArr[0, 0] = ERoomType._start;
                Debug.Log("Create Start Room");
                InstantiateRoom(_roomArr[_roomCount, 0]); //시작방 생성
            }
        }

        if (_roomCount == 1)
        {
            _roomArr[1, 0] = ERoomType._elite; //일반방 넣기
        }

        if (_roomCount == 2)
        {
            _roomArr[2, 0] = ERoomType._event; //이벤트방 넣기
        }

        if (_roomCount == 3)
        {
            _roomArr[3, 0] = ERoomType._normal; //아이템 엘리트방 넣기
        }

        if (_roomCount == 4)
        {
            _roomArr[4, 0] = ERoomType._shop; //상점방 넣기
        }

        if (_roomCount == 6 - 1)
        {
            _roomArr[6 - 1, 0] = ERoomType._boss; //보스방 따로 넣기
        }

        MakePortalText(_roomCount, _roomArr);
    }
}
