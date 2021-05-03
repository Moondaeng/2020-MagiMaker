using System;
using System.Collections.Generic;
using UnityEngine;

public class CTestController : MonoBehaviour
{
    private Dictionary<KeyCode, Action> _selectedDictionary;
    private Dictionary<KeyCode, Action> _characterControlDictionary;
    private Dictionary<KeyCode, Action> _monsterControlDictionary;
    private Dictionary<KeyCode, Action> _otherControlDictionary;

    [Header("Testing Components")]
    public CPlayerCommand commander;
    public CUIManager ui;
    public CMonsterManager monsterManager;

    [Header("Debug UI")]
    public TMPro.TMP_Text testedDictionaryText;
    [Header("Tested Character UI")]
    public GameObject characterTestPanel;

    public int SelectedCharacterNumber 
    {
        get { return _selectedCharacterNumber; }
        private set 
        {
            _selectedCharacterNumber = value;

            if (Network.CTcpClient.instance?.IsConnect == true && !CClientInfo.IsSinglePlay())
            {
                Debug.Log("Multiplay Test");
                var message = Network.CPacketFactory.CreateChangePlayer(_selectedCharacterNumber);

                Network.CTcpClient.instance.Send(message.data);
            }
        }
    }
    private int _selectedCharacterNumber = 1;

    // 보통 Dictionary 초기화는 Awake에서 하나,
    // Singleton instance의 타 함수들을 사용하는 해당 클래스는 안정성을 위해 Start에서 초기화 함
    private void Start()
    {
        // 조작 관리
        _characterControlDictionary = new Dictionary<KeyCode, Action>
        {
            // Character Add / Delete
            [KeyCode.Insert] = () => commander.SetActivePlayers(4),
            [KeyCode.Delete] = () => KickPlayer(_selectedCharacterNumber),
            // Character Control
            [KeyCode.Alpha5]    = () => _selectedCharacterNumber = 0,
            [KeyCode.Alpha6]    = () => _selectedCharacterNumber = 1,
            [KeyCode.Alpha7]    = () => _selectedCharacterNumber = 2,
            [KeyCode.Alpha8]    = () => _selectedCharacterNumber = 3,
            [KeyCode.Alpha9]    = () => commander.SetMyCharacter(_selectedCharacterNumber),
            [KeyCode.O]         = () => commander.DamageToCharacter(_selectedCharacterNumber, 300),
            [KeyCode.J]         = () => commander.Follow(_selectedCharacterNumber),
            [KeyCode.K]         = () => commander.JumpMirror(_selectedCharacterNumber),
            [KeyCode.L]         = () => commander.RollMirror(_selectedCharacterNumber),
            [KeyCode.M]         = () => CPortalManager.instance.SetPortalUseSelect(_selectedCharacterNumber, CPortalManager.EPortalVote.Accept),
            [KeyCode.Comma]     = () => CPortalManager.instance.SetPortalUseSelect(_selectedCharacterNumber, CPortalManager.EPortalVote.Cancel),
            // Item
            [KeyCode.KeypadPlus] = () => CItemManager.SetItemToDropState(
                    CItemManager.instance.PopRandomItemByGrade(CItemManager.EItemGrade.Normal, CConstants.EQUIP_ITEM_TYPE),
                    GetHitPoint()),
            [KeyCode.KeypadMinus] = () => CItemManager.SetItemToDropState(
                    CItemManager.instance.PopRandomItemByGrade(CItemManager.EItemGrade.Normal, CConstants.CONSUM_ITEM_TYPE),
                    GetHitPoint()),
        };

        _monsterControlDictionary = new Dictionary<KeyCode, Action>
        {
            // Set Order Mode
            [KeyCode.U] = () => monsterManager.SetOrderMode(true),
            [KeyCode.I] = () => monsterManager.SetOrderMode(false),
            // 
            [KeyCode.J] = () => monsterManager.HitAllMonster(),
            [KeyCode.K] = () => monsterManager.AttackAllMonster(),
            [KeyCode.L] = () => monsterManager.SkillAllMonster(1),
        };

        _otherControlDictionary = new Dictionary<KeyCode, Action>
        {
            [KeyCode.U] = () => CCreateMap.instance.MakePortalText(),
            [KeyCode.I] = () => CCreateMap.instance.CreateNextRoomsInfo(),
            [KeyCode.O] = () => CCreateMap.instance.PrintCurrentRoomInfo(),
        };

        _selectedDictionary = _characterControlDictionary;
    }

    private void Update()
    {
        // 원격 조종 키를 바꿈
        if (Input.GetKeyDown(KeyCode.F5))
        {
            _selectedDictionary = _characterControlDictionary;
            testedDictionaryText.text = "캐릭터 조종 모드";
            characterTestPanel.SetActive(true);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.F6))
        {
            _selectedDictionary = _monsterControlDictionary;
            testedDictionaryText.text = "몬스터 조종 모드";
            characterTestPanel.SetActive(false);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {
            _selectedDictionary = _otherControlDictionary;
            testedDictionaryText.text = "그 외 설정 모드";
            characterTestPanel.SetActive(false);
            return;
        }

        if (Input.anyKeyDown)
        {
            foreach (var dic in _selectedDictionary)
            {
                if (Input.GetKeyDown(dic.Key))
                {
                    dic.Value();
                }
            }
        }
    }

    private Vector3 GetHitPoint()
    {
        int layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("PlayerSkill"));
        layerMask = ~layerMask;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return hit.point;
        }

        return Vector3.one;
    }

    #region Debug Message Call
    private void KickPlayer(int playerNumber)
    {
        var message = Network.CPacketFactory.CreateKickPlayer(playerNumber);

        Network.CTcpClient.instance.Send(message.data);
    }

    private void RequestNextRoom()
    {
        var message = Network.CPacketFactory.CreateDebugRequsstRoomTypeInfo();

        Network.CTcpClient.instance.Send(message.data);
    }
    #endregion
}
