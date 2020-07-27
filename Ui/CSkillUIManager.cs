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

    public List<Sprite> _comboClassSprites = new List<Sprite>();
    public List<Sprite> _comboSubjectSprites = new List<Sprite>();
    public List<Sprite> _comboSkillSprites = new List<Sprite>();

    private List<CSkillIUI> _comboList = new List<CSkillIUI>();
    private List<CSkillIUI> _baseList = new List<CSkillIUI>();

    public GameObject comboOn;
    public GameObject comboClass;
    public GameObject comboSubject;
    public GameObject comboNext;
    public GameObject comboNext2;

    private CSkillTimer _timer;

    // 갱신 시간 조절
    protected const float _updateTime = 0.1f;
    protected int _updateThreshold;
    protected int _updateCount;

    private void Awake()
    {
        _updateThreshold = (int)(_updateTime / Time.fixedDeltaTime);
        _updateCount = 0;

        //AddSprite(_comboClassSprites, "Sprite/skill_attack.png");
        //AddSprite(_comboClassSprites, "Sprite/skill_defence.png");
        //AddSprite(_comboClassSprites, "Sprite/skill_support.png");

        //AddSprite(_comboSubjectSprites, "Sprite/skill_empower.png");
        //AddSprite(_comboSubjectSprites, "Sprite/skill_area.png");
        //AddSprite(_comboSubjectSprites, "Sprite/skill_multi.png");
        //AddSprite(_comboSubjectSprites, "Sprite/skill_utility.png");

        //AddSprite(_comboSkillSprites, "Sprite/skill_QQQ");
        //AddSprite(_comboSkillSprites, "Sprite/skill_QQW");
        //AddSprite(_comboSkillSprites, "Sprite/skill_QQE");
        //AddSprite(_comboSkillSprites, "Sprite/skill_QWQ");
        //AddSprite(_comboSkillSprites, "Sprite/skill_QWW");
        //AddSprite(_comboSkillSprites, "Sprite/skill_QWE");
        //AddSprite(_comboSkillSprites, "Sprite/skill_QEQ");
        //AddSprite(_comboSkillSprites, "Sprite/skill_QEW");
        //AddSprite(_comboSkillSprites, "Sprite/skill_QEE");

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

    private void Start()
    {
        // 시작 시 콤보는 꺼져있는 상태이므로 UI를 끔
        DeactivateAll();
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

        Draw();
    }

    // ParentName : 부모로 CSkillTimer를 들고 있는 Object
    public void RegisterTimer(string parentName)
    {
        _timer = GameObject.Find(parentName).GetComponent<CSkillTimer>();
        _timer.TimerStart += CooldownEnable;
        _timer.TimerEnd += CooldownDisable;
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

    public void ActiveComboOn(bool value)
    {
        comboOn.SetActive(value);
    }

    public void ActivateComboUI(int comboUINumber)
    {
        _comboList[comboUINumber].ui.SetActive(true);
    }

    public void DrawComboClassState(int imageNumber)
    {
        comboClass.SetActive(true);
        comboNext.SetActive(true);
        comboClass.GetComponent<Image>().sprite = _comboClassSprites[imageNumber];
    }

    public void DrawComboSubjectState(int imageNumber)
    {
        comboSubject.SetActive(true);
        comboNext2.SetActive(true);
        comboSubject.GetComponent<Image>().sprite = _comboSubjectSprites[imageNumber];
    }

    // 이쪽은 네이밍이 개판났으므로 차후 수정 예정
    public void DeactivateAll()
    {
        DeactivateComboUIAll();
        ActiveComboOn(false);
        comboClass.SetActive(false);
        comboNext.SetActive(false);
        comboSubject.SetActive(false);
        comboNext2.SetActive(false);
    }

    public void DeactivateComboUIAll()
    {
        for(int combo = 0; combo < _comboList.Count; combo++)
        {
            DeactivateComboUI(combo);
        }
    }

    public void DeactivateComboUI(int comboUINumber)
    {
        _comboList[comboUINumber].ui.SetActive(false);
    }

    //public void SetBaseImage(int baseNumber, string baseImage)
    //{
    //    _baseList[baseNumber].image.sprite = Resources.Load<Sprite>(baseImage);
    //}

    public void SetComboClassImage(int comboNumber, int imageNumber)
    {
        _comboList[comboNumber].image.sprite = _comboClassSprites[imageNumber];
    }

    public void SetComboSubejectImage(int comboNumber, int imageNumber)
    {
        _comboList[comboNumber].image.sprite = _comboSubjectSprites[imageNumber];
    }

    public void SetComboSkillImage(int comboNumber, int imageNumber)
    {
        _comboList[comboNumber].image.sprite = _comboSkillSprites[imageNumber];
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
