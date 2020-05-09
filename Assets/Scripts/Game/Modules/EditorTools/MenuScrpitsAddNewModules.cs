using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Reflection;

//[InitializeOnLoad]
public class MenuScrpitsAddNewModules
{
//	[MenuItem("PureMvc Tools/Init AdditionalModules Scrpits")]
	public static void InitAllModules()
	{
		foreach(string moduleName in AdditionalModulesName.AdditionNames)
		{
			string modulePath = Path.Combine("Assets/Scripts/Game/Modules/AdditionalModule/", moduleName) + "/";
			string viewPath = modulePath + "/View/";
			string cmdPath =  modulePath + "/Cmd/";

			//模块主路径建立
			if (!Directory.Exists (modulePath))
				Directory.CreateDirectory (modulePath);
			//模块View文件夹创建
			if (!Directory.Exists (viewPath))
				Directory.CreateDirectory (viewPath);
			//模块Cmd文件夹创建
			if (!Directory.Exists (cmdPath))
				Directory.CreateDirectory (cmdPath);

			//建立view视图文件
			AddViewScript(viewPath, moduleName);
			AddItemScript(viewPath, moduleName);
			//建立主视图文件
			AddMediatorScript (modulePath, moduleName);
			AddModuleScript (modulePath, moduleName);
			AddProxyScript (modulePath, moduleName);
			AddNotesScript (modulePath, moduleName);
		}

//		EditorUtility.DisplayDialog("成功生成模块文件","缺失文件已生成，请耐心等待文件加载出来","确定");
	}

	#region 创建脚本
	/// <summary>
	/// 添加Itme脚本
	/// </summary>
	/// <param name="viewPath">文件路径.</param>
	/// <param name="moduleName">模块名字</param>
	public static void AddItemScript(string viewPath, string moduleName)
	{
		moduleName = moduleName + "Item";
		viewPath += moduleName + ".cs";

		if(!File.Exists(viewPath))
		{
			StreamWriter tWriter = File.CreateText(viewPath);

			//IMPORT
			tWriter.WriteLine ("using UnityEngine;");
			tWriter.WriteLine ("using System;");
			tWriter.WriteLine ("using System.Collections;");
			tWriter.WriteLine ("using System.Collections.Generic;");
			tWriter.WriteLine ();
			tWriter.WriteLine ("public class {0} : MonoBehaviour", moduleName);
			tWriter.WriteLine ("{");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\tvoid Start () {");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\tvoid Update () {");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();
			tWriter.WriteLine ("}");

			tWriter.Flush();
			tWriter.Close();
		}
	}

	/// <summary>
	/// 添加view脚本
	/// </summary>
	/// <param name="viewPath">文件路径.</param>
	/// <param name="moduleName">模块名字</param>
	public static void AddViewScript(string viewPath, string moduleName)
	{
		moduleName = moduleName + "View";
		viewPath += moduleName + ".cs";

		if(!File.Exists(viewPath))
		{
			StreamWriter tWriter = File.CreateText(viewPath);

			//IMPORT
			tWriter.WriteLine ("using UnityEngine;");
			tWriter.WriteLine ("using System;");
			tWriter.WriteLine ("using System.Collections;");
			tWriter.WriteLine ("using System.Collections.Generic;");
			tWriter.WriteLine ();
			tWriter.WriteLine ("public class {0} : MonoBehaviour", moduleName);
			tWriter.WriteLine ("{");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\tvoid Start () {");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\tvoid Update () {");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();
			tWriter.WriteLine ("}");

			tWriter.Flush();
			tWriter.Close();
		}
	}

