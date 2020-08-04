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

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 조작 관리
        keyDictionary = new Dictionary<KeyCode, Action>
        {
            {KeyCode.Alpha1, () => SkillSelect(0) },
            {KeyCode.Alpha2, () => SkillSelect(1) },
            {KeyCode.Alpha3, () => SkillSelect(2) },
            {KeyCode.Alpha4, () => SkillSelect(3) },
            {KeyCode.Mouse1, UseSkill },
        };
        //keyDictionary = new Dictionary<KeyCode, Action>
        //{
        //    {KeyCode.Tab, ChangeElement },
        //    {KeyCode.Alpha1, () => UseSkill(1) },
        //    {KeyCode.Alpha2, () => UseSkill(2) },
        //    {KeyCode.Alpha3, () => UseSkill(3) },
        //    {KeyCode.Alpha4, () => UseSkill(4) },
        //    {KeyCode.Mouse1, () => UseSkill(0) },
        //};

        // 콤보 스킬 세팅
        _comboSelector = new CSkillSelector();

        //gameEvent = GameObject.Find("GameEvent").GetComponent<CGameEvent>();
    }

    void Start()
    {
        _comboSelector.SetMainElement(0, CSkillSelector.SkillElement.Fire);
        _comboSelector.SetMainElement(1, CSkillSelector.SkillElement.Water);
        _comboSelector.SetSubElement(0, CSkillSelector.SkillElement.Water);
        _comboSelector.SetSubElement(1, CSkillSelector.SkillElement.Earth);
        _comboSelector.SetSubElement(2, CSkillSelector.SkillElement.Wind);
        _comboSelector.SetSubElement(3, CSkillSelector.SkillElement.Light);
    }

    // 임시 방안
    public void DeselectEnemy()
    {
        //player.GetComponent<CCntl>().CurrentEnemyDead();
    }

    // 기본적으로 이동에 해당
    // 현재 적 캐릭터에게 타겟팅 공격 기능이 포함되어 있으나, 필요없다면 해당 부분 제외함
    //private void Move()
    //{
    //    // Ray API 참고 직선 방향으로 나아가는 무한대 길이의 선
    //    // 카메라 클래스의 메인 카메라로 부터의 스크린의 점을 통해 레이 반환
    //    // 여기서 스크린의 점은, Input.mousePosition으로 받음
    //    // 포지션.z를 무시한 값임.
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        GameObject hitObject = hit.collider.gameObject;
    //        string s = hitObject.name;
    //        string s2 = hitObject.tag;
    //        bool stringExists = s2.Contains("Terrain");
    //        // 누른게 지형이면, 이동함.
    //        if (stringExists)
    //        {
    //            //gameEvent.PlayerMoveStart(player.transform.position, hit.point);
    //            player.GetComponent<CCntl>().MoveTo(hit.point);
    //        }
    //        else if (s2 == "Monster")//마우스 클릭한 대상이 적 캐릭터인 경우
    //        {
    //            player.GetComponent<CCntl>().TargetEnemy(hit.collider.gameObject);
    //        }
    //        else if (s2 == "ITEM")
    //        {
    //            var itemInfo = hitObject.GetComponent<CItemInformation>();
    //            player.SendMessage("EquipItem", itemInfo._item);
    //            //player.transform.GetChild(0).SendMessage("EquipItem", itemInfo._item);
    //            Destroy(hitObject);
    //        }
    //    }
    //}

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

    #region UseSkill1
    private void SkillSelect(int index)
    {
        _comboSelector.Combo(index);
    }

    // 스킬 사용
    private void UseSkill()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
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
    #endregion

    #region UseSkill2
    private void ChangeElement()
    {
        _comboSelector.ChangeElement();
    }

    private void UseSkill(int index)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //int comboSkillNum = _comboSelector.EndCombo();
            int comboSkillNum = _comboSelector.PickElementSkill(index);
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
    #endregion

    private void Attack()
    {
        RotateMotion();
        //player.GetComponent<CCntl>().AttackState();
    }
}
