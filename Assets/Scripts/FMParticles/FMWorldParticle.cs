using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FMWorldParticle : FMParticle
{
    public override void SetPosition(Vector3 position)
    {
        root.position = position;
    }
}
