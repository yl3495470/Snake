using System.Security.Cryptography;

public class MD5Util
{
    public static string Encrypt(string strPwd, bool isToUpper = true)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.Default.GetBytes(strPwd);
        byte[] md5data = md5.ComputeHash(data);
        md5.Clear();
        string str = "";
        for (int i = 0; i < md5data.Length; i++)
        {
            str += md5data[i].ToString("x").PadLeft(2, '0');
        }
        if (isToUpper)
        {
            return str.ToUpper();
        }
        else
        {
            return str.ToLower();
        }
       
    }
}
