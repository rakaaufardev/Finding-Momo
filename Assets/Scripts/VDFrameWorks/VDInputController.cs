using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VDInputController
{
    public abstract bool IsStartRunningPressed();
    public abstract bool IsProceedPressed();
    public abstract bool IsJumpPressed();
    public abstract bool IsChangeTransitionSideToBackPressed();
    public abstract bool IsChangeTransitionBackToSidePressed();
    public abstract bool IsSlidePressed();
    public abstract bool IsChangeLeftLanePressed();
    public abstract bool IsChangeRightLanePressed();
    public abstract bool IsSlideRelease();
    public abstract bool IsQuickLandPressed();
    public abstract bool IsSurfUp();
    public abstract bool IsSurfDown();
    public abstract bool IsRotate3DModelViewer();
    public abstract float GetRotate3DModelViewerValue();
    public abstract void DoUpdateInGame();
    public abstract void DoUpdateInShop();
}
