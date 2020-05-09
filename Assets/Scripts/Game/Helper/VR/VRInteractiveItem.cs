using System;
using UnityEngine;
using Basic.Managers;

public class VRInteractiveItem : MonoBehaviour
{
    //目光移动到了一个新的对象上的第一帧,相当于鼠标按下或者抬起;
    public event Action OnOver;             // Called when the gaze moves over this object
    public event Action OnOut;              // Called when the gaze leaves this object
    public event Action OnClick;            // Called when click input is detected while the gaze is over this object.
    public event Action OnDoubleClick;      // Called when double click input is detected whilst the gaze is over this object.
    public event Action OnUp;               // Called when Fire1 is released whilst the gaze is over this object.
    public event Action OnDown;             // Called when Fire1 is pressed whilst the gaze is over this object.

    protected bool _IsOver;

    public bool IsOver
    {
        get { return _IsOver; }              // Is the gaze currently over this object?
    }

    public void Over()
    {
        _IsOver = true;

        EventManager.instance.DispatchEvent(EventManager.instance, EventConfig.VR_ONOVER);

        if (OnOver != null)
        {
            OnOver();
        }
    }

    public void Out()
    {
        _IsOver = false;

        EventManager.instance.DispatchEvent(EventManager.instance, EventConfig.VR_ONOUT);

        if (OnOut != null)
        {
            OnOut();
        }
    }

    public void Click()
    {
        if (OnClick != null)
            OnClick();
    }

    public void DoubleClick()
    {
        if (OnDoubleClick != null)
            OnDoubleClick();
    }

    public void Up()
    {
        if (OnUp != null)
            OnUp();
    }

    public void Down()
    {
        if (OnDown != null)
            OnDown();
    }
}
