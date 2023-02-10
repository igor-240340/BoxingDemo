using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour, Resettable
{
    [SerializeField] private GameObject playerHP;
    [SerializeField] private GameObject enemyHP;

    public void ResetToDefault()
    {
        playerHP.GetComponent<TextMeshProUGUI>().text = "100";
        enemyHP.GetComponent<TextMeshProUGUI>().text = "100";
        
        gameObject.SetActive(false);
    }
}