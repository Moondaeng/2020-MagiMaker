using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCameraControl : MonoBehaviour
{ 
    public GameObject _player;

    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - _player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = _player.transform.position + offset;
    }
}
