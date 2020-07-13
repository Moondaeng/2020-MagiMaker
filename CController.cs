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
    private CComboSelector _comboSelector;
    private bool _isCombo;
    public CGameEvent gameEvent;

    public GameObject player;
    public bool telepotationCheck = true;
    [SerializeField]
    private GameObject[] spell;
    private GameObject temp, temp2;
    CPlayerPara myPara;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _isCombo = false;

        // 조작 관리
        keyDictionary = new Dictionary<KeyCode, Action>
        {
            {KeyCode.Q, () => UseSkill(0) },
            {KeyCode.W, () => UseSkill(1) },
            {KeyCode.E, () => UseSkill(2) },
            {KeyCode.R, () => UseSkill(3) },
            {KeyCode.Space, () => Warp() },
            {KeyCode.Mouse0, Move },
            {KeyCode.Mouse1, Attack },
            {KeyCode.Tab, StartCombo},
            {KeyCode.BackQuote, EndCombo}
        };

        gameEvent = GameObject.Find("GameEvent").GetComponent<CGameEvent>();
    }

    void Start()
    {
        // 콤보 스킬 세팅
        //_comboSelector = new CComboSelector(player);
        _comboSelector = new CComboSelector(player.transform.GetChild(0).gameObject);
        _comboSelector.ReturnSkill = CallCombo;
    }

    // 임시 방안
    public void DeselectEnemy()
    {
        player.GetComponent<CCntl>().CurrentEnemyDead();
    }

    // 기본적으로 이동에 해당
    // 현재 적 캐릭터에게 타겟팅 공격 기능이 포함되어 있으나, 필요없다면 해당 부분 제외함
    private void Move()
    {
        // Ray API 참고 직선 방향으로 나아가는 무한대 길이의 선
        // 카메라 클래스의 메인 카메라로 부터의 스크린의 점을 통해 레이 반환
        // 여기서 스크린의 점은, Input.mousePosition으로 받음
        // 포지션.z를 무시한 값임.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            string s = hitObject.name;
            string s2 = hitObject.tag;
            bool stringExists = s2.Contains("Terrain");
            // 누른게 지형이면, 이동함.
            if (stringExists)
            {
                gameEvent.PlayerMoveStart(player.transform.position, hit.point);
                player.GetComponent<CCntl>().MoveTo(hit.point);
            }
            else if (s2 == "Monster")//마우스 클릭한 대상이 적 캐릭터인 경우
            {
                player.GetComponent<CCntl>().TargetEnemy(hit.collider.gameObject);
            }
            else if(s2 == "ITEM")
            {
                var itemInfo = hitObject.GetComponent<CItemInformation>();
                player.transform.GetChild(0).SendMessage("EquipItem", itemInfo._item);
                Destroy(hitObject);
            }
        }
    }

    void RotateMotion()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //player.GetComponent<CPlayerFSM>().TurnTo(hit.point);
            //player.GetComponent<CPlayerFSM>().SkillState();
        }
    }

    void Update()
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
    }

    // 스킬 사용
    private void UseSkill(int number)
    {
        if (_isCombo)
        {
            _comboSelector.Combo(number);
        }
        else
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                bool? isSkillUse = player.transform.GetChild(0).GetComponent<CPlayerSkill>().UseSkillToPosition(number, hit.point);
                // 성공 시 플레이어 스킬 사용 이벤트 수행
                if(isSkillUse == true)
                {
                    player.GetComponent<CCntl>().SkillAction(2, hit.point);
                    gameEvent.PlayerAction(number, player.transform.position, hit.point);
                }
            }
        }
    }

    private void Attack()
    {
        RotateMotion();
        //player.GetComponent<CCntl>().AttackState();
    }

    // 워프 스킬
    // 이제 캐릭터의 쿨타임과 연계됨
    private void Warp()
    {
        telepotationCheck = false;
        Destroy(temp2);
        player.GetComponent<CPlayerAni>().ChangeAni(CPlayerAni.ANI_WARP);
        temp = Instantiate(spell[0], player.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        Invoke("WarpToDestination", 1f);
    }

    private void StartCombo()
    {
        Debug.Log("Pressed Tab");
        _isCombo = true;
        _comboSelector.DrawCurrentState();
        _comboSelector.DrawSelectableSkill();
    }

    private void EndCombo()
    {
        Debug.Log("Pressed BackQuote");
        _isCombo = false;
        _comboSelector.EndCombo();
    }

    private void CallCombo(int skillNumber)
    {
        _isCombo = false;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            bool? isSkillUse =  player.transform.GetChild(0).GetComponent<CPlayerSkill>().UseComboSkillToPosition(skillNumber, hit.point);
            if(isSkillUse == true)
            {

            }
        }
        //_comboSelector.EndCombo();
    }
}
