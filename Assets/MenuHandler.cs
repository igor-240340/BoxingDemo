using Unity.Netcode;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject prepareCanvas;
    private int cnt = 0;

    void Start()
    {
    }

    public void StartAsHost()
    {
        Debug.Log("StartAsHost");

        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    public void StartAsClient()
    {
        Debug.Log("StartAsClient");
        NetworkManager.Singleton.StartClient();

        GameObject.Find("MenuCanvas").SetActive(false);
        prepareCanvas.SetActive(true);
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("ApprovalCheck");

        response.Approved = true;
        response.CreatePlayerObject = true;

        if (++cnt == 2)
        {
            Debug.Log("Both clients has just connected.");

            GameObject.Find("MenuCanvas").SetActive(false);
            prepareCanvas.SetActive(true);
        }
    }
}