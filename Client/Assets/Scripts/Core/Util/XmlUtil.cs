using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace Game.Core.Util
{
    /************************* 
	* 作者： wenyueyun 
	* 时间： 2018/4/26 19:34:49 
	* 描述： XmlUtil 
	*************************/
    public class XmlUtil
    {
        /// <summary>
        /// 序列化对象 XML
        /// </summary>
        public static bool Serialize(string path, object obj)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            try
            {
                XmlSerializer xs = new XmlSerializer(obj.GetType());
                xs.Serialize(sw, obj);
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("Serialize:  path{0}  Type:{1}  error:{2}", path, obj.GetType(),e.Message));
            }
            fs.Close();
            sw.Close();
            return true;
        }

        /// <summary>
        /// 反序列化 XML
        /// </summary>
        public static T Deserialize<T>(string path) where T:class
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            T obj = null;
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                obj = xs.Deserialize(sr) as T;
            }
            catch(Exception e)
            {
                Debug.Log(string.Format("Deserialize:  path{0}  error:{1}", path, e.Message));
            }
            return obj;
        }

        /// <summary>
        /// 序列化对象 Bytes (待求证)
        /// </summary>
        public static bool Serialize(object obj)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
            try
            {
                XmlSerializer xs = new XmlSerializer(obj.GetType());
                xs.Serialize(sw, obj);
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("Serialize:  Type:{0}  error:{1}", obj.GetType(), e.Message));
            }
            ms.Close();
            sw.Close();
            return true;
        }

        /// <summary>
        /// 反序列化 Bytes
        /// </summary>
        public static T Deserialize<T>(byte[] bytes) where T : class
        {
            MemoryStream ms = new MemoryStream(bytes);
            XmlSerializer xs = new XmlSerializer(typeof(T));
            T obj = null;
            try
            {
                obj = xs.Deserialize(ms) as T;
            }
            catch(Exception e)
            {
                Debug.Log(string.Format("Deserialize: Bytes  error:{0}", e.Message));
            }
            return obj;
        }
    }
}
