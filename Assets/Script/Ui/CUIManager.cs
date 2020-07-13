using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 모든 UI 관리 클래스
 * 기본적으로 CUI Manager만 만지면 된다
 */
public class CUIManager : MonoBehaviour
{
    private static CLogComponent _logger;
     
    // 플레이어 UI - 플레이어를 추적해서 그림
    public GameObject UiTargetObject;
    public GameObject hpBarObject;
    // Scene Canvas 설정 - Ui가 제대로 추가될 수 있도록 설정
    public Transform UiCanvasTransform;

    private CSkillUIManager _skillUIManager;
    private CSkillTimerListUi _skillTimerUiList;
    private CBuffTimerListUI _buffTimerUiList;
    private COtherPlayerUiManager _otherPlayerUi;

    // 언제 어디서나 쉽게 접금할수 있도록 하기위해 만든 정적변수
    public static CUIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // 하위 UI 관리
        _skillUIManager = gameObject.GetComponent<CSkillUIManager>();
        _skillTimerUiList = gameObject.GetComponent<CSkillTimerListUi>();
        _buffTimerUiList = gameObject.GetComponent<CBuffTimerListUI>();
        _otherPlayerUi = gameObject.GetComponent<COtherPlayerUiManager>();

        _logger = new CLogComponent(ELogType.UI);
    }

    private void Start()
    {
        SetSceneCanvas();
        SetUiTarget(UiTargetObject);
    }

    // for test code
    private void Update()
    {
    }

    // 지정 캐릭터에 대한 UI를 그림
    // 지정 캐릭터가 없다면 그릴 필요 없음
    public void SetUiTarget(GameObject target)
    {
        // 이전 타겟 설정 제거
        if (UiTargetObject != null)
        {
            _skillUIManager.DeregisterTimer(UiTargetObject);
            _skillTimerUiList.DeregisterTimer(UiTargetObject);
            _buffTimerUiList.DeregisterTimer(UiTargetObject);
            hpBarObject.GetComponent<CUiHpBar>().Deregister(UiTargetObject.GetComponent<CharacterPara>());
        }

        UiTargetObject = target;

        if (UiTargetObject == null)
            return;
        
        // 현재 타겟 설정
        _skillUIManager.RegisterTimer(UiTargetObject);
        _skillTimerUiList.RegisterTimer(UiTargetObject);
        _buffTimerUiList.RegisterTimer(UiTargetObject);
        hpBarObject.GetComponent<CUiHpBar>().Register(UiTargetObject.GetComponent<CharacterPara>());
    }

    // 기본 Canvas 설정(초기화용)
    private void SetSceneCanvas()
    {
        _skillTimerUiList.SetCanvas(UiCanvasTransform);
        _buffTimerUiList.SetCanvas(UiCanvasTransform);
        _otherPlayerUi.SetCanvas(UiCanvasTransform);
    }
}

