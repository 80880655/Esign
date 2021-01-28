using System;
using System.Collections.Generic;
using System.Web;
using System.Reflection;

namespace Comfy.App.Web
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    public class AjaxGetValue : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string dllName = context.Request.QueryString["dllName"];
                string className = context.Request.QueryString["className"];
                string tagPrefix = context.Request.QueryString["tagPrefix"];

                string wGId = context.Request.QueryString["wGId"];
                string dSId = context.Request.QueryString["dSId"];
                string isAdd = context.Request.QueryString["isAdd"];
                string isSearch = context.Request.QueryString["isSearch"];
                string Ikey = context.Request.QueryString["Ikey"];

                Type type = Assembly.Load(dllName).GetType(className);
                string retStr = "";

                retStr += "&lt;" + tagPrefix + ":WebGridView ID=\"" + wGId + "\" runat=\"server\" DataSourceId=\"" + dSId + "\" PageSize=\"10\" CreateSearchPanel=\"" + isSearch + "\" CreateAddPanel=\"" + isAdd + "\"" +
          " KeyFieldName=\"" + Ikey + "\"&gt;<br/>";

                foreach (System.Reflection.PropertyInfo info in type.GetProperties())
                {
                    if (info.Name == "GridRowCount" || info.Name == "DataState")
                        continue;
                    retStr += "&lt;" + tagPrefix + ":Field FieldName=\"" + info.Name + "\" Caption=\"" + info.Name + "\" ";
                    if (info.PropertyType.Equals(typeof(DateTime)))
                    {
                        retStr += "FieldType=\"Date\" DateFormat=\"yyyy-MM-dd hh:mm:ss\"";
                    }
                    retStr += "&gt;&lt;/" + tagPrefix + ":Field&gt;<br/>";
                }

                retStr += "&lt;/" + tagPrefix + ":WebGridView&gt;";
                context.Response.Write(retStr);
            }
            catch (Exception ex)
            {
                context.Response.Write("出錯,請檢查你的dll名、類名是否正確");
            }
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
