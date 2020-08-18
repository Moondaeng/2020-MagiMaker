using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

<<<<<<< HEAD
public class CPlayerInputManager : MonoBehaviour
{
    delegate void Action();

    private Dictionary<KeyCode, Action> keyDictionary;

    private CSkillUIManager _skillUIManager;
    private CTimerListUiManager _timerUiList;
    private CComboSelector _comboSelector;
    private CGameEvent _gameEvent;
    private CProjectileSkill _projectileSkill;
    private CBuffSkill _buffSkill;

    private List<CSkillFormat> _baseSkillList;
    private List<CSkillFormat> _comboSkillList;

    private bool _isCombo;
    
    private void Awake()
    {
        _baseSkillList = new List<CSkillFormat>();
        _comboSkillList = new List<CSkillFormat>();
        _skillUIManager = GameObject.Find("SkillScript").GetComponent<CSkillUIManager>();
        _timerUiList = GameObject.Find("SkillScript").GetComponent<CTimerListUiManager>();
        _projectileSkill = GameObject.Find("SkillScript").GetComponent<CProjectileSkill>();
        _buffSkill = GameObject.Find("SkillScript").GetComponent<CBuffSkill>();
        _gameEvent = GameObject.Find("GameEvent").GetComponent<CGameEvent>();
        _comboSelector = new CComboSelector();

        _isCombo = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        _skillUIManager.RegisterTimer("SkillScript");
        _timerUiList.RegisterTimer("SkillScript");

        // 스킬은 나중에 플레이어 관련 클래스에서 처리하도록 변경
        // 기본 스킬 포맷 추가
        _baseSkillList.Add(new CSkillFormat(0, 1.0f));
        _baseSkillList.Add(new CSkillFormat(1, 7.0f));
        _baseSkillList.Add(new CSkillFormat(2, 9.0f));
        _baseSkillList.Add(new CSkillFormat(3, 10.0f));

        // 기본 스킬 등록
        _baseSkillList[0].RegisterSkill(_projectileSkill.Fireball);
        _baseSkillList[1].RegisterSkill(_buffSkill.DefenceUp);

        // 콤보 스킬 포맷 등록
        for (int i = 0; i < 36; i++)
        {
            _comboSkillList.Add(new CSkillFormat(4 + i, 10.5f + 1.0f * i));
        }

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

        _comboSelector.ReturnSkill = CallCombo;

        keyDictionary = new Dictionary<KeyCode, Action>
        {
            {KeyCode.Q, KeyDownQ },
            {KeyCode.W, KeyDownW },
            {KeyCode.E, KeyDownE },
            {KeyCode.Mouse0, KeyDownMouseLeft },
            {KeyCode.Mouse1, KeyDownMouseRight },
            {KeyCode.Tab, StartCombo},
            {KeyCode.BackQuote, EndCombo}
        };
    }

    // Update is called once per frame
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

    private void KeyDownQ()
    {
        Debug.Log("Pressed Keyboard Q");
        if(_isCombo)
        {
            _comboSelector.Combo(0);
            _comboSelector.DrawCurrentState();
            _comboSelector.DrawSelectableSkill();
        }
        else
        {
            UseSkillToMousePos(_baseSkillList[0]);
        }
    }

    private void KeyDownW()
    {
        Debug.Log("Pressed Keyboard W");
        
        if (_isCombo)
        {
            _comboSelector.Combo(1);
            _comboSelector.DrawCurrentState();
            _comboSelector.DrawSelectableSkill();
        }
        else
        {
            UseSkillToMousePos(_baseSkillList[1]);
        }
    }

    private void KeyDownE()
    {
        Debug.Log("Pressed Keyboard E");
        if (_isCombo)
        {
            _comboSelector.Combo(2);
            _comboSelector.DrawCurrentState();
            _comboSelector.DrawSelectableSkill();
        }
        else
        {
            UseSkillToMousePos(_baseSkillList[2]);
        }
    }

    private void CallCombo(int skillNumber)
    {
        _isCombo = false;
        UseSkillToMousePos(_comboSkillList[skillNumber]);
    }

    private void KeyDownMouseLeft()
    {
        Debug.Log("Pressed Mouse Left");
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            Debug.LogFormat("Click Position : {0}, {1}, {2}", hit.point.x, hit.point.y, hit.point.z);
            _gameEvent.PlayerMoveStop(new Tuple<float, float>(hit.point.x, hit.point.z));
        }
    }

    private void KeyDownMouseRight()
    {
        Debug.Log("Pressed Mouse Right");
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

    private void UseSkillToMousePos(CSkillFormat skillFormat)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            skillFormat.Use(hit.point);
        }
    }
=======
/*
 * 키 세팅 관리 클래스
 * 이후 키 커스터마이징 기능 지원 시 여기서 관리
 */
public class CPlayerInputManager : MonoBehaviour
{
    
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
}
