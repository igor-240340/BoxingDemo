using Unity.Netcode;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    private NetworkManager netManager;

    void Start()
    {
        netManager = GetComponentInParent<NetworkManager>();
    }

    public void StartAsHost()
    {
        Debug.Log("StartAsHost");

        netManager.ConnectionApprovalCallback = ApprovalCheck;
        netManager.StartHost();
    }

    public void StartAsClient()
    {
        Debug.Log("StartAsClient");
        netManager.StartClient();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("ApprovalCheck");
        response.Approved = true;
    }
}