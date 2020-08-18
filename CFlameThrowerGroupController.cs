using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFlameThrowerGroupController : MonoBehaviour
{
    [Tooltip("불 돌아가는 속도")]
    public float degreePerSecond = 3F;
    // Update is called once per frame
    void Update()
    {
        float speed = degreePerSecond * Time.deltaTime;
        transform.Rotate(Vector3.up * speed);
    }
}
