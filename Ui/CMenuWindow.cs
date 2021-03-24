using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CMenuWindow : MonoBehaviour
{
    public static CMenuWindow instance;

    public Button QuitToLobbyBtn;
    public Button ReturnToGameBtn;
    public Button HelpBtn;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        QuitToLobbyBtn.onClick.AddListener(QuitToLobby);

        gameObject.SetActive(false);
    }

    private void QuitToLobby()
    {
        Debug.Log("Quit to lobby");
        // 싱글 게임이면 StartScene으로, 멀티 게임이면 Lobby로 나가져야 함
        SceneManager.LoadScene("Start");
        // 멀티 게임의 경우 서버 및 다른 플레이어에게 나갔음 알림을 보내야 함
        // InvokeQuit();
    }
}
