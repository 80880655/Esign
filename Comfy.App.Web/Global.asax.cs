using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Comfy.Utils;
using Comfy.App.Web.WebReference1;
using System.Text;
using System.Threading;
using System.Timers;

namespace Comfy.App.Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {


        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
          //  Application[GenericError.ErrorInfo] = Utils.AppContext.GetError(Server.GetLastError().GetBaseException());
              Application[GenericError.ErrorUrl] = Request.Url;
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
          //  Comfy.Utils.AppContext.LogService.Info("Application End", "", "");
        }
    
    }
}