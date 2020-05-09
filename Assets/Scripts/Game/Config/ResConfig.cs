using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResConfig
{
    public enum SceneName
    {
        Login,
        Game
    }

    public static string[] RESDIR = { 
        "AUDIO/", "FX/", "GUI/", "TPL/", "MUSIC/", "MODEL/",
        "ANIMATION/", "TEXTURE/", "OTHER/", "TEXT/",
    };
		
    //动画 ANIMATION;

    //音效 AUDIO;
    public const string AUDIO_OK = "Cancle";                //确认按钮;
    public const string AUDIO_CLOSE = "Close";              //关闭;
    public const string AUDIO_CANCLE = "Cancle";            //取消;
    public const string AUDIO_RETURN = "Return";            //返回;
    public const string AUDIO_HINTNOTICE = "HintNotice";    //错误提示;
    public const string AUDIO_BGM = "BGM";                  //背景音乐;

    //特效 EFFECTS;

    //UI界面 GUI;

    //配置文件 TPL;

    //音乐 MUSIC;

    //模型 MODEL;

    //材质 TEXTURE;

    //其他 OTHER;
    public const string OT = "OT";
    public const string CORE = "Core";
    public const string TPL_ID = "TPL";
    public const string TRAIL_NAME = "GUI_Trail";
    public const string AUDIO_ID = "AUDIO";
    public const string MODEL_ID = "MODEL";
    public const string OTHER_ID = "OTHER";
	public const string TEXT_ID = "TEXT";
    public const string MIKUDANCING = "MikuDancing";
}
