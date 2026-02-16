using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadialMenuEntry : MonoBehaviour//, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void RadialMenuEntryDelegate(RadialMenuEntry pEntry);

    [SerializeField] TextMeshProUGUI Label;
    [SerializeField] Sprite Icon;
    RectTransform Rect;
    RadialMenuEntryDelegate Callback;
    private void Start()
    {
        Rect = GetComponent<RectTransform>();
    }

    public void SetLabel(string pText)
    {
        Label.text = pText;
    }
    public void SetIcon(Sprite pIcon)
    {
        Icon = pIcon;
    }

    public Sprite GetIcon()
    {
        return (Icon);
    }

    public void SetCallback(RadialMenuEntryDelegate pCallback)
    {
        Callback?.Invoke(this);
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{

    //}

    //public void OnPointerEnter(PointerEventData eventData)
    //{

    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{

    //}
}
