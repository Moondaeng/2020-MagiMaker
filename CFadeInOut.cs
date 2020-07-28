using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFadeInOut : MonoBehaviour
{
    public float fadeTime = 2f; // Fade효과 재생시간
    Image fadeImg;
    float start;
    float end;
    float time = 0f;
    bool isPlaying = false;
    public static CFadeInOut instance = null;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;

        fadeImg = GetComponent<Image>();

        //PlayFadeIn();
    }

    public void PlayFadeIn()

    {
        if (isPlaying == true) //중복재생방지
        {
            return;
        }

        start = 1f;
        end = 0f;

        StartCoroutine("FadeInOut");    //코루틴 실행
    }

    public void PlayFadeOut()

    {
        if (isPlaying == true) //중복재생방지
        {
            return;
        }

        start = 0f;
        end = 1f;
        StartCoroutine("FadeInOut");
    }

    IEnumerator FadeInOut()
    {
        isPlaying = true;

        Color fadeColor = fadeImg.color;
        time = 0f;
        fadeColor.a = Mathf.Lerp(start, end, time);

        while (fadeColor.a > 0f)

        {
            time += Time.deltaTime / fadeTime;
            fadeColor.a = Mathf.Lerp(start, end, time);
            fadeImg.color = fadeColor;
            yield return null;
        }

        isPlaying = false;
    }
}
