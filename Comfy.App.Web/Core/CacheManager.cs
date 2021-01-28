#region Copyright (c) 2010 Comfy.Framework 版权所有。
//----------------------------------------------------------------
// CLR 版本：2.0.50727.3053
// 文件名：Cache
// 文件功能描述：
//
// 
// 创建标识：eric.rz.liang 2010/8/10 9:50:34
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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Caching;


namespace Comfy.App.Web.Core
{
    public class CacheManager
    {
        const string _navItemKey = "{B988431E-A90D-4d93-9B38-80CC069A843A}";
        const string _pageCmdKey = "{2F867332-1ECF-4efe-AB2A-E969EB01B9F1}";
        const string _appUserKey = "{BD6E2669-5E72-4cf4-9E09-9727582C029B}";
        const string _employeeKey = "{2FBE01EF-4788-436d-AD3F-A5134DDA432E}";

        public static object NavItemCache
        {
            get { return HttpContext.Current.Cache.Get(_navItemKey); }
            set
            {
                if (value == null)
                    HttpContext.Current.Cache.Remove(_navItemKey);
                else
                    HttpContext.Current.Cache.Add(_navItemKey, value, null, Cache.NoAbsoluteExpiration,
                        TimeSpan.FromHours(2), CacheItemPriority.High, null);
            }
        }

        public static object AppUserCache
        {
            get { return HttpContext.Current.Cache.Get(_appUserKey); }
            set
            {
                if (value == null)
                    HttpContext.Current.Cache.Remove(_appUserKey);
                else
                    HttpContext.Current.Cache.Add(_appUserKey, value, null, Cache.NoAbsoluteExpiration,
                        TimeSpan.FromHours(2), CacheItemPriority.High, null);
            }
        }

        public static object EmployeeCache
        {
            get { return HttpContext.Current.Cache.Get(_employeeKey); }
            set
            {
                if (value == null)
                    HttpContext.Current.Cache.Remove(_employeeKey);
                else
                    HttpContext.Current.Cache.Add(_employeeKey, value, null, Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes(10), CacheItemPriority.Low, null);
            }
        }

        public static object PageCmdCache
        {
            get { return HttpContext.Current.Cache.Get(_pageCmdKey); }
            set
            {
                if (value == null)
                    HttpContext.Current.Cache.Remove(_pageCmdKey);
                else
                    HttpContext.Current.Cache.Add(_pageCmdKey, value, null, Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes(10), CacheItemPriority.Low, null);
            }
        }
    }
}
