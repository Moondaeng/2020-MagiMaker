﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

//해야할 것 : 돌 소환 직전에 마법진 띄워주기, 돌 바닥에서부터 올라오는 연출, 포탈방
public class CSpawnStone : MonoBehaviour
{
    GameObject _stone;
    GameObject _stoneSpawner;
    float _timer;
    int _waitingTime;
    System.Random r;

    void Start()
    {
        _timer = 0.0f;
        _stoneSpawner = gameObject;

        for (int i = 0; i < 6; i++)
            _stoneSpawner.transform.GetChild(i).gameObject.SetActive(false);
    }

    void Update()
    {
        _timer += Time.deltaTime;
        _waitingTime = 2;

        if (_timer > _waitingTime) //이 코드를 통해서 돌이 너무 빠르게 나오지 않게함.
        {
            r = new System.Random();
            StartCoroutine(SpawnStone(r.Next(3, 6), 0));
            StartCoroutine(SpawnStone(r.Next(3, 6), 1));
            StartCoroutine(SpawnStone(r.Next(3, 6), 2));
            StartCoroutine(SpawnStone(r.Next(3, 6), 3));
            StartCoroutine(SpawnStone(r.Next(3, 6), 4));
            StartCoroutine(SpawnStone(r.Next(3, 6), 5));

            _timer = 0;
        }
    }

    IEnumerator SpawnStone(int sec, int pos)
    {
        yield return new WaitForSeconds(sec);
        StartCoroutine(OnMagicCircle(pos));
        yield return new WaitForSeconds(1);
        _stone = Resources.Load("Stone") as GameObject;
        Instantiate(_stone, new Vector3(_stoneSpawner.transform.position.x + 2.5f * pos, _stoneSpawner.transform.position.y,
            _stoneSpawner.transform.position.z) - new Vector3(0, _stone.transform.localScale.y / 2, 0), Quaternion.identity);
    }

    IEnumerator OnMagicCircle(int pos)  //돌이 생성되기 1초전 마법진 빛나줌.
    {
        _stoneSpawner.transform.GetChild(pos).gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        _stoneSpawner.transform.GetChild(pos).gameObject.SetActive(false);
    }
}
