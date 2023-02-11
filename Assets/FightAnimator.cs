using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class FightAnimator : MonoBehaviour, Resettable
{
    [SerializeField] private GameObject[] characterPrefabs;

    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private GameObject playerHPText;
    [SerializeField] private GameObject enemyHPText;
    [SerializeField] private GameObject roundNumberText;
    [SerializeField] private GameObject menuEscText;

    private GameObject playerCharacter;
    private GameObject enemyCharacter;

    private Animator playerAnim;
    private Animator enemyAnim;

    public int[] attackSchemeLocal;
    public int[] defenceSchemeRemote;

    public int[] attackSchemeRemote;
    public int[] defenceSchemeLocal;

    private string[] attackAnims =
    {
        "hp_straight_A",
        "hp_straight_right_A",
        "bp_upper_left_A",
        "bp_hook_right_A",
        "bp_upper_left_A",
        "bp_hook_right_A",
        "bp_upper_left_A",
        "bp_hook_right_A"
    };

    private string[] defenceAnims =
    {
        "hb_front_A",
        "hb_front_A",
        "bb_front_A",
        "bb_front_A",
        "bb_front_A",
        "bb_front_A",
        "bb_front_A",
        "bb_front_A"
    };

    private string noDefenceAnim = "hd_front_B";

    private int cnt;

    private Action animationCompleteCb = null;

    private bool isLastFight;

    private bool localIsFirst;

    private int playerHP;
    private int enemyHP;

    private Coroutine currentCoroutine;

    private bool animationIsCompleted;

    private void OnEnable()
    {
        roundNumberText.SetActive(true);
        menuEscText.SetActive(false);

        InitCharacter();
    }

    private void Update()
    {
        // Даем возможность выйти в меню паузы (где находится кнопка дисконнекта)
        // только, если текущий бой был финальным.
        if (isLastFight && animationIsCompleted && Input.GetKeyDown(KeyCode.Escape))
            GameObject.Find("Game").GetComponent<Game>().Disconnect();
    }

    private void InitCharacter()
    {
        DestroyCurrentCharacter();

        // Создаем и позиционируем игрока.
        playerCharacter = Instantiate(characterPrefabs[0], new Vector3(-0.5f, 0, 3), Quaternion.Euler(0, 90, 0));
        playerAnim = playerCharacter.GetComponent<Animator>();
        playerCharacter.transform.SetParent(gameObject.transform);

        // Создаем и позиционируем противника.
        enemyCharacter = Instantiate(characterPrefabs[1], new Vector3(0.5f, 0, 3), Quaternion.Euler(0, -90, 0));
        enemyAnim = enemyCharacter.GetComponent<Animator>();
        enemyCharacter.transform.SetParent(gameObject.transform);
    }

    private void DestroyCurrentCharacter()
    {
        if (playerCharacter == null && enemyCharacter == null)
        {
            Debug.Log("FightAnimator.DestroyCurrentCharacter");
            return;
        }

        Destroy(playerCharacter);
        Destroy(enemyCharacter);
    }

    public void AnimateRound(RoundSpec roundSpec,
        ulong clientId1, int health1, int[] attackScheme1, int[] defenceScheme1,
        ulong clientId2, int health2, int[] attackScheme2, int[] defenceScheme2,
        Action cb)
    {
        Debug.Log($"FightAnimator.AnimateRound. IsHost: {NetworkManager.Singleton.IsHost}");
        Debug.Log($"roundSpec.isLast: {roundSpec.isLast}, number: {roundSpec.roundNumber}");

        Debug.Log($"attackScheme1: {attackScheme1.Length}");
        Debug.Log($"attackScheme2: {attackScheme2.Length}");
        Debug.Log($"defenceScheme1: {defenceScheme1.Length}");
        Debug.Log($"defenceScheme2: {defenceScheme2.Length}");

        // Когда от сервера приходит сигнал начать анимацию, клиент может быть в меню паузы,
        // в которое он вышел, находясь в режиме подготовки к раунду.
        // Просто возвращаем его в игру и "заставляем" смотреть анимацию.
        // Если он вышел в меню паузы и нажал Disconnect, то игра в принципе будет завершена и мы сюда не попадем вовсе.
        pauseMenu.SetActive(false);

        roundNumberText.GetComponent<TextMeshProUGUI>().text = $"ROUND: {roundSpec.roundNumber}";

        isLastFight = roundSpec.isLast;

        animationCompleteCb = cb;

        // Определяем, по какому индексу лежит локальный игрок.
        if (clientId1 == NetworkManager.Singleton.LocalClientId)
        {
            localIsFirst = true;

            playerHP = health1;
            enemyHP = health2;

            attackSchemeLocal = attackScheme1;
            defenceSchemeLocal = defenceScheme1;

            attackSchemeRemote = attackScheme2;
            defenceSchemeRemote = defenceScheme2;
        }
        else
        {
            localIsFirst = false;

            playerHP = health2;
            enemyHP = health1;

            attackSchemeLocal = attackScheme2;
            defenceSchemeLocal = defenceScheme2;

            attackSchemeRemote = attackScheme1;
            defenceSchemeRemote = defenceScheme1;
        }

        cnt = 2;

        if (localIsFirst)
            currentCoroutine = StartCoroutine(AnimatePlayerAttackFirst(attackSchemeLocal, attackSchemeRemote,
                defenceSchemeLocal, defenceSchemeRemote));
        else
            currentCoroutine = StartCoroutine(AnimateEnemyAttackFirst(attackSchemeLocal, attackSchemeRemote,
                defenceSchemeLocal, defenceSchemeRemote));
    }

    IEnumerator AnimatePlayerAttackFirst(int[] attackSchemeLocal, int[] attackSchemeRemote, int[] defenceSchemeLocal,
        int[] defenceSchemeRemote)
    {
        Debug.Log("FightAnimator.AnimatePlayerAttackFirst");

        Debug.Log("Animate player attack");
        for (int i = 0; i < 8; i++)
        {
            if (attackSchemeLocal[i] == 0)
                continue;

            playerAnim.Play(attackAnims[i], 0);
            enemyAnim.Play(defenceSchemeRemote[i] == 0 ? defenceAnims[i] : noDefenceAnim, 0);

            yield return new WaitForSeconds(playerAnim.GetCurrentAnimatorStateInfo(0).length);
        }

        enemyHPText.GetComponent<TextMeshProUGUI>().text = $"{enemyHP}";

        if (enemyHP <= 0)
        {
            Debug.Log("enemyHP <= 0");
            
            PlayAnimPlayerWin();
            HandleFightFinish();
            yield break;
        }

        Debug.Log("Animate remote attack");
        for (int i = 0; i < 8; i++)
        {
            if (attackSchemeRemote[i] == 0)
                continue;

            // Debug.Log($"Has started attack/defence for {i}");

            enemyAnim.Play(attackAnims[i], 0);
            playerAnim.Play(defenceSchemeLocal[i] == 0 ? defenceAnims[i] : noDefenceAnim, 0);

            yield return new WaitForSeconds(playerAnim.GetCurrentAnimatorStateInfo(0).length);
            // Debug.Log($"Has finished attack/defence for {i}\n");
        }

        playerHPText.GetComponent<TextMeshProUGUI>().text = $"{playerHP}";

        if (playerHP <= 0)
        {
            Debug.Log("playerHP <= 0");
            
            PlayAnimPlayerLose();
            HandleFightFinish();
            yield break;
        }

        // Если это не последний раунд,
        // то сообщаем серверу о завершении анимации,
        // чтобы получить от него сигнал подгтовки к следующему раунду.
        if (!isLastFight)
            animationCompleteCb();
    }

    IEnumerator AnimateEnemyAttackFirst(int[] attackSchemeLocal, int[] attackSchemeRemote, int[] defenceSchemeLocal,
        int[] defenceSchemeRemote)
    {
        Debug.Log("FightAnimator.AnimateEnemyAttackFirst");

        Debug.Log("Animate enemy attack");
        for (int i = 0; i < 8; i++)
        {
            if (attackSchemeRemote[i] == 0)
                continue;

            enemyAnim.Play(attackAnims[i], 0);
            playerAnim.Play(defenceSchemeLocal[i] == 0 ? defenceAnims[i] : noDefenceAnim, 0);

            yield return new WaitForSeconds(playerAnim.GetCurrentAnimatorStateInfo(0).length);
        }
        playerHPText.GetComponent<TextMeshProUGUI>().text = $"{playerHP}";

        if (playerHP <= 0)
        {
            Debug.Log("playerHP <= 0");
            
            PlayAnimPlayerLose();
            HandleFightFinish();
            yield break;
        }

        Debug.Log("Animate player attack");
        for (int i = 0; i < 8; i++)
        {
            if (attackSchemeLocal[i] == 0)
                continue;

            playerAnim.Play(attackAnims[i], 0);
            enemyAnim.Play(defenceSchemeRemote[i] == 0 ? defenceAnims[i] : noDefenceAnim, 0);

            yield return new WaitForSeconds(playerAnim.GetCurrentAnimatorStateInfo(0).length);
        }
        enemyHPText.GetComponent<TextMeshProUGUI>().text = $"{enemyHP}";

        if (enemyHP <= 0)
        {
            Debug.Log("enemyHP <= 0");
            
            PlayAnimPlayerWin();
            HandleFightFinish();
            yield break;
        }

        // Если это не последний раунд,
        // то сообщаем серверу о завершении анимации,
        // чтобы получить от него сигнал подгтовки к следующему раунду.
        if (!isLastFight)
            animationCompleteCb();
        else
        {
            HandleFightFinish();
        }
    }

    public void ResetToDefault()
    {
        Debug.Log("FightAnimator.ResetToDefault");

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        animationIsCompleted = false;
        isLastFight = false;
        gameObject.SetActive(false);
    }

    private void HandleFightFinish()
    {
        animationIsCompleted = true;
        menuEscText.SetActive(true);
    }

    private void PlayAnimPlayerWin()
    {
        playerAnim.Play("win_A", 0);
        enemyAnim.Play("knockdown_A", 0);
    }
    
    private void PlayAnimPlayerLose()
    {
        playerAnim.Play("knockdown_A", 0);
        enemyAnim.Play("win_A", 0);
    }
}