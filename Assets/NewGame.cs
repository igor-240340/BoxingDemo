using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using Random = UnityEngine.Random;

public class NewGame : NetworkBehaviour
{
    [SerializeField] private GameObject prepareRoundMenu;
    [SerializeField] private GameObject fightAnimator;

    public struct Player : INetworkSerializeByMemcpy
    {
        public ulong clientId;
        public int[] attackScheme;
        public int[] defenceScheme;
        public int health;

        public Player(ulong clientId, int[] attackScheme, int[] defenceScheme, int health = 100)
        {
            this.clientId = clientId;
            this.attackScheme = attackScheme;
            this.defenceScheme = defenceScheme;
            this.health = health;
        }
    }

    private Dictionary<ulong, Player> players = new();

    private int readyForRoundCnt;

    private bool prepTimerIsActive;
    private float elapsedSeconds;
    private float leftSeconds;
    private int timerMaxSeconds = 20;

    private int animationCompleteCnt;

    private int currentRoundNumber = 1;

    private int
        localCharacterIndex =
            -1; // Определяет индекс, по которому хостовый и обычный клиенты берут модели своих персонажей.

    void Start()
    {
    }

    void Update()
    {
        if (prepTimerIsActive)
        {
            elapsedSeconds += Time.deltaTime;
            leftSeconds = timerMaxSeconds - elapsedSeconds;

            Debug.Log($"NewGame.Update elapsedSeconds: {elapsedSeconds}, leftSeconds: {leftSeconds}");

            prepareRoundMenu.GetComponent<PrepareRoundMenu>().timerText.GetComponent<TextMeshProUGUI>().text =
                $"00:{(int) leftSeconds}";

            if (leftSeconds <= 0)
                StopPrepTimer();
        }

        if (prepTimerIsActive && Input.GetKeyDown(KeyCode.Return))
            StopPrepTimer();
    }

    public void StartAsHost(Action<bool> cb)
    {
        Debug.Log("NewGame.StartAsHost");

        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;

        NetworkManager.Singleton.OnClientConnectedCallback += clientId =>
        {
            Debug.Log("NewGame.OnClientConnectedOnHost");

            if (NetworkManager.Singleton.LocalClientId != clientId)
            {
                Debug.Log("Both clients has connected.");

                cb(true);
                PrepareForRoundClientRpc(localCharacterIndex);
            }
        };
        NetworkManager.Singleton.StartHost();
    }

    [ClientRpc]
    private void PrepareForRoundClientRpc(int hostCharacterIndex = default)
    {
        Debug.Log(
            $"NewGame.PrepareForRoundClientRpc. clientId: {NetworkManager.Singleton.LocalClientId}, IsLocal: {IsLocalPlayer}");

        // Не хостовый клиент при первом вызове должен выбрать себе персонажа, противоположного хостовому.
        // Если это хостовый клиент, то индекс у него уже инициализирован при создании игры.
        if (!IsHost && localCharacterIndex == -1)
        {
            localCharacterIndex = hostCharacterIndex ^ 1;
            fightAnimator.GetComponent<FightAnimator>().InitCharacter(localCharacterIndex);
        }

        fightAnimator.SetActive(false);
        prepareRoundMenu.SetActive(true);

        StartPrepTimer();
    }

    private void StartPrepTimer()
    {
        Debug.Log($"NewGame.StartPrepTimer. IsHost: {IsHost}");

        elapsedSeconds = leftSeconds = 0;
        prepTimerIsActive = true;
    }

