using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PureMVC.Patterns;

public class BattleMediator : Mediator
{
	public new static string NAME = "BattleMediator";

	private BattleProxy battleproxy = null;

	private BattleView BattleView
	{
		get { return (BattleView)ViewComponent; }
	}

	public BattleMediator(BattleView view = null) : base(NAME, view)
	{

	}

	public override void OnRegister()
	{
		base.OnRegister ();
		battleproxy = Facade.RetrieveProxy (BattleProxy.NAME) as BattleProxy;
		if (null == battleproxy)
			throw new Exception ("获取" + BattleProxy.NAME + "代理失败");

		//Adds

	}

	public override void OnRemove()
	{

	}

	public override IList<string> ListNotificationInterests()
	{
		return new List<String>()
		{

		};
	}

	public override void HandleNotification(PureMVC.Interfaces.INotification notification)
	{
		switch (notification.Name) 
		{

		default:
			break;
		}
	}
}
