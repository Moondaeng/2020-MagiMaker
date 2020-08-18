using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 스킬의 UI를 통합관리하는 클래스
 * 스킬이 UI 이름를 최대한 모르게 하고, UI를 같이 쓰는 일을 방지함
 * 스킬 타이머가 UI를 관리하는 일을 최소화함
<<<<<<< HEAD
=======
 * 
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
 */ 
public class CSkillUIManager : MonoBehaviour
{
    public enum EUIName
    {
<<<<<<< HEAD
        Base0, Base1, Base2, Base3, Combo0, Combo1, Combo2, Combo3
    }


    class CSkillIUI
=======
        Base, Combo1, Combo2, Combo3, Combo4
    }

    class CSkillUi
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    {
        public GameObject ui;
        public Image image;
        public CTimerDrawer drawer;
        public int preemptSkillNumber;
    }

<<<<<<< HEAD
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

=======
    public Transform skillUiObject;
    public Transform crossHairObject;
    
    private List<CSkillUi>[] _elementSkillLists = new List<CSkillUi>[3];
    private List<CSkillUi> _SelectElementList = new List<CSkillUi>();
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    private CSkillTimer _timer;

    // 갱신 시간 조절
    protected const float _updateTime = 0.1f;
    protected int _updateThreshold;
    protected int _updateCount;
<<<<<<< HEAD

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
=======
    
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
                DeregisterSkillUi(i, j);
            }
        }

        for(int i = 0; i < 5; i++)
        {
            AddSkillUI(crossHairObject.GetChild(i), _SelectElementList);
            crossHairObject.GetChild(i).gameObject.SetActive(false);
        }
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
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

<<<<<<< HEAD
        Draw();
    }

    // ParentName : 부모로 CSkillTimer를 들고 있는 Object
    public void RegisterTimer(string parentName)
    {
        _timer = GameObject.Find(parentName).GetComponent<CSkillTimer>();
=======
        if(_timer != null)
            Draw();
        
    }
    
    public void RegisterTimer(GameObject timerOwner)
    {
        _timer = timerOwner.GetComponent<CSkillTimer>();
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        _timer.TimerStart += CooldownEnable;
        _timer.TimerEnd += CooldownDisable;
    }

<<<<<<< HEAD
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
=======
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
        var skillUiObejct = skillUiObject.GetChild(elementUiNumber).GetChild(skillUiNumber).gameObject;
        skillUiObejct.SetActive(true);
        skillUiObejct.GetComponent<Image>().sprite = GetImageByRegisterNumber(skillNumber);
    }
    
    public void DeregisterSkillUi(int elementUiNumber, int skillUiNumber)
    {
        if (elementUiNumber < 0 || elementUiNumber >= 3) return;
        if (skillUiNumber < 0 || skillUiNumber >= 5) return;

        _elementSkillLists[elementUiNumber][skillUiNumber].preemptSkillNumber = -1;
        skillUiObject.GetChild(elementUiNumber).GetChild(skillUiNumber).gameObject.SetActive(false);
    }

    public void ShowSelectElement(int elementUiNumber)
    {
        for (int i = 0; i < 5; i++)
        {
            _SelectElementList[i].preemptSkillNumber = _elementSkillLists[elementUiNumber][i].preemptSkillNumber;
            if (_SelectElementList[i].preemptSkillNumber != -1)
            {
                crossHairObject.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    // 주원소의 elementSkillNumber에 해당하는 스킬만 밝게 보여준다
    // 0일 경우 주원소 기본 스킬 / -1일 경우 보여주지 않는다
    public void ShowSelectSkill(int elementSubSkillNumber)
    {
        if(elementSubSkillNumber == -1)
        {
            foreach(var skillUi in _SelectElementList)
            {
                skillUi.ui.SetActive(false);
            }
        }
        else
        {
            for(int i = 0; i < _SelectElementList.Count; i++)
            {
                //SetImageAlpha(_SelectElementList[i].image, i == elementSubSkillNumber ? 1f : 0.3f);
                _SelectElementList[i].drawer.SetAlpha(i == elementSubSkillNumber ? 1f : 0.3f);
            }
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
        }
    }

    // 모든 스킬 UI에 쿨타임을 그리는 명령을 내림
    // 단, 활성화되지 않은 경우 그리지 않음
    public void Draw()
    {
<<<<<<< HEAD
        foreach(var skillUI in _baseList)
        {
            if(skillUI.preemptSkillNumber != -1)
            {
                skillUI.drawer.Draw(
                    _timer.GetCurrentCooldown(skillUI.preemptSkillNumber),
                    _timer.GetMaxCooldown(skillUI.preemptSkillNumber)
=======
        foreach(var skillUiList in _elementSkillLists)
        {
            foreach(var skillUi in skillUiList)
            {
                skillUi.drawer.Draw(
                    _timer.GetCurrentCooldown(skillUi.preemptSkillNumber),
                    _timer.GetMaxCooldown(skillUi.preemptSkillNumber)
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
                    );
            }
        }

<<<<<<< HEAD
        foreach (var skillUI in _comboList)
        {
            if (skillUI.preemptSkillNumber != -1)
            {
                skillUI.drawer.Draw(
                    _timer.GetCurrentCooldown(skillUI.preemptSkillNumber),
                    _timer.GetMaxCooldown(skillUI.preemptSkillNumber)
                    );
            }
=======
        foreach(var elementSkill in _SelectElementList)
        {
            elementSkill.drawer.Draw(
                    _timer.GetCurrentCooldown(elementSkill.preemptSkillNumber),
                    _timer.GetMaxCooldown(elementSkill.preemptSkillNumber)
                );
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
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

<<<<<<< HEAD
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
=======
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
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
            }
        }

        return null;
    }
<<<<<<< HEAD
=======

    private void SetImageAlpha(Image image, float alpha)
    {
        var tempColor = image.color;
        tempColor.a = alpha;
        image.color = tempColor;
    }
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
}
