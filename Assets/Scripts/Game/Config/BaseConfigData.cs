using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class BaseConfigData {
	
	//public List<string> ExampleType = new List<string>();		//飞机类型数据

	public void Init(string pSourcData)
	{
		/*	加载样例代码
		ExampleType.Clear ();
		if (!string.IsNullOrEmpty(pSourcData))
		{
			JSONNode node = JSON.Parse(pSourcData);
			string tempStr = node ["ExampleType"].ToString ();
			tempStr = tempStr.Substring (1, tempStr.Length - 2);
			string [] strArray = tempStr.Split (',');
			for (int i = 0; i < strArray.Length; ++i) 
			{
				if(!string.IsNullOrEmpty(strArray [i]))
					ExampleType.Add (strArray [i]);
			}
		}
		*/
	}
}
