using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class TextUtil
{
    /// <summary>
    /// 将text文本中需要替换的内容（#1，#2，...）替换为args中列举的字符串
    /// </summary>
    public static string Replace(string text, params string[] args)
    {
        return Regex.Replace(text, @"#\d", (match) =>
        {
            int index = int.Parse(match.Groups[0].Value.Substring(1));
            return args[index - 1];
        });
    }

    public static byte[] Read(string url)
    {
#if !UNITY_WEBPLAYER
        if (File.Exists(url))
        {
            try
            {
                return File.ReadAllBytes(url);
            }
            catch (Exception error)
            {
                Debug.LogError("Error: " + error.Message);
            }
        }
#endif

        return null;
    }

    public static void Write(string url, byte[] bytes)
    {
#if !UNITY_WEBPLAYER
        try
        {
            File.WriteAllBytes(url, bytes);
        }
        catch (Exception error)
        {
            Debug.LogError("Error: " + error.Message);
        }
#endif

    }

    // 加密解密
    public static string EncryptDecryptStr(string str, byte key = 0)
    {
        if (key == 0) return str;
        byte[] bs = Encoding.Default.GetBytes(str);
        for (int i = 0; i < bs.Length; i++)
        {
            bs[i] = (byte)(bs[i] ^ key);
        }
        return Encoding.Default.GetString(bs);
    }


    /// <summary>
    /// 字符串长度(按字节算)
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int StrLength(string str)
    {
        int len = 0;
        byte[] b;

        for (int i = 0; i < str.Length; i++)
        {
            b = Encoding.Default.GetBytes(str.Substring(i, 1));
            if (b.Length > 1)
                len += 2;
            else
                len++;
        }

        return len;
    }
}

