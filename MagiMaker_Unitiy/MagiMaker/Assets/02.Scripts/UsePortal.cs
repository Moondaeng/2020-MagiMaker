using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsePortal : MonoBehaviour
{
    public Vector3 position;
    GameObject player;

    private void OnCollisionEnter(Collision coll)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        Debug.Log("OnTrigger!!!!");
        if (coll.collider.tag == "Player")
        {
            Debug.Log("PORTAL USED!");
            Transform ParentTransform = coll.transform;
            while (true)
            {
                if (ParentTransform.parent == null)
                    break;
                else
                    ParentTransform = ParentTransform.parent;
            }

            Debug.Log("Position = " + ParentTransform.position);

            switch(this.tag)
            {
                case "PORTAL":                                     
                    ParentTransform.position += new Vector3(0, 0, CConstants.PORTAL_DISTANCE_Z);
                    player.GetComponent<PlayerFSM>().IdleState();
                    break;

                case "RIGHT_PORTAL":                                   
                    ParentTransform.position += new Vector3(CConstants.PORTAL_DISTANCE_X, 0, CConstants.PORTAL_DISTANCE_Z);
                    player.GetComponent<PlayerFSM>().IdleState();
                    break;

                case "LEFT_PORTAL":
                    ParentTransform.position += new Vector3(-CConstants.PORTAL_DISTANCE_X, 0, CConstants.PORTAL_DISTANCE_Z);
                    player.GetComponent<PlayerFSM>().IdleState();
                    break;

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
