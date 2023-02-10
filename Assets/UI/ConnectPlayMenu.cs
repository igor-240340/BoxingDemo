using UnityEngine;

public class ConnectPlayMenu : MonoBehaviour, Resettable
{
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject waitForHostMenu;
    
    [SerializeField] public GameObject ipInput;
    [SerializeField] public GameObject portInput;
    
    void Start()
    {
    }

    void Update()
    {
    }

    public void OnConnectButtonClick()
    {
        DeactivateThisMenu();
        waitForHostMenu.SetActive(true);
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

    public void ResetToDefault()
    {
        Debug.Log("ConnectPlayMenu.ResetToDefault");
        
        gameObject.SetActive(false);
    }
}