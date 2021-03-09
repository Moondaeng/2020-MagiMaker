using UnityEngine;
using UnityEngine.UI;

public class CInventoryWindow : MonoBehaviour
{
    public static CInventoryWindow instance;

    [SerializeField] Button PlayerInventoryBtn;
    [SerializeField] Button TeamInventoryBtn;
    [SerializeField] CPlayerInventoryWindow playerInventoryWindow;
    [SerializeField] CTeamInventoryWindow teamInventoryWindow;
    [SerializeField] TMPro.TMP_Text _goldText;

    private CInventory _inventory;
    private bool isOpenPlayerInventory = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        gameObject.SetActive(false);

        PlayerInventoryBtn.onClick.AddListener(OpenPlayerInventory);
        TeamInventoryBtn.onClick.AddListener(OpenTeamInventory);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwapInventory();
        }
    }

    // 플레이어, 팀원 정보 갱신
    private void UpdateInventoryWindow()
    {
        playerInventoryWindow.gameObject.SetActive(true);
        teamInventoryWindow.gameObject.SetActive(true);
        playerInventoryWindow.OpenInventory(_inventory);
        teamInventoryWindow.OpenInventory(_inventory);
        playerInventoryWindow.gameObject.SetActive(false);
        teamInventoryWindow.gameObject.SetActive(false);
    }

    public void CloseWindow()
    {
        isOpenPlayerInventory = false;
    }

    public void OpenInventory(CInventory inventory)
    {
        _inventory = inventory;

        UpdateInventoryWindow();
        UpdateGoldInfo();
        OpenPlayerInventory();
    }

    private void SwapInventory()
    {
        if (!isOpenPlayerInventory)
        {
            OpenPlayerInventory();
        }
        else
        {
            OpenTeamInventory();
        }
    }

    private void OpenPlayerInventory()
    {
        teamInventoryWindow.gameObject.SetActive(false);
        playerInventoryWindow.gameObject.SetActive(true);
        isOpenPlayerInventory = true;
    }

    private void OpenTeamInventory()
    {
        playerInventoryWindow.gameObject.SetActive(false);
        teamInventoryWindow.gameObject.SetActive(true);
        isOpenPlayerInventory = false;
    }

    private void UpdateGoldInfo()
    {
        _goldText.text = _inventory.Gold.ToString();
    }
}
