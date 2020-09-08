using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 플레이어 오브젝트 조작 인터페이스 클래스
 * 
 */
public class CController : MonoBehaviour
{
    delegate void Action();

    private Dictionary<KeyCode, Action> keyDictionary;
    private CSkillSelector _comboSelector;
    //public CGameEvent gameEvent;

    public GameObject player;

    private CCntl _playerControl;

    float x;
    float z;

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
            {KeyCode.Mouse1, UseSkill },
        };

        //gameEvent = GameObject.Find("GameEvent").GetComponent<CGameEvent>();
    }

    void Start()
    {
        // 콤보 스킬 세팅
        _comboSelector = new CSkillSelector();

        _comboSelector.SetMainElement(0, CSkillSelector.SkillElement.Fire);
        _comboSelector.SetMainElement(1, CSkillSelector.SkillElement.Water);
        _comboSelector.SetSubElement(0, CSkillSelector.SkillElement.Water);
        _comboSelector.SetSubElement(1, CSkillSelector.SkillElement.Earth);
        _comboSelector.SetSubElement(2, CSkillSelector.SkillElement.Wind);
        _comboSelector.SetSubElement(3, CSkillSelector.SkillElement.Light);

        if (player != null)
        {
            _playerControl = player.GetComponent<CCntl>();
        }
    }

    // 임시 방안
    public void DeselectEnemy()
    {
        //player.GetComponent<CCntl>().CurrentEnemyDead();
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

    private void Attack()
    {
        _playerControl.Attack();
    }

    private void Jump()
    {
        _playerControl.Jump();
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

    private void GetItem()
    {
        print("Get Item");
        int layerMask = 1 << LayerMask.NameToLayer("Item");
        print(layerMask);
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 10, layerMask))
        {
            print("I'm looking at " + hit.transform.name);

        }

        var playerPara = player.GetComponent<CPlayerPara>();
        if (playerPara != null)
        {
            var item = hit.transform.gameObject.GetComponent<CEquipComponent>();
            if(item != null)
            {
                bool tryAddEquip = playerPara.Inventory.AddEquip(item.equipStat);
                if (tryAddEquip)
                {
                    hit.transform.gameObject.SetActive(false);
                }
            }
            else
            {
                var consumable = hit.transform.gameObject.GetComponent<CConsumableComponent>();
                bool tryAddConsumable = playerPara.Inventory.AddConsumableItem(consumable.ConsumableStat);
                if (tryAddConsumable)
                {
                    hit.transform.gameObject.SetActive(false);
                }
            }
        }
    }
    
    private void SkillSelect(int index)
    {
        _comboSelector.Combo(index);
    }

    // 스킬 사용
    private void UseSkill()
    {
        int layerMask = 1 << 9;
        layerMask = ~layerMask;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            print("I'm looking at " + hit.transform.name);
            int comboSkillNum = _comboSelector.EndCombo();
            if (comboSkillNum == -1) return;
            bool? isSkillUse = player.GetComponent<CPlayerSkill>().UseSkillToPosition(comboSkillNum, hit.point);
            // 성공 시 플레이어 스킬 사용 이벤트 수행
            if (isSkillUse == true)
            {
                //player.GetComponent<CCntl>().SkillAction(2, hit.point);
                //gameEvent.PlayerAction(number, player.transform.position, hit.point);
            }
        }
    }
}
