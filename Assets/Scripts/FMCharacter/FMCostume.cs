using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMCostume : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Transform rootSurfboard;
    [SerializeField] private List<GameObject> costumeParts;

    public Animator Anim
    {
        get
        {
            return anim;
        }
    }

    public Transform Surfboard
    {
        get
        {
            return rootSurfboard;
        }
    }

    public List<GameObject> CostumeParts
    {
        get
        {
            return costumeParts;
        }
        set
        {
            costumeParts = value;
        }
    }
}
