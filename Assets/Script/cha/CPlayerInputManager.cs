using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPlayerInputManager : MonoBehaviour
{
    delegate void Action();

    private Dictionary<KeyCode, Action> keyDictionary;

    private CSkillUIManager _skillUIManager;
    private CTimerListUiManager _timerUiList;
    private CComboSelector _comboSelector;
    private Network.CTcpClient _networkClient;

    private List<CSkillFacade> _baseSkillList;
    private List<CSkillFacade> _comboSkillList;

    private bool _isCombo;
    
    private void Awake()
    {
        _baseSkillList = new List<CSkillFacade>();
        _comboSkillList = new List<CSkillFacade>();
        _skillUIManager = GameObject.Find("SkillScript").GetComponent<CSkillUIManager>();
        _timerUiList = GameObject.Find("SkillScript").GetComponent<CTimerListUiManager>();
        _networkClient = GameObject.Find("Network").GetComponent<Network.CTcpClient>();
        _comboSelector = new CComboSelector();

        _isCombo = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        _skillUIManager.RegisterTimer("SkillScript");
        _timerUiList.RegisterTimer("SkillScript");

        //// 스킬은 나중에 플레이어 관련 클래스에서 처리하도록 변경
        //_baseSkillList.Add(new CSkillFacade(5.0f, CSkillUIManager.EUIName.Base0));
        //_baseSkillList.Add(new CSkillFacade(7.0f, CSkillUIManager.EUIName.Base1));
        //_baseSkillList.Add(new CSkillFacade(9.0f, CSkillUIManager.EUIName.Base2));
        //_baseSkillList.Add(new CSkillFacade(10.0f, CSkillUIManager.EUIName.Base3));

        //for(int i = 0; i < 36; i++)
        //{
        //    _comboSkillList.Add(new CSkillFacade(10.5f + 1.0f * i));
        //}

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
            _baseSkillList[0].Use();
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
            _baseSkillList[1].Use();
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
            _baseSkillList[2].Use();
        }
    }

    private void CallCombo(int skillNumber)
    {
        _isCombo = false;
        _comboSkillList[skillNumber].Use();
    }

    private void KeyDownMouseLeft()
    {
        Debug.Log("Pressed Mouse Left");
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            Debug.LogFormat("Click Position : {0}, {1}, {2}", hit.point.x, hit.point.y, hit.point.z);
            _networkClient.SendMoveStop(hit.point.x, hit.point.z);
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
}
