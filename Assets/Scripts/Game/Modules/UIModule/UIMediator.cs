using UnityEngine;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIMediator : Mediator
{
    public new static string NAME = "UIMediator";

	private UIProxy uiProxy = null;

	public UIView UIView
	{
		get { return (UIView)ViewComponent; }
	}
    
	public UIMediator(UIView view = null) : base(NAME, view)
    {
		//在这将视图与消息机制绑定
    }

    public override void OnRegister()
    {
		base.OnRegister ();
		uiProxy = Facade.RetrieveProxy (UIProxy.NAME) as UIProxy;
		if (null == uiProxy)
			throw new Exception ("获取" + UIProxy.NAME + "代理失败");
		
		// Adds
		
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
