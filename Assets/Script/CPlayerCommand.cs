using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerCommand : MonoBehaviour
{
    private static CLogComponent _logger;

    public List<GameObject> players = new List<GameObject>();
    public int activePlayersCount;

    private CController _controller;
    private CUIManager _playerUi;
    private COtherPlayerUiManager _othersUiList;
    private CCameraControl _camera;

    private void Awake()
    {
        _logger = new CLogComponent(ELogType.Ctrl);
        _controller = GameObject.Find("Controller").GetComponent<CController>();
        _playerUi = GameObject.Find("UiScript").GetComponent<CUIManager>();
        _othersUiList = GameObject.Find("UiScript").GetComponent<COtherPlayerUiManager>();
        _camera = GameObject.Find("Main Camera").GetComponent<CCameraControl>();
    }

    private void Start()
    {
        activePlayersCount = 1;
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
            _othersUiList.AddOtherPlayerUi(players[i]);
        }
    }

    // 해당 캐릭터 내 캐릭터로 선택
    // 임시 구현 : 앞으로 구현에 따라 방식 변경 가능
    public void SetMyCharacter(int charId)
    {
        Debug.Log("setting Character : " + charId);

        var character = players?[charId];
        if (character == null) return;
        Debug.Log("player object : " + players.Count);
        character.tag = "Player";

        _controller.player = character;
        _playerUi.SetUiTarget(character);
        _othersUiList.DeleteOtherPlayerUi(character);
    }

    // 캐릭터 이동
    public void Move(int charId, Vector3 movePos)
    {
        var character = players?[charId];
        if (character == null) return;

        var playerState = character.GetComponent<CCntl>();
        playerState.MoveTo(movePos);
    }

    // 캐릭터 강제 이동
    public void Teleport(int charId, Vector2 movePos)
    {
        var character = players?[charId];
        if (character == null) return;

        character.transform.position = movePos;
    }

    // 스킬 사용
    public void UseSkill(int charId, int skillNumber, Vector3 targetPos)
    {
        Debug.Log($"Use Skill {charId} {skillNumber}");
        var character = players?[charId];
        if (character == null) return;

        var charSkill = character.GetComponent<CCharacterSkill>();
        var charState = character.GetComponent<CCntl>();
        charSkill.UseSkillToPosition(skillNumber, targetPos);
        charState.SkillAction(2);
    }

    // 해당 캐릭터에게 데미지 주기
    public void DamageToCharacter(int charId, float damageScale)
    {
        var character = players?[charId];
        if (character == null) return;

        var charStat = character.GetComponent<CharacterPara>();
        charStat.SetEnemyAttack(damageScale);
    }
}
