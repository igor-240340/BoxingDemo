using UnityEngine;

public class MainMenu : MonoBehaviour, Resettable
{
    [SerializeField] private GameObject playMenu;

    private void OnEnable()
    {
        Debug.Log("MainMenu.OnEnable");
    }

    public void OnPlayClick()
    {
        DeactivateThisMenu();
        playMenu.SetActive(true);
    }

    public void OnExitClick()
    {
        Application.Quit();
    }

    private void DeactivateThisMenu()
    {
        gameObject.SetActive(false);
    }

    public void ResetToDefault()
    {
        gameObject.SetActive(true);
    }
}