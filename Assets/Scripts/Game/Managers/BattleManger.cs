using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;
using System.IO;
using UnityStandardAssets.CrossPlatformInput;

public class BattleManger : Singleton<BattleManger>
{
    private Action _luaUpdate;
    private Action _luaOnDestroy;

    private Action _turnUp;
    private Action _turnDown;
    private Action _turnLeft;
    private Action _turnRight;

    internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    private LuaTable scriptEnv;

    private Transform UiRoot;
    private Joystick joystick;

    public void Init()
    {        
        Debug.Log("Create Battle");
    }

    void Awake()
    {
        ResourceManger.LoadTextAsset("Assets.XLua.MyLua.Resources.GameManger.lua.txt", InitLua);
    }

    void InitLua(TextAsset luaStr)
    {
        joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
        UiRoot = GameObject.Find("UIRoot").transform;

        scriptEnv = luaEnv.NewTable();

        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);
        scriptEnv.Set("UiRoot", UiRoot);

        luaEnv.DoString(luaStr.text, "GameManger", scriptEnv);

        Action luaAwake = scriptEnv.Get<Action>("awake");
        scriptEnv.Get("update", out _luaUpdate);
        scriptEnv.Get("ondestroy", out _luaOnDestroy);

        scriptEnv.Get("TurnUp", out _turnUp);
        scriptEnv.Get("TurnDown", out _turnDown);
        scriptEnv.Get("TurnLeft", out _turnLeft);
        scriptEnv.Get("TurnRight", out _turnRight);

        if (luaAwake != null)
        {
            luaAwake();
        }
    }


    float hValue = 0.0f;
    float vValue = 0.0f;
    void Update()
    {
        hValue = CrossPlatformInputManager.GetAxis("Horizontal");
        vValue = CrossPlatformInputManager.GetAxis("Vertical");
        
        if (Math.Abs(hValue) > Math.Abs(vValue))
            vValue = 0;
        else
            hValue = 0;

        if (hValue < -0.35f)
        {
            if (_turnLeft != null)
                _turnLeft();
        }
        else if(hValue > 0.35f)
        {
            if (_turnRight != null)
                _turnRight();
        }
        if (vValue > 0.35f)
        {
            if (_turnUp != null)
                _turnUp();
        }
        else if (vValue < -0.35f)
        {
            if (_turnDown != null)
                _turnDown();
        }

        if (_luaUpdate != null)
        {
            _luaUpdate();
        }
        if (Time.time - BattleManger.lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            BattleManger.lastGCTime = Time.time;
        }
    }

    void OnDestroy()
    {
        if (_luaOnDestroy != null)
        {
            _luaOnDestroy();
        }
        _luaOnDestroy = null;
        _luaUpdate = null;
        scriptEnv.Dispose();
    }
}