    private void StopPrepTimer()
    {
        Debug.Log($"NewGame.StopPrepTimer. IsHost: {IsHost}");

        prepTimerIsActive = false;
        prepareRoundMenu.GetComponent<PrepareRoundMenu>().timerText.GetComponent<TextMeshProUGUI>().text =
            "00:00";

        var attackScheme = prepareRoundMenu.GetComponent<PrepareRoundMenu>().attackScheme;
        var defenceScheme = prepareRoundMenu.GetComponent<PrepareRoundMenu>().defenceScheme;
        ReadyForRoundServerRpc(attackScheme, defenceScheme);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReadyForRoundServerRpc(int[] attackScheme, int[] defenceScheme,
        ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        Debug.Log($"NewGame.ReadyForFight. IsLocalClient: {NetworkManager.Singleton.LocalClientId == clientId}");
        Debug.Log(
            $"attackScheme: {string.Join(string.Empty, attackScheme)}, defenceScheme: {string.Join(string.Empty, defenceScheme)}");

        if (!players.ContainsKey(clientId))
            players.Add(clientId, new Player(clientId, attackScheme, defenceScheme));
        else
            players[clientId] = new Player(clientId, attackScheme, defenceScheme, players[clientId].health);

        if (++readyForRoundCnt == 2)
        {
            readyForRoundCnt = 0;

            Debug.Log("NewGame.ReadyForFight. Both clients are ready for fight");

            // Хост заранее просчитывает исход раунда и передает результаты клиентам для анимации.
            RoundSpec roundSpec = CalculateRound();
            Player[] ps = players.Values.ToArray();
            AnimateRoundClientRpc(roundSpec, ps[0].clientId, ps[0].health, ps[0].attackScheme, ps[0].defenceScheme,
                ps[1].clientId, ps[1].health, ps[1].attackScheme, ps[1].defenceScheme);
        }
    }

    private RoundSpec CalculateRound()
    {
        Debug.Log($"NewGame.CalculateRound. IsHost: {IsHost}");

        Player[] ps = players.Values.ToArray();

        int firstDamage = AttackAndReturnDamage(ps[1].attackScheme, ps[0].defenceScheme);
        players[ps[0].clientId] = new Player(ps[0].clientId, ps[0].attackScheme, ps[0].defenceScheme,
            ps[0].health - firstDamage);

        int secondDamage = AttackAndReturnDamage(ps[0].attackScheme, ps[1].defenceScheme);
        players[ps[1].clientId] = new Player(ps[1].clientId, ps[1].attackScheme, ps[1].defenceScheme,
            ps[1].health - secondDamage);

        bool isLastRound = ps[0].health - firstDamage <= 0 || ps[1].health - secondDamage <= 0;
        return new RoundSpec(isLastRound, currentRoundNumber++);
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
    private void AnimateRoundClientRpc(RoundSpec roundSpec,
        ulong clientId1, int health1, int[] attackScheme1, int[] defenceScheme1,
        ulong clientId2, int health2, int[] attackScheme2, int[] defenceScheme2)
    {
        Debug.Log($"NewGame.AnimateRoundClientRpc. IsHost: {IsHost}");

        prepareRoundMenu.SetActive(false);
        fightAnimator.SetActive(true);
        fightAnimator.GetComponent<FightAnimator>().AnimateRound(roundSpec,
            clientId1, health1, attackScheme1, defenceScheme1,
            clientId2, health2, attackScheme2, defenceScheme2,
            OnAnimationCompleteServerRpc);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnAnimationCompleteServerRpc()
    {
        Debug.Log($"NewGame.OnAnimationCompleteServerRpc.");

        if (++animationCompleteCnt == 2)
        {
            animationCompleteCnt = 0;

            Debug.Log("Both clients completed animation.");

            PrepareForRoundClientRpc();
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            response.Approved = false;
            response.Reason = "Host already has two clients connected.";
        }
        else
            response.Approved = true;
    }

    public void StartAsClient(Action<bool, string> cb)
    {
        Debug.Log("NewGame.StartAsClient");

        NetworkManager.Singleton.OnClientDisconnectCallback += clientId =>
        {
            Debug.Log(
                $"NewGame.OnClientDisconnectCallback on client. clientId: {clientId}, DisconnectReason: {NetworkManager.Singleton.DisconnectReason}");

            cb(false, NetworkManager.Singleton.DisconnectReason);
        };
        NetworkManager.Singleton.OnClientConnectedCallback += clientId =>
        {
            Debug.Log($"NewGame.OnClientConnectedCallback on client. clientId: {clientId}");
            cb(true, "");
        };
        NetworkManager.Singleton.StartClient();
    }

    private int[] CreateRandomAttackScheme()
    {
        int[] attackScheme = {0, 0, 0, 0, 0, 0, 0, 0};

        for (int i = 0; i < 4; i++)
        {
            int bodyPartIndex = Random.Range(0, 7);
            attackScheme[bodyPartIndex]++;
        }

        return attackScheme;
    }

    private int[] CreateRandomDefenceScheme()
    {
        int[] defenceScheme = {1, 1, 1, 1, 1, 1, 1, 1};

        for (int i = 0; i < 4; i++)
        {
            int bodyPartIndex = Random.Range(0, 7);
            defenceScheme[bodyPartIndex] = 0;
        }

        return defenceScheme;
    }

    public int LocalCharacterIndex
    {
        set
        {
            localCharacterIndex = value;
            fightAnimator.GetComponent<FightAnimator>().InitCharacter(localCharacterIndex);
        }
    }
}