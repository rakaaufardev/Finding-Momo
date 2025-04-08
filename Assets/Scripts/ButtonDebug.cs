using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDebug : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform root;

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        root.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        VDDebugTool.Get().GetDebugMenu().DebugPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        VDDebugTool.Get().GetDebugMenu().Show();
    }
}
