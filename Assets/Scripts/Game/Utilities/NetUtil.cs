using UnityEngine;
using System;
using System.Net;

public class NetUtil
{

    public static string GetIPByDns(string url)
    {
        try
        {
            if(System.Text.RegularExpressions.Regex.IsMatch(url, @"((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)"))
            {
                return url;
            }

            IPHostEntry hostinfo = Dns.GetHostEntry(url);
            IPAddress[] aryIP = hostinfo.AddressList;

            //Log.Debug("ip = " + aryIP[0].ToString());
            if (aryIP.Length == 0)
            {
                return null;
            }
            else
            {
                return aryIP[0].ToString();
            }
            
        }
        catch (Exception e)
        {
            Debug.LogError("GetIPByDns:" + e.Message);
            return null;
        }
    }
}
