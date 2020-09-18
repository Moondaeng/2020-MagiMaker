﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CPlayerCommand : MonoBehaviour
{
    private static CLogComponent _logger;
    public static CPlayerCommand instance;

    public List<GameObject> players = new List<GameObject>();
    [SerializeField]
    public int activePlayersCount;
    [SerializeField]
    public int ControlCharacterId;

    private CController _controller;
    private UnityStandardAssets.Cameras.FreeLookCam _camera;
    private COtherPlayerUiManager _othersUiList;
    private bool _isObservingMode = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        _logger = new CLogComponent(ELogType.Ctrl);
        _controller = GameObject.Find("Controller").GetComponent<CController>();
        _othersUiList = GameObject.Find("UiScript").GetComponent<COtherPlayerUiManager>();
        _camera = GameObject.Find("FreeLookCameraRig").GetComponent<UnityStandardAssets.Cameras.FreeLookCam>();
    }

    private void Start()
    {
        activePlayersCount = 1;
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
            _othersUiList.AddOtherPlayerUi(players[i]);
            //_othersUiList.AddOtherPlayerUi(players[i].transform.GetChild(0).gameObject);
        }
    }

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
        var charState = character.GetComponent<CCntl>();
        //charSkill.UseSkillToPosition(skillNumber, targetPos);
    }

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
        charStat.buffParameter.Buff(CBuffList.AttackBuff, buffTime, buffScale);
    }
}