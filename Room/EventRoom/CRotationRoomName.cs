using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRotationRoomName : MonoBehaviour
{
    [Tooltip("회전 속도")]
    public float rotateSpeed = 1f;
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
    }
}
