using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemyFSM : MonoBehaviour
{
    public int _spawnID { get; set; }
    public float _rotAnglePerSecond = 360f; //1초에 플레이어의 방향을 360도 회전
    public float _moveSpeed { get; set; } //초당 2미터의 속도로 이동
    public float _attackDelay { get; set; } // 공격을 한번 하고 다시 공격할 때까지의 지연
    public float _attackTimer { get; set; } //공격을 하고 난 뒤에 경과되는 시간을 계산하기 위한 변수
    public float _attackDistance { get; set; } // 공격 거리 (적과의 거리)


    void Start()
    {
        InitStat();
    }

    public virtual void InitStat()
    {

    }
    public virtual void MoveState()
    {
        TurnToDestination();
        MoveToDestination();
    }
    
    public virtual void TurnToDestination()
    {

    }
    
    public virtual void MoveToDestination()
    {

    }

    public List<GameObject> DetectPlayer(List<GameObject> players)
    {
        players.Clear();
        GameObject temp = null;
        for (int i = 1; i < 5; i++)
        {
            if ((temp = GameObject.Find("Player" + i.ToString())) != null)
            {
                players.Add(temp);
            }
        }
        return players;
    }

    public List<float> CalculateDistance(List<GameObject> players)
    {
        List<float> distances = new List<float>();
        for (int i = 0; i < players.Count; i++)
        {
            distances.Add(Vector3.Distance(
                players[i].transform.position, transform.position));
        }
        //_distances.Sort();
        return distances;
    }
}