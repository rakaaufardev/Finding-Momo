using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VDActionState
{
    protected bool stateReady;

    public abstract IEnumerator Enter(object prev, params object[] values);
    public abstract IEnumerator Exit(object next);
    public abstract void DoUpdate();
    public abstract void DoFixedUpdate();
}
