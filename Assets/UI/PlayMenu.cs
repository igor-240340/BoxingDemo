using UnityEngine;

public class PlayMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private GameObject singlePlayMenu;
    [SerializeField] private GameObject hostPlayMenu;
    [SerializeField] private GameObject connectPlayMenu;

    void Start()
    {
    }

    void Update()
    {
    }

    public void OnSingleButtonClick()
    {
        DeactivateThisMenu();
        singlePlayMenu.SetActive(true);
    }

    public void OnHostButtonClick()
    {
        DeactivateThisMenu();
        hostPlayMenu.SetActive(true);
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
}