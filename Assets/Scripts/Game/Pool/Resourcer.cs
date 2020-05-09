using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Resourcer
{
    private static Resourcer _instance = new Resourcer();
    public static Resourcer instance { get { return _instance; } }
    public Resourcer()
    {
        if (_instance != null)
        {
            Debug.LogError("请使用instance获取Resourcer");
        }
    }

    public static T Get<T>(string resId, string name)
    {
        T t = default(T);

        if (t == null)
        {
            t = (T)(object)Resources.Load(resId + "/" + name, typeof(T));
            if (t == null)
            {
                foreach (string dir in ResConfig.RESDIR)
                {
                    t = (T)(object)Resources.Load(dir + name, typeof(T));
                    if (t != null)
                    {
                        break;
                    }
                }
            }
        }
        return t;
    }

    public static T Get<T>(string name)
    {
        T t = default(T);

        if (t == null)
        {
            foreach (string dir in ResConfig.RESDIR)
            {
                t = (T)(object)Resources.Load(dir + name, typeof(T));
                if (t != null)
                {
                    break;
                }
            }
        }
        return t;
    }

    public static AudioClip GetAudio(string name, int index = 0)
    {
        if (index > 0)
        {
            return Get<AudioClip>(ResConfig.AUDIO_ID, name + "_" + index.ToString());
        }
        return Get<AudioClip>(ResConfig.AUDIO_ID, name);
    }

    public static GameObject GetGameObject(string name, string dirName = null)
    {
        GameObject go = null;

        GameObject prefab = null;
        if (dirName != null)
        {
            prefab = Get<GameObject>(dirName, name);
        }
        else
        {
            prefab = Get<GameObject>(name);
        }

        if (prefab != null)
        {
            go = GameObject.Instantiate(prefab) as GameObject;
            go.name = name;
        }
        return go;
    }
}

