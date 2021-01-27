using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventShooterController : MonoBehaviour
{
    [Tooltip("슈터가 움직이는 속도")]
    public int shooterSpeed;
    [Tooltip("슈터 최대 높이")]
    public int shooterMaxHeight;
    [Tooltip("슈터 최소 높이")]
    public int shooterMinHeight;
    [Tooltip("발사 주기")]
    public float firingCycle;

    private bool moveUp;
    // Start is called before the first frame update
    void Start()
    {
        if (shooterSpeed == 0)
            shooterSpeed = 3;
        if (shooterMaxHeight == 0)
            shooterMaxHeight = 8;
        if (shooterMinHeight == 0)
            shooterMinHeight = 4;
        if (firingCycle == 0)
            firingCycle = 0.05f;

        moveUp = true;
        StartCoroutine("spreadBullet", firingCycle);
    }

    // Update is called once per frame
    void Update()
    {
        moveUpNDown();
    }

    private void OnDestroy()
    {
        Debug.Log("Destroy Stone");

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("EventRoomStone");

        foreach (GameObject gameObject in gameObjects)
            Object.Destroy(gameObject);
    }

    IEnumerator spreadBullet(float delayTime)
    {
        GameObject bullet = Resources.Load("Object/EventBullet") as GameObject;
        Object.Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(delayTime);
        StartCoroutine("spreadBullet", firingCycle);
    }

    private void moveUpNDown()
    {
        if (moveUp)
        {
            transform.Translate(Vector3.up * shooterSpeed * Time.deltaTime);

            if (transform.position.y >= shooterMaxHeight)
                moveUp = false;
        }
        else
        {
            transform.Translate(Vector3.down * shooterSpeed * Time.deltaTime);

            if (transform.position.y <= shooterMinHeight)
                moveUp = true;
        }
    }
}
