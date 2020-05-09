using System;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    public GameObject dotaScene { get { return _dotaScene; } }
    private GameObject _dotaScene;
    public GameObject townScene { get { return _townScene; } }
    private GameObject _townScene;

    private static GameScene _instance;
    public static GameScene instance { get { return _instance; } }
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);

            //_dotaScene = UIUtil.GetChildByName<Transform>(gameObject, "DotaScene").gameObject;
            //_townScene = UIUtil.GetChildByName<Transform>(gameObject, "TownScene").gameObject;

            //_townScene.AddComponent<TouchWatcher>();
            //_townScene.AddComponent<HollUI>();
            //CameraDrag dragger = _townScene.AddComponent<CameraDrag>();

            //Layout_CameraControl layout_control = UIUtil.GetChildByName<Layout_CameraControl>(gameObject, "Main Camera Delegate");
            //CameraControl control = layout_control.gameObject.AddComponent<CameraControl>();
            //control.layout = layout_control;
            //control.dragger = dragger;
        }
        else
        {
            Destroy(this);
        }
    }
}