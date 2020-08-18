using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerEventController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
<<<<<<< HEAD

=======
>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    }

    public void SendAttackEnemy()
    {
<<<<<<< HEAD
        transform.parent.gameObject.SendMessage("AttackCalculate");
    }
=======
        transform.root.gameObject.SendMessage("AttackCalculate");
    }

>>>>>>> 106e3c281a077f42e1e08ffc8215c72bfb9bddf3
    // Update is called once per frame
    void Update()
    {

    }
}