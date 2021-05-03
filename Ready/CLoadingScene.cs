using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CLoadingScene : MonoBehaviour
{
    public Image progressBar;

    public Text loadingText;
    public string contents = "Connecting";
    public float dotInterval = 0.2f;
    public int maxDot = 3;

    private int dotCount;
    private string initContents;

    void Start()
    {
        InvokeRepeating("Progress", dotCount, dotInterval);
        loadingText.text = contents;
    }

    void Progress()
    {
        if (dotCount == maxDot)
        {
            dotCount = 0;
            loadingText.text = contents;
        }

        loadingText.text += ".";
        dotCount++;
    }
}
