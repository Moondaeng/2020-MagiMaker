using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class COtherPlayerUiManager : MonoBehaviour
{
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

    [SerializeField]
    private List<CStatusViewer> _otherPlayerUiArr = new List<CStatusViewer>();

    public void ActiveOtherPlayerUi(int playerCount)
    {
        if (playerCount <= 0)
        {
            Debug.Log($"Error : player count is {playerCount}");
            return;
        }

        for (int i = 0; i < playerCount - 1; i++)
        {
            _otherPlayerUiArr[i].SetActive(true);
            SetUiTarget(CPlayerCommand.instance.players[i + 1], i);
        }
        for (int i = playerCount - 1; i < _otherPlayerUiArr.Count; i++)
        {
            _otherPlayerUiArr[i].SetActive(false);
        }
    }

    // 플레이어 나갔을 때 UI 끄는 함수 - 개선 필요
    public void DeactivateOtherPlayerUI(int playerNumber)
    {
        int otherPlayerElemPos = -1;
        for (int i = 0; i <= playerNumber; i++)
        {
            if (i == CPlayerCommand.instance.ControlCharacterID)
            {
                --otherPlayerElemPos;
            }
            ++otherPlayerElemPos;
        }
        _otherPlayerUiArr[otherPlayerElemPos].SetActive(false);
    }

    private void SetUiTarget(GameObject target, int uiNumber)
    {
        if (target?.GetComponent<CPlayerPara>() == null)
        {
            Debug.Log($"target is null");
            return;
        }

        _otherPlayerUiArr[uiNumber].Register(target.GetComponent<CPlayerPara>());
    }
}
