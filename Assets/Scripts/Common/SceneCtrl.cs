using UnityEngine;
using System.Collections;
using System;

public class SceneCtrl : MonoBehaviour
{
    private static SceneCtrl _instance;
    public static SceneCtrl Instance { get { return _instance as SceneCtrl; } }

    public Camera uiCamera2D { get { return _uiCamera2D; } }
    private Camera _uiCamera2D;
    public Camera uiFxCamera { get { return _uiFxCamera; } }
    private Camera _uiFxCamera;
    public Transform uiLayer2D { get { return _uiLayer2D; } }
    private Transform _uiLayer2D;
    public Transform uiLayerFx { get { return _uiLayerFx;} }
    private Transform _uiLayerFx;

    public static Action OnSceneLoaded;
    public static Action OnSceneDestroy;

	[HideInInspector]
	public Transform initTran = null;

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }
        _instance = this;

        _InitLayer();

        if (OnSceneLoaded != null)
        {
            OnSceneLoaded();
        }
    }

    private void _InitLayer()
    {
        _uiCamera2D = Camera.main;
        _uiLayer2D = _uiCamera2D.transform.parent;

        _uiCamera2D.rect = Camera.main.rect;
   
    }
    

#if UNITY_WEBPLAYER
    void Update()
    {
        AspectUtility.instance.UpdateLayout();
        _UpdateLayout();

        if (AspectUtility.screenWidth * GameConfig.UISIZE.y > AspectUtility.screenHeight * GameConfig.UISIZE.x)
        {
            _uiCamera2D.rect = _uiFxCamera.rect = Camera.main.rect;
        }
        else
        {
            _uiCamera2D.rect = _uiFxCamera.rect = Camera.main.rect;
        }
    }
#endif

    void OnDestroy()
    {
        if (OnSceneDestroy != null)
        {
            OnSceneDestroy();
        }
    }
}
