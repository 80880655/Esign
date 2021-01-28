using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

[assembly: WebResource("Comfy.UI.WebControls.WebCalendar.js.calendar.js", "application/x-javascript", PerformSubstitution = true)]
[assembly: WebResource("Comfy.UI.WebControls.WebCalendar.images.date.gif", "image/gif")]

namespace Comfy.UI.WebControls.WebCalendar
{
    [ToolboxBitmap(typeof(WebCalendar), "Comfy.UI.WebControls.WebCalendar.images.CalendarBox.ico")]
    public class WebCalendar : TextBox
    {
        protected override void OnPreRender(EventArgs e)
        {
            this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
            base.OnPreRender(e);
        }
        public WebCalendar() { }
        public WebCalendar(string url) { ImaginURL = url; }
        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.ReadOnly)
            {
                this.Attributes.Add("onfocus", "setday(this,'" + this.DateFormat + "')");
            }
           // this.Attributes.Add("onchange", "checkDate(this.value)");
            this.Attributes.Add("style", "background:url(" + ImaginURL + ") no-repeat right;background-color:#ffffff;border:solid 1px #7F9DB9");
            this.ReadOnly = true;
            if (!string.IsNullOrEmpty(this.Check))
            {
                base.Attributes.Add("check", this.Check);
                base.Attributes.Add("cnname", this.CnName);
            }
            base.Render(writer);
        }

        void Page_PreRenderComplete(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), "Comfy.UI.WebControls.WebCalendar.js.calendar.js");
        }

        public string Check { get; set; }

        public string CnName { get; set; }

        /// <summary>
        /// 弹出日期控件小图片的地址
        /// </summary>
        [Bindable(true)]
        [Category("图标设置")]
        //  [DefaultValue("444imagin/date.gif")]
        [Localizable(true)]
        public string ImaginURL
        {
            get
            {
                String s = (String)ViewState["ImaginURL"];
                object dt = null;
                try
                {
                    dt = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebCalendar.images.date.gif");
                }
                catch (Exception e)
                {
                    dt = null;
                }
                if (s != null)
                {
                    return s;
                }
                if (dt != null)
                {
                    ViewState["ImaginURL"] = dt.ToString();
                    return dt.ToString();
                }
                return "";
            }
            set
            {
                ViewState["ImaginURL"] = value;
            }
        }

        /// <summary>
        /// 设置日期，时间的初始格式。
        /// </summary>
        [Bindable(true)]
        [Category("初始化设置")]
        [DefaultValue(false)]
        [Localizable(true)]
        public string DateFormat
        {
            get
            {
                if (ViewState["DateFormat"] != null)
                {
                    return string.IsNullOrEmpty(((string)ViewState["DateFormat"])) ? "yyyy-MM-dd hh:mm:ss" : (string)ViewState["DateFormat"];
                }
                return "yyyy-MM-dd hh:mm:ss";
            }
            set
            {
                ViewState["DateFormat"] = value;
            }
        }


        /// <summary>
        /// 注样式文件
        /// </summary>
        /// <param name="path"></param>
        private void RegisterCssFile(string path)
        {
            HtmlLink link1 = new HtmlLink();
            link1.Attributes["type"] = "text/css";
            link1.Attributes["rel"] = "stylesheet";
            link1.Attributes["href"] = path;
            this.Page.Header.Controls.Add(link1);
        }
    }
}
