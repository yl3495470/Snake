using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using Basic.Managers.Helper;

public class UIModule : BaseModule
{
    protected override void _Start()
    {
		facade.RegisterProxy(new UIProxy());
		facade.RegisterMediator(new UIMediator());
    }

    protected override void _Dispose()
    {
        facade.RemoveProxy(UIProxy.NAME);
        facade.RemoveMediator(UIMediator.NAME);
    }

	protected override void _Init()
	{
		_scene = "Start";
		base._Init ();
	}
}
