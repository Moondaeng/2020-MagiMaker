using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inputKey : MonoBehaviour
{
    Image W, A, S, D, C, Space, Attack, Z;

    // Start is called before the first frame update
    void Start()
    {
        W = transform.GetChild(0).GetComponent<Image>();
        A = transform.GetChild(1).GetComponent<Image>();
        S = transform.GetChild(2).GetComponent<Image>();
        D = transform.GetChild(3).GetComponent<Image>();
        C = transform.GetChild(4).GetComponent<Image>();
        Space = transform.GetChild(5).GetComponent<Image>();
        Attack = transform.GetChild(6).GetComponent<Image>();
        Z = transform.GetChild(7).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))         W.color = Color.red;
        else if (Input.GetKeyUp(KeyCode.W))      W.color = Color.white;
        if (Input.GetKeyDown(KeyCode.A))         A.color = Color.red;
        else if (Input.GetKeyUp(KeyCode.A))      A.color = Color.white;
        if (Input.GetKeyDown(KeyCode.S))         S.color = Color.red;
        else if (Input.GetKeyUp(KeyCode.S))      S.color = Color.white;
        if (Input.GetKeyDown(KeyCode.D))         D.color = Color.red;
        else if (Input.GetKeyUp(KeyCode.D))      D.color = Color.white;
        if (Input.GetKeyDown(KeyCode.C))         C.color = Color.red;
        else if (Input.GetKeyUp(KeyCode.C))      C.color = Color.white;
        if (Input.GetKeyDown(KeyCode.Space))     Space.color = Color.red;
        else if (Input.GetKeyUp(KeyCode.Space))  Space.color = Color.white;
        if (Input.GetKeyDown(KeyCode.Mouse0))    Attack.color = Color.red;
        else if (Input.GetKeyUp(KeyCode.Mouse0)) Attack.color = Color.white;
        if (Input.GetKeyDown(KeyCode.Z))         Z.color = Color.red;
        else if (Input.GetKeyUp(KeyCode.Z))      Z.color = Color.white;
    }
}
