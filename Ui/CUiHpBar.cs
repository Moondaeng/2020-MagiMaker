using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * HP바 업데이트 등 부가기능 설정
 * 나중에 타격에 따른 애니메이션까지 지원 예정
 */
public class CUiHpBar : MonoBehaviour
{
    public Image HpBarImage;

    private float _targetPercent;
    private float _animationPercent;

    private const int ANIM_DRAW_FRAME_COUNT = 30;

    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

    void Start()
    {
        HpBarImage = gameObject.GetComponent<Image>();
        _animationPercent = _targetPercent;
    }

    public void Register(CharacterPara cPara)
    {
        cPara.damageEvent.AddListener(Draw);
        _targetPercent = cPara._curHp / cPara._maxHp;
        _animationPercent = cPara._curHp / cPara._maxHp;
    }

    public void Deregister(CharacterPara cPara)
    {
        cPara.damageEvent.RemoveListener(Draw);
    }

    public void Draw(int curHp, int maxHp)
    {
        _targetPercent = (float)curHp / (float)maxHp;
        StopCoroutine("DrawHpAnimation");
        StartCoroutine("DrawHpAnimation");
    }

    private IEnumerator DrawHpAnimation()
    {
        float interpolatePercent = (_targetPercent - _animationPercent) / ANIM_DRAW_FRAME_COUNT;
        for(int i = 0; i < ANIM_DRAW_FRAME_COUNT; i++)
        {
            _animationPercent += interpolatePercent;
            HpBarImage.fillAmount = _animationPercent;
            yield return WaitForFixedUpdate;
        }
        _animationPercent = _targetPercent;
        HpBarImage.fillAmount = _animationPercent;
    }
}
