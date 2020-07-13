using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 스킬의 UI를 통합관리하는 클래스
 * 스킬이 UI 이름를 최대한 모르게 하고, UI를 같이 쓰는 일을 방지함
 * 스킬 타이머가 UI를 관리하는 일을 최소화함
 */ 
public class CSkillUIManager : MonoBehaviour
{
    public enum EUIName
    {
        Base0, Base1, Base2, Base3, Combo0, Combo1, Combo2, Combo3
    }

    class CSkillIUI
    {
        public GameObject ui;
        public Image image;
        public CTimerDrawer drawer;
        public int preemptSkillNumber;
    }
    
    private List<CSkillIUI> _comboList = new List<CSkillIUI>();
    private List<CSkillIUI> _baseList = new List<CSkillIUI>();

    private CSkillTimer _timer;

    // 갱신 시간 조절
    protected const float _updateTime = 0.1f;
    protected int _updateThreshold;
    protected int _updateCount;
    
    // 언제 어디서나 쉽게 접금할수 있도록 하기위해 만든 정적변수
    public static CSkillUIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        _updateThreshold = (int)(_updateTime / Time.fixedDeltaTime);
        _updateCount = 0;

        AddSkillUI("Combo 1", _comboList);
        AddSkillUI("Combo 2", _comboList);
        AddSkillUI("Combo 3", _comboList);
        AddSkillUI("Combo 4", _comboList);

        AddSkillUI("Skill 1", _baseList);
        AddSkillUI("Skill 2", _baseList);
        AddSkillUI("Skill 3", _baseList);
        AddSkillUI("Skill 4", _baseList);

        Preempt(EUIName.Base0, 0);
        Preempt(EUIName.Base1, 1);
        Preempt(EUIName.Base2, 2);
        Preempt(EUIName.Base3, 3);
    }

    // ui 갱신
    protected void FixedUpdate()
    {
        _updateCount++;

        // 업데이트 작동 횟수 조절
        if (_updateCount % _updateThreshold != 1)
        {
            return;
        }

        if(_timer != null)
            Draw();
        
    }

    // ParentName : 부모로 CSkillTimer를 들고 있는 Object
    public void RegisterTimer(GameObject timerOwner)
    {
        _timer = timerOwner.GetComponent<CSkillTimer>();
        _timer.TimerStart += CooldownEnable;
        _timer.TimerEnd += CooldownDisable;
    }

    public void DeregisterTimer(GameObject timerOwner)
    {
        _timer = timerOwner.GetComponent<CSkillTimer>();
        _timer.TimerStart -= CooldownEnable;
        _timer.TimerEnd -= CooldownDisable;
    }

    // 스킬 UI 등록
    // 중복 등록을 방지하여 한 스킬의 쿨타임만 돌아갈 수 있게 함
    public void Preempt(EUIName uiName, int registeredNumber)
    {
        int listPos = 0;
        if ((int)uiName < (int)EUIName.Combo0)
        {
            listPos = (int)uiName;
            _baseList[listPos].preemptSkillNumber = registeredNumber;
        }
        else
        {
            listPos = (int)uiName - (int)EUIName.Combo0;
            _comboList[listPos].preemptSkillNumber = registeredNumber;
        }
    }

    // 스킬 UI 등록 해제
    public void Free(EUIName uiName)
    {
        int listPos = 0;
        if((int)uiName < (int)EUIName.Combo0)
        {
            listPos = (int)uiName;
            _baseList[listPos].preemptSkillNumber = -1;
        }
        else
        {
            listPos = (int)uiName - (int)EUIName.Combo0;
            _comboList[listPos].preemptSkillNumber = -1;
        }
    }

    // 모든 스킬 UI에 쿨타임을 그리는 명령을 내림
    // 단, 활성화되지 않은 경우 그리지 않음
    public void Draw()
    {
        foreach(var skillUI in _baseList)
        {
            if(skillUI.preemptSkillNumber != -1)
            {
                skillUI.drawer.Draw(
                    _timer.GetCurrentCooldown(skillUI.preemptSkillNumber),
                    _timer.GetMaxCooldown(skillUI.preemptSkillNumber)
                    );
            }
        }

        foreach (var skillUI in _comboList)
        {
            if (skillUI.preemptSkillNumber != -1)
            {
                skillUI.drawer.Draw(
                    _timer.GetCurrentCooldown(skillUI.preemptSkillNumber),
                    _timer.GetMaxCooldown(skillUI.preemptSkillNumber)
                    );
            }
        }
    }

    public void CooldownEnable(int registeredNumber)
    {
        var skillUI = FindPreemptNumber(registeredNumber);

        if(skillUI != null)
        {
            skillUI.drawer.CooldownEnable();
        }
    }

    public void CooldownDisable(int registeredNumber)
    {
        var skillUI = FindPreemptNumber(registeredNumber);

        if (skillUI != null)
        {
            skillUI.drawer.CooldownDisable();
        }
    }
    
    private void AddSprite(List<Sprite> sprites, string path)
    {
        Sprite sprite = Resources.Load(path) as Sprite;
        if (sprite == null)
            Debug.LogFormat("sprite not find : " + path);
        sprites.Add(Resources.Load<Sprite>(path));
    }

    private void AddSkillUI(string uiName, List<CSkillIUI> list)
    {
        CSkillIUI skillUI = new CSkillIUI();
        skillUI.ui = GameObject.Find(uiName);
        skillUI.image = skillUI.ui.GetComponent<Image>();
        skillUI.drawer = skillUI.ui.GetComponent<CTimerDrawer>();
        skillUI.preemptSkillNumber = -1;
        list.Add(skillUI);
    }

    private CSkillIUI FindPreemptNumber(int registeredNumber)
    {
        foreach (var skillUI in _baseList)
        {
            if (skillUI.preemptSkillNumber == registeredNumber)
            {
                return skillUI;
            }
        }

        foreach (var skillUI in _comboList)
        {
            if (skillUI.preemptSkillNumber == registeredNumber)
            {
                return skillUI;
            }
        }

        return null;
    }
}
