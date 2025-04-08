using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMKeyboardController : VDInputController
{
    public override bool IsStartRunningPressed()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public override bool IsProceedPressed()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public override bool IsJumpPressed()
    {
        return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W);
    }

    public override bool IsChangeTransitionSideToBackPressed()
    {
        return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
    }

    public override bool IsChangeTransitionBackToSidePressed()
    {
        return Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
    }

    public override bool IsSlidePressed()
    {
        return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
    }

    public override bool IsChangeLeftLanePressed()
    {
        return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
    }

    public override bool IsChangeRightLanePressed()
    {
        return Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
    }

    public override bool IsSlideRelease()
    {
        return Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S);
    }

    public override bool IsQuickLandPressed()
    {
        return Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
    }

    public override bool IsSurfUp()
    {
        return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space);
    }

    public override bool IsSurfDown()
    {
        return Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.Space);
    }

    public override bool IsRotate3DModelViewer()
    {
        return false;
    }

    public override float GetRotate3DModelViewerValue()
    {
        return 0;
    }

    public override void DoUpdateInGame()
    {

    }

    public override void DoUpdateInShop()
    {

    }
}
