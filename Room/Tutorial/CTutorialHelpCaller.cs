using UnityEngine;

public class CTutorialHelpCaller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CWindowFacade.instance.OpenHelp();        
    }
}
