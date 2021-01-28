using System;
using System.Collections.Generic;

namespace Comfy.Data
{
    public class RefObjectCreator : MarshalByRefObject
    {
        static Dictionary<string, string> assembly = new Dictionary<string, string>();
        object sync = new object();

        public T Create<T>()
        {
            Type type = typeof(T);
            if (type.IsInterface)
            {
                lock (sync)
                {
                    if (assembly.ContainsKey(type.FullName))
                    {
                        string[] array = assembly[type.FullName].Split(',');
                        return (T)System.Reflection.Assembly.Load(array[0]).CreateInstance(array[1]);
                    }
                    string setting = System.Configuration.ConfigurationManager.AppSettings[type.FullName];
                    if (!string.IsNullOrEmpty(setting))
                    {
                        string[] array = setting.Split(',');
                        assembly.Add(type.FullName, setting);
                        return (T)System.Reflection.Assembly.Load(array[0]).CreateInstance(array[1]);
                    }
                    string typeName = type.FullName.Remove(type.FullName.LastIndexOf(type.Name), 1);
                    string assembleName = type.Assembly.GetName().Name;
                    assembly.Add(type.FullName, assembleName + "," + typeName);
                    return (T)System.Reflection.Assembly.Load(assembleName).CreateInstance(typeName);
                }
            }
            else
                return (T)System.Reflection.Assembly.Load(type.Assembly.GetName().Name).CreateInstance(type.FullName);
        }
    }
}
