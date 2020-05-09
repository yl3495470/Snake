using UnityEngine;
using System.Collections;

public class GameUtility : MonoBehaviour {

    /// <summary>  
    /// 获取子节点  
    /// </summary>  
    public static Transform GetChild(GameObject root, string path)
    {
        Transform tra = root.transform.Find(path);
        if (tra == null) Debug.Log(path + "not find");
        return tra;
    }

    /// <summary>  
    /// 获取子节点组件  
    /// </summary>  
    public static T GetChildComponent<T>(GameObject root, string path) where T : Component
    {
        Transform tra = root.transform.Find(path);
        if (tra == null) Debug.Log(path + "not find");
        T t = tra.GetComponent<T>();
        return t;
    } 
 
    //shaw 2016/10/09  获取深度子节点
    public static Transform FindDeepChild(GameObject _target, string _childName)
    {
        Transform resultTrs = null;
        resultTrs = _target.transform.Find(_childName);
        if (resultTrs == null)
        {
            foreach (Transform trs in _target.transform)
            {
                resultTrs = GameUtility.FindDeepChild(trs.gameObject, _childName);
                if (resultTrs != null)
                    return resultTrs;
            }
        }
        return resultTrs;
    }

}
