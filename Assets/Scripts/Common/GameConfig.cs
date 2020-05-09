using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
	public bool IsShowRecenter = false;

    #region singleton
    public static GameConfig instance { get { return _instance; } }
    private static GameConfig _instance;
    #endregion

    #region game_config
    public static Vector2 UISIZE { get { return instance.uiSize; } }
    public static int FRAME_RATE { get { return instance.frameRate; } }

    public Vector2 uiSize = new Vector2(960, 640);
    public int frameRate = 60;
    #endregion

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
    }
}

