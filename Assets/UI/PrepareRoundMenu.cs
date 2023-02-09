using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PrepareRoundMenu : MonoBehaviour
{
    [SerializeField] public GameObject timerText;
    [SerializeField] public GameObject hud;
    [SerializeField] public GameObject roundNumberText;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject defenceButtonsContainer;
    [SerializeField] private GameObject attackButtonsContainer;

    [SerializeField] private Sprite[] defenceSprites;
    [SerializeField] private Sprite[] attackSprites;

    public int[] attackScheme;
    public int[] defenceScheme;

    private int currentDefencePoints;
    private int maxDefencePoints = 4;

    private int currentAttackPoints;
    private int maxAttackPoints = 4;

    private void OnEnable()
    {
        attackScheme = new[] {0, 0, 0, 0, 0, 0, 0, 0};
        defenceScheme = new[] {1, 1, 1, 1, 1, 1, 1, 1};

        hud.SetActive(true);

        roundNumberText.SetActive(false);
    }

    void Start()
    {
        InstantiateDefenceButtons();
        InstantiateAttackButtons();
    }

    private void InstantiateDefenceButtons()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject button = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
            button.transform.SetParent(defenceButtonsContainer.transform);
            button.GetComponent<Image>().sprite = defenceSprites[1];

            int bodyPartIndex = i;
            button.GetComponent<MyButton>().onLeftClick.AddListener(() =>
            {
                // Если блок уже установлен, то пропускаем.
                if (defenceScheme[bodyPartIndex] == 0)
                    return;

                // Если количество блоков уже достигло максимума, то пропускаем.
                if (currentDefencePoints == maxDefencePoints)
                    return;

                defenceScheme[bodyPartIndex] = 0;
                button.GetComponent<Image>().sprite = defenceSprites[0];
                currentDefencePoints++;
            });

            button.GetComponent<MyButton>().onRightClick.AddListener(() =>
            {
                if (defenceScheme[bodyPartIndex] == 1)
                    return;

                // Если блок установлен, то снимаем его.
                defenceScheme[bodyPartIndex] = 1;
                button.GetComponent<Image>().sprite = defenceSprites[1];
                currentDefencePoints--;
            });
        }
    }

    private void InstantiateAttackButtons()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject button = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
            button.transform.SetParent(attackButtonsContainer.transform);
            button.GetComponent<Image>().sprite = attackSprites[0];

            int bodyPartIndex = i;
            button.GetComponent<MyButton>().onLeftClick.AddListener(() =>
            {
                Debug.Log($"Mouse left click on attack button for: {bodyPartIndex}");

                if (currentAttackPoints == maxAttackPoints)
                    return;

                button.GetComponent<Image>().sprite = attackSprites[1];
                attackScheme[bodyPartIndex]++;
                currentAttackPoints++;

                TextMeshProUGUI text = button.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                text.text = attackScheme[bodyPartIndex].ToString();
            });
            button.GetComponent<MyButton>().onRightClick.AddListener(() =>
            {
                Debug.Log($"Mouse right click on attack button for: {bodyPartIndex}");

                if (attackScheme[bodyPartIndex] != 0)
                {
                    attackScheme[bodyPartIndex]--;
                    currentAttackPoints--;

                    TextMeshProUGUI text = button.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
                    text.text = attackScheme[bodyPartIndex].ToString();

                    if (attackScheme[bodyPartIndex] == 0)
                    {
                        button.GetComponent<Image>().sprite = attackSprites[0];
                        text.text = "";
                    }
                }
            });
        }
    }
}