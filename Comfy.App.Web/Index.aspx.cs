using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Comfy.App.Core.QualityCode;

namespace Comfy.App.Web
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          ///////* 调试 by LYH */
            //HttpContext.Current.Session.Timeout = 60;
            //HttpContext.Current.Session["UserId"] = "zhangheget";
            //HttpContext.Current.Session["fun"] = "1";
            //CustomerManager manager = new CustomerManager();
            //manager.GetUserPower("zhangheget");    //sdsf*/
            
            //获取用户的权限

            if (Request.QueryString["userId"] != null)
            {
                HttpContext.Current.Session.Timeout = 60;
                HttpContext.Current.Session["UserId"] = Request.QueryString["userId"].ToString();
                CustomerManager manager = new CustomerManager();
                manager.GetUserPower(Request.QueryString["userId"].ToString());
            }
            if (Request.QueryString["fun"] != null)
            {
                HttpContext.Current.Session["fun"] = Request.QueryString["fun"].ToString();
            }
        }
    }
}