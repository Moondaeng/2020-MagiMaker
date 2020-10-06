using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어 오브젝트 조작 인터페이스 클래스
 * 
 */
[DisallowMultipleComponent]
public class CController : MonoBehaviour
{
    delegate void Action();

    private Dictionary<KeyCode, Action> keyDictionary;
    private CConsumableItemViewer _consumableViewer;

    private CUIManager _playerUi;
    private Network.CTcpClient _network;
    //public CGameEvent gameEvent;

    public GameObject player;

    private CCntl _playerControl;

    float x;
    float z;

    private GameObject _viewingObject;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 조작 관리
        keyDictionary = new Dictionary<KeyCode, Action>
        {
            {KeyCode.Mouse0, Attack},
            {KeyCode.Space, Jump},
            {KeyCode.Alpha1, () => SkillSelect(0) },
            {KeyCode.Alpha2, () => SkillSelect(1) },
            {KeyCode.Alpha3, () => SkillSelect(2) },
            {KeyCode.Alpha4, () => SkillSelect(3) },
            {KeyCode.Q, ChangeConsumable },
            {KeyCode.E, UseConsumable },
            {KeyCode.F, GetItem },
            {KeyCode.Z, Roll },
            {KeyCode.Mouse1, UseSkill },
        };

        //gameEvent = GameObject.Find("GameEvent").GetComponent<CGameEvent>();
    }

    void Start()
    {
        _playerUi = CUIManager.instance;

        if (player != null)
        {
            _playerControl = player.GetComponent<CCntl>();
        }
    }

    void Update()
    {
        z = Input.GetAxisRaw("Horizontal");
        x = -(Input.GetAxisRaw("Vertical"));
        if (player != null)
        {
            _playerControl = player.GetComponent<CCntl>();
        }
        _playerControl.Move(x, z);
        ViewInteractionPopup();

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
    }

    public void SetControlCharacter(GameObject controlCharacter)
    {
        player = controlCharacter;
        _playerUi.SetUiTarget(controlCharacter);
    }
    
    private void Attack()
    {
        _playerControl.Attack();
    }

    private void Skill()
    {
        _playerControl.Skill();
    }

    private void Jump()
    {
        _playerControl.Jump();
    }

    private void Roll()
    {
        _playerControl.Roll();
    }

    private void ChangeConsumable()
    {
        var playerPara = player.GetComponent<CPlayerPara>();
        if (playerPara != null)
        {
            playerPara.Inventory.GetNextConsumable();
        }
    }

    private void UseConsumable()
    {
        var playerPara = player.GetComponent<CPlayerPara>();
        if (playerPara != null)
        {
            playerPara.Inventory.UseSelectedConsumable();
        }
    }

    private void ViewInteractionPopup()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Item");
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 10, layerMask))
        {
            _viewingObject = hit.transform.gameObject;

            var playerPara = player.GetComponent<CPlayerPara>();
            if (playerPara == null)
            {
                return;
            }

            var npc = _viewingObject.GetComponent<CEventRoomNpcClick>();
            if (npc != null)
            {
                //CEventRoomNpcClick.instance.UseNPC();
            }

            var itemComponent = _viewingObject.GetComponent<CItemComponent>();
            if (itemComponent != null)
            {
                CDropItemInfoPopup.instance.gameObject.SetActive(true);
                CDropItemInfoPopup.instance.DrawItemInfo(itemComponent.Item);
            }
        }
        else
        {
            CDropItemInfoPopup.instance.gameObject.SetActive(false);
        }
    }

    private void GetItem()
    {
        if(_viewingObject == null)
        {
            return;
        }

        var playerPara = player.GetComponent<CPlayerPara>();
        if (playerPara == null)
        {
            return;
        }

        var npc = _viewingObject.GetComponent<CEventRoomNpcClick>();
        if (npc != null)
        {
<<<<<<< HEAD
            var npc = hit.transform.gameObject.GetComponent<CEventRoomNpcClick>();

            if (npc != null)
            {
                CEventRoomNpcClick.instance.UseNPC();
            }

            var item = hit.transform.gameObject.GetComponent<CEquipComponent>();

            if (item != null)
=======
            //CEventRoomNpcClick.instance.UseNPC();
        }

        var itemComponent = _viewingObject.GetComponent<CItemComponent>();
        if (itemComponent != null)
        {
            bool canAddItem = false;
            if (itemComponent.Item is Item.CEquip)
>>>>>>> a61cd1855e2c045ac12a1cf948cb08d8ede0c189
            {
                canAddItem = playerPara.Inventory.AddEquip(itemComponent.Item as Item.CEquip);
            }
            else if (itemComponent.Item is Item.CConsumable)
            {
                canAddItem = playerPara.Inventory.AddConsumableItem(itemComponent.Item as Item.CConsumable);
            }

            if (canAddItem)
            {
                itemComponent.gameObject.SetActive(false);
                _viewingObject = null;
            }
        }
    }

    private void SkillSelect(int index)
    {
        player.GetComponent<CCharacterSkill>().SkillSelect(index);
    }

    // 스킬 사용
    private void UseSkill()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Player");
        layerMask = ~layerMask;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            print("I'm looking at " + hit.transform.name);
            player.GetComponent<CPlayerSkill>().UseSkillToPosition(hit.point);
        }
    }
}
