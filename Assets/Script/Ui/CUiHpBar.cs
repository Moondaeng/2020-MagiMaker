using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * HP바 업데이트 등 부가기능 설정
 * 나중에 타격에 따른 애니메이션까지 지원 예정
 */
public class CUiHpBar : MonoBehaviour
{
    [System.Serializable]
    public enum HpBarType
    {
        horizontal, vertical
    }

    public Image HpBarImage;
    public HpBarType hpBarType;

    // Start is called before the first frame update
    void Start()
    {
        //HpBarImage = gameObject.GetComponent<Image>();
    }

    public void Register(CharacterPara cPara)
    {
        cPara.damageEvent.AddListener(Draw);
    }

    public void Draw(int curHp, int maxHp)
    {
        // 수평 방식
        if(hpBarType == HpBarType.horizontal)
        {
            HpBarImage.fillAmount = (float)curHp / (float)maxHp;
        }
        // 수직 방식
        else 
        {
            HpBarImage.fillAmount = (float)curHp / (float)maxHp;
        }
    }
}
