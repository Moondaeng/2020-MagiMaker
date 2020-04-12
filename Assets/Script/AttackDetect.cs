using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetect : CGoblinFSM
{
    CGoblinFSM myFSM;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        myFSM = transform.parent.parent.GetComponent<CGoblinFSM>();
    }
    
    void checkMyAction()
    {
        if(myFSM.currentState == EState.Attack)
        {
            gameObject.GetComponent<Collider>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    private void Update()
    {
        checkMyAction();
    }
}
