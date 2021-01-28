using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Comfy.App.Web
{
    public partial class GenericError : System.Web.UI.Page
    {
        public const string ErrorInfo = "ErrorInfo";
        public const string ErrorUrl = "ErrorUrl";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Exception exc = Application[ErrorInfo] as Exception;
                if (exc != null)
                {
                    lblMessage.Text = exc.ToString();
                    Application.Remove(ErrorInfo);
                }
                if (Application[ErrorUrl] != null)
                {
                    lnkReturn.NavigateUrl = Application[ErrorUrl].ToString();
                    Application.Remove(ErrorUrl);
                }
            }
        }
    }
}
