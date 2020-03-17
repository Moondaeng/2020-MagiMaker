using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleMove : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;
    private float r = 0.0f;

    private Transform tr;

    public float moveSpeed = 10.0f;

    public float rotSpeed = 80.0f;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        Debug.Log("h=" + h.ToString());
        Debug.Log("v=" + v.ToString());

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);

        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);
    }
}
