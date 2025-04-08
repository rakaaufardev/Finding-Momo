using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMUnityMainCamera : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Transform centerCamera;
    [SerializeField] private Transform inkCamera;

    public Camera Camera {
        get
        {
            return camera;
        }
        set
        {
            camera = value;
        }
    }

    public Transform CenterCamera
    {
        get
        {
            return centerCamera;
        }
        set
        {
            centerCamera = value;
        }
    }
    
    public Transform InkCamera
    {
        get
        {
            return inkCamera;
        }
        set
        {
            inkCamera = value;
        }
    }
}
