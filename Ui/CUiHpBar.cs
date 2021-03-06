﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * HP바 업데이트 등 부가기능 설정
 * 나중에 타격에 따른 애니메이션까지 지원 예정
 */
public class CUiHpBar : MonoBehaviour
{
    [SerializeField] private Image HpBarImage;
    [SerializeField] private Image HpBarDamaged;

    private CharacterPara drawTarget;

    private float _targetPercent;
    private float _animationPercent;

    [SerializeField] private int ANIM_DRAW_FRAME_COUNT = 30;

    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

    void Start()
    {
        _animationPercent = _targetPercent;
    }

    public void SetActive(bool isActive)
    {
        if (isActive == false)
        {
            StopAllCoroutines();
        }
        gameObject.SetActive(isActive);
    }

    public void Change(CharacterPara cPara)
    {
        if (drawTarget != null)
        {
            drawTarget.hpDrawEvent.RemoveListener(Draw);
        }
        drawTarget = cPara;
        cPara.hpDrawEvent.AddListener(Draw);
        _targetPercent = cPara.CurrentHp / cPara.TotalMaxHp;
        _animationPercent = cPara.CurrentHp / cPara.TotalMaxHp;
    }

    public void Register(CharacterPara cPara)
    {
        cPara.hpDrawEvent.AddListener(Draw);
        _targetPercent = cPara.CurrentHp / cPara.TotalMaxHp;
        _animationPercent = cPara.CurrentHp / cPara.TotalMaxHp;
    }

    public void Deregister(CharacterPara cPara)
    {
        cPara.hpDrawEvent.RemoveListener(Draw);
    }

    public void Draw(int curHp, int maxHp)
    {
        _targetPercent = (float)curHp / (float)maxHp;
        HpBarImage.fillAmount = _targetPercent;
        StopCoroutine("DrawHpAnimation");
        if (gameObject.activeSelf)
        {
            StartCoroutine("DrawHpAnimation");
        }
    }

    private IEnumerator DrawHpAnimation()
    {
        float interpolatePercent = (_targetPercent - _animationPercent) / ANIM_DRAW_FRAME_COUNT;
        for(int i = 0; i < ANIM_DRAW_FRAME_COUNT; i++)
        {
            _animationPercent += interpolatePercent;
            HpBarDamaged.fillAmount = _animationPercent;
            yield return WaitForFixedUpdate;
        }
        _animationPercent = _targetPercent;
        HpBarDamaged.fillAmount = _animationPercent;
    }
}
