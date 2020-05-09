using UnityEngine;
using System.Collections;
using Basic.Managers;
using System;
using Basic.Interfaces;
using System.Collections.Generic;

public class Engine :MonoBehaviour
{
    public static Engine Instance { get { return _instance as Engine; } }
    private static Engine _instance;

    public static bool isQuitting { get { return _instance._isQuitting; } }
    private bool _isQuitting = false;

    public GameObject splash { get { return _splash; } set { _splash = value; } }
    private GameObject _splash;

    public string loadingLanguage { get { return _loadingLanguage; } }
    private string _loadingLanguage;

    public static float mTimeScale
    {
        get
        {
            return Time.timeScale;
        }
        set
        {
            Time.timeScale = value;
        }
    }
    public delegate void EventListener<T>(T args);

    #region TickObjectQueue
    private List<ITickObject> _tickObjects = new List<ITickObject>();
    public static void AddTickObject(ITickObject tickObject)
    {
        if (tickObject == null)
        {
            Debug.LogError("Warning: Don't add the null object to tick object list");
            return;
        }
        if (_instance._tickObjects.Exists(t => t == tickObject))
        {
            Debug.LogError("Warning: Don't add the same tick object twice.");
            return;
        }
        _instance._tickObjects.Add(tickObject);
    }

    public static void RemoveTickObject(ITickObject tickObject)
    {
        if (tickObject == null)
        {
            Debug.LogError("Warning: Can not remove the null object from the tick object list");
            return;
        }
        if (!_instance._tickObjects.Remove(tickObject))
        {
            Debug.LogError("Warning: Remove tick object error. May be the tick object is not in list.");
        }
    }
    #endregion

    private bool _isStarted;//Web版本的Code是最先加载的，用于预先启动所有统计

	#region PreConfig 

	public bool isConnectSever = false;

	#endregion
    
	#region Init

    protected void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
     
        new GameObject("Pool").AddComponent<Pool>();
    }

    void Start()
    {
		_isStarted = !isConnectSever; 		//根据是否连接服务器来判断是否直接开始
		_Init();
    }

	private void _Init()
    {
		ModuleManager.instance.GotoModule(typeof(InitializeModule));
        if (_isStarted) 
        {

        }
    }

	#endregion

    /// <summary>
    /// Update Game (None MonoBehaviour Partition.)
    /// </summary>
    protected void Update()
    {
        for (int i = 0; i < _tickObjects.Count; i++)
        {
            _tickObjects[i].Update();
        }
    }

	void OnLevelWasLoaded ()
	{
		mTimeScale = 1f;
	}
	
	void OnApplicationQuit ()
	{
        _isQuitting = true;
    }

	public void DoAfter(int frame, Action action)
	{
        StartCoroutine(_DoAfter(frame, action));
	}

	public void DoAfter(float time, Action action)
	{
		StartCoroutine(_DoAfter(time, action));
	}
	
	public IEnumerator _DoAfter(float time, Action onComplete)
	{
        yield return new WaitForSeconds(time);
		if (onComplete != null) onComplete();
	}
	
	public IEnumerator _DoAfter(int frame, Action onComplete)
	{
		for (int i = 0; i < frame; i++) yield return null;
		if (onComplete != null) onComplete();
	}
}