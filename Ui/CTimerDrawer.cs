using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 쿨타임 등을 표기하는 정사각형 UI 조작 클래스
 */
public class CTimerDrawer : MonoBehaviour
{
    public Image cooldownImage;
    public Text cooldownText;
    public Text stackText;

    private readonly int baseSideLength = 80;

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

    public void SetSize(int sideLength)
    {
        float changeScale = sideLength / (float)baseSideLength;
        GetComponent<RectTransform>().localScale = new Vector3(changeScale, changeScale, 1.0f);
    }

    public void SetAlpha(float alpha)
    {
        SetImageAlpha(GetComponent<Image>(), alpha);
        SetImageAlpha(cooldownImage, alpha);
        SetTextAlpha(cooldownText, alpha);
        SetTextAlpha(stackText, alpha);
    }

    public void Draw(float currentCooldown, float maxCooldown, int stack = -1)
    {
        cooldownImage.fillAmount = currentCooldown / maxCooldown;

        if (!cooldownText.IsActive()) return;
        currentCooldown = (float)(System.Math.Truncate(currentCooldown * 10) * 0.1);

        // 쿨타임 없을 때는 표기하지 않음
        if (currentCooldown <= 0)
        {
            cooldownText.text = "";
        }
        else
        {
            cooldownText.text = currentCooldown.ToString();
        }

        if (stack == -1 || !stackText.IsActive()) return;
        stackText.text = stack.ToString();
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        var tempColor = image.color;
        tempColor.a = alpha;
        image.color = tempColor;
    }

    private void SetTextAlpha(Text text, float alpha)
    {
        var tempColor = text.color;
        tempColor.a = alpha;
        text.color = tempColor;
    }
}
