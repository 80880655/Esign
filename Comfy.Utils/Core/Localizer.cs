using System;
using System.Collections.Generic;
using System.Text;
using Comfy.Data;
using System.Web;
using System.Resources;

namespace Comfy.Utils.Core
{
    public class Localizer
    {
        public ResourceManager TitleResourceManager { get; set; }

        public ResourceManager ErrorResourceManager { get; set; }

        public Localizer()
        {

        }

        public Localizer(ResourceManager title, ResourceManager error)
        {
            TitleResourceManager = title;
            ErrorResourceManager = error;
        }

        #region web only

        public string GetLocalString(string virtualPath, string key, string defaultText)
        {
            string result = HttpContext.GetLocalResourceObject(virtualPath, key) as string;
            return string.IsNullOrEmpty(result) ? defaultText : result;
        }

        public string GetLocalString(string virtualPath, string key)
        {
            return GetLocalString(virtualPath, key, key);
        }

        public string GetGlobalString(string classKey, string key, string defaultText)
        {
            string result = HttpContext.GetGlobalResourceObject(classKey, key) as string;
            return string.IsNullOrEmpty(result) ? defaultText : result;
        }

        public string GetGlobalString(string classKey, string key)
        {
            return GetGlobalString(classKey, key, key);
        }

        #endregion

        public string GetString(string key, string defaultText, ResourceManager resource)
        {
            if (!string.IsNullOrEmpty(key) && resource != null)
            {
                string text = resource.GetObject(key) as string;
                if (!string.IsNullOrEmpty(text))
                    return text;
            }
            return defaultText;
        }

        public string GetString(string key, ResourceManager resource)
        {
            return GetString(key, key, resource);
        }

        public string GetTitleText(string key, string defaultText)
        {
            if (HttpContext.Current != null)
            {
                if (TitleResourceManager != null)
                    return GetString(key, defaultText, TitleResourceManager);
                return GetGlobalString("TitleRes", key, defaultText);
            }
            else
                return GetString(key, defaultText, TitleResourceManager);
        }

        public string GetTitleText(string key)
        {
            return GetTitleText(key, key);
        }

        public string GetErrorText(string key, string defaultText)
        {
            if (HttpContext.Current != null)
            {
                if (TitleResourceManager != null)
                    return GetString(key, defaultText, ErrorResourceManager);
                return GetGlobalString("ErrorRes", key, defaultText);
            }
            else
                return GetString(key, defaultText, ErrorResourceManager);
        }

        public string GetErrorText(string key)
        {
            return GetErrorText(key, key);
        }

        public string GetErrorText(ValidationException exc)
        {
            if (exc.ErrorInfos.Count > 0)
            {
                StringBuilder text = new StringBuilder();
                foreach (ErrorInfo info in exc.ErrorInfos)
                    foreach (ErrorText error in info.Errors)
                        text.AppendFormat(GetErrorText(error.Key, error.Text) + "。", error.Args);
                return text.ToString();
            }
            return exc.Message;
        }
    }
}
