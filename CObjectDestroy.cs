using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CObjectDestroy : MonoBehaviour
{
    public void RemoveFromWorld()
    {
        Destroy(transform.parent.gameObject);
    }
}
