using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CStartScene : MonoBehaviour
{
    enum MessageCode
    {
        LoginSuccess = 110,
        RegisterSuccess = 111,
        LobbySuccess = 112,
        RegisterFail = 113,
        LoginFail = 114,
    }

    [SerializeField]
    private Network.CTcpClient _tcpManager;

    // Inspector에서 반드시 컴포넌트를 설정해줘야함
    public Button tutorialBtn;
    public Button startBtn;
    public Button networkBtn;
    public Button exitBtn;

    public GameObject LoginPopup;
    public InputField LoginID;
    public InputField LoginPW;
    public Button RegisterBtn;
    public Button LoginBtn;
    public Button CancelBtn;

    public GameObject loading;
    public Text errorHandlingDisplay;

    [SerializeField] GameObject _debugPanel;
    [SerializeField] Button _debugIpChangeButton;
    [SerializeField] InputField _debugIpField;
    [SerializeField] Button _debugPortChangeButton;
    [SerializeField] InputField _debugPortField;

    public int timeout = 7;

    private void Start()
    {
        _tcpManager = (Network.CTcpClient)FindObjectOfType(typeof(Network.CTcpClient));

        if (tutorialBtn != null)
        {
            tutorialBtn.onClick.AddListener(StartTutorial);
        }
        if (startBtn != null)
        {
            startBtn.onClick.AddListener(StartSingleGame);
        }
        if (networkBtn != null)
        {
            networkBtn.onClick.AddListener(ReadyToNetwork);
        }
        if (exitBtn != null)
        {
            exitBtn.onClick.AddListener(() => Application.Quit());
        }
        if (RegisterBtn != null)
        {
            RegisterBtn.onClick.AddListener(RegisterAccess);
        }
        if (LoginBtn != null)
        {
            LoginBtn.onClick.AddListener(LoginAccess);
        }
        if (CancelBtn != null)
        {
            CancelBtn.onClick.AddListener(CancelNetwork);
        }

        if (!CClientInfo.IsDebugMode)
        {
            _debugPanel.SetActive(false);
        }
        else
        {
            DrawDebug();
            _debugIpChangeButton.onClick.AddListener(ChangeIP);
            _debugPortChangeButton.onClick.AddListener(ChangePort);
        }
    }

    private void StartTutorial()
    {
        CClientInfo.CreateRoom(0);
        SceneManager.LoadScene("Tutorial");
    }

    private void StartSingleGame()
    {
        CClientInfo.CreateRoom(0);
        SceneManager.LoadScene("Prototype");
    }

    private void ReadyToNetwork()
    {
        startBtn.interactable = false;
        networkBtn.interactable = false;
        exitBtn.interactable = false;

        LoginPopup.SetActive(true);

        _tcpManager.StartClient();
        _tcpManager.SetPacketInterpret(PacketInterpret);
    }

    private void CancelNetwork()
    {
        LoginPopup.SetActive(false);

        startBtn.interactable = true;
        networkBtn.interactable = true;
        exitBtn.interactable = true;
        Debug.Log("Cancel Button");

        _tcpManager.SendShutdown();
    }

    private void RegisterAccess()
    {
        string id = LoginID.text;
        string pw = LoginPW.text;

        if(id.Length == 0 || pw.Length == 0)
        {
            ErrorHandling("아이디와 비밀번호를 입력해주세요!");
            return;
        }

        var packet = Network.CPacketFactory.CreateRegisterRequest(id, pw);

        _tcpManager.Send(packet.data);
    }

    private void LoginAccess()
    {
        string id = LoginID.text;
        string pw = LoginPW.text;

        if (id.Length == 0 || pw.Length == 0)
        {
            ErrorHandling("아이디와 비밀번호를 입력해주세요!");
            return;
        }

        var message = Network.CPacketFactory.CreateLoginRequest(id, pw);

        _tcpManager.Send(message.data);
    }

    private void PacketInterpret(byte[] data)
    {
        // 헤더 읽기
        Network.CPacket packet = new Network.CPacket(data);
        packet.ReadHeader(out byte payloadSize, out short messageType);
        Debug.LogFormat("Header : payloadSize = {0}, messageType = {1}", payloadSize, messageType);

        switch ((int)messageType)
        {
            case (int)MessageCode.LoginSuccess:
                InterpretLoginSuccess(packet);
                break;
            case (int)MessageCode.RegisterSuccess:
                //InterpretRegisterSuccess(packet);
                ErrorHandling("회원가입 완료");
                break;
            case (int)MessageCode.LobbySuccess:
                InterpretLobbySuccess(packet);
                break;
            case (int)MessageCode.RegisterFail:
                ErrorHandling("이미 가입된 아이디 입니다!");
                break;
            case (int)MessageCode.LoginFail:
                ErrorHandling("아이디 혹은 비밀번호가 일치하지 않습니다.");
                break;
            default:
                ErrorHandling("오류가 발생했습니다. 개발자에게 문의하세요.");
                break;
        }
    }

    private void InterpretLoginSuccess(Network.CPacket packet)
    {
        int uid = packet.ReadInt32();
        string id = packet.ReadString(16);
        int clear = packet.ReadInt32();
        CClientInfo.ThisUser = new CClientInfo.User(uid, id, clear);

        var message = Network.CPacketFactory.CreateLobbyRequest();

        _tcpManager.Send(message.data);
    }

    private void InterpretRegisterSuccess(Network.CPacket packet)
    {
        int uid = packet.ReadInt32();
        string id = packet.ReadString(16);
        int clear = packet.ReadInt32();
        CClientInfo.ThisUser = new CClientInfo.User(uid, id, clear);

        var message = Network.CPacketFactory.CreateLobbyRequest();
        
        _tcpManager.Send(message.data);
    }

    private void InterpretLobbySuccess(Network.CPacket packet)
    {
        _tcpManager.DeletePacketInterpret();
        SceneManager.LoadScene("Lobby");
    }

    private void ErrorHandling(string errorMsg)
    {
        errorHandlingDisplay.text = errorMsg;
    }

    #region Debug
    private void DrawDebug()
    {
        DrawIP();
        DrawPort();
    }

    private void DrawIP()
    {
        _debugIpField.text = _tcpManager.ipString;
    }

    private void DrawPort()
    {
        _debugPortField.text = _tcpManager.port.ToString();
    }

    private void ChangeIP()
    {
        _tcpManager.ipString = _debugIpField.text;
        Debug.Log($"Change IP to {_tcpManager.ipString}");
    }

    private void ChangePort()
    {
        _tcpManager.port = int.Parse(_debugPortField.text);
        Debug.Log($"Change Port to {_tcpManager.port}");
    }
    #endregion

    // 에러 메세지 쓰기
    //private void ErrorHandling(string errorMsg)
    //{
    //    errorHandlingDisplay.text = errorMsg;
    //    tcpManager.AlertSocketCloseToServer(true);
    //    PasswordForm.text = ""; // 일반적으로 패스워드 text는 없앤다.
    //    ClientUIScripts.ActiveAllForm();
    //    loading.SetActive(false); // loading창 비활성화

    //    handling = false;
    //    requestResult = 0;
    //}

    //// timeout이 지나면 쓰레드를 종료
    //IEnumerator ConnectionTimeout(Thread t)
    //{
    //    Debug.Log("Timeout Start");
    //    yield return new WaitForSeconds(timeout);
    //    Debug.Log("Timeout!");

    //    ErrorHandling("서버로부터 응답이 없습니다. (Timeout)");
    //    t.Abort();
    //}
}
