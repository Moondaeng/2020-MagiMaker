using UnityEngine;
using UnityEngine.UI;

public class CConsumableItemViewer : MonoBehaviour
{
    public static CConsumableItemViewer instance;

    private Image _consumableImage;
    private Text _consumableName;
    private Text _consumableQuantity;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Start()
    {
        _consumableImage = GetComponent<Image>();
        _consumableName = transform.GetChild(0).GetComponent<Text>();
        _consumableQuantity = transform.GetChild(1).GetComponent<Text>();
    }

    public void Register(CInventory inventory)
    {
        inventory.changeConsumableEvent.AddListener(SetView);
    }

    public void Deregister(CInventory inventory)
    {
        inventory.changeConsumableEvent.RemoveListener(SetView);
    }

    private void SetView(Sprite image, string name, int quantity)
    {
        if(image == null || quantity == 0)
        {
            _consumableImage.sprite = null;
            _consumableName.text = "";
            _consumableQuantity.text = "";
            return;
        }

        Debug.Log("Set View");
        _consumableImage.sprite = image;
        _consumableName.text = name;
        _consumableQuantity.text = quantity.ToString();
    }
}
