using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skillcam : MonoBehaviour
{
    public float offsetX = 0f;
    public float offsetY = 5f;
    public float offsetZ = -35f;
    Vector3 cameraPosition;
    // 카메라 Transform 변수
    private Transform tr;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (Input.inputString)
        {
            case "Q":
                KeyDown_Q();
                break;

            case "W":
                KeyDown_W();
                break;

            case "E":
                KeyDown_E();
                break;
        }
        tr.transform.position = cameraPosition;
    }

    private void KeyDown_Q()
    {
        cameraPosition.x = offsetX;
        Debug.Log("A");
    }

    private void KeyDown_W()
    {
        cameraPosition.x = offsetX + 10f;
        Debug.Log("B");

    }

    private void KeyDown_E()
    {
        cameraPosition.x = offsetX + 20f;
        Debug.Log("C");
    }
}
