using System.Net;
using Unity.Netcode;
using UnityEngine;

public class Game : NetworkBehaviour
{
    [SerializeField] private GameObject fightAnimator;

    private int pos = -1;

    private int[][] attackSchemes = new int[2][];
    private int[][] defenceSchemes = new int[2][];
    private ulong[] clientIds = new ulong[2];
    private int[] healths = new int[2];

    private int roundCount = 0;

    private int cnt;

    [ServerRpc(RequireOwnership = false)]
    public void ReadyServerRpc(int[] attackScheme, int[] defenceScheme, int health,
        ServerRpcParams serverRpcParams = default)
    {
        cnt = 2;

        var clientId = serverRpcParams.Receive.SenderClientId;
        Debug.Log(
            $"Ready from {clientId} with attack scheme: {string.Join(string.Empty, attackScheme)}, defence scheme: {string.Join(string.Empty, defenceScheme)} and health: {health}");
        Debug.Log($"pos: {pos}");

        pos++;
        clientIds[pos] = clientId;

        attackSchemes[pos] = new int[8];
        attackSchemes[pos] = attackScheme;

        defenceSchemes[pos] = new int[8];
        defenceSchemes[pos] = defenceScheme;
        healths[pos] = health;

        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            var client = NetworkManager.ConnectedClients[clientId];
        }

        // Оба клиента скинули свои схемы боя.
        if (pos == 1)
        {
            fightAnimator.SetActive(true);
            fightAnimator.GetComponent<FightAnimator>().StartAnimationClientRpc(
                attackSchemes[0], defenceSchemes[0],
                attackSchemes[1], defenceSchemes[1]);

            pos = -1;
        }
    }

    private int AttackAndReturnDamage(int[] attackA, int[] defenceB)
    {
        int totalDamage = 0;

        // Последовательно "атакуем" каждую часть тела.
        // Если текущая часть тела - под защитой, то AttackPoints для неё множится на ноль.
        for (int i = 0; i < 8; i++)
            totalDamage += attackA[i] * defenceB[i];

        return totalDamage;
    }

    [ClientRpc]
    void RoundFinishedClientRpc(int newHealth, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log($"Game.RoundFinishedClientRpc, IsHost: {IsHost}, Id: {NetworkObjectId}, new health: {newHealth}");

        NetworkManager.LocalClient.PlayerObject.GetComponent<Player>().health = newHealth;
        NetworkManager.LocalClient.PlayerObject.GetComponent<Player>().GetReadyForRound();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AnimCompletedServerRpc()
    {
        Debug.Log($"AnimCompleted. IsHost: {IsHost}");

        // Обе анимации завершились
        if (--cnt == 0)
        {
            healths[1] -= AttackAndReturnDamage(attackSchemes[0], defenceSchemes[1]);
            healths[0] -= AttackAndReturnDamage(attackSchemes[1], defenceSchemes[0]);

            if (healths[0] <= 0 || healths[1] <= 0)
            {
                Debug.Log("Game finished");
                Debug.Log($"healthA: {healths[0]}\n healthB: {healths[1]}");
                return;
            }

            Debug.Log($"Round {roundCount} results:");
            Debug.Log($"Player1: health: {healths[0]}");
            Debug.Log($"Player2: health: {healths[1]}");

            ClientRpcParams clientRpcParams1 = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] {clientIds[0]}
                }
            };
            GameObject.Find("CountTimer").GetComponent<CountdownTimer>()
                .StartTimerClientRpc(healths[0], clientRpcParams1);
            // RoundFinishedClientRpc(healths[0], clientRpcParams1);

            ClientRpcParams clientRpcParams2 = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] {clientIds[1]}
                }
            };
            GameObject.Find("CountTimer").GetComponent<CountdownTimer>()
                .StartTimerClientRpc(healths[1], clientRpcParams2);
            // RoundFinishedClientRpc(healths[1], clientRpcParams2);
        }
    }
}