#region Copyright (c) 2010 Comfy.Framework 版权所有。
//----------------------------------------------------------------
// CLR 版本：2.0.50727.3053
// 文件名：CookieManager
// 文件功能描述：
//
// 
// 创建标识：Eric.rz.Liang 2010/8/18 16:42:22
//
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
// 
//----------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Web;

namespace Comfy.App.Web.Core
{
    public class CookieManager
    {
        public static CookieKey CookieKey = new CookieKey();

        public static string GetCookie(string name)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(name);
            if (cookie != null && cookie.Value != null)
                return cookie.Value;
            return "";
        }
        public static string GetCookie(string name, string key)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(name);
            if (cookie != null && cookie.Values.Count > 0)
                return cookie.Values[key];
            return "";
        }
    }
}
