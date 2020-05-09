using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class ClientDataConfig {

	#region 单例

	private ClientDataConfig()
	{
		
	}

	private static ClientDataConfig _instance;

	public static ClientDataConfig Instance
	{
		get{
			if (_instance == null)
				_instance = new ClientDataConfig ();
			return _instance;
		}
	}

	#endregion

	#region 数据载体

	public BaseConfigData mBaseData;

	#endregion

	#region 加载

	public void LoadFromLocal()
	{
		LoadBaseDataFromLocal ();
	}

	public void LocalFormSever(string pBaseData)
	{
		LoadBaseDataFromSever (pBaseData);
	}

	private void LoadBaseDataFromLocal()
	{
		mBaseData = new BaseConfigData ();
		TextAsset tAsset = Resourcer.Get<TextAsset> (ResConfig.TEXT_ID, "BaseConfigData");
		if (tAsset != null)
		{
			string tConfigStr = tAsset.text;
			mBaseData.Init (tConfigStr);
		}
	}

	private void LoadBaseDataFromSever(string pData)
	{
		mBaseData = new BaseConfigData ();
		string tConfigStr = pData;
		mBaseData.Init (tConfigStr);
	}

	#endregion 
}
