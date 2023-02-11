using UnityEngine;

public class PlayMenu : MonoBehaviour, Resettable
{
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private GameObject singlePlayMenu;
    [SerializeField] private GameObject waitForClientMenu;
    [SerializeField] private GameObject connectPlayMenu;

    private void OnEnable()
    {
        Debug.Log("MainMenu.OnEnable");
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void OnSinglePlayButtonClick()
    {
    }

    public void OnHostButtonClick()
    {
        DeactivateThisMenu();
        waitForClientMenu.SetActive(true);
    }

    public void OnConnectButtonClick()
    {
        DeactivateThisMenu();
        connectPlayMenu.SetActive(true);
    }

    public void OnBackButtonClick()
    {
        DeactivateThisMenu();
        mainMenu.SetActive(true);
    }

    private void DeactivateThisMenu()
    {
        gameObject.SetActive(false);
    }

    public void ResetToDefault()
    {
        gameObject.SetActive(false);
    }
}