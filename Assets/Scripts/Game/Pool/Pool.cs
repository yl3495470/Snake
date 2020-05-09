using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pool : MonoBehaviour
{
    private static Pool _instance;
    public static Pool instance { get { return _instance; } }

    private IDictionary<string, GameObject> _pooler = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private GameObject _Reuse(string name, string dirName = null)
    {
        if (_pooler.ContainsKey(name)) return _pooler[name];
        return Resourcer.GetGameObject(name, dirName);
    }

    public static GameObject GetPrefab(string name)
    {
        return Resourcer.Get<GameObject>(name);
    }

    public static GameObject GetPrefab(string name, string dirName)
    {
        GameObject go = _instance._Reuse(name, dirName);
        if (go != null)
            go.SetActive(true);
        return go;
    }

    public static GameObject Get(string name, Transform parent, string dirName = null)
    {
        return Get(name, parent, Vector3.zero, Quaternion.identity, dirName);
    }

    public static GameObject Get(string name, Transform parent, Vector3 localPosition, Quaternion localRotation, string dirName = null)
    {
        GameObject go = _instance._Reuse(name, dirName);
        if (go != null)
        {
            go.transform.parent = parent;
            go.transform.localPosition = localPosition;
            go.transform.localRotation = localRotation;
            go.transform.localScale = Vector3.one;
            go.SetActive(true);
        }
        return go;
    }

    public static GameObject Get(string name, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, string dirName = null)
    {
        GameObject go = _instance._Reuse(name, dirName);
        if (go != null)
        {
            go.transform.parent = parent;
            go.transform.localPosition = localPosition;
            go.transform.localRotation = localRotation;
            go.transform.localScale = localScale;
            go.SetActive(true);
        }
        return go;
    }

    public static GameObject Get(string name, string dirName = null)
    {
        return Get(name, Vector3.zero, Quaternion.identity, dirName);
    }

    public static GameObject Get(string name, Vector3 position, Quaternion rotation, string dirName = null)
    {
        GameObject go = _instance._Reuse(name, dirName);
        if (go != null)
        {
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.SetActive(true);
        }
        return go;
    }

    public static GameObject Get(GameObject prefab, Transform parent)
    {
        return Get(prefab, parent, Vector3.zero, Quaternion.identity);
    }

    public static GameObject Get(GameObject prefab)
    {
        return Get(prefab, Vector3.zero, Quaternion.identity);
    }

    public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab != null)
        {
            GameObject go = _instance._Reuse(prefab.name);
            if (go == null)
            {
                go = GameObject.Instantiate(prefab) as GameObject;
                go.name = go.name.Replace("(Clone)", "");
            }
            if (go != null)
            {
                go.transform.position = position;
                go.transform.rotation = rotation;
                go.SetActive(true);
            }
            return go;
        }
        return null;
    }

    public static GameObject Get(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        if (prefab != null)
        {
            GameObject go = _instance._Reuse(prefab.name);
            if (go == null)
            {
                go = GameObject.Instantiate(prefab) as GameObject;
                go.name = go.name.Replace("(Clone)", "");
            }
            if (go != null)
            {
                go.transform.parent = parent;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.SetActive(true);
            }
            return go;
        }
        return null;
    }

    public static void Destroy(GameObject go)
    {
        if (Engine.isQuitting || go == null)
            return;

        if (_instance._pooler.ContainsKey(go.name)) _instance._pooler.Remove(go.name);
        GameObject.Destroy(go);
    }

    public static void Recycle(GameObject go)
    {
        if (Engine.isQuitting || go == null)
            return;

        Recycler r = go.GetComponent<Recycler>();
        if (r != null && r.isPooled)
        {
            go.SetActive(false);
            go.transform.parent = _instance.transform;

            ParticleScaler[] pScalers = go.GetComponentsInChildren<ParticleScaler>(true);
            for (int i = 0; i < pScalers.Length; i++)
            {
                pScalers[i].ResetScale();
            }
        }
        else
        {
            GameObject.Destroy(go);
        }
    }

    public static void Recycle(Recycler rcl)
    {
        Recycle(rcl.gameObject);
    }

    public static T GetComponent<T>(GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }

    public static T GetComponent<T>(Component component) where T : Component
    {
        return GetComponent<T>(component.gameObject);
    }
}

