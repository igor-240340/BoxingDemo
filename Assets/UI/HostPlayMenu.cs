using UnityEngine;

public class HostPlayMenu : MonoBehaviour
{
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject waitClientMenu;

    void Start()
    {
    }

    void Update()
    {
    }

    public void OnRedButtonClick()
    {
        DeactivateThisMenu();
        waitClientMenu.SetActive(true);
    }

    public void OnBlueButtonClick()
    {
        DeactivateThisMenu();
        waitClientMenu.SetActive(true);
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