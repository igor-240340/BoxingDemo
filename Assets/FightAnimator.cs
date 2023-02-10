using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class FightAnimator : MonoBehaviour
{
    [SerializeField] private GameObject localHPText;
    [SerializeField] private GameObject remoteHPText;
    [SerializeField] private GameObject roundNumberText;
    [SerializeField] private GameObject[] characterPrefabs;

    private GameObject localCharacter;
    private GameObject remoteCharacter;

    private Animator localAnim;
    private Animator remoteAnim;

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

    private int localHP;
    private int remoteHP;

    private void OnEnable()
    {
        roundNumberText.SetActive(true);
    }

    public void InitCharacter(int localCharacterIndex)
    {
        DestroyCurrentCharacter();

        // Создаем и позиционируем локального персонажа.
        localCharacter = Instantiate(characterPrefabs[localCharacterIndex], new Vector3(-0.5f, 0, 3), Quaternion.Euler(0, 90, 0));
        localAnim = localCharacter.GetComponent<Animator>();
        localCharacter.transform.SetParent(gameObject.transform);

        // Создаем и позиционируем удаленного персонажа.
        remoteCharacter = Instantiate(characterPrefabs[localCharacterIndex ^ 1], new Vector3(0.5f, 0, 3), Quaternion.Euler(0, -90, 0));
        remoteAnim = remoteCharacter.GetComponent<Animator>();
        remoteCharacter.transform.SetParent(gameObject.transform);
    }

    private void DestroyCurrentCharacter()
    {
        if (localCharacter == null && remoteCharacter == null)
        {
            Debug.Log("FightAnimator.DestroyCurrentCharacter");
            return;
        }

        Destroy(localCharacter);
        Destroy(remoteCharacter);
    }

    private void Start()
    {
    }

    void Update()
    {
    }

    public void AnimateRound(RoundSpec roundSpec,
        ulong clientId1, int health1, int[] attackScheme1, int[] defenceScheme1,
        ulong clientId2, int health2, int[] attackScheme2, int[] defenceScheme2,
        Action cb)
    {
        Debug.Log($"FightAnimator.AnimateRound. IsHost: {NetworkManager.Singleton.IsHost}");
        Debug.Log($"roundSpec.isLast: {roundSpec.isLast}, number: {roundSpec.roundNumber}");
        Debug.Log($"attackScheme1: {attackScheme1.Length}");

        roundNumberText.GetComponent<TextMeshProUGUI>().text = $"РАУНД: {roundSpec.roundNumber}";

        isLastFight = roundSpec.isLast;

        animationCompleteCb = cb;

        // Определяем, по какому индексу лежит локальный игрок.
        if (clientId1 == NetworkManager.Singleton.LocalClientId)
        {
            localIsFirst = true;

            localHP = health1;
            remoteHP = health2;

            attackSchemeLocal = attackScheme1;
            defenceSchemeLocal = defenceScheme1;

            attackSchemeRemote = attackScheme2;
            defenceSchemeRemote = defenceScheme2;
        }
        else
        {
            localIsFirst = false;

            localHP = health2;
            remoteHP = health1;

            attackSchemeLocal = attackScheme2;
            defenceSchemeLocal = defenceScheme2;

            attackSchemeRemote = attackScheme1;
            defenceSchemeRemote = defenceScheme1;
        }

        cnt = 2;

        if (localIsFirst)
            StartCoroutine(AnimateLocalAttackFirst(attackSchemeLocal, attackSchemeRemote, defenceSchemeLocal,
                defenceSchemeRemote));
        else
            StartCoroutine(AnimateRemoteAttackFirst(attackSchemeLocal, attackSchemeRemote, defenceSchemeLocal,
                defenceSchemeRemote));
    }

    IEnumerator AnimateLocalAttackFirst(int[] attackSchemeLocal, int[] attackSchemeRemote, int[] defenceSchemeLocal,
        int[] defenceSchemeRemote)
    {
        Debug.Log("FightAnimator.AnimateLocalFirst");

        Debug.Log("Animate local attack");
        for (int i = 0; i < 8; i++)
        {
            if (attackSchemeLocal[i] == 0)
                continue;

            // Debug.Log($"Has started attack/defence for {i}");

            localAnim.Play(attackAnims[i], 0);
            remoteAnim.Play(defenceSchemeRemote[i] == 0 ? defenceAnims[i] : noDefenceAnim, 0);

            yield return new WaitForSeconds(localAnim.GetCurrentAnimatorStateInfo(0).length);
            // Debug.Log($"Has finished attack/defence for {i}\n");
        }

        remoteHPText.GetComponent<TextMeshProUGUI>().text = $"ПРОТИВНИК: {remoteHP}";

        Debug.Log("Animate remote attack");
        for (int i = 0; i < 8; i++)
        {
            if (attackSchemeRemote[i] == 0)
                continue;

            // Debug.Log($"Has started attack/defence for {i}");

            remoteAnim.Play(attackAnims[i], 0);
            localAnim.Play(defenceSchemeLocal[i] == 0 ? defenceAnims[i] : noDefenceAnim, 0);

            yield return new WaitForSeconds(localAnim.GetCurrentAnimatorStateInfo(0).length);
            // Debug.Log($"Has finished attack/defence for {i}\n");
        }

        localHPText.GetComponent<TextMeshProUGUI>().text = $"ВЫ: {localHP}";

        // Если это не последний раунд,
        // то сообщаем серверу о завершении анимации,
        // чтобы получить от него сигнал подгтовки к следующему раунду.
        if (!isLastFight)
            animationCompleteCb();
        else
        {
            if (localHP <= 0 && remoteHP <= 0)
            {
                localAnim.Play("knockdown_A", 0);
                remoteAnim.Play("knockdown_A", 0);
            }
            else if (localHP <= 0)
            {
                localAnim.Play("knockdown_A", 0);
                remoteAnim.Play("win_A", 0);
            }
            else
            {
                localAnim.Play("win_A", 0);
                remoteAnim.Play("knockdown_A", 0);
            }
        }
    }

    IEnumerator AnimateRemoteAttackFirst(int[] attackSchemeLocal, int[] attackSchemeRemote, int[] defenceSchemeLocal,
        int[] defenceSchemeRemote)
    {
        Debug.Log("FightAnimator.AnimateRemoteFirst");

        Debug.Log("Animate remote attack");
        for (int i = 0; i < 8; i++)
        {
            if (attackSchemeRemote[i] == 0)
                continue;

            // Debug.Log($"Has started attack/defence for {i}");

            remoteAnim.Play(attackAnims[i], 0);
            localAnim.Play(defenceSchemeLocal[i] == 0 ? defenceAnims[i] : noDefenceAnim, 0);

            yield return new WaitForSeconds(localAnim.GetCurrentAnimatorStateInfo(0).length);
            // Debug.Log($"Has finished attack/defence for {i}\n");
        }

        localHPText.GetComponent<TextMeshProUGUI>().text = $"ВЫ: {localHP}";

        Debug.Log("Animate local attack");
        for (int i = 0; i < 8; i++)
        {
            if (attackSchemeLocal[i] == 0)
                continue;

            // Debug.Log($"Has started attack/defence for {i}");

            localAnim.Play(attackAnims[i], 0);
            remoteAnim.Play(defenceSchemeRemote[i] == 0 ? defenceAnims[i] : noDefenceAnim, 0);

            yield return new WaitForSeconds(localAnim.GetCurrentAnimatorStateInfo(0).length);
            // Debug.Log($"Has finished attack/defence for {i}\n");
        }

        remoteHPText.GetComponent<TextMeshProUGUI>().text = $"ПРОТИВНИК: {remoteHP}";

        // Если это не последний раунд,
        // то сообщаем серверу о завершении анимации,
        // чтобы получить от него сигнал подгтовки к следующему раунду.
        if (!isLastFight)
            animationCompleteCb();
        else
        {
            if (localHP <= 0 && remoteHP <= 0)
            {
                localAnim.Play("knockdown_A", 0);
                remoteAnim.Play("knockdown_A", 0);
            }
            else if (localHP <= 0)
            {
                localAnim.Play("knockdown_A", 0);
                remoteAnim.Play("win_A", 0);
            }
            else
            {
                localAnim.Play("win_A", 0);
                remoteAnim.Play("knockdown_A", 0);
            }
        }
    }
}