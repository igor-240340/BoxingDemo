using UnityEngine;

public class SinglePlayMenu : MonoBehaviour, Resettable
{
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject prepareForRoundMenu;

    void Start()
    {
    }

    void Update()
    {
    }

    public void OnRedButtonClick()
    {
        Debug.Log("OnRedButtonClick");
    }

    public void OnBlueButtonClick()
    {
        Debug.Log("OnBlueButtonClick");
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
        Debug.Log("SinglePlayMenu.ResetToDefault");
        
        gameObject.SetActive(false);
    }
}