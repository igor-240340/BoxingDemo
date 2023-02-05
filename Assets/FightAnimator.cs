using System.Collections;
using UnityEngine;

public class FightAnimator : MonoBehaviour
{
    [SerializeField] private GameObject redGuy;
    [SerializeField] private GameObject blueGuy;

    private Animator redAnim;
    private Animator blueAnim;

    private int[] attackScheme;
    private int[] defenceScheme;

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

    void Start()
    {
        redAnim = redGuy.GetComponent<Animator>();
        blueAnim = blueGuy.GetComponent<Animator>();

        CreateRandomAttackScheme();
        CreateRandomDefenceScheme();

        Debug.Log($"attack: {string.Join(string.Empty, attackScheme)}");
        Debug.Log($"defence: {string.Join(string.Empty, defenceScheme)}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(PerformFight());
        }
    }

    IEnumerator PerformFight()
    {
        for (int i = 0; i < 8; i++)
        {
            if (attackScheme[i] == 0)
                continue;

            // Debug.Log($"Has started attack/defence for {i}");

            redAnim.Play(attackAnims[i], 0);
            blueAnim.Play(defenceScheme[i] == 0 ? defenceAnims[i] : noDefenceAnim, 0);

            yield return new WaitForSeconds(redAnim.GetCurrentAnimatorStateInfo(0).length);
            // Debug.Log($"Has finished attack/defence for {i}\n");
        }
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
    
    public void OnDefencePrepareButtonClick(int bodyPartIndex)
    {
        Debug.Log($"Defence body part index: {bodyPartIndex}");
    }
    
    public void OnAttackPrepareButtonClick(int bodyPartIndex)
    {
        Debug.Log($"Attack body part index: {bodyPartIndex}");
    }
}