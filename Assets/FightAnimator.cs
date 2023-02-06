using System;
using System.Collections;

using UnityEngine;
using Unity.Netcode;
using Random = UnityEngine.Random;

public class FightAnimator : NetworkBehaviour
{
    [SerializeField] private GameObject redGuy;
    [SerializeField] private GameObject blueGuy;

    private Animator redAnim;
    private Animator blueAnim;

    public int[] attackSchemeA;
    public int[] defenceSchemeB;

    public int[] attackSchemeB;
    public int[] defenceSchemeA;

    private void Start()
    {
    }

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

    [ClientRpc]
    public void StartAnimationClientRpc(int[] attackSchemeA, int[] defenceSchemeA, int[] attackSchemeB, int[] defenceSchemeB)
    {
        Debug.Log($"StartAnimationClientRpc. IsLocal: {IsLocalPlayer}");

        this.attackSchemeA = attackSchemeA;
        this.defenceSchemeA = defenceSchemeA;
        
        this.attackSchemeB = attackSchemeB;
        this.defenceSchemeB = defenceSchemeB;
        
        cnt = 2;
        
        redAnim = redGuy.GetComponent<Animator>();
        blueAnim = blueGuy.GetComponent<Animator>();

        // CreateRandomAttackScheme();
        // CreateRandomDefenceScheme();

        // Debug.Log($"attack: {string.Join(string.Empty, attackSchemeA)}");
        // Debug.Log($"defence: {string.Join(string.Empty, defenceSchemeB)}");

        StartCoroutine(PerformFightAB(this.attackSchemeA, this.defenceSchemeB));
    }

    void Update()
    {
    }

    IEnumerator PerformFightAB(int[] attackSchemeA, int[] defenceSchemeB)
    {
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
        StartCoroutine(PerformFightBA(this.attackSchemeB, this.defenceSchemeA));
    }
    
    IEnumerator PerformFightBA(int[] attackSchemeB, int[] defenceSchemeA)
    {
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
        
        gameObject.SetActive(false);
        GameObject.Find("Game").GetComponent<Game>().AnimCompletedServerRpc();
    }

    private void CreateRandomAttackScheme()
    {
        attackSchemeA = new[] {0, 0, 0, 0, 0, 0, 0, 0};

        for (int i = 0; i < 4; i++)
        {
            int bodyPartIndex = Random.Range(0, 7);
            attackSchemeA[bodyPartIndex]++;
        }
    }

    private void CreateRandomDefenceScheme()
    {
        defenceSchemeB = new[] {1, 1, 1, 1, 1, 1, 1, 1};

        for (int i = 0; i < 4; i++)
        {
            int bodyPartIndex = Random.Range(0, 7);
            defenceSchemeB[bodyPartIndex] = 0;
        }
    }
}