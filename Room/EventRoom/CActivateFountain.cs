using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CActivateFountain : MonoBehaviour
{
    [Tooltip("뿌리는 아이템 개수")]
    public int itemCount = 300;
    [Tooltip("뿌리는 주기")]
    public float delayTime = 0.01f;
    [Tooltip("뿌리는 힘")]
    public int force = 1000;

    private Queue<int> _coinItemElement;
    private bool useFlag = false;
    void ActivateFountain()
    {
        _coinItemElement = new Queue<int>();
        for (int i = 0; i < itemCount; i++)
        {
            if (i == itemCount - 1) //원소 뿌리기       
                _coinItemElement.Enqueue(2);
            else if (i % (itemCount / 2) == 0) //아이템 뿌리기
                _coinItemElement.Enqueue(1);
            else
                _coinItemElement.Enqueue(0);//코인 뿌리기
        }

        Debug.Log("Start Coroutine");

        if (!useFlag) //한번 사용시 더이상 사용불가
        {
            useFlag = true;
            StartCoroutine("SpreadItem", delayTime);
        }
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
                    item = Resources.Load("Object/FountainElement") as GameObject;
                    break;
            }
        else
            Destroy(gameObject);

        if (item == null)
        {
            Debug.LogError("SpreadItem. item is Null");
        }
        else
        {
            item = Instantiate(item, transform.position, Quaternion.Euler(new Vector3(UnityEngine.Random.Range(-60, 0),
                UnityEngine.Random.Range(0, 360), 0)));
            item.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * force, ForceMode.Force);

            yield return new WaitForSeconds(delayTime);
            StartCoroutine("SpreadItem", delayTime);
        }
    }
}
