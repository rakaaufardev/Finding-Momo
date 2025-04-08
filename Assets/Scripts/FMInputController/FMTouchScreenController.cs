using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SwipeDirection
{
    NONE,
    Up,
    Right,
    Down,
    left
}

public enum ScreenSection
{
    Left,
    Right
}

public enum TouchType
{
    Down,
    Hold,
    Up
}

public enum TouchGesture
{
    None,
    Single,
    Double,
    Multiple,
    PinchIn,
    PinchOut,
    Swipe
}

public class FMTouchScreenController : VDInputController
{
    private Vector2 startMousePosition;
    private Vector2 endMousePosition;
    private SwipeDirection swipeResult;
    private TouchGesture lastTouchGesture;

    private bool isRotating;
    private float rotateValue;
    private float currentYRotation;

    public override bool IsChangeLeftLanePressed()
    {
#if ENABLE_CHEAT
        if (VDDebugTool.Get().GetDebugMenu().DebugPressed)
        {
            return false;
        }
#endif

        return GetSwipeResult(SwipeDirection.left);
    }

    public override bool IsChangeRightLanePressed()
    {
#if ENABLE_CHEAT
        if (VDDebugTool.Get().GetDebugMenu().DebugPressed)
        {
            return false;
        }
#endif

        return GetSwipeResult(SwipeDirection.Right);
    }

    public override bool IsChangeTransitionBackToSidePressed()
    {
#if ENABLE_CHEAT
        if (VDDebugTool.Get().GetDebugMenu().DebugPressed)
        {
            return false;
        }
#endif

        return TouchScreen(TouchType.Down);
    }

    public override bool IsChangeTransitionSideToBackPressed()
    {
#if ENABLE_CHEAT
        if (VDDebugTool.Get().GetDebugMenu().DebugPressed)
        {
            return false;
        }
#endif

        //return TouchScreen(TouchType.Down);
        return GetSwipeResult(SwipeDirection.Up);
    }

    public override bool IsJumpPressed()
    {
#if ENABLE_CHEAT
        if (VDDebugTool.Get().GetDebugMenu().DebugPressed)
        {
            return false;
        }
#endif
        bool isJump = (TouchScreen(TouchType.Up) && lastTouchGesture == TouchGesture.Single) && !IsTouchOverUI() || GetSwipeResult(SwipeDirection.Up);
        //return TouchSideScreen(ScreenSection.Right, TouchType.Down);
        //return GetSwipeResult(SwipeDirection.Up);
        return isJump;
    }

    public override bool IsProceedPressed()
    {
#if ENABLE_CHEAT
        if (VDDebugTool.Get().GetDebugMenu().DebugPressed)
        {
            return false;
        }
#endif

        return TouchScreen(TouchType.Up);
    }

    public override bool IsQuickLandPressed()
    {
        return GetSwipeResult(SwipeDirection.Down);
    }

    public override bool IsSlidePressed()
    {
#if ENABLE_CHEAT
        if (VDDebugTool.Get().GetDebugMenu().DebugPressed)
        {
            return false;
        }
#endif

        //return TouchSideScreen(ScreenSection.Left, TouchType.Hold);
        return GetSwipeResult(SwipeDirection.Down);
    }

    public override bool IsSlideRelease()
    {
#if ENABLE_CHEAT
        if (VDDebugTool.Get().GetDebugMenu().DebugPressed)
        {
            return false;
        }
#endif

        return TouchScreen(TouchType.Down);
    }

    public override bool IsStartRunningPressed()
    {
#if ENABLE_CHEAT
        if (VDDebugTool.Get().GetDebugMenu().DebugPressed)
        {
            return false;
        }
#endif

        return TouchScreen(TouchType.Up) && !IsTouchOverUI();
    }

    public override bool IsSurfDown()
    {
#if ENABLE_CHEAT
        if (VDDebugTool.Get().GetDebugMenu().DebugPressed)
        {
            return false;
        }
#endif

        return TouchScreen(TouchType.Up);
    }

    public override bool IsSurfUp()
    {
#if ENABLE_CHEAT
        if (VDDebugTool.Get().GetDebugMenu().DebugPressed)
        {
            return false;
        }
#endif

        return TouchScreen(TouchType.Hold);
    }

