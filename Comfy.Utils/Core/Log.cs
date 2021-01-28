using System;
using System.Text;
using System.Threading;
using System.Security.Principal;
using System.Web;
using Comfy.Utils.LogService;
using Comfy.Data;

namespace Comfy.Utils.Core
{
    public class Log
    {
        public void Info(string message)
        {
            Info(message, "");
        }
        public void Info(string message, string sql)
        {
            Write(message, sql, LogLevel.Info);
        }
        public void Info(string message, string sql, string logger)
        {
            Write(message, sql, LogLevel.Info, logger);
        }
        public void Warn(string message)
        {
            Warn(message, "");
        }
        public void Warn(string message, string sql)
        {
            Write(message, sql, LogLevel.Warn);
        }
        public void Warn(string message, string sql, string logger)
        {
            Write(message, sql, LogLevel.Warn, logger);
        }
        public void Debug(string message)
        {
            Debug(message, "");
        }
        public void Debug(string message, string sql)
        {
            Write(message, sql, LogLevel.Debug);
        }
        public void Debug(string message, string sql, string logger)
        {
            Write(message, sql, LogLevel.Debug, logger);
        }
        public void Error(string message)
        {
            Error(message, "");
        }
        public void Error(SqlException exc)
        {
            Error(exc, exc.Sql);
        }
        public void Error(Exception exc)
        {
            Error(exc, "");
        }
        public void Error(Exception exc, string sql)
        {
            Error(exc.ToString(), sql);
        }
        public void Error(string message, string sql)
        {
            Write(message, sql, LogLevel.Error);
        }
        public void Error(string message, string sql, string logger)
        {
            Write(message, sql, LogLevel.Error, logger);
        }
        public void Fatal(string message)
        {
            Fatal(message, "");
        }
        public void Fatal(Exception exc)
        {
            Fatal(exc, "");
        }
        public void Fatal(Exception exc, string sql)
        {
            Fatal(exc.ToString(), sql);
        }
        public void Fatal(string message, string sql)
        {
            Write(message, sql, LogLevel.Fatal);
        }
        public void Fatal(string message, string sql, string logger)
        {
            Write(message, sql, LogLevel.Fatal, logger);
        }
        public void Write(EventLog model)
        {
            try
            {
                AsyncDelegate ad = new AsyncDelegate(write);
                ad.BeginInvoke(model, new AsyncCallback(CallbackMethod), ad);
            }
            catch { }
        }

        void Write(string message, string sql, LogLevel level)
        {
            if (AppContext.User != null)
                Write(message, sql, level, AppContext.User.Identity.Name);
            else
                Write(message, sql, level, "");
        }

        void Write(string message, string sql, LogLevel level, string logger)
        {
            EventLog model = new EventLog();
            model.Environment = GetClientInfo();
            model.Ip = GetUserHostAddress();
            model.Logger = logger;
            model.LogLevel = level;
            model.LogTime = DateTime.Now;
            model.Message = message;
            model.Sql = sql;
            Write(model);
        }

        static void write(EventLog model)
        {
            try
            {
                Service s = new Service();
                string url = System.Configuration.ConfigurationManager.AppSettings["LogService"];
                if (string.IsNullOrEmpty(url)) return;
                s.Url = url;
                model.AppId = new Guid(System.Configuration.ConfigurationManager.AppSettings["AppGuid"]);
                s.Credentials = System.Net.CredentialCache.DefaultCredentials;
                s.Write(model);
            }
            catch (Exception exc)
            {
                try
                {
                    string desc = string.Format("ServiceError:{3}\n\nClientIP:{0}\nAppID:{1}\nMessage:{2}",
                        model.Ip,
                        model.AppId,
                        model.Message,
                        exc.ToString());
                    System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog();
                    eventLog.Log = "Application";
                    eventLog.Source = "LogService";
                    eventLog.WriteEntry(desc.Substring(0, desc.Length > 5000 ? 5000 : desc.Length), System.Diagnostics.EventLogEntryType.Error);
                }
                catch { }
            }
        }

        void CallbackMethod(IAsyncResult ar)
        { }
        delegate void AsyncDelegate(EventLog model);

        static string GetUserHostAddress()
        {
            if (HttpContext.Current == null)
                return System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())[0].ToString();
            else
                return HttpContext.Current.Request.UserHostAddress;
        }

        static string GetClientInfo()
        {
            if (HttpContext.Current == null)
                return string.Format("OSVersion:{0},UserDomainName:{1},MachineName:{2}",
                    System.Environment.OSVersion,
                    System.Environment.UserDomainName,
                    System.Environment.MachineName
                    );
            else
            {
                HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
                return string.Format("Browser:{0},W3C:{1},Platform:{2},EcmaScript:{3},Frames:{4},Cookies:{5}",
                    browser.Type,
                    browser.W3CDomVersion,
                    browser.Platform,
                    browser.EcmaScriptVersion,
                    browser.Frames,
                    browser.Cookies
                    );
            }
        }
    }
}
