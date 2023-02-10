using UnityEngine;

public class HostPlayMenu : MonoBehaviour, Resettable
{
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject waitForClientMenu;

    void Start()
    {
    }

    void Update()
    {
    }

    public void OnRedButtonClick()
    {
        DeactivateThisMenu();
        waitForClientMenu.SetActive(true);

        GameObject.Find("Game").GetComponent<Game>().LocalCharacterIndex = 0;
    }

    public void OnBlueButtonClick()
    {
        DeactivateThisMenu();
        waitForClientMenu.SetActive(true);

        GameObject.Find("Game").GetComponent<Game>().LocalCharacterIndex = 1;
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
        Debug.Log("HostPlayMenu.ResetToDefault");

        gameObject.SetActive(false);
    }
}