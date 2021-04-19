using UnityEngine;

public class CWaitingLoadViewer : DestroyableSingleton<CWaitingLoadViewer>
{
    [SerializeField] GameObject LoadingTextObject;

    public void FinishLoading()
    {
        LoadingTextObject.SetActive(false);
    }
}
