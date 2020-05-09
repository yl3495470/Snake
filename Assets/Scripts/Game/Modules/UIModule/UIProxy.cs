using UnityEngine;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;

public class UIProxy : Proxy
{
    public new static string NAME = "UIProxy";

    public UIProxy() : base(NAME, new List<UIItem>())
    {
		
    }

	public IList<UIItem> UIItems
	{
		get { return (IList<UIItem>)base.Data; }
	}

    public override void OnRegister()
    {
        base.OnRegister();
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }
}
