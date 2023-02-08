using TMPro;
using UnityEngine;

public class WaitHostMenu : MonoBehaviour
{
    [SerializeField] private GameObject connectPlayMenu;
    [SerializeField] private TextMeshProUGUI rightCornerMessage;

    void Start()
    {
    }

    void Update()
    {
    }

    private void OnEnable()
    {
        GameObject.Find("NewGame").GetComponent<NewGame>().StartAsClient(OnConnectCb);
    }

    private void OnConnectCb(bool success, string reason = default)
    {
        Debug.Log($"WaitHostMenu.OnClientConnected: {success}, {reason}");

        if (!success)
        {
            rightCornerMessage.color = new Color(242, 47, 70);
            rightCornerMessage.text = reason;
        }
        else
            DeactivateThisMenu();
    }

    public void OnBackButtonClick()
    {
        DeactivateThisMenu();
        connectPlayMenu.SetActive(true);
    }

    private void DeactivateThisMenu()
    {
        gameObject.SetActive(false);
    }
}