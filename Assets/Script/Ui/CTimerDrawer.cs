using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CTimerDrawer : MonoBehaviour
{
    public Image cooldownImage;
    public Text cooldownText;

    public void CooldownEnable()
    {
        cooldownImage.enabled = true;
        cooldownText.enabled = true;
    }

    public void CooldownDisable()
    {
        cooldownImage.enabled = false;
        cooldownText.enabled = false;
    }

    public void SetSize(int width, int height)
    {
        gameObject.GetComponent<Image>().rectTransform.sizeDelta
            = new Vector2(width, height);
        transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta
            = new Vector2(width, height);
    }

    public void Draw(float currentCooldown, float maxCooldown)
    {
        cooldownImage.fillAmount = currentCooldown / maxCooldown;
        currentCooldown = (float)(System.Math.Truncate(currentCooldown * 10) * 0.1);

        // 쿨타임 없을 때는 표기하지 않음
        if(currentCooldown <= 0)
        {
            cooldownText.text = "";
        }
        else
        {
            cooldownText.text = currentCooldown.ToString();
        }
    }
}
