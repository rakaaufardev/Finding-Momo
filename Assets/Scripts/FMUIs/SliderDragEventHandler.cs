using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Slider))]
public class SliderDragEventHandler : MonoBehaviour, IPointerUpHandler
{
    public UnityEvent<float> OnEndDrag;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnEndDrag.Invoke(slider.value);
    }
}
