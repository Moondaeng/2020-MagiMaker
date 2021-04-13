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
    private delegate void Action();

    #region 컨트롤러 모드 관리

    private bool _isControlMode = true;
    private Dictionary<KeyCode, Action> keyDictionary;
    private CMouseFollower _camera;

    #endregion 컨트롤러 모드 관리

    //private CConsumableItemViewer _consumableViewer;

    [SerializeField] private GameObject MousePointer;

    private CUIManager _playerUi;
    private CGameEvent gameEvent;

    public static CController instance;

    [SerializeField] public GameObject player;

    private CCntl _playerControl;

    // 이동 패킷 관련
    private const float moveTraceTime = 0.1f;

    private Vector3 previousPlayerPos;

    private float x;
    private float z;

    private GameObject _viewingObject;

    public RaycastHit hit;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // 조작 관리
        keyDictionary = new Dictionary<KeyCode, Action>
        {
            {KeyCode.Mouse0, Attack},
            {KeyCode.Space, Jump},
            //{KeyCode.Alpha1, () => SkillSelect(0) },
            //{KeyCode.Alpha2, () => SkillSelect(1) },
            //{KeyCode.Alpha3, () => SkillSelect(2) },
            //{KeyCode.Alpha4, () => SkillSelect(3) },
            {KeyCode.Q, ChangeConsumable },
            {KeyCode.E, UseConsumable },
            //{KeyCode.F, GetItem },
            {KeyCode.Z, Roll },
            //{KeyCode.Mouse1, UseSkill },
        };
    }

    private void Start()
    {
        // Singleton 선언해놓은 클래스들 받는 변수
        _playerUi = CUIManager.instance;
        gameEvent = CGameEvent.instance;
        _camera = CMouseFollower.instance;

        if (player != null)
        {
            _playerControl = player.GetComponent<CCntl>();
            SetControlCharacter(player);
            previousPlayerPos = player.transform.position;
            StartCoroutine("MoveTracer");
        }

        // Callback 전달
        CWindowFacade.instance.SetControlLockCallback = SetControlLock;
    }

    private void Update()
    {
        //int layerMask = 1 << LayerMask.NameToLayer("Player");
        //layerMask = ~layerMask;
        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        //{
        //    MousePointer.transform.position = hit.point;
        //}

        //ViewInteractionPopup();

        z = Input.GetAxisRaw("Horizontal");
        x = -(Input.GetAxisRaw("Vertical"));

        // 모드에 따라 조작되는 키
        if (_isControlMode)
        {
            _playerControl.Move(x, z);
        }

        if (Input.anyKeyDown && _isControlMode)
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

    private IEnumerator MoveTracer()
    {
        while (true)
        {
            // 비교
            var differ = previousPlayerPos - player.transform.position;
            if (differ.magnitude > 0.01f)
            {
                // 전송 - 이동 명령
                //Debug.Log($"move character {previousPlayerPos.x}, {previousPlayerPos.y}, {previousPlayerPos.z}" +
                //    $"to {player.transform.position.x}, {player.transform.position.y}, {player.transform.position.z}");
                gameEvent.PlayerMoveStart(previousPlayerPos, player.transform.position);
            }

            // 과거 위치 갱신
            previousPlayerPos = player.transform.position;
            yield return new WaitForSeconds(moveTraceTime);
        }
    }

    public void SetControlCharacter(GameObject controlCharacter)
    {
        player = controlCharacter;
        _playerUi.SetUiTarget(controlCharacter);
        CWindowFacade.instance.SetTarget(controlCharacter);
    }

    private void SetControlLock(bool isLock)
    {
        _camera.SetLockCursor(!isLock);
        _isControlMode = !isLock;
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
    // 내가 안쓰는 것들
    //private void ViewInteractionPopup()
    //{
    //    int layerMask = 1 << LayerMask.NameToLayer("Item");
    //    RaycastHit hit;
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray, out hit, 10, layerMask))
    //    {
    //        _viewingObject = hit.transform.gameObject;

    //        var playerPara = player.GetComponent<CPlayerPara>();
    //        if (playerPara == null)
    //        {
    //            return;
    //        }

    //        var npc = _viewingObject.GetComponent<CEventRoomNpcClick>();
    //        if (npc != null)
    //        {
    //            CInterationPopup.instance.gameObject.SetActive(true);
    //        }

    //        var itemComponent = _viewingObject.GetComponent<CItemComponent>();
    //        if (itemComponent != null)
    //        {
    //            CDropItemInfoPopup.instance.gameObject.SetActive(true);
    //            CDropItemInfoPopup.instance.DrawItemInfo(itemComponent.Item);
    //        }
    //    }
    //    else
    //    {
    //        CDropItemInfoPopup.instance.gameObject.SetActive(false);
    //        CInterationPopup.instance.gameObject.SetActive(false);
    //    }
    //}

    //private void GetItem()
    //{
    //    if (_viewingObject == null)
    //    {
    //        return;
    //    }

    //    var playerPara = player.GetComponent<CPlayerPara>();
    //    if (playerPara == null)
    //    {
    //        return;
    //    }

    //    var npc = _viewingObject.GetComponent<CEventRoomNpcClick>();
    //    if (npc != null)
    //    {
    //        CEventRoomNpcClick.instance.UseNPC();
    //    }

    //    var itemComponent = _viewingObject.GetComponent<CItemComponent>();
    //    if (itemComponent != null)
    //    {
    //        bool canAddItem = false;
    //        if (itemComponent.Item is Item.CEquip)
    //        {
    //            canAddItem = playerPara.Inventory.AddEquip(itemComponent.Item as Item.CEquip);
    //        }
    //        else if (itemComponent.Item is Item.CConsumable)
    //        {
    //            canAddItem = playerPara.Inventory.AddConsumableItem(itemComponent.Item as Item.CConsumable);
    //        }

    //        if (canAddItem)
    //        {
    //            itemComponent.gameObject.SetActive(false);
    //            _viewingObject = null;
    //        }
    //    }
    //}

    //private void SkillSelect(int index)
    //{
    //    player.GetComponent<CCharacterSkill>().SkillSelect(index);
    //}

    //private void UseSkill()
    //{
    //    int layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("PlayerSkill"));
    //    layerMask = ~layerMask;
    //    RaycastHit hit;
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
    //    {
    //        player.GetComponent<CPlayerSkill>().UseSkillToPosition(hit.point);
    //    }
    //}
}