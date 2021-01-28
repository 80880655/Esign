using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Reflection;

namespace Comfy.App.Web
{
    /// <summary>
    /// AjaxCallMethodOne1 的摘要说明
    /// </summary>
    public class AjaxCallMethodOne1 : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string dllName = getReQ(context, "dllName");
                string className = getReQ(context, "className");
                string methodName = getReQ(context, "methodName");
                string[] methodParameter = null;
                if (!string.IsNullOrEmpty(context.Request.QueryString["methodParameter"]))
                {
                    methodParameter = getReQ(context, "methodParameter").Split(new string[]{"#@"},StringSplitOptions.None);
                }
                Type type = Assembly.Load(dllName).GetType(className);
                object o = Activator.CreateInstance(type);
                MethodInfo me = type.GetMethod(methodName);
                BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
                object returnValue = me.Invoke(o, flag, Type.DefaultBinder, methodParameter, null);
                context.Response.Write(returnValue == null ? "true" : returnValue.ToString());
            }
            catch (Exception ex)
            {
                context.Response.Write("出錯,請檢查你的dll名、類名、方法名、參數是否正確");
            }
        }
        public string getReQ(HttpContext context, string strName)
        {
            return context.Server.UrlDecode(context.Request.QueryString[strName]);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}