using UnityEngine;
using Unity.Netcode;

public class WaitClientMenu : MonoBehaviour
{
    [SerializeField] private GameObject hostPlayMenu;

    void Start()
    {
    }

    void Update()
    {
    }

    private void OnEnable()
    {
        GameObject.Find("NewGame").GetComponent<NewGame>().StartAsHost(res =>
        {
            if (res) DeactivateThisMenu();
        });
    }

    public void OnBackButtonClick()
    {
        DeactivateThisMenu();
        hostPlayMenu.SetActive(true);
    }

    private void DeactivateThisMenu()
    {
        gameObject.SetActive(false);
    }
}