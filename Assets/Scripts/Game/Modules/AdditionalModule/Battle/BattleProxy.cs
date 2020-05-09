using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PureMVC.Patterns;

public class BattleProxy : Proxy
{
	public new static string NAME = "BattleProxy";

	public BattleProxy() : base(NAME, new List<Mission>())
	{

	}
    
    public IList<Mission> MissionItems
    {
        get { return (IList<Mission>)base.Data; }
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
