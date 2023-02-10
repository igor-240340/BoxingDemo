using TMPro;
using UnityEngine;

public class WaitForHostMenu : MonoBehaviour, Resettable
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
        rightCornerMessage.color = new(0.09019608f, 0.4784314f, 0.5686275f);
        rightCornerMessage.text = "CONNECTING...";

        string ip = connectPlayMenu.GetComponent<ConnectPlayMenu>().ipInput.GetComponent<TMP_InputField>().text;
        string port = connectPlayMenu.GetComponent<ConnectPlayMenu>().portInput.GetComponent<TMP_InputField>().text;

        GameObject.Find("Game").GetComponent<Game>().StartAsClient(ip, port, OnConnectCb);
    }

    private void OnConnectCb(bool success, string reason = default)
    {
        Debug.Log($"WaitHostMenu.OnClientConnected: {success}, {reason}");

        if (!success)
        {
            rightCornerMessage.color = new Color(0.9333333f, 0.2352941f, 0.2352941f);
            rightCornerMessage.text = reason.ToUpper();
        }
        else
            DeactivateThisMenu();
    }

    public void OnBackButtonClick()
    {
        GameObject.Find("Game").GetComponent<Game>().Disconnect(false);

        DeactivateThisMenu();
        connectPlayMenu.SetActive(true);
    }

    private void DeactivateThisMenu()
    {
        gameObject.SetActive(false);
    }

    public void ResetToDefault()
    {
        Debug.Log("WaitForHostMenu.ResetToDefault");

        gameObject.SetActive(false);
    }
}