using UnityEngine;
using System.Collections;
using System;

public class CColorFlicker : MonoBehaviour
{
    public Renderer _obj;
    Color originColor;
    float time = 10f;
    // Use this for initialization
    void Start()
    {
        originColor = _obj.material.color;
    }

    void fuck()
    {
        StartCoroutine(Fuckyou());
    }

    // Update is called once per frame
    IEnumerator Fuckyou()
    {
        while(true)
        {
            float flicker = Mathf.Abs(Mathf.Sin(Time.time * 10));
            _obj.material.color = originColor * flicker;
            yield return null;
        }
    }
}