	/// <summary>
	/// 添加Mediator脚本
	/// </summary>
	/// <param name="modulePath">文件路径.</param>
	/// <param name="moduleName">模块名字</param>
	public static void AddMediatorScript(string modulePath, string moduleName)
	{
		moduleName = moduleName + "Mediator";
		modulePath += moduleName + ".cs";

		if(!File.Exists(modulePath))
		{
			StreamWriter tWriter = File.CreateText(modulePath);

			string proxyName = moduleName.Replace ("Mediator", "Proxy");
			string viewName = moduleName.Replace ("Mediator", "View");

			//IMPORT
			tWriter.WriteLine ("using UnityEngine;");
			tWriter.WriteLine ("using System;");
			tWriter.WriteLine ("using System.Collections;");
			tWriter.WriteLine ("using System.Collections.Generic;");
			tWriter.WriteLine ("using PureMVC.Patterns;");
			tWriter.WriteLine ();
			tWriter.WriteLine ("public class {0} : Mediator", moduleName);
			tWriter.WriteLine ("{");
			tWriter.WriteLine ("\tpublic new static string NAME = \"{0}\";", moduleName);
			tWriter.WriteLine ();
			tWriter.WriteLine ("\tprivate {0} {1} = null;", proxyName, proxyName.ToLower());
			tWriter.WriteLine ();

			tWriter.WriteLine ("\tprivate {0} {1}", viewName, viewName);
			tWriter.WriteLine ("\t{");
			tWriter.WriteLine ("\t\tget {{ return ({0})ViewComponent; }}", viewName);
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();

			tWriter.WriteLine ("\tpublic {0}({1} view = null) : base(NAME, view)", moduleName, viewName);
			tWriter.WriteLine ("\t{");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();

			tWriter.WriteLine ("\tpublic override void OnRegister()");
			tWriter.WriteLine ("\t{");
			tWriter.WriteLine ("\t\tbase.OnRegister ();");
			tWriter.WriteLine ("\t\t{0} = Facade.RetrieveProxy ({1}.NAME) as {2};", proxyName.ToLower (), proxyName, proxyName);
			tWriter.WriteLine ("\t\tif (null == {0})", proxyName.ToLower ());
			tWriter.WriteLine ("\t\t\tthrow new Exception (\"获取\" + {0}.NAME + \"代理失败\");", proxyName);
			tWriter.WriteLine ();
			tWriter.WriteLine ("\t\t//Adds");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();

			tWriter.WriteLine ("\tpublic override void OnRemove()");
			tWriter.WriteLine ("\t{");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();

			tWriter.WriteLine ("\tpublic override IList<string> ListNotificationInterests()");
			tWriter.WriteLine ("\t{");
			tWriter.WriteLine ("\t\treturn new List<String>()");
			tWriter.WriteLine ("\t\t{");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\t\t};");
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();

			tWriter.WriteLine ("\tpublic override void HandleNotification(PureMVC.Interfaces.INotification notification)");
			tWriter.WriteLine ("\t{");
			tWriter.WriteLine ("\t\tswitch (notification.Name) ");
			tWriter.WriteLine ("\t\t{");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\t\tdefault:");
			tWriter.WriteLine ("\t\t\tbreak;");
			tWriter.WriteLine ("\t\t}");
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ("}");

			tWriter.Flush();
			tWriter.Close();
		}
	}

	/// <summary>
	/// 添加Module脚本
	/// </summary>
	/// <param name="modulePath">文件路径.</param>
	/// <param name="moduleName">模块名字</param>
	public static void AddModuleScript(string modulePath, string moduleName)
	{
		moduleName = moduleName + "Module";
		modulePath += moduleName + ".cs";

		if(!File.Exists(modulePath))
		{
			StreamWriter tWriter = File.CreateText(modulePath);

			string proxyName = moduleName.Replace ("Module", "Proxy");
			string mediatorName = moduleName.Replace ("Module", "Mediator");

			//IMPORT
			tWriter.WriteLine ("using System;");
			tWriter.WriteLine ("using UnityEngine;");
			tWriter.WriteLine ("using System.Collections;");
			tWriter.WriteLine ("using System.Collections.Generic;");
			tWriter.WriteLine ("using PureMVC.Patterns;");
			tWriter.WriteLine ("using Basic.Managers.Helper;");
			tWriter.WriteLine ();

			tWriter.WriteLine ("public class {0} : BaseModule", moduleName);
			tWriter.WriteLine ("{");
			tWriter.WriteLine ("\tprotected override void _Start()");
			tWriter.WriteLine ("\t{");
			tWriter.WriteLine ("\t\tfacade.RegisterProxy(new {0}());", proxyName);
			tWriter.WriteLine ("\t\tfacade.RegisterMediator(new {0}());", mediatorName);
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();

			tWriter.WriteLine ("\tprotected override void _Dispose()");
			tWriter.WriteLine ("\t{");
			tWriter.WriteLine ("\t\tfacade.RemoveProxy({0}.NAME);", proxyName);
			tWriter.WriteLine ("\t\tfacade.RemoveMediator({0}.NAME);", mediatorName);
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();

			tWriter.WriteLine ("\tprotected override void _Init()");
			tWriter.WriteLine ("\t{");
			tWriter.WriteLine ("\t\t_scene = \"{0}\";", moduleName);
			tWriter.WriteLine ("\t\tbase._Init ();");
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();
	
			tWriter.WriteLine ("}");

			tWriter.Flush();
			tWriter.Close();
		}
	}

	/// <summary>
	/// 添加Proxy脚本
	/// </summary>
	/// <param name="modulePath">文件路径.</param>
	/// <param name="moduleName">模块名字</param>
	public static void AddProxyScript(string modulePath, string moduleName)
	{
		moduleName = moduleName + "Proxy";
		modulePath += moduleName + ".cs";

		if(!File.Exists(modulePath))
		{
			StreamWriter tWriter = File.CreateText(modulePath);

			string itemName = moduleName.Replace ("Proxy", "Item");

			//IMPORT
			tWriter.WriteLine ("using UnityEngine;");
			tWriter.WriteLine ("using System.Collections;");
			tWriter.WriteLine ("using System.Collections.Generic;");
			tWriter.WriteLine ("using PureMVC.Patterns;");
			tWriter.WriteLine ();

			tWriter.WriteLine ("public class {0} : Proxy", moduleName);
			tWriter.WriteLine ("{");

			tWriter.WriteLine ("\tpublic new static string NAME = \"{0}\";", moduleName);
			tWriter.WriteLine ();

			tWriter.WriteLine ("\tpublic {0}() : base(NAME, new List<{1}>())", moduleName, itemName);
			tWriter.WriteLine ("\t{");
			tWriter.WriteLine ();
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();

			tWriter.WriteLine ("\tpublic IList<{0}> {1}s", itemName, itemName);
			tWriter.WriteLine ("\t{");
			tWriter.WriteLine ("\t\tget {{ return (IList<{0}>)base.Data; }}", itemName);
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();

			tWriter.WriteLine ("\tpublic override void OnRegister()");
			tWriter.WriteLine ("\t{");
			tWriter.WriteLine ("\t\tbase.OnRegister();");
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();

			tWriter.WriteLine ("\tpublic override void OnRemove()");
			tWriter.WriteLine ("\t{");
			tWriter.WriteLine ("\t\tbase.OnRemove();");
			tWriter.WriteLine ("\t}");
			tWriter.WriteLine ();

			tWriter.WriteLine ("}");

			tWriter.Flush();
			tWriter.Close();
		}
	}

	/// <summary>
	/// 添加Notes脚本
	/// </summary>
	/// <param name="modulePath">文件路径.</param>
	/// <param name="moduleName">模块名字</param>
	public static void AddNotesScript(string modulePath, string moduleName)
	{
		moduleName = moduleName + "Notes";
		modulePath += moduleName + ".cs";

		if(!File.Exists(modulePath))
		{
			StreamWriter tWriter = File.CreateText(modulePath);

			tWriter.WriteLine ("public class {0}", moduleName);
			tWriter.WriteLine ("{");
			tWriter.WriteLine ();
			tWriter.WriteLine ("}");

			tWriter.Flush();
			tWriter.Close();
		}
	}

	#endregion
}