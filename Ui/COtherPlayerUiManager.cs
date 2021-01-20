using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class COtherPlayerUiManager : MonoBehaviour
{
    private static CLogComponent _logger;

    /*
     * 다른 플레이어 캐릭터 UI 그릴 위치 설정
     */
    [System.Serializable]
    public struct OtherPlayerUiPosition
    {
        public int x;
        public int y;
        public int buffSquareSize;
        public int buffSpaceSize;
        public int distance;
    }

    internal struct OtherPlayerUiData
    {
        public Text playerName;
        public GameObject target;
        public CBuffTimerListUI buffList;
        public GameObject hpBar;
    }

    public GameObject timerDrawer;
    public GameObject hpBar;
    public OtherPlayerUiPosition uiPos;

    private Transform _uiCanvasTransform;
    private LinkedList<OtherPlayerUiData> _otherPlayerUiList;

    private void Awake()
    {
        _otherPlayerUiList = new LinkedList<OtherPlayerUiData>();

        _logger = new CLogComponent(ELogType.UI);
    }

    public void SetCanvas(Transform canvasTransform)
    {
        _uiCanvasTransform = canvasTransform;
    }

    // 다른 플레이어 Ui 추가
    public void AddOtherPlayerUi(GameObject player)
    {
        var playerUiData = new OtherPlayerUiData()
        {
            target = player,
            buffList = gameObject.AddComponent<CBuffTimerListUI>(),
            hpBar = Instantiate(hpBar, _uiCanvasTransform)
        };

        // 버프 리스트
        playerUiData.buffList.RegisterTimer(player);
        playerUiData.buffList.timerDrawer = timerDrawer;
        playerUiData.buffList.SetCanvas(_uiCanvasTransform);
        //playerUiData.buffList.timerUiTransform.SquareSize = uiPos.buffSquareSize;
        //playerUiData.buffList.timerUiTransform.SpaceSize = uiPos.buffSpaceSize;

        // 체력바
        playerUiData.hpBar.transform.SetParent(_uiCanvasTransform, false);
        playerUiData.target.GetComponent<CharacterPara>().hpDrawEvent.AddListener(
            playerUiData.hpBar.GetComponent<CUiHpBar>().Draw);

        _otherPlayerUiList.AddLast(playerUiData);
        Relocate();
    }

    // 다른 플레이어 Ui 제거
    public void DeleteOtherPlayerUi(GameObject player)
    {
        var otherPlayerNode = FindOtherPlayerUiTarget(player);
        if (otherPlayerNode == null)
        {
            return;
        }

        _logger.Log("delete");

        // 이전에 Ui 관련 전부 빼기
        Destroy(otherPlayerNode.Value.buffList);
        Destroy(otherPlayerNode.Value.hpBar);
        _otherPlayerUiList.Remove(otherPlayerNode);
        Relocate();
    }

    // 다른 플레이어 UI를 재배치
    private void Relocate()
    {
        var otherPlayerUi = _otherPlayerUiList.First;
        int posY = uiPos.y;

        for (otherPlayerUi = _otherPlayerUiList.First; otherPlayerUi != null; otherPlayerUi = otherPlayerUi.Next)
        {
            // 이름 위치 갱신
            

            // HP 위치 갱신
            var hpBarPos = otherPlayerUi.Value.hpBar.transform.position;
            hpBarPos.x = uiPos.x;
            hpBarPos.y = posY;
            otherPlayerUi.Value.hpBar.transform.position = hpBarPos;

            // 버프 위치 갱신
            //otherPlayerUi.Value.buffList.timerUiTransform.PosX = uiPos.x;
            //otherPlayerUi.Value.buffList.timerUiTransform.PosY = posY - uiPos.buffSquareSize;

            posY -= uiPos.distance;
        }
    }

    private void HpUpdate(int curHp, int maxHp)
    {
       
    }

    // Ui 그리고 있는 대상인지 탐색
    private LinkedListNode<OtherPlayerUiData> FindOtherPlayerUiTarget(GameObject otherPlayer)
    {
        LinkedListNode<OtherPlayerUiData> node;
        for (node = _otherPlayerUiList.First; node != null; node = node.Next)
        {
            if (node.Value.target == otherPlayer)
            {
                return node;
            }
        }
        return node;
    }
}
