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
    public Transform crossHairObject;
    
    private List<CSkillUi>[] _elementSkillLists = new List<CSkillUi>[3];
    private List<CSkillUi> _SelectElementList = new List<CSkillUi>();

    private GameObject _uiTarget;

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
                _elementSkillLists[i][j].preemptSkillNumber = -1;
                skillUiObject.GetChild(i).GetChild(j).gameObject.SetActive(false);
            }
        }

        for(int i = 0; i < 5; i++)
        {
            AddSkillUI(crossHairObject.GetChild(i), _SelectElementList);
            crossHairObject.GetChild(i).gameObject.SetActive(false);
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

    public void Register(GameObject uiTarget)
    {
        _uiTarget = uiTarget;
        CCharacterSkill charSkill = uiTarget.GetComponent<CCharacterSkill>();
        if(charSkill != null)
        {
            RegisterTimer(uiTarget);
            charSkill.skillSelectEvent.AddListener(ShowSelectSkill);

            if(charSkill is CPlayerSkill)
            {
                var playerSkill = charSkill as CPlayerSkill;
                playerSkill.mainElementLearnEvent.AddListener(ChangeMainElementUi);
                playerSkill.subElementLearnEvent.AddListener(ChangeSubElementUi);
                playerSkill.elementSelectEvent.AddListener(ShowSelectElement);
            }
        }
    }

    public void Deregister(GameObject uiTarget)
    {
        CCharacterSkill charSkill = uiTarget.GetComponent<CCharacterSkill>();
        if (charSkill != null)
        {
            DeregisterTimer(uiTarget);
            charSkill.skillSelectEvent.RemoveListener(ShowSelectSkill);

            if (charSkill is CPlayerSkill)
            {
                var playerSkill = charSkill as CPlayerSkill;
                playerSkill.mainElementLearnEvent.AddListener(ChangeMainElementUi);
                playerSkill.subElementLearnEvent.AddListener(ChangeSubElementUi);
                playerSkill.elementSelectEvent.RemoveListener(ShowSelectElement);
            }
        }
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

    public void ChangeMainElementUi(int mainElementIndex, int mainElementNumber)
    {
        for(int i = 0; i < 5; i++)
        {
            int skillNumber = _uiTarget.GetComponent<CPlayerSkill>().GetRegisterNumber(mainElementIndex, i-1);
            _elementSkillLists[mainElementIndex][i].preemptSkillNumber = skillNumber;

            if (skillNumber != -1)
            {
                var skillUiObejct = skillUiObject.GetChild(mainElementIndex).GetChild(i).gameObject;
                skillUiObejct.SetActive(true);
                skillUiObejct.GetComponent<Image>().sprite = GetImageByRegisterNumber(skillNumber);
            }
        }
    }

    public void ChangeSubElementUi(int subElementIndex, int subElementNumber)
    {
        for (int i = 0; i < 3; i++)
        {
            int skillNumber = _uiTarget.GetComponent<CPlayerSkill>().GetRegisterNumber(i, subElementIndex);
            _elementSkillLists[i][subElementIndex+1].preemptSkillNumber = skillNumber;

            if (skillNumber != -1)
            {
                var skillUiObejct = skillUiObject.GetChild(i).GetChild(subElementIndex+1).gameObject;
                skillUiObejct.SetActive(true);
                skillUiObejct.GetComponent<Image>().sprite = GetImageByRegisterNumber(skillNumber);
            }
        }
    }

    // 주원소의 스킬들을 보여준다
    // mainElementIndex가 -1일 경우 보여주지 않는다
    public void ShowSelectElement(int mainElementIndex)
    {
        if(mainElementIndex == -1)
        {
            foreach (var skillUi in _SelectElementList)
            {
                skillUi.ui.SetActive(false);
            }
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            _SelectElementList[i].preemptSkillNumber = _elementSkillLists[mainElementIndex][i].preemptSkillNumber;
            if (_SelectElementList[i].preemptSkillNumber != -1)
            {
                crossHairObject.GetChild(i).gameObject.SetActive(true);
                crossHairObject.GetChild(i).gameObject.GetComponent<Image>().sprite 
                    = GetImageByRegisterNumber(_SelectElementList[i].preemptSkillNumber);
            }
        }
    }

    // 주원소의 elementSkillNumber에 해당하는 스킬만 밝게 보여준다
    public void ShowSelectSkill(int elementSubSkillNumber)
    {
        for (int i = 0; i < _SelectElementList.Count; i++)
        {
            _SelectElementList[i].drawer.SetAlpha(i == (elementSubSkillNumber+1) ? 1f : 0.3f);
        }
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

        foreach(var elementSkill in _SelectElementList)
        {
            elementSkill.drawer.Draw(
                    _timer.GetCurrentCooldown(elementSkill.preemptSkillNumber),
                    _timer.GetMaxCooldown(elementSkill.preemptSkillNumber)
                );
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

    private Sprite GetImageByRegisterNumber(int registeredNumber)
    {
        if (!CSkillList.SkillList.TryGetValue(registeredNumber, out string spritePath))
        {
            return Resources.Load<Sprite>("Clean Vector Icons/T_0_empty_");
        }
        else
        {
            return Resources.Load<Sprite>(spritePath);
        }
    }

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
    private void SetImageAlpha(Image image, float alpha)
    {
        var tempColor = image.color;
        tempColor.a = alpha;
        image.color = tempColor;
    }
}
