using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
//            if (!Application.isPlaying)
//            {
//                Debuger.LogWarning("[Singleton] Instance '" + typeof(T) +
//                    " Won't create InEditorModel - returning null.");
//                return null;
//            }
//
//            if (applicationIsQuitting)
//            {
//                Debuger.LogWarning("[Singleton] Instance '" + typeof(T) +
//                    "' already destroyed on application quit." +
//                    " Won't create again - returning null.");
//                return null;
//            }

            lock(_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if(FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return _instance;
                    }

                    if(_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString();//"(singleton)" + typeof(T).ToString();
                        addToOneContainer(singleton.transform);
                        
                        DontDestroyOnLoad(singleton);
                    }
                    else
                    {
//                        Debuger.Log("[Singleton] Using instance already created: " +
//                            _instance.gameObject.name);
                    }
                }
                return _instance;
            }
        }
    }

    private static void addToOneContainer(Transform pTrans)
    {
        /*
        string tContainerName = "SingleTonContainer";
        GameObject tContainer = GameObject.Find(tContainerName);
        if (tContainer == null) 
            tContainer = new GameObject(tContainerName);
            */

        DontDestroyOnLoad(pTrans);

        pTrans.parent = null;//tContainer.transform;
    }
}