using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHpBarCond : MonoBehaviour
{
    Transform _cam;
    // Start is called before the first frame update
    void Start()
    {
        _cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + _cam.rotation * Vector3.forward,
            _cam.rotation * Vector3.up);
    }
}
