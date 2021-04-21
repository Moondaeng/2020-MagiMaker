using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPillarLightController : MonoBehaviour
{
    GameObject _pillarGroup;
    System.Random r;
    SphereCollider sphereCollider;
    Light light;
    GameObject _directionalLight;

    // Start is called before the first frame update
    void Start()
    {
        _pillarGroup = gameObject;
        r = new System.Random();
        _directionalLight = GameObject.Find("Directional Light");
        CGlobal.isEvent = true;
        CCreateMap.instance.NotifyPortal();

        StartCoroutine("PillarLightEvent");
    }

    IEnumerator PillarLightEvent()
    {
        //_directionalLight.SetActive(false);
        yield return new WaitForSeconds(4.0f);
        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine("ExtendSphereCollider");
        }
        //_directionalLight.SetActive(true);

        CGlobal.isEvent = false;
        CCreateMap.instance.NotifyPortal();
    }

    IEnumerator ExtendSphereCollider()
    {
        int pillarNum = r.Next() % 4;
        Debug.Log(pillarNum);
        sphereCollider = _pillarGroup.transform.GetChild(pillarNum).GetChild(1).GetComponent<SphereCollider>();
        light = _pillarGroup.transform.GetChild(pillarNum).GetChild(0).GetChild(0).GetComponent<Light>();

        sphereCollider.enabled = true;
        light.intensity = 7;
        while (sphereCollider.radius < 5f)
        {
            sphereCollider.radius += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(3f);


        light.intensity = 0;
        sphereCollider.radius = 0.1f;
        sphereCollider.enabled = false;

        yield return new WaitForSeconds(1.0f);
    }
}