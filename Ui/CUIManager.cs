using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 모든 UI 관리 클래스
 * 기본적으로 CUI Manager만 만지면 된다
 */
[DisallowMultipleComponent]
public class CUIManager : MonoBehaviour
{
    // 플레이어 UI - 플레이어를 추적해서 그림
    public GameObject UiTargetObject;
    public CUiHpBar hpBarObject;

    [SerializeField] private CSkillUIManager _skillUIManager;
    [SerializeField] private CBuffTimerListUI _buffTimerUiList;
    [SerializeField] private CConsumableItemViewer _consumableViewer;
    private COtherPlayerUiManager _otherPlayerUi;

    // 언제 어디서나 쉽게 접금할수 있도록 하기위해 만든 정적변수
    public static CUIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        // 하위 UI 관리
        _skillUIManager = gameObject.GetComponent<CSkillUIManager>();
        _otherPlayerUi = gameObject.GetComponent<COtherPlayerUiManager>();
    }

    // 지정 캐릭터에 대한 UI를 그림
    // 지정 캐릭터가 없다면 그릴 필요 없음
    public void SetUiTarget(GameObject target)
    {
        // 이전 타겟 설정 제거
        if (UiTargetObject != null)
        {
            //_skillUIManager.DeregisterTimer(UiTargetObject);
            _skillUIManager.Deregister(UiTargetObject);
            _buffTimerUiList.DeregisterTimer(UiTargetObject);
            hpBarObject.Deregister(UiTargetObject.GetComponent<CharacterPara>());
            _consumableViewer.Deregister(UiTargetObject.GetComponent<CPlayerPara>().Inventory);
        }

        UiTargetObject = target;

        if (UiTargetObject == null)
            return;

        // 현재 타겟 설정
        //_skillUIManager.RegisterTimer(UiTargetObject);
        _skillUIManager.Register(UiTargetObject);
        _buffTimerUiList.RegisterTimer(UiTargetObject);
        hpBarObject.Register(UiTargetObject.GetComponent<CharacterPara>());
        _consumableViewer.Register(UiTargetObject.GetComponent<CPlayerPara>().Inventory);
    }
}

