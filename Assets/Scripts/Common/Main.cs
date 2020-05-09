using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{
    private static Main _instance;
	public static Main instance { get { return _instance; } }

	[HideInInspector]
	public string chooseModel = ""; 
	[HideInInspector]
	public string chooseTerrain = "";

    void Awake() 
	{
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
}
