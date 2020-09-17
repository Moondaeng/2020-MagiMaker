using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using static System.Console;

// 해야할 것 정리 : 포탈 생성 삭제하고 포탈 사용 시 페이드 아웃하면서 맵에 잇는 오브젝트들 삭제하고 새로운 맵 불러오고 플레이어 위치 조정
// _rooms는 방을 관리하는 링크드 리스트지만 변경사항으로 실제로 존재하는 방은 1개만 계속 유지되기에 추후 수정필요. 링크드 리스트는 필요없다.
public class CCreateMap : MonoBehaviour
{
    public CCreateStage map;
    static public CCreateMap instance = null;
    public int createStageNumber;

    private void Start()
    {
        createStageNumber = 1;
        map = new CCreateStage(createStageNumber);
        map.RandomRoomEnqueue();
        map.CreateStage();
        if (instance == null)
            instance = this;
    }

    private void Update()
    {
        map.CtrlPortal(); //스테이지 클리어 하는 즉시 포탈 true로 바꿔줌
    }
}