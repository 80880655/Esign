using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;


[assembly: WebResource("Comfy.UI.WebControls.WebButtonEdit.Image.edtEllipsis.png", "image/png")]
[assembly: WebResource("Comfy.UI.WebControls.WebButtonEdit.Js.ButtonEdit.js", "application/x-javascript", PerformSubstitution = true)]
namespace Comfy.UI.WebControls.WebButtonEdit
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:EventTextBox runat=server></{0}:EventTextBox>")]
    public class WebButtonEdit : TextBox
    {

        public string ButtonClick
        {
            get
            {
                String s = (String)ViewState["ButtonClick"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["ButtonClick"] = value;
            }
        }

        public string ClientJs
        {

            get
            {
                string js = ButtonEditJs.Js;
                return js.Replace("$tableId$", this.ID);
            }
        }

        /// <summary>
        /// 弹出日期控件小图片的地址
        /// </summary>
        [Bindable(true)]
        [Category("图标设置")]
        //  [DefaultValue("444imagin/date.gif")]
        [Localizable(true)]
        public string ImageUrl
        {
            get
            {
                String s = (String)ViewState["ImageURL"];
                object dt = null;

                if (Page != null)
                {
                    dt = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebButtonEdit.Image.edtEllipsis.png");
                }
                if (s != null)
                {
                    return s;
                }
                if (dt != null)
                {
                    ViewState["ImageURL"] = dt.ToString();
                    return dt.ToString();
                }
                return "";
            }
            set
            {
                ViewState["ImageURL"] = value;
            }
        }

        public string Check { get; set; }

        public string CnName { get; set; }
        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);
        }

        protected override void Render(HtmlTextWriter writer)
        {

            int intWidth = string.IsNullOrEmpty(this.Width.ToString()) ? 149 : Convert.ToInt32(this.Width.ToString().Substring(0, this.Width.ToString().Length - 2));
            int intHeight = string.IsNullOrEmpty(this.Height.ToString()) ? 20 : Convert.ToInt32(this.Height.ToString().Substring(0, this.Height.ToString().Length - 2));

            if (string.IsNullOrEmpty(this.Height.ToString()))
            {
                this.Attributes.Add("style", "float:left;border:0px;height:20px");
            }
            else
            {
                this.Attributes.Add("style", "float:left;border:0px;height:" + (intHeight) + "px;");
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Style, "border:solid 1px #AECAF0;background-color:white;width:" + (intWidth + 25) + "px;height:" + intHeight + "px");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Style, "width:" + (intWidth + 24) + "px;");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);


            if (!string.IsNullOrEmpty(this.Check))
            {
                base.Attributes.Add("check", this.Check);
                base.Attributes.Add("cnname", this.CnName);
            }
            base.Render(writer);  //把textbox放在這裡

            writer.AddAttribute(HtmlTextWriterAttribute.Style, "width:20px;float:right;height:" + (intHeight - 4) + "px;border:solid 1px #A3C0E8;margin:1px 0 0 0;background:url(" + ImageUrl + ")  #CBE1FB no-repeat center;");
            if (ButtonClick.ToLower().Contains("javascript:"))
            {

                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, ButtonClick);

            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "javascript:" + ButtonClick);
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ID + "ClickDiv");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();

            writer.RenderEndTag();
            writer.RenderEndTag();
            // base.Render(writer);
        }
        protected override void OnPreRender(EventArgs e)
        {

            this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);

            base.OnPreRender(e);
        }

        void Page_PreRenderComplete(object sender, EventArgs e)
        {
            /*  string imgUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "ServerControl1.Image.edtEllipsis.png");
              Control parent = this.Parent;
              if (DesignMode == false)
              {
                  TextBox textBox = this;
                  parent.Controls.Remove(textBox);
               
               
                  HtmlGenericControl div2 = new HtmlGenericControl("div");
                  HtmlGenericControl div1 = new HtmlGenericControl("div");
                  HtmlGenericControl div = new HtmlGenericControl("div");
                  HtmlGenericControl center = new HtmlGenericControl("center");

  
                  int intWidth = string.IsNullOrEmpty(this.Width.ToString()) ? 149 : Convert.ToInt32(this.Width.ToString().Substring(0, this.Width.ToString().Length - 2));
                  int intHeight = string.IsNullOrEmpty(this.Height.ToString()) ? 20 : Convert.ToInt32(this.Height.ToString().Substring(0, this.Height.ToString().Length -2));


                  div1.Attributes.Add("style", "width:20px;float:right;height:" + (intHeight - 4) + "px;border:solid 1px #A3C0E8;margin:1px 0 0 0;background:url(" + imgUrl + ")  #CBE1FB no-repeat center;");
                  div1.ID = this.ID + "ClickDiv";
                  div.Attributes.Add("style", "width:" + (intWidth + 24) + "px;");
              
                  div2.Attributes.Add("style", "border:solid 1px #7F9DB9;background-color:white;width:" + (intWidth + 25) + "px;height:" + intHeight+ "px");

                  if (ButtonClick.ToLower().Contains("javascript:"))
                  {
                      div1.Attributes.Add("onclick", ButtonClick);

                  }
                  else
                  {
                      div1.Attributes.Add("onclick", "javascript:" + ButtonClick);
                  }


                  if (string.IsNullOrEmpty(this.Height.ToString()))
                  {
                      textBox.Attributes.Add("style", "float:left;border:0px;height:20px");
                  }
                  else
                  {
                      textBox.Attributes.Add("style", "float:left;border:0px;height:"+(intHeight)+"px;");
                  }

                  center.Controls.Add(textBox);
                  center.Controls.Add(div1);
                  div.Controls.Add(center);
                  div2.Controls.Add(div);
                  parent.Controls.Add(div2);
              */

            Page.ClientScript.RegisterClientScriptResource(this.GetType(), "Comfy.UI.WebControls.WebButtonEdit.Js.ButtonEdit.js");//向頁面注入js

            string js = ButtonEditJs.Js;
            js = js.Replace("$tableId$", this.ID);
            Page.ClientScript.RegisterClientScriptBlock(typeof(string), this.ID + "ButtonEditJs", js);

            // }
        }
    }
}
