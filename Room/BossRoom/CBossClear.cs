using UnityEngine;

[DisallowMultipleComponent]
public class CBossClear : MonoBehaviour
{
    [SerializeField] private CEnemyPara _bossStatus;

    // Start is called before the first frame update
    void Start()
    {
        _bossStatus.deadEvent.AddListener(BossKilled);
    }

    private void BossKilled()
    {
        Debug.Log("Boss Dead");
        CGlobal.isClear = true;
        CCreateMap.instance.NotifyPortal();
    }

    public void GameClear()
    {
        // 임시로 나가기 구현
        UnityEngine.SceneManagement.SceneManager.LoadScene("Start");
    }
}
