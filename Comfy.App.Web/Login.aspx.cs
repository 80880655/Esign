using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Security;
using Comfy.App.Web.Core;
using Comfy.Utils;
using Comfy.Utils.Authentication;
using System.Configuration;


namespace Comfy.App.Web
{
    [System.ComponentModel.DataObject]
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Abandon();

            FormsAuthentication.SignOut();
            if (!IsPostBack)
            {
                LoadCookie();
                hlkForgetPwd.NavigateUrl = ConfigurationManager.AppSettings["FogotPwd"];

                string s = Application["PageAddress"] as string;
                if (s == null)
                {
                    string sb = Request.Url.ToString();
                    if (sb.Contains("?"))
                    {
                        sb = sb.Substring(0, sb.IndexOf("?"));
                    }
                    Application["PageAddress"] = sb;
                }

            }
        }

        void LoadCookie()
        {
            HttpCookie cookie = Request.Cookies.Get(CookieManager.CookieKey.LoginInfo);
            if (cookie != null)
                txtUser.Text = cookie.Values[CookieManager.CookieKey.UserId];
        }

        void SaveCookie(string userId)
        {
            HttpCookie cookie = new HttpCookie(CookieManager.CookieKey.LoginInfo);
            cookie.Values[CookieManager.CookieKey.UserId] = userId;
            cookie.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Set(cookie);
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {


            string urlM = Request.QueryString["ReturnUrl"];

            SaveCookie(txtUser.Text.Trim());
            System.Web.Security.FormsAuthentication.SetAuthCookie(txtUser.Text.Trim(), cboPersist.Checked);
            Response.Write("<script type='text/javascript'>window.location='Default.aspx'</script>");



        }


    }
}
