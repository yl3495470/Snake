using UnityEngine;
using System.Collections;
using Basic.Managers;
using System;

public class TouchWatcher : MonoBehaviour
{
    public int rayDistance = 1000;
    public static Action onMouseUp;

    private HitInfo _hittedObject = new HitInfo();
    private Ray _ray;
    private RaycastHit _hit;
    private Vector3 _touchPos;
    private bool _notHitCollider = true;

    private LayerMask uiCameraLayer
    {
        get
        {
            return (1 << LayerMask.NameToLayer(Global.LayerType.UI));
        }
    }

    private LayerMask mainCameraLayer
    {
        get
        {
            return (1 << LayerMask.NameToLayer(Global.LayerType.Default));
        }
    }

    void Awake()
    {
        Input.multiTouchEnabled = false;
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _OnPress();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _OnRelease();
        }
    }

    private void _OnPress(params object[] args)
    {
        _touchPos = Input.mousePosition;

        _ray = SceneCtrl.Instance.uiCamera2D.ScreenPointToRay(_touchPos);
        Physics.Raycast(_ray, out _hit, rayDistance, uiCameraLayer);
        _notHitCollider = _hit.collider != null ? false : true;
        if (_notHitCollider)
        {
            _ray = Camera.main.ScreenPointToRay(_touchPos);

            if (Physics.Raycast(_ray, out _hit, rayDistance, mainCameraLayer))
            {
                _hittedObject.hit = _hit;
                _hittedObject.hitTarget = _hit.collider.gameObject;
                _hittedObject.hitPosition = _hit.point;
                EventManager.instance.DispatchEvent(EventManager.instance, EventConfig.DETECT_NON_UI_OBJECT, _hittedObject);
            }
        }
    }

    private void _OnRelease()
    {
        if (onMouseUp != null)
        {
            onMouseUp();
        }
    }
}

public class HitInfo
{
    public RaycastHit hit;
    public GameObject hitTarget;
    public Vector3 hitPosition;
}
