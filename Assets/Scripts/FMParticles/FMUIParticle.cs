using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMUIParticle : FMParticle
{    
    public override void SetPosition(Vector3 position)
    {
        RectTransform rootRect = root as RectTransform;
        rootRect.localPosition = position;
    }
}
