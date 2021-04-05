using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTestController : MonoBehaviour
{
    delegate void Action();

    private Dictionary<KeyCode, Action> keyDictionary;

    public CPlayerCommand commander;
    public CUIManager ui;
    public CWaitingForAccept waiting;
    public CMonsterManager monsterManager;

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

    private void Start()
    {
        // 조작 관리
        keyDictionary = new Dictionary<KeyCode, Action>
        {
            // Character Add / Delete
            //[KeyCode.Insert] = () => commander.SetActivePlayers(4),   // 무한루프 버그 때문에 봉인
            [KeyCode.Delete] = () => KickPlayer(_selectedCharacterNumber),
            // Character Control
            [KeyCode.Alpha5]    = () => _selectedCharacterNumber = 0,
            [KeyCode.Alpha6]    = () => _selectedCharacterNumber = 1,
            [KeyCode.Alpha7]    = () => _selectedCharacterNumber = 2,
            [KeyCode.Alpha8]    = () => _selectedCharacterNumber = 3,
            [KeyCode.U]         = () => commander.SetMyCharacter(_selectedCharacterNumber),
            [KeyCode.O]         = () => commander.DamageToCharacter(_selectedCharacterNumber, 300),
            [KeyCode.J]         = () => commander.Follow(_selectedCharacterNumber),
            [KeyCode.K]         = () => commander.Call(_selectedCharacterNumber),
            [KeyCode.L]         = () => commander.SkillTo(_selectedCharacterNumber),
            [KeyCode.M]         = () => CWaitingForAccept.instance.SetPortalUseSelect(_selectedCharacterNumber, CWaitingForAccept.EAccept._accept),
            [KeyCode.Comma]     = () => CWaitingForAccept.instance.SetPortalUseSelect(_selectedCharacterNumber, CWaitingForAccept.EAccept._cancle),
            // Item
            [KeyCode.KeypadPlus] = () => CItemDropTable.SetItemToDropState(
                    CItemDropTable.instance.PopRandomItemByGrade(CItemDropTable.ItemGrade.Normal, CConstants.EQUIP_ITEM_TYPE),
                    GetHitPoint()),
            [KeyCode.KeypadMinus] = () => CItemDropTable.SetItemToDropState(
                    CItemDropTable.instance.PopRandomItemByGrade(CItemDropTable.ItemGrade.Normal, CConstants.CONSUM_ITEM_TYPE),
                    GetHitPoint()),
        };
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (var dic in keyDictionary)
            {
                if (Input.GetKeyDown(dic.Key))
                {
                    dic.Value();
                }
            }
        }

        if (monsterManager == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            monsterManager.DestroyAllMonsters();
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

    private void KickPlayer(int playerNumber)
    {
        var message = Network.CPacketFactory.CreateKickPlayer(playerNumber);

        Network.CTcpClient.instance.Send(message.data);
    }
}
