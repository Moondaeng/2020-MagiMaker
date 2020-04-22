using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class CCntl : MonoBehaviour
{
    delegate void Action();

    private Dictionary<KeyCode, Action> keyDictionary;
    private CComboSelector _comboSelector;
    private bool _isCombo;

    GameObject player;
    private CPlayerSkill _cPlayerSkill;
    public bool telepotationCheck = true;
    [SerializeField]
    private GameObject[] spell;
    private GameObject temp, temp2;
    CPlayerPara myPara;
    
    // 콤보 스킬 배우기
    public void LearnComboSkill(int skillNumber)
    {
        _comboSelector.LearnSkill(skillNumber);
    }

    private void Awake()
    {
        _isCombo = false;
        // 콤보 스킬 세팅
        _comboSelector = new CComboSelector();
        _comboSelector.ReturnSkill = CallCombo;

        // 콤보 스킬 배움(임시 처리)
        _comboSelector.LearnSkill(0);
        _comboSelector.LearnSkill(1);
        _comboSelector.LearnSkill(2);
        _comboSelector.LearnSkill(3);
        _comboSelector.LearnSkill(7);
        _comboSelector.LearnSkill(12);
        _comboSelector.LearnSkill(17);
        _comboSelector.LearnSkill(25);
        _comboSelector.LearnSkill(31);
        _comboSelector.LearnSkill(35);

        // 조작 관리
        keyDictionary = new Dictionary<KeyCode, Action>
        {
            {KeyCode.Q, () => UseSkill(0) },
            {KeyCode.W, () => UseSkill(1) },
            {KeyCode.E, () => UseSkill(2) },
            {KeyCode.R, () => UseSkill(3) },
            {KeyCode.Space, () => Warp() },
            {KeyCode.Mouse0, CheckClick },
            {KeyCode.Mouse1, Attack },
            {KeyCode.Tab, StartCombo},
            {KeyCode.BackQuote, EndCombo}
        };
    }

    void Start()
    {
        // 플레이어 태그를 가진 개체를 받아옴
        player = GameObject.FindGameObjectWithTag("Player");
        _cPlayerSkill = player.GetComponent<CPlayerSkill>();
    }

    // 기본적으로 이동에 해당
    // 현재 적 캐릭터에게 타겟팅 공격 기능이 포함되어 있으나, 필요없다면 해당 부분 제외함
    private void CheckClick()
    {
        // Ray API 참고 직선 방향으로 나아가는 무한대 길이의 선
        // 카메라 클래스의 메인 카메라로 부터의 스크린의 점을 통해 레이 반환
        // 여기서 스크린의 점은, Input.mousePosition으로 받음
        // 포지션.z를 무시한 값임.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            string s = hit.collider.gameObject.name;
            string s2 = hit.collider.gameObject.tag;
            //Debug.Log(s);
            //Debug.Log(s2);
            bool stringExists = s.Contains("Terrain");
            // 누른게 지형이면, 이동함.
            if (stringExists)
            {
                 player.GetComponent<CPlayerFSM>().MoveTo(hit.point);
            }
            else if (s2 == "Monster")//마우스 클릭한 대상이 적 캐릭터인 경우
            {
                 //player.GetComponent<CPlayerFSM>().
                 player.GetComponent<CPlayerFSM>().AttackEnemy(hit.collider.gameObject);
            }
        }
    }

    void RotateMotion()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            player.GetComponent<CPlayerFSM>().TurnTo(hit.point);
            //player.GetComponent<CPlayerFSM>().SkillState();
        }
    }

    void WarpMotion()
    {
        Destroy(temp2);
        player.GetComponent<CPlayerAni>().ChangeAni(CPlayerAni.ANI_WARP);
        temp = Instantiate(spell[0], player.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
        Invoke("WarpToDestination",1f);
    }

    void WarpToDestination()
    {
        Destroy(temp);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            string s = hit.collider.gameObject.name;
            bool stringExists = s.Contains("Terrain");
            // 누른게 지형이면, 이동함.
            if (stringExists)
            {
                temp2 = Instantiate(spell[0], hit.point + new Vector3(0, 0.2f, 0), Quaternion.identity);
                player.transform.position = hit.point;
            }
        }
        player.GetComponent<CPlayerAni>().ChangeAni(CPlayerAni.ANI_IDLE);
    }

    private IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(1.0f);
        telepotationCheck = true;
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
    // 
    private void UseSkill(int number)
    {
        Debug.Log(player.GetComponent<CPlayerFSM>().currentState);
        player.GetComponent<CPlayerFSM>().SkillState();

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
                _cPlayerSkill.UseSkillToPosition(number, hit.point);
            }
        }
    }

    private void Attack()
    {
        RotateMotion();
        player.GetComponent<CPlayerFSM>().AttackState();
        // RealAttack();
        
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
            _cPlayerSkill.UseComboSkillToPosition(skillNumber, hit.point);
        }
    }

    private void UseSkillToMousePos(CSkillFormat skillFormat)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            skillFormat.Use(hit.point);
        }
    }
}
