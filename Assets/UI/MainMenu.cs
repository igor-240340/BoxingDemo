using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject playMenu;

    void Start()
    {
    }

    void Update()
    {
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
}