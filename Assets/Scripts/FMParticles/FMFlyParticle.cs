using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FMFlyParticle : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    [SerializeField] private RectTransform rootParticle;
    [SerializeField] private Image imageObject;

    public void SetSprite(Sprite inSprite)
    {
        imageObject.sprite = inSprite;
    }
    
}
