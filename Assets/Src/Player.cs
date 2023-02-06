using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public int health = 100;
    public int[] attackScheme;
    public int[] defenceScheme; // 1 - часть тела не защищена, 0 - защищена.

    public override void OnNetworkSpawn()
    {
        Debug.Log($"Player.OnNetworkSpawn, Id: {NetworkObjectId}, IsLocal: {IsLocalPlayer}");

        Debug.Log(string.Join(string.Empty, attackScheme));
        Debug.Log(string.Join(string.Empty, defenceScheme));
    }

    void Start()
    {
        Debug.Log($"Player.Start, Id: {NetworkObjectId}, IsLocal: {IsLocalPlayer}");

        if (IsLocalPlayer)
        {
            // GetReadyForRound();
        }

        // Признак, что оба клиента подключились.
        if (IsHost && !IsLocalPlayer)
            GameObject.Find("CountTimer").GetComponent<CountdownTimer>().StartTimerClientRpc();
    }

    private void CreateRandomAttackScheme()
    {
        attackScheme = new[] {0, 0, 0, 0, 0, 0, 0, 0};

        for (int i = 0; i < 4; i++)
        {
            int bodyPartIndex = Random.Range(0, 7);
            attackScheme[bodyPartIndex]++;
        }
    }

    private void CreateRandomDefenceScheme()
    {
        defenceScheme = new[] {1, 1, 1, 1, 1, 1, 1, 1};

        for (int i = 0; i < 4; i++)
        {
            int bodyPartIndex = Random.Range(0, 7);
            defenceScheme[bodyPartIndex] = 0;
        }
    }

    public void GetReadyForRound()
    {
        CreateRandomAttackScheme();
        CreateRandomDefenceScheme();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        game.ReadyServerRpc(attackScheme, defenceScheme, health);
    }
}