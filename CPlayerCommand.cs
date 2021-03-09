using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CPlayerCommand : MonoBehaviour
{
    public static CPlayerCommand instance;

    // 0번은 조작할 캐릭터, 나머지는 더미 캐릭터
    public List<GameObject> players = new List<GameObject>();
    [SerializeField]
    public int activePlayersCount;
    // id만 바꾸고 조종 시 스왑을 이용해서 움직이기
    [SerializeField]
    public int ControlCharacterId;

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

    private void Start()
    {
        SetActivePlayers(1);
        SetMyCharacter(0);
    }

    // 캐릭터 활성화
    // 멀티플레이 시 필요한 캐릭터 수만큼 활성화
    public void SetActivePlayers(int playerCount)
    {
        if(playerCount > players.Count)
        {
            Debug.Log("CPlayerCommand - Active Wrong Size Players");
            playerCount = players.Count;
        }

        activePlayersCount = playerCount;
        for (int i = 0; i < playerCount; i++)
        {
            players[i].SetActive(true);
        }
        _othersUiList.ActiveOtherPlayerUi(playerCount);
    }

    #region use with CCntl
    /*
    // 해당 캐릭터 내 캐릭터로 선택
    public void SetMyCharacter(int charId)
    {
        Debug.Log("setting Character : " + charId);

        var character = players?[charId];
        if (character == null) return;
        ControlCharacterId = charId;
        character.tag = "Player";

        _controller.SetControlCharacter(character);
        if(!_isObservingMode)
        {
            _camera.SetTarget(character.transform);
        }
    }

    // 캐릭터 이동
    public void Move(int charId, Vector3 movePos)
    {
        var character = players?[charId];
        if (character == null) return;

        var playerState = character.GetComponent<CCntl>();
        //playerState.MoveTo(movePos);
    }

    // 캐릭터 강제 이동
    public void Teleport(int charId, Vector3 movePos)
    {
        var character = players?[charId];
        if (character == null) return;

        character.transform.position = movePos;
    }

    // 스킬 사용
    public void UseSkill(int charId, int skillNumber, Vector3 nowPos, Vector3 targetPos)
    {
        Debug.Log($"Use Skill {charId} {skillNumber}");
        var character = players?[charId];
        if (character == null) return;

        Teleport(charId, nowPos);
        var charSkill = character.GetComponent<CCharacterSkill>();
        //charSkill.UseSkillToPosition(skillNumber, targetPos);
    }
    */
    #endregion

    // 더미 캐릭터(MultiDoll)를 이용한 방법
    #region use with Dummy
    // 캐릭터는 바꾸지 않고 조종 번호만 바꿈
    public void SetMyCharacter(int charId)
    {
        Debug.Log("setting Character : " + charId);

        SwapCharacterPos(charId);
        SwapCharacterId(charId);
    }

    private void SwapCharacterPos(int charId)
    {
        // 더미 캐릭터 위치와 조종 캐릭터 위치 스왑
        var temp = players[charId].transform.position;
        players[charId].transform.position = players[ControlCharacterId].transform.position;
        players[ControlCharacterId].transform.position = temp;
    }

    private void SwapCharacterId(int charId)
    {
        GameObject temp = players[ControlCharacterId];
        players[ControlCharacterId] = players[charId];
        players[charId] = temp;

        Debug.Log($"{ControlCharacterId} is changed to {charId}");
        ControlCharacterId = charId;
    }

    // 캐릭터 이동
    public void Move(int charId, Vector3 movePos)
    {
        if (charId == ControlCharacterId)
        {
            Debug.Log("Can't Control Playable Character");
            return;
        }
        var character = players?[charId];
        if (character == null) return;

        character = players[charId];

        var playerState = character.GetComponent<CMultiDoll>();
        Debug.Log($"dummy {charId} go to {movePos.x}, {movePos.y}, {movePos.z}");
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

    // 구르기 명령
    public void Roll(int charId, Vector3 nowPos, Vector3 movePos)
    {
        if (charId == ControlCharacterId)
        {
            Debug.Log("Can't Control Playable Character");
            return;
        }
        var character = players?[charId];
        if (character == null) return;

        character = players[charId];

        Teleport(charId, nowPos);
        var playerState = character.GetComponent<CMultiDoll>();
        playerState.RollTo(movePos);
    }

    // 스킬 사용 명령
    public void UseSkill(int charId, int skillNumber, Vector3 nowPos, Vector3 targetPos)
    {
        if (charId == ControlCharacterId)
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
    }

    // For Test
    public void Follow(int charId) => Move(charId, players[0].transform.position);
    public void Call(int charId) => Teleport(charId, players[0].transform.position);
    public void SkillTo(int charId) => UseSkill(charId, 0, players[charId].transform.position, players[0].transform.position);
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

}
