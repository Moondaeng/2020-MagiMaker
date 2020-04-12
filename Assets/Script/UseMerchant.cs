using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseMerchant : MonoBehaviour
{
    private Camera mainCamera; //메인 카메라
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        ClickMerchant();
    }

    void ClickMerchant()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        Physics.Raycast(ray, out hit);

        if (hit.collider.gameObject.tag == "MERCHANT")//마우스 클릭한 대상이 상인 캐릭터인 경우
        {
            UseShop();
        }
    }

    void UseShop()
    {

    }
}
