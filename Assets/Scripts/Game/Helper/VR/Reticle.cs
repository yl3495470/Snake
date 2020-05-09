using UnityEngine;
using UnityEngine.UI;
using Basic.Managers;
using System;
using System.Collections;

public class Reticle : MonoBehaviour
{
    [SerializeField]
    private float _DefaultDistance = 5f;
    [SerializeField]
    private bool _UseNormal;
    [SerializeField]
    private Image _imgReticle;
    [SerializeField]
    private Image _imgSelectionBar;
    [SerializeField]
    private Transform _ReticleTransform;
    [SerializeField]
    private bool _HideOnStart = true;

    private Transform _Camera;
    private Vector3 _originalScale;
    private Quaternion _originalRotation;

    private bool _isSelectionRadialActive;
    private bool _radialFilled = false;
    public event Action OnSelectionComplete;
    [SerializeField]
    private float _SelectionDuration = 2f;

    private Coroutine _SelectionFillRoutine;

    private HitInfo hitInfo;

    public bool UseNormal
    {
        get { return _UseNormal; }
        set { _UseNormal = value; }
    }

    public Transform ReticleTransform
    {
        get { return _ReticleTransform; }
    }

    private Vector3 offset;

    void Awake()
    {
        enabled = true;
        GetComponent<Canvas>().sortingOrder = Int16.MaxValue;
        Canvas.ForceUpdateCanvases();

        _Camera = Camera.main.transform;
        _originalScale = _ReticleTransform.localScale;
        _originalRotation = _ReticleTransform.localRotation;
    }

    void Start()
    {
        _imgSelectionBar.fillAmount = 0f;
        if (_HideOnStart)
            SelectionBarHide();
        EventManager.AddListener(EventManager.instance, EventConfig.RETICLE_SETPOSITION, SetPosition);
        EventManager.AddListener(EventManager.instance, EventConfig.VR_ONOVER, OnOver);
        EventManager.AddListener(EventManager.instance, EventConfig.VR_ONOUT, OnOut);

//        if (NetCtrl.Instance.IsAutomaticStart)
//        {
//            _SelectionFillRoutine = StartCoroutine(FillSelectionRadial());
//        }

    }

    void OnDestroy()
    {
        EventManager.RemoveListener(EventManager.instance, EventConfig.RETICLE_SETPOSITION, SetPosition);
        EventManager.RemoveListener(EventManager.instance, EventConfig.VR_ONOVER, OnOver);
        EventManager.RemoveListener(EventManager.instance, EventConfig.VR_ONOUT, OnOut);
    }

    public void ReticleHide()
    {
        _imgReticle.enabled = false;
    }

    public void ReticleShow()
    {
        _imgReticle.enabled = true;
    }

    public void SelectionBarShow()
    {
        _imgSelectionBar.gameObject.SetActive(true);
        _isSelectionRadialActive = true;
        _imgReticle.gameObject.SetActive(true);
    }

    public void SelectionBarHide()
    {
        _imgSelectionBar.gameObject.SetActive(false);
        _isSelectionRadialActive = false;

        _imgSelectionBar.fillAmount = 0f;
    }

    private IEnumerator FillSelectionRadial()
    {

        _radialFilled = false;
        float timer = 0f;
        _imgSelectionBar.fillAmount = 0f;
        while (timer < _SelectionDuration)
        {
            _imgSelectionBar.fillAmount = timer / _SelectionDuration;
            timer += Time.deltaTime;
            yield return null;
        }
        _imgSelectionBar.fillAmount = 1f;
        _isSelectionRadialActive = false;
        _radialFilled = true;
        if (hitInfo != null)
        {
            EventManager.instance.DispatchEvent(EventManager.instance, EventConfig.VR_RETICLE_COMPLETE, hitInfo);
            yield return StartCoroutine(BackToZero());
        }
    }

    IEnumerator BackToZero()
    {
        yield return null;
        _imgSelectionBar.fillAmount = 0f;
    }

    public IEnumerator WaitForSelectionRadialToFill()
    {
        _radialFilled = false;
        SelectionBarShow();
        while (!_radialFilled)
        {
            yield return null;
        }
        SelectionBarHide();
    }

    public void UpdateReticle(bool isShow)
    {
        if (isShow)
        {
            ReticleShow();
        }
        else
        {
            ReticleHide();
        }
    }

    private void SetPosition(params object[] args)
    {
        //射线击中物体;
        if (args != null)
        {
            if (args[0] != null)
            {
                hitInfo = args[0] as HitInfo;
                if (hitInfo.hitTarget.layer.Equals(LayerMask.NameToLayer(Global.LayerType.UIModel3D))
                    || hitInfo.hitTarget.layer.Equals(LayerMask.NameToLayer(Global.LayerType.UI)))
                {

                    if (hitInfo.hitTarget.gameObject.name == "MoviePlane" || hitInfo.hitTarget.gameObject.name == "MovieBox")
                    {

                        _imgReticle.gameObject.SetActive(false);
                        _imgSelectionBar.gameObject.SetActive(false);
                        _isSelectionRadialActive = true;

                    }
                    else
                    {
                        SelectionBarShow();

                    }
                }
                else
                {
                    SelectionBarHide();
                }
                offset = hitInfo.hit.point - Camera.main.transform.position;
                offset = offset.normalized;
                _ReticleTransform.position = hitInfo.hit.point - offset * 0.1f;
                _ReticleTransform.localScale = _originalScale * hitInfo.hit.distance;
                if (_UseNormal)
                    _ReticleTransform.rotation = Quaternion.FromToRotation(Vector3.forward, hitInfo.hit.normal);
                else
                    _ReticleTransform.localRotation = _originalRotation;
            }
        }
        //射线没有击中物体,则将UI置于默认位置;
        else
        {
            _ReticleTransform.position = _Camera.position + _Camera.forward * _DefaultDistance;
            _ReticleTransform.localScale = _originalScale * _DefaultDistance;
            _ReticleTransform.localRotation = Quaternion.LookRotation(_Camera.forward);
        }
    }

    private void OnOver(params object[] args)
    {

        if (_isSelectionRadialActive)
        {
            _SelectionFillRoutine = StartCoroutine(FillSelectionRadial());

        }
    }

    private void OnOut(params object[] args)
    {
        if (_isSelectionRadialActive)
        {
            _imgSelectionBar.fillAmount = 0f;

            if (_SelectionFillRoutine != null)
                StopCoroutine(_SelectionFillRoutine);
        }
    }
}
