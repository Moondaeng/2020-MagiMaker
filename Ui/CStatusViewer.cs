using UnityEngine;
using UnityEngine.UI;

public class CStatusViewer : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text titleText;
    [SerializeField] private CUiHpBar hpBar;
    [SerializeField] private CBuffTimerListUI buffListUi;

    public CharacterPara target;

    public void SetActive(bool isActive)
    {
        nameText.gameObject.SetActive(isActive);
        titleText.gameObject.SetActive(isActive);
        hpBar.SetActive(isActive);
        buffListUi.SetActive(isActive);
    }

    public void Change(CharacterPara cPara)
    {
        if (target != null)
        {
            Deregister(target);
        }
        Register(cPara);
    }

    public void Register(CharacterPara cPara)
    {
        Debug.Log("Register");
        nameText.text = cPara.name;
        hpBar.Register(cPara);
        buffListUi.RegisterTimer(cPara.gameObject);
        target = cPara;
    }

    public void Deregister(CharacterPara cPara)
    {
        Debug.Log("Deregister");
        nameText.text = "";
        hpBar.Deregister(cPara);
        buffListUi.DeregisterTimer(cPara.gameObject);
        target = null;
    }
}
