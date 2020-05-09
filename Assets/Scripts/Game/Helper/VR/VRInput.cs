using System;
using UnityEngine;
using Basic.Managers;

public class VRInput : MonoBehaviour
{
    public enum SwipeDirection
    {
        NONE,
        UP,
        DOWN,
        LEFT,
        RIGHT
    };

    public event Action<SwipeDirection> OnSwipe;            // Called every frame passing in the swipe, including if there is no swipe.
    public event Action OnClick;                            // Called when Fire1 is released and it's not a double click.
    public event Action OnDown;                             // Called when Fire1 is pressed.
    public event Action OnUp;                               // Called when Fire1 is released.
    public event Action OnDoubleClick;                      // Called when a double click is detected.
    public event Action OnCancel;                           // Called when Cancel is pressed.

    [SerializeField]
    private float _DoubleClickTime = 0.3f;                  //The max time allowed between double clicks
    [SerializeField]
    private float _SwipeWidth = 0.3f;                       //The width of a swipe

    private Vector2 _MouseDownPosition;                     // The screen position of the mouse when Fire1 is pressed.
    private Vector2 _MouseUpPosition;                       // The screen position of the mouse when Fire1 is released.
    private float _LastMouseUpTime;                         // The time when Fire1 was last released.
    private float _LastHorizontalValue;                     // The previous value of the horizontal axis used to detect keyboard swipes.
    private float _LastVerticalValue;                       // The previous value of the vertical axis used to detect keyboard swipes.

    public float DoubleClickTime
    {
        get
        {
            return _DoubleClickTime;
        }
    }

    private void Update()
    {
        CheckInput();
    }

    //TODO,解耦此函数，使其可以灵活的更换键盘;
    private void CheckInput()
    {
        // Set the default swipe to be none.
        SwipeDirection swipe = SwipeDirection.NONE;

        CheckDown();

        // This if statement is to gather information about the mouse when the button is up.
        if (Input.GetButtonUp("Fire1"))
        {
            // When Fire1 is released record the position of the mouse.
            _MouseUpPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            // Detect the direction between the mouse positions when Fire1 is pressed and released.
            swipe = DetectSwipe();
        }

        // If there was no swipe this frame from the mouse, check for a keyboard swipe.
        if (swipe == SwipeDirection.NONE)
            swipe = DetectKeyboardEmulatedSwipe();

        // If there are any subscribers to OnSwipe call it passing in the detected swipe.
        if (OnSwipe != null)
            OnSwipe(swipe);

        // This if statement is to trigger events based on the information gathered before.
        if (Input.GetButtonUp("Fire1"))
        {
            // If anything has subscribed to OnUp call it.
            if (OnUp != null)
            {
                EventManager.instance.DispatchEvent(EventManager.instance, EventConfig.VR_ONUP);
                OnUp();
            }


            // If the time between the last release of Fire1 and now is less
            // than the allowed double click time then it's a double click.
            if (Time.time - _LastMouseUpTime < _DoubleClickTime)
            {
                // If anything has subscribed to OnDoubleClick call it.
                if (OnDoubleClick != null)
                {
                    EventManager.instance.DispatchEvent(EventManager.instance, EventConfig.VR_ONDOUBLECLICK);
                    OnDoubleClick();
                }

            }
            else
            {
                // If it's not a double click, it's a single click.
                // If anything has subscribed to OnClick call it.
                if (OnClick != null)
                {
                    EventManager.instance.DispatchEvent(EventManager.instance, EventConfig.VR_ONCLICK);
                    OnClick();
                }

            }

            // Record the time when Fire1 is released.
            _LastMouseUpTime = Time.time;
        }

        // If the Cancel button is pressed and there are subscribers to OnCancel call it.
        if (Input.GetButtonDown("Cancel"))
        {
            if (OnCancel != null)
            {
                EventManager.instance.DispatchEvent(EventManager.instance, EventConfig.VR_ONCANCEL);
                OnCancel();
            }
        }
    }

    private void CheckDown()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            // When Fire1 is pressed record the position of the mouse.
            _MouseDownPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            // If anything has subscribed to OnDown call it.
            if (OnDown != null)
            {
                EventManager.instance.DispatchEvent(EventManager.instance, EventConfig.VR_ONDOWN);
                OnDown();
            }
        }
    }

    private SwipeDirection DetectSwipe()
    {
        // Get the direction from the mouse position when Fire1 is pressed to when it is released.
        Vector2 swipeData = (_MouseUpPosition - _MouseDownPosition).normalized;

        // If the direction of the swipe has a small width it is vertical.
        bool swipeIsVertical = Mathf.Abs(swipeData.x) < _SwipeWidth;

        // If the direction of the swipe has a small height it is horizontal.
        bool swipeIsHorizontal = Mathf.Abs(swipeData.y) < _SwipeWidth;

        // If the swipe has a positive y component and is vertical the swipe is up.
        if (swipeData.y > 0f && swipeIsVertical)
            return SwipeDirection.UP;

        // If the swipe has a negative y component and is vertical the swipe is down.
        if (swipeData.y < 0f && swipeIsVertical)
            return SwipeDirection.DOWN;

        // If the swipe has a positive x component and is horizontal the swipe is right.
        if (swipeData.x > 0f && swipeIsHorizontal)
            return SwipeDirection.RIGHT;

        // If the swipe has a negative x component and is vertical the swipe is left.
        if (swipeData.x < 0f && swipeIsHorizontal)
            return SwipeDirection.LEFT;

        // If the swipe meets none of these requirements there is no swipe.
        return SwipeDirection.NONE;
    }

    private SwipeDirection DetectKeyboardEmulatedSwipe()
    {
        // Store the values for Horizontal and Vertical axes.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Store whether there was horizontal or vertical input before.
        bool noHorizontalInputPreviously = Mathf.Abs(_LastHorizontalValue) < float.Epsilon;
        bool noVerticalInputPreviously = Mathf.Abs(_LastVerticalValue) < float.Epsilon;

        // The last horizontal values are now the current ones.
        _LastHorizontalValue = horizontal;
        _LastVerticalValue = vertical;

        // If there is positive vertical input now and previously there wasn't the swipe is up.
        if (vertical > 0f && noVerticalInputPreviously)
            return SwipeDirection.UP;

        // If there is negative vertical input now and previously there wasn't the swipe is down.
        if (vertical < 0f && noVerticalInputPreviously)
            return SwipeDirection.DOWN;

        // If there is positive horizontal input now and previously there wasn't the swipe is right.
        if (horizontal > 0f && noHorizontalInputPreviously)
            return SwipeDirection.RIGHT;

        // If there is negative horizontal input now and previously there wasn't the swipe is left.
        if (horizontal < 0f && noHorizontalInputPreviously)
            return SwipeDirection.LEFT;

        // If the swipe meets none of these requirements there is no swipe.
        return SwipeDirection.NONE;
    }

    private void OnDestroy()
    {
        // Ensure that all events are unsubscribed when this is destroyed.
        OnSwipe = null;
        OnClick = null;
        OnDoubleClick = null;
        OnDown = null;
        OnUp = null;
    }
}