    public override bool IsRotate3DModelViewer()
    {
#if ENABLE_CHEAT
        if (VDDebugTool.Get().GetDebugMenu().DebugPressed)
        {
            return false;
        }
#endif

        return isRotating;
    }

    public override float GetRotate3DModelViewerValue()
    {
        return rotateValue;
    }

    public override void DoUpdateInGame()
    {
        if (lastTouchGesture == TouchGesture.None || lastTouchGesture == TouchGesture.Single)
        {
            if (TouchScreen(TouchType.Down))
            {
                lastTouchGesture = TouchGesture.Single;
                startMousePosition = Input.mousePosition;
            }

            if (TouchScreen(TouchType.Hold))
            {
                endMousePosition = Input.mousePosition;
                swipeResult = DetectSwipe();
            }
        }
        else if(lastTouchGesture == TouchGesture.Swipe)
        {
            if (TouchScreen(TouchType.Up))
            {
                lastTouchGesture = TouchGesture.None;
                startMousePosition = Input.mousePosition;
            }
        }
    }

    public override void DoUpdateInShop()
    {
        if (TouchScreen(TouchType.Down))
        {
            isRotating = true;
            startMousePosition = Input.mousePosition;
        }

        if (isRotating && TouchScreen(TouchType.Hold))
        {
            Vector2 currentMousePosition = Input.mousePosition;
            Vector2 delta = currentMousePosition - startMousePosition;
            rotateValue = -delta.x * 0.1f;
            startMousePosition = Input.mousePosition;
        }

        if (TouchScreen(TouchType.Up))
        {
            rotateValue = 0;
        }
    }

    public void DoUpdateInWorld()
    {
        if (TouchScreen(TouchType.Down))
        {
            isRotating = true;
            startMousePosition = Input.mousePosition;
        }

        if (isRotating && TouchScreen(TouchType.Hold))
        {
            Vector2 currentMousePosition = Input.mousePosition;
            Vector2 delta = currentMousePosition - startMousePosition;
            rotateValue = -delta.x * 0.1f;
            startMousePosition = Input.mousePosition;
        }

        if (TouchScreen(TouchType.Up))
        {
            rotateValue = 0;
        }
    }

    private bool GetSwipeResult(SwipeDirection resultExpectation)
    {
        bool result = false;
        SwipeDirection actualResult = swipeResult;
        if(actualResult == resultExpectation)
        {
            result = true;
            swipeResult = SwipeDirection.NONE;
        }
        return result;
    }

    private bool TouchSideScreen(ScreenSection screenSection, TouchType touchType)
    {
        bool isTouch = TouchScreen(touchType);
        bool result = false;

        if (isTouch)
        {
            Vector2 touchPosition = Input.mousePosition;
            switch (screenSection)
            {
                case ScreenSection.Left:
                    result = touchPosition.x < Screen.width / 2;
                    break;
                case ScreenSection.Right:
                    result = touchPosition.x > Screen.width / 2;
                    break;
            }
        }

        return result;
    }

    public bool TouchScreen(TouchType touchType)
    {
        bool result = false;

        switch (touchType)
        {
            case TouchType.Down:
                result = Input.GetMouseButtonDown(0);
                break;
            case TouchType.Hold:
                result = Input.GetMouseButton(0);
                break;
            case TouchType.Up:
                result = Input.GetMouseButtonUp(0);
                break;
        }

        return result;
    }

    private SwipeDirection DetectSwipe()
    {
        SwipeDirection result = SwipeDirection.NONE;

        Vector2 direction = endMousePosition - startMousePosition;

        bool isSwipe = direction.magnitude > 50;
        if (isSwipe)
        {
            lastTouchGesture = TouchGesture.Swipe;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                // Horizontal swipe
                if (direction.x > 0)
                {
                    result = SwipeDirection.Right;
                }
                else
                {
                    result = SwipeDirection.left;
                }
            }
            else
            {
                // Vertical swipe
                if (direction.y > 0)
                {
                    result = SwipeDirection.Up;
                }
                else
                {
                    result = SwipeDirection.Down;
                }
            }
        }

        return result;
    }

    private bool IsTouchOverUI()
    {
        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        return false;
    }
}
