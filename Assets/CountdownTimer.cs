using TMPro;
using UnityEngine;
using Unity.Netcode;

public class CountdownTimer : NetworkBehaviour
{
    [SerializeField] private GameObject timerText;
    [SerializeField] private GameObject prepareCanvas;
    [SerializeField] private GameObject fightAnimator;

    private float elapsedSeconds;
    private int maxSeconds = 15;

    private int remainedSeconds;

    public bool isRun = false;

    void Start()
    {
    }

    void Update()
    {
        if (isRun)
        {
            elapsedSeconds += Time.deltaTime;
            remainedSeconds = maxSeconds - (int) elapsedSeconds;
            timerText.GetComponent<TextMeshProUGUI>().text =
                remainedSeconds >= 15 ? $"00:{remainedSeconds}" : $"00:0{remainedSeconds}";
            if (elapsedSeconds / maxSeconds >= 1)
            {
                // Только хост решает, когда таймер истек и уведомляет всех клиентов, что пора драться.
                if (IsHost)
                    StopTimerClientRpc();
                else
                    isRun = false;
            }
        }
    }

    [ClientRpc]
    public void StartTimerClientRpc(int health = -999, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log($"StartTimerClientRpc. IsHost: {IsHost}");

        // Есть результаты боя.
        if (health != -999)
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().health = health;
        
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().attackScheme = new[] {0, 0, 0, 0, 0, 0, 0, 0};
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().defenceScheme = new[] {1, 1, 1, 1, 1, 1, 1, 1};
        
        elapsedSeconds = 0;
        isRun = true;
        prepareCanvas.SetActive(true);
    }

    [ClientRpc]
    public void StopTimerClientRpc()
    {
        Debug.Log($"StopTimerClientRpc. IsHost: {IsHost}");

        isRun = false;
        Debug.Log($"Enough, elapsed time (s): {elapsedSeconds}");

        // Каждый клиент отправляет серверу свои схемы атаки и защиты.
        int health = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().health;
        int[] attackScheme = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().attackScheme;
        int[] defenceScheme = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().defenceScheme;

        Debug.Log($"health: {health}");
        Debug.Log(string.Join(string.Empty, attackScheme));
        Debug.Log(string.Join(string.Empty, defenceScheme));

        Game game = GameObject.Find("Game").GetComponent<Game>();
        game.ReadyServerRpc(attackScheme, defenceScheme, health);

        prepareCanvas.SetActive(false);
    }
}