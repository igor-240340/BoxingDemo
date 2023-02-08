using UnityEngine;

public class ConnectPlayMenu : MonoBehaviour
{
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject waitHostMenu;
    
    void Start()
    {
    }

    void Update()
    {
    }

    public void OnConnectButtonClick()
    {
        DeactivateThisMenu();
        waitHostMenu.SetActive(true);
    }
    
    public void OnBackButtonClick()
    {
        DeactivateThisMenu();
        playMenu.SetActive(true);
    }

    private void DeactivateThisMenu()
    {
        gameObject.SetActive(false);
    }
}