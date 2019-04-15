using LitJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

/************************* 
* 作者： wenyueyun 
* 时间： 2018/6/27 10:30:19 
* 描述： EditorUtil 
*************************/
public class EditorUtil
{
    public static readonly string resources_path = Path.Combine(Application.dataPath, "../../Resources/");
    public static string Platform
    {
        get
        {
#if UNITY_ANDROID
            return "android";
#elif UNITY_IOS
                        return "ios";
#else
                        return "windows";
#endif
        }
    }

    //完整路径转资源路径
    public static string FullPathToAssetPath(string fullPath)
    {
        return "Assets" + fullPath.Replace('\\', '/').Substring(Application.dataPath.Length);
    }

    //将json数据进行格式化
    public static string JsonFormat(string str)
    {
        JsonSerializer serializer = new JsonSerializer();
        StringReader sReader = new StringReader(str);
        JsonTextReader jReader = new JsonTextReader(sReader);
        object readerObj = serializer.Deserialize(jReader);
        if (readerObj != null)
        {
            StringWriter sWriter = new StringWriter();
            JsonTextWriter jWriter = new JsonTextWriter(sWriter)
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                Indentation = 2,
                IndentChar = ' '
            };
            serializer.Serialize(jWriter, readerObj);
            return sWriter.ToString();
        }
        else
        {
            return str;
        }
    }

    //创建文件夹
    public static void CreateDir(string dir, bool delete = false)
    {
        if (Directory.Exists(dir))
        {
            if (delete)
            {
                Directory.Delete(dir, true);
                Directory.CreateDirectory(dir);
            }
        }
        else
        {
            Directory.CreateDirectory(dir);
        }
    }

    //复制文件夹
    public static void CopyDir(string source, string target, string[] ignore = null)
    {
        source = source.TrimEnd(new char[] { '/', '\\' });
        target = target.TrimEnd(new char[] { '/', '\\' });
        if (!Directory.Exists(target)) Directory.CreateDirectory(target);
        string[] sfiles = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
        foreach (string sfile in sfiles)
        {
            if (ignore != null && isIgnore(sfile, ignore))
            {
                continue;
            }
            var tfile = target + sfile.Substring(source.Length);
            var tdir = Path.GetDirectoryName(tfile);
            if (!Directory.Exists(tdir)) Directory.CreateDirectory(tdir);
            File.Copy(sfile, tfile, true);
        }
    }

    //忽略文件
    private static bool isIgnore(string str, string[] ignore)
    {
        for (int i = 0; i < ignore.Length; i++)
            if (str.Contains(ignore[i])) return true;
        return false;
    }

    //删除文件夹
    public static void DeleteDir(string dir)
    {
        if (Directory.Exists(dir))
        {
            Directory.Delete(dir);
        }
    }

    //复制文件
    public static void CopyFile(string source, string target)
    {
        File.Copy(source, target, true);
    }

    //时间版本号
    public static string Version()
    {
        return System.DateTime.Now.ToString("yyyyMMddHHmmss");
    }

    //生成配置文件
    public static void ModifyConfig(string key, string value)
    {
        string path = Path.Combine(Application.dataPath, "Resource/config.json");
        if (File.Exists(path))
        {
            JsonData json;
            using (FileStream fs = File.Open(path, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string str = sr.ReadToEnd();
                    json = JsonMapper.ToObject(str);
                    json[key] = value;
                }
                File.WriteAllText(path, EditorUtil.JsonFormat(json.ToJson()));
            }

            CopyFile(path, Path.Combine(Application.dataPath, "StreamingAssets/config.json"));
        }
    }

    // 文件夹生成MD5
    public static void MD5(string directory)
    {
        JsonData json = new JsonData();
        string[] files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if (file.Contains(".manifest")) continue;
            if (file.Contains(".meta")) continue;
            if (file.Contains(".json")) continue;
            string fullName = file.Replace('\\', '/');
            //生成MD5
            FileStream fs = new FileStream(fullName, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            //计算size
            FileInfo fileInfo = new FileInfo(fullName);

            //创建json
            JsonData temp = new JsonData();
            temp["path"] = fullName.Substring(directory.Length);
            temp["md5"] = sb.ToString();
            temp["size"] = fileInfo.Length;
            json.Add(temp);
        }

        File.WriteAllText(Path.Combine(directory, "md5.json"), EditorUtil.JsonFormat(json.ToJson()));

        Debug.Log("生成MD5:" + Path.Combine(directory, "md5.json"));
    }
}
