using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Web;
using System.Windows.Forms;

namespace Comfy.Utils
{
    /// <summary>
    /// 读写xml格式配置文件的类
    /// </summary>
    public class XmlConfig
    {
        XmlDocument doc = new XmlDocument();
        XmlElement root;
        string xmlFile;

        /// <summary>
        /// 默认配置文件为"AppSetting.Config"。
        /// 初始化时调用Load()。
        /// </summary>
        public XmlConfig() : this("AppSetting.Config") { }
        /// <summary>
        /// 配置文件为 xmlFile。
        /// 初始化时调用Load()。
        /// </summary>
        /// <param name="xmlFile"></param>
        public XmlConfig(string xmlFile)
        {            
            this.xmlFile = xmlFile;
            Load();
        }

        /// <summary>
        /// 读取配置文件。
        /// </summary>
        public void Load()
        {
            try
            {
                doc.Load(xmlFile);
                root = doc.DocumentElement;
            }
            catch
            {
                doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><configuration></configuration>");
                if (Save()) Load();
            }
        }

        /// <summary>
        /// 保存到配置文件。
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            doc.Save(xmlFile);
            return true;
        }

        public T GetValue<T>(string xpath, T defaultValue)
        {
            try
            {
                XmlNode xn = root.SelectSingleNode(xpath);
                if (xn == null)
                    throw new XmlException("找不到键[" + xpath + "]的值。");
                if (typeof(T).GetInterface("System.IConvertible") != null)
                    return (T)Convert.ChangeType(xn.InnerText, typeof(T));
                else
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (T)formatter.Deserialize(new MemoryStream(Convert.FromBase64String(xn.InnerText)));
                }
            }
            catch
            {
                SaveValue(xpath, defaultValue);
                return defaultValue;
            }
        }

        public void SetValue<T>(string xpath, T value)
        {
            XmlNode xn = root.SelectSingleNode(xpath);
            if (xn == null)
                xn = CreateNode(xpath);
            xn.InnerText = GetText(value);
        }

        public bool SaveValue<T>(string xpath, T value)
        {
            SetValue(xpath, value);
            return Save();
        }

        public List<T> GetList<T>(string xpath, List<T> defaultValue)
        {
            try
            {
                XmlNode xn = root.SelectSingleNode(xpath);
                if (xn == null)
                    throw new XmlException("找不到键[" + xpath + "]的值。");
                if (typeof(T).GetInterface("System.IConvertible") != null)
                {
                    List<T> result = new List<T>();
                    foreach (XmlNode n in xn.ChildNodes)
                    {
                        result.Add((T)Convert.ChangeType(n.InnerText, typeof(T)));
                    }
                    return (result);
                }
                else
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    List<T> result = new List<T>();
                    foreach (XmlNode n in xn.ChildNodes)
                    {
                        result.Add((T)formatter.Deserialize(new MemoryStream(Convert.FromBase64String(n.InnerText))));
                    }
                    return (result);
                }
            }
            catch
            {
                SaveList(xpath, defaultValue);
                return defaultValue;
            }
        }

        public void SetList<T>(string xpath, List<T> list)
        {
            XmlNode xn = root.SelectSingleNode(xpath);
            if (xn == null)
                xn = CreateNode(xpath);
            else
                xn.RemoveAll();
            foreach (T value in list)
            {
                XmlNode n = doc.CreateElement("add");
                xn.AppendChild(n);
                n.InnerText = GetText(value);
            }
        }

        public bool SaveList<T>(string xpath, List<T> list)
        {
            SetList(xpath, list);
            return Save();
        }

        string GetText<T>(T value)
        {
            if (value is IConvertible)
                return value.ToString();
            else if (typeof(T).IsSerializable)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, value);
                return Convert.ToBase64String(stream.ToArray());
            }
            else
                throw new NotSupportedException(typeof(T).FullName);
        }

        XmlNode CreateNode(string xpath)
        {
            string[] nodes = xpath.Split('/');
            XmlNode xn1 = root;
            XmlNode xn2;
            foreach (string node in nodes)
            {
                if ((xn2 = xn1.SelectSingleNode(node)) == null)
                {
                    xn2 = doc.CreateElement(node);
                    xn1.AppendChild(xn2);
                    xn1 = xn2;
                }
                else
                    xn1 = xn2;
            }
            return xn1;
        }
    }
}

