using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFlameThrowerGroupController : MonoBehaviour
{
    [Tooltip("불 회전하는 속도")]
    public float degreePerSecond;
    [HideInInspector]
    public Vector3 _flamethrowerDir;

    private void Start()
    {
        degreePerSecond = UnityEngine.Random.Range(3, 10);
        switch (UnityEngine.Random.Range(0, 2))
        {
            case 0:
                _flamethrowerDir = Vector3.up;
                break;
            case 1:
                _flamethrowerDir = Vector3.down;
                break;
        }
    }
}