using UnityEngine;

public class PrepareRoundMenu : MonoBehaviour
{
    [SerializeField] public GameObject timerText;
    [SerializeField] public GameObject hud;
    [SerializeField] public GameObject roundNumberText;

    public int[] attackScheme;
    public int[] defenceScheme;

    private void OnEnable()
    {
        attackScheme = new[] {0, 0, 0, 0, 0, 0, 0, 0};
        defenceScheme = new[] {1, 1, 1, 1, 1, 1, 1, 1};
        
        hud.SetActive(true);
        
        roundNumberText.SetActive(false);
    }
    
    void Start()
    {
    }

    void Update()
    {
    }

    public void OnDefenceButtonClick(int bodyPartIndex)
    {
        Debug.Log($"PrepareRoundMenu.OnDefenceButtonClick. bodyPartIndex: {bodyPartIndex}");
        
        defenceScheme[bodyPartIndex] = defenceScheme[bodyPartIndex] ^ 1;
    }

    public void OnAttackButtonClick(int bodyPartIndex)
    {
        Debug.Log($"PrepareRoundMenu.OnAttackButtonClick. bodyPartIndex: {bodyPartIndex}");

        attackScheme[bodyPartIndex]++;
    }
}