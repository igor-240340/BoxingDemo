using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    private TextMeshProUGUI text;

    private Color defaultColor = new(0.09019608f, 0.4784314f, 0.5686275f);
    private Color highlightedColor = new(0.9176471f, 0.9294118f, 0.9019608f);

    private bool isSelected;

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
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("CustomButton.OnPointerExit");
        text.color = defaultColor;
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
        text.color = defaultColor;
        isSelected = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("CustomButton.OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("CustomButton.OnPointerUp");
    }
}