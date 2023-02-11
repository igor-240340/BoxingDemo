using UnityEngine;

public class WaitForClientMenu : MonoBehaviour, Resettable
{
    [SerializeField] private GameObject playMenu;

    private void OnEnable()
    {
        GameObject.Find("Game").GetComponent<Game>().StartAsHost(res =>
        {
            if (res) DeactivateThisMenu();
        });
    }

    public void OnBackButtonClick()
    {
        GameObject.Find("Game").GetComponent<Game>().Disconnect(false);

        DeactivateThisMenu();
        playMenu.SetActive(true);
    }

    private void DeactivateThisMenu()
    {
        gameObject.SetActive(false);
    }

    public void ResetToDefault()
    {
        Debug.Log("WaitForClientMenu.ResetToDefault");

        gameObject.SetActive(false);
    }
}