using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHpBarCond : MonoBehaviour
{
<<<<<<< HEAD
    Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
=======
    Transform _cam;
    // Start is called before the first frame update
    void Start()
    {
        _cam = Camera.main.transform;
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD
        transform.LookAt(transform.position + cam.rotation * Vector3.forward,
            cam.rotation * Vector3.up);
=======
        transform.LookAt(transform.position + _cam.rotation * Vector3.forward,
            _cam.rotation * Vector3.up);
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }
}
