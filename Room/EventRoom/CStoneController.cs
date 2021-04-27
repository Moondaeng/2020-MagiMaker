using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStoneController : MonoBehaviour
{
    [Tooltip("돌 데미지")]
    public int stoneDamage;
    private float _speed;
    private bool _upFinish; //돌 올라갔는지 확인할 플래그
    void Start()
    {
        if (stoneDamage == 0)
            stoneDamage = 100;

        _speed = 10;
        _upFinish = false;
    }

    void Update()
    {
        if (_upFinish) //돌이 다 올라왔음
        {
            gameObject.transform.Rotate(new Vector3(-100, 0, 0) * Time.deltaTime * _speed);
            transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(0, 0, -100), _speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(0, 100, 0), _speed * Time.deltaTime);
        }

        if (transform.position.y >= transform.localScale.y / 2 - 0.2) //돌의 높이
            _upFinish = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "WALL")
        {
            Destroy(gameObject);
        }

        if (other.tag == "Player")
        {
            CController.instance.player.GetComponent<CPlayerPara>().DamagedDisregardDefence(stoneDamage);
            Debug.Log("Rolling stone");
            Destroy(gameObject);
        }
    }
}