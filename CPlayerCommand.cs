using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CPlayerCommand : MonoBehaviour
{
    public static CPlayerCommand instance;

    // 0번은 조작할 캐릭터, 나머지는 더미 캐릭터
    public List<GameObject> players = new List<GameObject>();

    public int ActivatedPlayersCount
    {
        get
        {
            int activated = 0;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].activeSelf)
                {
                    ++activated;
                }
            }
            return activated;
        }
    }
    // id만 바꾸고 조종 시 스왑을 이용해서 움직이기
    public int ControlCharacterID { get; private set; }

    private CController _controller;
    private CMouseFollower _camera;
    private COtherPlayerUiManager _othersUiList;
    private bool _isObservingMode = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        _controller = CController.instance;
        _othersUiList = GameObject.Find("UiScript").GetComponent<COtherPlayerUiManager>();
        _camera = GameObject.Find("FreeLookCameraRig").GetComponent<CMouseFollower>();
    }

    #region 캐릭터 활성화 및 비활성화
    /// <summary>
    /// 필요한 캐릭터 수(playerCount : 1~4)만큼 활성화
    /// </summary>
    /// <param name="playerCount"></param>
    public void SetActivePlayers(int playerCount)
    {
        if (playerCount > players.Count)
        {
            Debug.Log("CPlayerCommand - Active Wrong Size Players");
            playerCount = players.Count;
        }

        Debug.Log($"Set Active Player {playerCount}");

        for (int i = 0; i < playerCount; i++)
        {
            players[i].SetActive(true);
        }
        _othersUiList.ActiveOtherPlayerUi(playerCount);
    }

    public void DeactivatePlayer(int playerNumber)
    {
        if (playerNumber >= players.Count)
        {
            Debug.Log("CPlayerCommand - Wrong Player Number");
            return;
        }

        players[playerNumber].SetActive(false);
        _othersUiList.DeactivateOtherPlayerUI(playerNumber);
    }
    #endregion

    // 더미 캐릭터(MultiDoll)를 이용한 방법
    #region use with Dummy
    // 캐릭터는 바꾸지 않고 조종 번호만 바꿈
    public void SetMyCharacter(int charId)
    {
        Debug.Log("setting Character : " + charId);

        SwapCharacterPos(charId);
        SwapCharacterID(charId);
    }

    private void SwapCharacterPos(int charId)
    {
        // 더미 캐릭터 위치와 조종 캐릭터 위치 스왑
        var temp = players[charId].transform.position;
        players[charId].transform.position = players[ControlCharacterID].transform.position;
        players[ControlCharacterID].transform.position = temp;
    }

    private void SwapCharacterID(int charId)
    {
        GameObject temp = players[ControlCharacterID];
        players[ControlCharacterID] = players[charId];
        players[charId] = temp;

        Debug.Log($"{ControlCharacterID} is changed to {charId}");
        ControlCharacterID = charId;
    }

    // 캐릭터 이동
    public void Move(int charId, Vector3 movePos)
    {
        if (charId == ControlCharacterID)
        {
            Debug.Log("Can't Control Playable Character");
            return;
        }
        var character = players?[charId];
        if (character == null) return;

        character = players[charId];

        var playerState = character.GetComponent<CMultiDoll>();
        //Debug.Log($"dummy {charId} go to {movePos.x}, {movePos.y}, {movePos.z}");
        playerState.MoveTo(movePos);
    }

    // 캐릭터 강제 이동
    public void Teleport(int charId, Vector3 movePos)
    {
        var character = players?[charId];
        if (character == null) return;

        character = players[charId];

        character.transform.position = movePos;
    }

    public void Attack(int charID, Vector3 nowPos, Vector3 rotateAngle)
    {
        if (charID == ControlCharacterID)
        {
            Debug.Log("Can't Control Playable Character");
            return;
        }
        var character = players?[charID];
        if (character == null) return;

        character = players[charID];

        Teleport(charID, nowPos);
        var playerState = character.GetComponent<CMultiDoll>();
        playerState.AttackTo(rotateAngle);
    }

    public void Jump(int charID, Vector3 nowPos, Vector3 rotateAngle)
    {
        if (charID == ControlCharacterID)
        {
            Debug.Log("Can't Control Playable Character");
            return;
        }
        var character = players?[charID];
        if (character == null) return;

        character = players[charID];

        Teleport(charID, nowPos);
        var playerState = character.GetComponent<CMultiDoll>();
        playerState.JumpTo(rotateAngle);
    }

    // 구르기 명령
    public void Roll(int charId, Vector3 nowPos, Vector3 rollAngle)
    {
        if (charId == ControlCharacterID)
        {
            Debug.Log("Can't Control Playable Character");
            return;
        }
        var character = players?[charId];
        if (character == null) return;

        character = players[charId];

        Teleport(charId, nowPos);
        var playerState = character.GetComponent<CMultiDoll>();
        playerState.RollTo(rollAngle);
    }

    // 스킬 사용 명령
    public void UseSkill(int charId, int skillNumber, Vector3 nowPos, Vector3 targetPos)
    {
        if (charId == ControlCharacterID)
        {
            Debug.Log("Can't Control Playable Character");
            return;
        }
        Debug.Log($"Use Skill {charId} {skillNumber}");
        var character = players?[charId];
        if (character == null) return;

        character = players[charId];

        Teleport(charId, nowPos);
        var charSkill = character.GetComponent<CCharacterSkill>(); 
        charSkill.UseSkillToPosition(skillNumber, targetPos);
        //if (charSkill is CPlayerSkill)
        //{
        //    var playerSkill = charSkill as CPlayerSkill;
        //    playerSkill.UseSkillToPosition(skillNumber, targetPos);
        //}
        //else
        //{
            
        //}
    }
    #endregion

    // 해당 캐릭터에게 데미지 주기
    public void DamageToCharacter(int charId, int damageScale)
    {
        var character = players?[charId];
        if (character == null) return;

        var charStat = character.GetComponent<CharacterPara>();
        charStat.DamegedRegardDefence(damageScale);
    }

    public void BuffToCharacter(int charId, float buffTime, float buffScale)
    {
        var character = players?[charId];
        if (character == null) return;

        var charStat = character.GetComponent<CharacterPara>();
        //charStat.buffParameter.Buff(CBuffList.AttackBuff, buffTime, buffScale);
    }

    #region 골드 처리
    public void EarnMoneyAllCharacter(int amount)
    {
        Debug.Log($"players earn {amount} gold");
        foreach (var player in players)
        {
            if (player.activeSelf)
            {
                player.GetComponent<CPlayerPara>().Inventory.Gold += amount;
            }
        }
    }

    public void LoseMoneyAllCharacter(int amount)
    {
        Debug.Log($"players lose {amount} gold");
        foreach (var player in players)
        {
            if (player.activeSelf)
            {
                player.GetComponent<CPlayerPara>().Inventory.Gold -= amount;
            }
        }
    }
    #endregion

    #region 내부 함수
    private bool IsControlableCharacter(int charID)
    {
        if (charID == ControlCharacterID || IsInvalidCharacter(charID))
        {
            return false;
        }

        return true;
    }

    private bool IsInvalidCharacter(int charID)
    {
        return players?[charID] == null ? true : false;
    }
    #endregion

    #region Debug
    public void Follow(int charId) => Move(charId, players[0].transform.position);
    public void Call(int charId) => Teleport(charId, players[0].transform.position);
    public void SkillTo(int charId) => UseSkill(charId, 0, players[charId].transform.position, players[0].transform.position);
    public void AttackMirror(int charId) => Jump(charId, players[charId].transform.position, players[0].transform.rotation.eulerAngles);
    public void JumpMirror(int charId) => Jump(charId, players[charId].transform.position, players[0].transform.rotation.eulerAngles);
    public void RollMirror(int charId) => Roll(charId, players[charId].transform.position, players[0].transform.rotation.eulerAngles);
    #endregion
}
