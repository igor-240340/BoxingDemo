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

        GameObject.Find("NewGame").GetComponent<NewGame>().LocalCharacterIndex = 0;
    }

    public void OnBlueButtonClick()
    {
        DeactivateThisMenu();
        waitClientMenu.SetActive(true);

        GameObject.Find("NewGame").GetComponent<NewGame>().LocalCharacterIndex = 1;
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