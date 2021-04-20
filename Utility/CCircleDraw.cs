using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCircleDraw : MonoBehaviour
{
    public int segments = 72;
    public float xradius = 5;
    public float zradius = 5;
    LineRenderer line;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.startColor = Color.yellow;
        line.endColor = Color.yellow;
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        CreatePoints();
    }

    void CreatePoints()
    {
        float x;
        float y;
        float z = 0f;

        float angle = 5f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Sin(Mathf.Deg2Rad * angle) * zradius;

            line.SetPosition(i, new Vector3(x + transform.position.x, 0.1f, z + transform.position.z));
            angle += (360f / segments);
        }
    }
}
