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
        if (Network.CTcpClient.instance.IsConnect)
        {
            if (CClientInfo.IsSinglePlay())
            {
                SceneManager.LoadScene("Lobby");
            }
            else
            {
                var message = Network.CPacketFactory.CreateReturnLobby(CClientInfo.JoinRoom.IsHost);

                Network.CTcpClient.instance.Send(message.data);
            }
        }
        else
        {
            SceneManager.LoadScene("Start");
        }
    }
}
