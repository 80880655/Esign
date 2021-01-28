using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Comfy.Data;
using System.Web;
using System.Web.Security;
using Comfy.Utils.Core;
using System.Threading;
using System.Security.Principal;
using Comfy.Utils.Authentication;

namespace Comfy.Utils
{
    public static class AppContext
    {
        public static string APServiceUrl { get; set; }

        static Log log;

        static Localizer localizer;

        static MessageHelper message;

        public static MessageHelper MessageHelper
        {
            get
            {
                if (message == null)
                    message = new MessageHelper();
                return message;
            }
        }

        public static Log LogService
        {
            get
            {
                if (log == null)
                    log = new Log();
                return log;
            }
        }

       
        public static Localizer Localizer
        {
            get
            {
                if (localizer == null)
                    localizer = new Localizer();
                return localizer;
            }
        }

        public static T CreateRefObj<T>()
        {
            try
            {
                RefObjectCreator creator;
                if (APServiceUrl == null)
                    APServiceUrl = ConfigurationManager.AppSettings["AppService"] ?? "";
                if (!string.IsNullOrEmpty(APServiceUrl))
                    creator = (RefObjectCreator)Activator.GetObject(typeof(RefObjectCreator), APServiceUrl);
                else
                    creator = new RefObjectCreator();
                return creator.Create<T>();
            }
            catch (Exception exc)
            {
                LogService.Error(exc);
                throw exc;
            }
        }

        public static IPrincipal User
        {
            get
            {
                if (HttpContext.Current == null)
                    return Thread.CurrentPrincipal;
                else
                    return HttpContext.Current.User;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.User = value;
                Thread.CurrentPrincipal = value;
            }
        }

        public static AppException GetError(Exception exc)
        {
            Type type = exc.GetType();
            if (type == typeof(ValidationException))
                return new AppException(Localizer.GetErrorText((ValidationException)exc));
            if (type == typeof(AppException))
                return (AppException)exc;
            if (type == typeof(Comfy.Data.SqlException))
                LogService.Error((Comfy.Data.SqlException)exc);
            else
                LogService.Fatal(exc);
            return new AppException(Localizer.GetErrorText("Error") + exc.Message, exc);
        }

        public static UserService GetUserService()
        {
            UserService service = new UserService();
            string[] urls = ConfigurationManager.AppSettings["UserService"].Split(',');
            string timeout = ConfigurationManager.AppSettings["UserServiceTimeout"];
            int time;
            if (Int32.TryParse(timeout, out time))
                service.Timeout = time;
            else
                service.Timeout = 2000;
            for (int i = 0; i < 2; i++)
            {
                foreach (string url in urls)
                {
                    try
                    {
                        service.Url = url;
                        service.IsConnected();
                        service.Timeout = 60 * 1000;
                        return service;
                    }
                    catch { }
                }
            }
            throw new TimeoutException(Localizer.GetErrorText("UserServiceConnectFaild"));
        }
    }
}
