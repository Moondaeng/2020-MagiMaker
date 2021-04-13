using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPillarLightController : MonoBehaviour
{
    GameObject _pillarGroup;
    System.Random r;
    // Start is called before the first frame update
    void Start()
    {
        _pillarGroup = gameObject;
        r = new System.Random();

        StartCoroutine("PillarLightEvent");
    }

    IEnumerator PillarLightEvent()
    {
        yield return new WaitForSeconds(4.0f);

        int pillarNum = r.Next() % 4;
        _pillarGroup.transform.GetChild(pillarNum).GetChild(0).GetChild(0).GetComponent<Light>().intensity = 7;
        Debug.Log(_pillarGroup.transform.GetChild(pillarNum).GetChild(0).GetChild(0).name);
        yield return new WaitForSeconds(1.0f);
        _pillarGroup.transform.GetChild(pillarNum).GetChild(0).GetChild(0).GetComponent<Light>().intensity = 0;

        pillarNum = r.Next() % 4;
        _pillarGroup.transform.GetChild(pillarNum).GetChild(0).GetChild(0).GetComponent<Light>().intensity = 7;
        yield return new WaitForSeconds(1.0f);
        _pillarGroup.transform.GetChild(pillarNum).GetChild(0).GetChild(0).GetComponent<Light>().intensity = 0;

        pillarNum = r.Next() % 4;
        _pillarGroup.transform.GetChild(pillarNum).GetChild(0).GetChild(0).GetComponent<Light>().intensity = 7;
        yield return new WaitForSeconds(1.0f);
        _pillarGroup.transform.GetChild(pillarNum).GetChild(0).GetChild(0).GetComponent<Light>().intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
