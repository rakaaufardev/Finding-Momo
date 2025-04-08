using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VDScene : MonoBehaviour
{
    private VDSceneController controller;

    public abstract IEnumerator Enter(params object[] dataContainer);
    public abstract IEnumerator Exit();
}
