using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullParent : MonoBehaviour
{
    [SerializeField] private Seagull[] seagullChildArray;

    public Seagull[] SeagullGroup
    {
        get
        {
            return seagullChildArray;
        }
    }

    public void ResetSeagulls()
    {
        int count = seagullChildArray.Length;
        for (int i = 0; i < count; i++)
        {
            Seagull seagull = seagullChildArray[i];
            seagull.SeagullCollider.enabled = true;
            seagull.SeagullVisual.gameObject.SetActive(true);
            seagull.SeagullVisual.transform.localPosition = Vector3.zero;
            seagull.SeagullAnimator.SetTrigger("Idle");
        }
    }
}
