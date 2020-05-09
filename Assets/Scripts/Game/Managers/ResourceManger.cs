using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Tangzx.ABSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ResourceManger : Singleton<ResourceManger>
{
    public AssetBundleManager AbManager;

    private const string urlParentPath = "http://118.25.125.147:80/Snake/AssetBundles/";


    private Dictionary<string, Hash128> localHash = new Dictionary<string, Hash128>();
    private Queue<string> downLoadFiles = new Queue<string>();
    private int maxDownCount = 0;

    private FileInfo file;

    public void Init()
    {
        AbManager = gameObject.AddComponent<AssetBundleManager>();
        AssetBundleManager.Instance.Init(() =>
        {
            InitComplete();
        });
    }

    public static void LoadGameObject(string path, UnityAction<GameObject> onComplete)
    {
        AssetBundleLoader loader = ResourceManger.Instance.AbManager.Load(path, (a) =>
        {
            GameObject go = Instantiate(a.mainObject) as GameObject;
            onComplete(go);
        });
    }

    public static void LoadTextAsset(string path, UnityAction<TextAsset> onComplete)
    {
        AssetBundleLoader loader = ResourceManger.Instance.AbManager.Load(path, (a) =>
        {
            TextAsset text = a.mainObject as TextAsset;
            onComplete(text);
        });
    }
    

    public static string GetSavePath()
    {
        string filePath = null;
#if UNITY_EDITOR
        filePath = string.Format("file://{0}/AssetBundles/", Application.streamingAssetsPath);
#elif UNITY_STANDALONE_WIN
        filePath = string.Format("file://{0}/AssetBundles/", Application.streamingAssetsPath);
#elif UNITY_ANDROID
        filePath = string.Format("{0}/AssetBundles/", Application.streamingAssetsPath);
#elif UNITY_IOS
        filePath = string.Format("file://{0}/Raw/AssetBundles/", Application.dataPath);
#else
        throw new System.NotImplementedException();
#endif
        return filePath;
    }

    public void InitComplete()
    {
        if(Engine.Instance.isConnectSever)
            StartCoroutine(LoadFromPackage());
        else
        {
            //本地测试
            Invoke("DelayLocal", 1.0f);
        }
    }

    private void DelayLocal()
    {
        BattleManger.Instance.Init();
    }

    private IEnumerator LoadFromPackage()
    {
        Debug.Log("Pass LoadFromPackage");
        //加载主体
        string path = GetSavePath() + "AssetBundles";
        WWW www = new WWW(path);
        yield return www;

        //加载完缓存一份，便于下次快速加载
        if (www.error == null)
        {
            AssetBundle ab = www.assetBundle;
            AssetBundleManifest manifest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;

            string[] names = manifest.GetAllAssetBundles();

            for (int i = 0; i < names.Length; ++i)
            {
                localHash.Add(names[i], manifest.GetAssetBundleHash(names[i]));
            }
            ab.Unload(true);
        }else
        {
            Debug.Log("Pass WWW.ERROR");
        }

        www.Dispose();
        www = null;
        try
        {
            StartCoroutine(DownManifest(urlParentPath + "AssetBundles"));
        }
        catch (Exception ex)
        {
            Debug.Log("Error：" + ex.Message);
        }
    }

    IEnumerator DownManifest(string url)
    {
        Debug.Log("Pass DownManifest");
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            Debug.Log("url:" + url);
            Debug.Log("Begin Download:" + url + ",request.isHttpError:" + request.isHttpError + ",request.isHttpError:" + request.isNetworkError);
            yield return request.SendWebRequest();
            Debug.Log("End Download:" + request.error);

            if (request.isDone)
            {
                AssetBundle ab = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
                AssetBundleManifest manifest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;

                string[] names = manifest.GetAllAssetBundles();

                for (int i = 0; i < names.Length; ++i)
                {
                    Hash128 hash = manifest.GetAssetBundleHash(names[i]);
                    if (!localHash.ContainsKey(names[i]) || !hash.Equals(localHash[names[i]]))
                    {
                        if (localHash.ContainsKey(names[i]))
                            Debug.Log(hash.ToString() + "/" + localHash[names[i]].ToString());
                        downLoadFiles.Enqueue(names[i]);
                    }
                }
                maxDownCount = downLoadFiles.Count;
                Debug.Log("maxDownCount:" + maxDownCount);
                ResourceManger.LoadGameObject("Assets.Resources.GUI.DownloadPanel.prefab", BeginLoadResource);
            }
            Debug.Log("End isDone");
        }
    }

    public void BeginLoadResource(GameObject go)
    {
        Debug.Log("Pass BeginLoadResource");
        go.transform.SetParent(GameObject.Find("UIRoot").transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        DownloadPanel downloadPanel = go.GetComponent<DownloadPanel>();
        StartCoroutine(DownAssetBundle(downloadPanel));
    }

    IEnumerator DownAssetBundle(DownloadPanel panel)
    {
        if(downLoadFiles.Count == 0)
        {
            GameObject.Destroy(panel.gameObject);
            StartCoroutine(UpdateMF());
            yield break;
        }

        string abName = downLoadFiles.Dequeue();

        string url = urlParentPath + abName;
        string localPath = AbManager.pathResolver.BundleCacheDir + "/" + abName;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SendWebRequest();

            string info = "正在下载第" + (maxDownCount - downLoadFiles.Count) + "/" + maxDownCount + "个更新文件";
            while (!request.isDone)
            {
                panel.UpdateProcess(info, request.downloadProgress);
                yield return null;
            }
            CreatFile(localPath, request.downloadHandler.data);
            StartCoroutine(DownAssetBundle(panel));
        }
    }

    IEnumerator UpdateMF()
    {
        string abName = "AssetBundles";

        string url = urlParentPath + abName;
        string localPath = AbManager.pathResolver.BundleCacheDir + "/" + abName;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            while (!request.isDone)
            {
                Debug.Log("更新资源目录:" + request.downloadProgress / 100.0f);
            }
            CreatFile(localPath, request.downloadHandler.data);
            BattleManger.Instance.Init();
        }
    }

    void CreatFile(string path, byte[] bytes)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        path = path.Replace("file://", "");
#endif
        if (File.Exists(path))//判断一下本地是否有了该音频  如果有就不需下载
        {
            File.Delete(path);
        }

        FileInfo file = new FileInfo(path);
        Stream stream;
        stream = file.Create();
        stream.Write(bytes, 0, bytes.Length);
        stream.Close();
        stream.Dispose();
    }
}
