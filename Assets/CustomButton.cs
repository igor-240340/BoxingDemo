using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    private TextMeshProUGUI text;

    private Color defaultColor = new(0.9568627f, 0.6156863f, 0.2156863f);
    private Color highlightedColor = Color.white;

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