using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CActivateFountain : MonoBehaviour
{
    [Tooltip("뿌리는 아이템 개수")]
    public int itemCount = 70;
    private Queue<int> _coinItemElement;
    void ActivateFountain()
    {
        _coinItemElement = new Queue<int>();
        for (int i = 0; i < itemCount; i++)
        {
            if (i == itemCount - 1) //원소 뿌리기       
                _coinItemElement.Enqueue(2);
            else if (i % (itemCount / 4) == 0) //아이템 뿌리기
                _coinItemElement.Enqueue(1);
            else
                _coinItemElement.Enqueue(0);//코인 뿌리기
        }

        Debug.Log("Start Coroutine");
        StartCoroutine("SpreadItem", 0.1);
    }

    IEnumerator SpreadItem(float delayTime)
    {
        GameObject item = null;
        if (_coinItemElement.Count != 0)
            switch (_coinItemElement.Dequeue())
            {
                case 0:
                    item = Resources.Load("Object/FountainCoin") as GameObject;
                    break;
                case 1:
                    item = Resources.Load("Object/FountainItem") as GameObject;
                    break;
                case 2:
                    item = Resources.Load("Object/Test") as GameObject;
                    break;
            }

        if (item == null)
        {
            Debug.LogError("SpreadItem. item is Null");
        }
        else
        {
            item = Object.Instantiate(item, transform.position, Quaternion.Euler(new Vector3(UnityEngine.Random.Range(-60, 0),
                UnityEngine.Random.Range(0, 360), 0)));
            item.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * 750, ForceMode.Force);

            yield return new WaitForSeconds(delayTime);
            StartCoroutine("SpreadItem", delayTime);
        }
    }
}
