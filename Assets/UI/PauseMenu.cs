using System;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour, Resettable
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject hud;

    public GameObject lastMenu;

    private void OnEnable()
    {
        hud.SetActive(false);
    }

    private void OnDisable()
    {
        hud.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeactivateThisMenu();
            lastMenu.SetActive(true);
        }
    }

    public void OnDisconnectButtonClick()
    {
        Debug.Log("PauseMenu.OnDisconnectButtonClick");

        GameObject.Find("Game").GetComponent<Game>().Disconnect();

        DeactivateThisMenu();
        hud.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void DeactivateThisMenu()
    {
        gameObject.SetActive(false);
    }

    public void ResetToDefault()
    {
        Debug.Log("PauseMenu.ResetToDefault");

        gameObject.SetActive(false);
    }
}