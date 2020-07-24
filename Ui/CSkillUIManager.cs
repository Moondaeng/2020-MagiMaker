using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 스킬의 UI를 통합관리하는 클래스
 * 스킬이 UI 이름를 최대한 모르게 하고, UI를 같이 쓰는 일을 방지함
 * 스킬 타이머가 UI를 관리하는 일을 최소화함
 * 
 */ 
public class CSkillUIManager : MonoBehaviour
{
    public enum EUIName
    {
        Base, Combo1, Combo2, Combo3, Combo4
    }

    class CSkillUi
    {
        public GameObject ui;
        public Image image;
        public CTimerDrawer drawer;
        public int preemptSkillNumber;
    }

    public Transform skillUiObject;
    
    private List<CSkillUi>[] _elementSkillLists = new List<CSkillUi>[3];
    private CSkillTimer _timer;

    // 갱신 시간 조절
    protected const float _updateTime = 0.1f;
    protected int _updateThreshold;
    protected int _updateCount;
    
    public static CSkillUIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        _updateThreshold = (int)(_updateTime / Time.fixedDeltaTime);
        _updateCount = 0;

        Transform ElementUi;
        for (int i = 0; i < 3; i++)
        {
            _elementSkillLists[i] = new List<CSkillUi>();
            ElementUi = skillUiObject.GetChild(i);
            for (int j = 0; j < 5; j++)
            {
                var skillUi = ElementUi.GetChild(j);
                AddSkillUI(skillUi, _elementSkillLists[i]);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                DeregisterSkillUi(i, j);
            }
        }
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
    
    public void RegisterSkillUi(int elementUiNumber, int skillUiNumber, int skillNumber)
    {
        if (elementUiNumber < 0 || elementUiNumber >= 3) return;
        if (skillUiNumber < 0 || skillUiNumber >= 5) return;

        _elementSkillLists[elementUiNumber][skillUiNumber].preemptSkillNumber = skillNumber;
        skillUiObject.GetChild(elementUiNumber).GetChild(skillUiNumber).gameObject.SetActive(true);
    }
    
    public void DeregisterSkillUi(int elementUiNumber, int skillUiNumber)
    {
        if (elementUiNumber < 0 || elementUiNumber >= 3) return;
        if (skillUiNumber < 0 || skillUiNumber >= 5) return;

        _elementSkillLists[elementUiNumber][skillUiNumber].preemptSkillNumber = -1;
        skillUiObject.GetChild(elementUiNumber).GetChild(skillUiNumber).gameObject.SetActive(false);
    }

    // 모든 스킬 UI에 쿨타임을 그리는 명령을 내림
    // 단, 활성화되지 않은 경우 그리지 않음
    public void Draw()
    {
        foreach(var skillUiList in _elementSkillLists)
        {
            foreach(var skillUi in skillUiList)
            {
                skillUi.drawer.Draw(
                    _timer.GetCurrentCooldown(skillUi.preemptSkillNumber),
                    _timer.GetMaxCooldown(skillUi.preemptSkillNumber)
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
    
    //private void AddSprite(List<Sprite> sprites, string path)
    //{
    //    Sprite sprite = Resources.Load(path) as Sprite;
    //    if (sprite == null)
    //        Debug.LogFormat("sprite not find : " + path);
    //    sprites.Add(Resources.Load<Sprite>(path));
    //}

    private void AddSkillUI(Transform skillUi, List<CSkillUi> list)
    {
        var settingSkillUi = new CSkillUi()
        {
            ui = skillUi.gameObject,
            image = skillUi.GetComponent<Image>(),
            drawer = skillUi.GetComponent<CTimerDrawer>(),
            preemptSkillNumber = -1
        };
        list.Add(settingSkillUi);
    }

    private CSkillUi FindPreemptNumber(int registeredNumber)
    {
        foreach (var skillUiList in _elementSkillLists)
        {
            foreach (var skillUi in skillUiList)
            {
                if (skillUi.preemptSkillNumber == registeredNumber)
                {
                    return skillUi;
                }
            }
        }

        return null;
    }
}
