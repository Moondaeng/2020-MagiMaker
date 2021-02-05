using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CInterationPopup : MonoBehaviour
{
    public static CInterationPopup instance;

    Text text;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        text = gameObject.GetComponent<Text>();
    }
}
