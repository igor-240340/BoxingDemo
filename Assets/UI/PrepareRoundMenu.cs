using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrepareRoundMenu : MonoBehaviour, Resettable
{
    [SerializeField] private GameObject menuEscText;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] public GameObject timerText;
    [SerializeField] public GameObject hud;
    [SerializeField] public GameObject roundNumberText;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject defenceButtonsContainer;
    [SerializeField] private GameObject attackButtonsContainer;

    [SerializeField] private Sprite[] defenceSprites;
    [SerializeField] private Sprite[] attackSprites;

    [SerializeField] private Texture[] characterSprites;

    [SerializeField] private GameObject defenceContainer;
    [SerializeField] private GameObject attackContainer;

    public int[] attackScheme;
    public int[] defenceScheme;

    private int currentDefencePoints;
    private int maxDefencePoints = 4;

    private int currentAttackPoints;
    private int maxAttackPoints = 4;

    public bool madeDecision;

    private void OnEnable()
    {
        Debug.Log("PrepareForRoundMenu.OnEnable");
        
        hud.SetActive(true);
        pauseMenu.SetActive(false);
        roundNumberText.SetActive(false);
        menuEscText.SetActive(true);

        int localCharacterIndex = GameObject.Find("Game").GetComponent<Game>().LocalCharacterIndex;
        defenceContainer.GetComponent<RawImage>().texture = characterSprites[localCharacterIndex];
        attackContainer.GetComponent<RawImage>().texture = characterSprites[localCharacterIndex ^ 1];
    }

    private void Start()
    {
        Debug.Log("PrepareForRoundMenu.Start");

        CreateEmptyFightScheme();
        InstantiateButtons();
    }

    private void CreateEmptyFightScheme()
    {
        attackScheme = new[] {0, 0, 0, 0, 0, 0, 0, 0};
        defenceScheme = new[] {1, 1, 1, 1, 1, 1, 1, 1};
    }

    private void InstantiateButtons()
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
                if (madeDecision)
                    return;
                
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
                if (madeDecision)
                    return;
                
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
                
                if (madeDecision)
                    return;
                
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

                if (madeDecision)
                    return;
                
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeactivateThisMenu();
            
            pauseMenu.GetComponent<PauseMenu>().lastMenu = gameObject;
            pauseMenu.SetActive(true);
        }
    }

    private void DeactivateThisMenu()
    {
        gameObject.SetActive(false);
    }

    private void ResetButtons()
    {
        foreach (Transform child in defenceButtonsContainer.transform)
            child.gameObject.GetComponent<Image>().sprite = defenceSprites[1];

        foreach (Transform child in attackButtonsContainer.transform)
        {
            child.gameObject.GetComponent<Image>().sprite = attackSprites[0];
            TextMeshProUGUI text = child.gameObject.GetComponentInChildren(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
            text.text = "";
        }
    }

    public void ResetToDefault()
    {
        Debug.Log("PrepareForRoundMenu.ResetToDefault");

        madeDecision = false;
        currentDefencePoints = currentAttackPoints = 0;
        CreateEmptyFightScheme();
        ResetButtons();
        
        gameObject.SetActive(false);
    }
}