using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class FightAnimator : MonoBehaviour
{
    [SerializeField] private GameObject redGuy;
    [SerializeField] private GameObject blueGuy;

    private Animator redAnim;
    private Animator blueAnim;

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

    private void Start()
    {
    }

    private void OnEnable()
    {
    }

    public void AnimateRound(RoundSpec roundSpec,
        ulong clientId1, int health1, int[] attackScheme1, int[] defenceScheme1,
        ulong clientId2, int health2, int[] attackScheme2, int[] defenceScheme2,
        Action cb)
    {
        Debug.Log($"FightAnimator.AnimateRound. IsHost: {NetworkManager.Singleton.IsHost}");

        Debug.Log($"roundSpec.isLast: {roundSpec.isLast}, number: {roundSpec.roundNumber}");

        isLastFight = roundSpec.isLast;

        animationCompleteCb = cb;

        Debug.Log($"attackScheme1: {attackScheme1.Length}");

        // Определяем, по какому индексу лежит локальный игрок.
        if (clientId1 == NetworkManager.Singleton.LocalClientId)
        {
            attackSchemeLocal = attackScheme1;
            defenceSchemeLocal = defenceScheme1;

            attackSchemeRemote = attackScheme2;
            defenceSchemeRemote = defenceScheme2;
        }
        else
        {
            attackSchemeLocal = attackScheme2;
            defenceSchemeLocal = defenceScheme2;

            attackSchemeRemote = attackScheme1;
            defenceSchemeRemote = defenceScheme1;
        }

        cnt = 2;

        redAnim = redGuy.GetComponent<Animator>();
        blueAnim = blueGuy.GetComponent<Animator>();

        StartCoroutine(AnimateLocalVsRemote(attackSchemeLocal, defenceSchemeRemote));
    }

    void Update()
    {
    }

    IEnumerator AnimateLocalVsRemote(int[] attackSchemeA, int[] defenceSchemeB)
    {
        Debug.Log("FightAnimator.AnimateLocalVsRemote");

        for (int i = 0; i < 8; i++)
        {
            if (attackSchemeA[i] == 0)
                continue;

            // Debug.Log($"Has started attack/defence for {i}");

            redAnim.Play(attackAnims[i], 0);
            blueAnim.Play(defenceSchemeB[i] == 0 ? defenceAnims[i] : noDefenceAnim, 0);

            yield return new WaitForSeconds(redAnim.GetCurrentAnimatorStateInfo(0).length);
            // Debug.Log($"Has finished attack/defence for {i}\n");
        }

        // Анимируем схему для второго
        StartCoroutine(AnimateRemoteVsLocal(attackSchemeRemote, defenceSchemeLocal));
    }

    IEnumerator AnimateRemoteVsLocal(int[] attackSchemeB, int[] defenceSchemeA)
    {
        Debug.Log("FightAnimator.AnimateRemoteVsLocal");

        for (int i = 0; i < 8; i++)
        {
            if (attackSchemeB[i] == 0)
                continue;

            // Debug.Log($"Has started attack/defence for {i}");

            blueAnim.Play(attackAnims[i], 0);
            redAnim.Play(defenceSchemeA[i] == 0 ? defenceAnims[i] : noDefenceAnim, 0);

            yield return new WaitForSeconds(redAnim.GetCurrentAnimatorStateInfo(0).length);
            // Debug.Log($"Has finished attack/defence for {i}\n");
        }

        // Если это не последний раунд,
        // то сообщаем серверу о завершении анимации,
        // чтобы получить от него сигнал подгтовки к следующему раунду.
        if (!isLastFight)
            animationCompleteCb();
    }
}