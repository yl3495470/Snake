using System;
using UnityEngine;
using Basic.Managers;

public class VREyeRaycaster : MonoBehaviour
{
    private Transform _Camera;
    [SerializeField]
    private VRInput _VrInput;
    [SerializeField]
    private bool _ShowDebugRay;
    [SerializeField]
    private float _RayLength = 500f;

    private int _detectLayerMask
    {
        get
        {
            return (1 << LayerMask.NameToLayer(Global.LayerType.UI)
                | (1 << LayerMask.NameToLayer(Global.LayerType.Default))
                | (1 << LayerMask.NameToLayer(Global.LayerType.UIModel3D)));
        }
    }

    private VRInteractiveItem _CurrentInteractible;
    private VRInteractiveItem _LastInteractible;        

    private HitInfo _hitInfo = new HitInfo();

    public VRInteractiveItem CurrentInteractible
    {
        get { return _CurrentInteractible; }
    }

    public void InitInput(VRInput pVRInput)
    {
        _VrInput = pVRInput;

        _VrInput.OnClick += HandleClick;
        _VrInput.OnDoubleClick += HandleDoubleClick;
        _VrInput.OnUp += HandleUp;
        _VrInput.OnDown += HandleDown;
    }
    
    private void OnDisable()
    {
        _VrInput.OnClick -= HandleClick;
        _VrInput.OnDoubleClick -= HandleDoubleClick;
        _VrInput.OnUp -= HandleUp;
        _VrInput.OnDown -= HandleDown;
    }
    
    private void Update()
    {
        EyeRaycast();
    }
    
    private void EyeRaycast()
    {
        if (_ShowDebugRay)
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * _RayLength, Color.blue);
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _RayLength, _detectLayerMask))
        {
            _hitInfo.hit = hit;
            _hitInfo.hitTarget = hit.collider.gameObject;
            _hitInfo.hitPosition = hit.point;

            EventManager.instance.DispatchEvent(EventManager.instance, EventConfig.RETICLE_SETPOSITION, _hitInfo);

            VRInteractiveItem interactible = hit.collider.GetComponent<VRInteractiveItem>();
            _CurrentInteractible = interactible;

            if (interactible && interactible != _LastInteractible)
                interactible.Over();

            if (interactible != _LastInteractible)
                DeactiveLastInteractible();

            _LastInteractible = interactible;            
        }
        else
        {
            DeactiveLastInteractible();

            _CurrentInteractible = null;

            EventManager.instance.DispatchEvent(EventManager.instance, EventConfig.RETICLE_SETPOSITION, null);
        }
    }

    private void DeactiveLastInteractible()
    {
        if (_LastInteractible == null)
            return;

        _LastInteractible.Out();
        _LastInteractible = null;
    }

    private void HandleUp()
    {
        if (_CurrentInteractible != null)
            _CurrentInteractible.Up();
    }

    private void HandleDown()
    {
        if (_CurrentInteractible != null)
            _CurrentInteractible.Down();
    }

    private void HandleClick()
    {
        if (_CurrentInteractible != null)
            _CurrentInteractible.Click();
    }

    private void HandleDoubleClick()
    {
        if (_CurrentInteractible != null)
            _CurrentInteractible.DoubleClick();
    }
}