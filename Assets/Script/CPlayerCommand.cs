using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerCommand : MonoBehaviour
{
    private static CLogComponent _logger;

    public GameObject playerCharacter;

    private List<GameObject> _players = new List<GameObject>();

    private CCntl _controller;
    private CUIManager _playerUi;
    private COtherPlayerUiManager _othersUiList;
    private CCameraControl _camera;

    private void Start()
    {
        var startCharacter = GameObject.Find("hong");
        _players.Add(startCharacter);
        Debug.Log("player object : " + _players.Count);

        _logger = new CLogComponent(ELogType.Ctrl);
        _controller = GameObject.Find("Controller").GetComponent<CCntl>();
        _playerUi = GameObject.Find("UiScript").GetComponent<CUIManager>();
        _othersUiList = GameObject.Find("UiScript").GetComponent<COtherPlayerUiManager>();
    }

    // 캐릭터 추가
    public void Add(Vector3 createPos)
    {
        var newPlayer = Instantiate(playerCharacter, createPos, Quaternion.identity);
        newPlayer.tag = "Allies";
        _players.Add(newPlayer);

        _othersUiList.AddOtherPlayerUi(newPlayer);

        Debug.Log("player object : " + _players.Count);
    }

    // 해당 캐릭터 내 캐릭터로 선택
    // 임시 구현 : 앞으로 구현에 따라 방식 변경 가능
    public void SetMyCharacter(int charId)
    {
        var preCharacter = _controller.player;
        preCharacter.tag = "Allies";
        Debug.Log("setting Character : " + charId);

        var character = _players?[charId];
        if (character == null) return;
        Debug.Log("player object : " + _players.Count);
        character.tag = "Player";

        _controller.player = character;
        _playerUi.SetUiTarget(character);
        _othersUiList.AddOtherPlayerUi(preCharacter);
        _othersUiList.DeleteOtherPlayerUi(character);
    }

    // 캐릭터 삭제
    public void Delete(int charId)
    {

    }

    // 캐릭터 이동
    public void Move(int charId, Vector3 movePos)
    {
        var character = _players?[charId];
        if (character == null) return;

        var playerState = character.GetComponent<CPlayerFSM>();
        playerState.MoveTo(movePos);
    }

    // 스킬 사용
    public void UseSkill(int charNumber, int skillNumber, Vector3 targetPos)
    {
        var charSkill = _players?[charNumber].GetComponent<CCharacterSkill>();
        charSkill.UseSkillToPosition(skillNumber, targetPos);
    }

    // 해당 캐릭터에게 데미지 주기
    public void DamageToCharacter(int charId, float damageScale)
    {
        var character = _players?[charId];
        if (character == null) return;

        var charStat = character.GetComponent<CharacterPara>();
        charStat.SetEnemyAttack(damageScale);
    }
}
