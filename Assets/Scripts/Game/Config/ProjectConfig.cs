using System.IO;
using SimpleJSON;
using UnityEngine;
using System.Collections;

public class ProjectConfig
{
    private static ProjectConfig _instance;

    public string serverIP;

    private ProjectConfig()
    {

    }

    public static ProjectConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ProjectConfig();
            }
            return _instance;
        }
    }

    public void InitConfigData()
    {
        StreamReader m_reader = null;
        m_reader = File.OpenText("./" + "ProjectConfig.txt");
        string str = m_reader.ReadToEnd();
        if (!string.IsNullOrEmpty(str))
        {
            JSONNode node = JSON.Parse(str);

            serverIP = (string)node["serverIP"];
            //Debug.Log(playerIndex + ":" + serverIP);
        }
        else
        {
            Debug.LogError("game config not found!!");
        }
        m_reader.Close();
        m_reader.Dispose();
    }
}
