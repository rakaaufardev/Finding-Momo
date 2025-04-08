using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FMCameraController : MonoBehaviour
{
    public abstract void Init();
    public abstract void ActivateCamera(object camMode);
    public abstract void DisableAllCamera();
}
