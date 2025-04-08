using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VDUIWindow : MonoBehaviour
{
    //todo : use ienumerator if animation exist
    public abstract void Show(params object[] dataContainer);
    public abstract void Hide();
    public abstract void OnRefresh();
    public abstract void DoUpdate();
}
