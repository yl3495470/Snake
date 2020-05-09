using UnityEngine;
using System.Collections;
using Basic.Managers.Helper;
using Basic.Managers;

public class InitializeModule : BaseModule
{
    public InitializeModule()
        : base()
    {
        
    }

    protected override void _Start()
    {
        Application.targetFrameRate = GameConfig.FRAME_RATE;
        Input.multiTouchEnabled = false;

        EventManager.instance.Init();

//		ProjectConfig.Instance.InitConfigData ();
		if (Engine.Instance.isConnectSever)
			;

		InitManagers();
        AddModules();
    }

    private void AddModules()
    {
        ModuleManager.instance.AddAdditionalModule(typeof(UIModule));
        ModuleManager.instance.GotoModule(typeof(BattleModule));
    }

    private void InitManagers()
    {
		OTManager.instance.AddOT(Resourcer.Get<TextAsset>(ResConfig.OT).text);
    }

	protected override void _Dispose ()
	{
		
	}
}
