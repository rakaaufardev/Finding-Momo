using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[SaveDuringPlay]
public class FMCameraObject : CinemachineExtension
{
    [SerializeField] private bool lockYAxis;
    [SerializeField] private float lockYPos;

    public void Init()
    {
        lockYPos = VirtualCamera.transform.localPosition.y;
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (lockYAxis) 
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                pos.y = lockYPos;
                state.RawPosition = pos;
            }
        }
    }

    public void SetDamping(float dampingX,float dampingY,float dampingZ)
    {
        if (VirtualCamera is CinemachineVirtualCamera vcam)
        {
            var transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_XDamping = dampingX;
                transposer.m_YDamping = dampingY;
                transposer.m_ZDamping = dampingZ;
            }
        }
    }
}
