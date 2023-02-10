using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    private TextMeshProUGUI text;

    private Color defaultColor = new(0.2352941f, 0.572549f, 0.6078432f);
    private Color highlightedColor = new(0.9254902f, 0.9686275f, 0.9137255f);

    private bool isSelected;
    private bool isPressed;
    private bool isPointerLeft = true;

    private void OnEnable()
    {
        Debug.Log("CustomButton.OnEnable");
        
        if (text != null)
            text.color = defaultColor;
    }

    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("CustomButton.OnPointerEnter");

        text.color = highlightedColor;
        isPointerLeft = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("CustomButton.OnPointerExit");

        if (!isPressed && !isSelected)
            text.color = defaultColor;
        
        isPointerLeft = true;
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("CustomButton.OnSelect");
        
        text.color = highlightedColor;
        isSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("CustomButton.OnDeselect");

        if (isPointerLeft)
        {
            text.color = defaultColor;
            isSelected = false;
        }
        
        isSelected = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("CustomButton.OnPointerDown");

        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("CustomButton.OnPointerUp");

        isPressed = false;
        isSelected = false;
    }
}