using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PureMVC.Patterns;
using Basic.Managers.Helper;

public class BattleModule : BaseModule
{
	protected override void _Start()
	{
		facade.RegisterProxy(new BattleProxy());
		facade.RegisterMediator(new BattleMediator());
	}

	protected override void _Dispose()
	{
		facade.RemoveProxy(BattleProxy.NAME);
		facade.RemoveMediator(BattleMediator.NAME);
	}

	protected override void _Init()
	{
		_scene = "BattleScene";
        ResourceManger.Instance.Init();
        base._Init ();
	}

}
