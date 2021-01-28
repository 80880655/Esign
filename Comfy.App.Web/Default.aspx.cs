using System;
using Comfy.App.Web.Core;
using Comfy.Utils;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.Web.UI.MobileControls;
using System.Collections.Generic;



namespace Comfy.App.Web
{
    [System.ComponentModel.DataObject]
    public partial class Default : Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                btnCreate.Visible = false;
                btnApprove.Visible = false;
                btnMaintain.Visible = false;

                //  账号固定start
                HttpContext.Current.Session["UserId"] = "gaokui";
                List<string> listCount = new List<string>();
                listCount.Add("1");
                listCount.Add("2");
                listCount.Add("3");
                listCount.Add("4");
                HttpContext.Current.Session["UserPower"] = listCount;
                // 账号固定 end


                if (HttpContext.Current.Session["UserId"] == null || HttpContext.Current.Session["UserPower"] == null)
                {
                    btnCreate.Visible = false;
                    btnApprove.Visible = false;
                    btnMaintain.Visible = false;
                    btnSearch.Visible = false;
                }
                else if (HttpContext.Current.Session["UserPower"] != null)
                {
                    List<string> sl = (List<string>)HttpContext.Current.Session["UserPower"];
                    if (sl.Contains("1"))
                    {
                        btnCreate.Visible = true;
                    }
                    if (sl.Contains("2"))
                    {
                        btnApprove.Visible = true;

                    }
                    if (sl.Contains("3"))
                    {

                        btnMaintain.Visible = true;
                    }
                    if (sl.Contains("4"))
                    {
                        btnSearch.Visible = true;
                    }
                }

                if (HttpContext.Current.Session["fun"] != null)
                {
                    ClientScript.RegisterClientScriptBlock(typeof(string), "js", "openTabByFun('" + HttpContext.Current.Session["fun"].ToString() + "');", true);

                }

              
            }
        }

    }
}
