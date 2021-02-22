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
    [Tooltip("발사 힘")]
    public float launchVelocity;

    private List<GameObject> _bullets;

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
            firingCycle = 0.025f;
        if (launchVelocity == 0)
            launchVelocity = 5f;

        moveUp = true;

        _bullets = new List<GameObject>();

        for (int i = 0; i < 70; i++)
        {
            GameObject bullet = Resources.Load("Object/EventBullet") as GameObject;
            CBulletQueue.instance.BulletEnqueue(bullet);
        }
        StartCoroutine("spreadBullet", firingCycle);
    }

    // Update is called once per frame
    void Update()
    {
        moveUpNDown();
    }

    IEnumerator spreadBullet(float delayTime)
    {
        GameObject bullet = CBulletQueue.instance.BulletDequeue();
        if (bullet == null)
            Debug.Log("bullet is null");
        else
        {
            bullet.SetActive(true);
            bullet.transform.position = transform.position;
            bullet.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            bullet.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, -launchVelocity, 0), ForceMode.VelocityChange);
        }
        yield return new WaitForSeconds(delayTime);
        StartCoroutine("spreadBullet", delayTime);
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
