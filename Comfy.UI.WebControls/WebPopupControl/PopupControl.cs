using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web;
[assembly: WebResource("Comfy.UI.WebControls.WebPopupControl.Css.PopupControl.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("Comfy.UI.WebControls.WebPopupControl.Images.formHead.gif", "image/gif")]
[assembly: WebResource("Comfy.UI.WebControls.WebPopupControl.Js.PopupControl.js", "application/x-javascript", PerformSubstitution = true)]
[assembly: WebResource("Comfy.UI.WebControls.WebPopupControl.Images.pcCloseButton.png", "image/png")]
namespace Comfy.UI.WebControls.WebPopupControl
{
    public class PopupControl : WebControl,INamingContainer,ICallbackEventHandler
    {


        public new int Width
        {
            get
            {
                return ((ViewState["Width"] == null) ? 0 : (int)ViewState["Width"]);
            }
            set
            {
                ViewState["Width"] = value;
            }
        
        }


        public new int Height
        {
            get
            {
                return ((ViewState["Height"] == null) ? 0 : (int)ViewState["Height"]);
            }
            set
            {
                ViewState["Height"] = value;
            }

        }


        public  string HeadText
        {
            get
            {
                return ((ViewState["HeadText"] == null) ? string.Empty : (string)ViewState["HeadText"]);
            }
            set
            {
                ViewState["HeadText"] = value;
            }

        }

        public string Display
        {
            get
            {
                return ((HttpContext.Current.Session[this.ID + "popu" + GetFileName()] == null) ? "false" : (string)HttpContext.Current.Session[this.ID + "popu" + GetFileName()]);
            }
            set
            {
                if (Page!=null&&!Page.IsPostBack)
                {
                    HttpContext.Current.Session[this.ID + "popu" + GetFileName()] = value;
                }
                else
                {
                    if (value.Contains("QS"))
                    {
                        HttpContext.Current.Session[this.ID + "popu" + GetFileName()] = value.Replace("QS","");
                    }
                }

            }

        }

        private ITemplate contentCollection;
        [Browsable(false)]
        [TemplateContainer(typeof(ContentItem))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate ContentCollection 
        {
            get { return contentCollection; }
            set
            {
                contentCollection = value;
                base.ChildControlsCreated = false;
            }
        }

        void Page_PreRenderComplete(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), "Comfy.UI.WebControls.WebPopupControl.Js.PopupControl.js");//向頁面注入js
            string cssUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebPopupControl.Css.PopupControl.css"); //獲得css的資源路徑

            this.RegisterCssFile(cssUrl);  //向頁面注css文件

        }


        protected override void OnPreRender(EventArgs e)
        {
            //string strRefrence = Page.ClientScript.GetCallbackEventReference(this, "arg", this.ID + ".ReceiveDataFromServer", "context", false);

            //string strCallBack = "function " + this.ID + "_CallBackToTheServer(arg, context) {" + strRefrence + "};";

            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.ID + "_CallBackToTheServer", strCallBack, true); //注册JS函数CallBackToTheServer

            this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
            base.OnPreRender(e);
        }


        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<script type='text/javascript'>var " + this.ID + " = new PopupControl('"+this.ID+"');</script>");
            base.Render(writer);
        }
        protected override void CreateChildControls()
        {
            HtmlGenericControl div = new HtmlGenericControl("div");
            div.ID = this.ID;
            div.Attributes.Add("class", "popupOutDiv");
            div.Attributes.Add("style","width:"+this.Width+"px;height:"+this.Height+"px;display:"+(this.Display=="true"?"block":"none")+";");

            HtmlGenericControl divHead = new HtmlGenericControl("div");
            divHead.Attributes.Add("class", "popupHeadDiv");
            divHead.ID = this.ID + "head";
            divHead.Attributes.Add("style", "height: 27px; background: url('" + Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebPopupControl.Images.formHead.gif") + "') repeat-x;");


            Label HeadTextLable = new Label();
            HeadTextLable.Attributes.Add("style", "float:left; padding:6px 0 0 0;");
            HeadTextLable.Font.Size = 11;
            HeadTextLable.Text = this.HeadText;


            Image img = new Image();
            img.Attributes.Add("style","float: right; margin: 2px 2px 0 0;cursor: pointer;");
            img.ID = this.ID + "closeImg";
            img.ImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebPopupControl.Images.pcCloseButton.png");
            img.Attributes.Add("onclick","javascript:"+this.ID+".Hide();");

            divHead.Controls.Add(HeadTextLable);
            divHead.Controls.Add(img);

            HtmlGenericControl context = new HtmlGenericControl("div");
            context.Attributes.Add("style", "cursor: default;overflow:auto");
            context.ID = this.ID + "context";

            Label jsLab = new Label();
            jsLab.Text = "<script type='text/javascript'>" +
               " $('#" + this.ID + "_" + this.ID + "context').css({ 'width': ($('#" + this.ID + "_" + this.ID + "').outerWidth() - 7) });" +
              "  $('#" + this.ID + "_" + this.ID + "context').css({ 'height': ($('#" + this.ID + "_" + this.ID + "').outerHeight() - 35) });" +
           " </script>";
            context.Controls.Add(jsLab);




            if (this.ContentCollection != null)
            {
                this.Controls.Clear();
                ContentItem item = new ContentItem();
                this.ContentCollection.InstantiateIn(item);
                context.Controls.Add(item);
                div.Controls.Add(divHead);
                div.Controls.Add(context);
                this.Controls.Add(div);
                ChildControlsCreated = true;
            }
        }


        /**/
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

        #region ICallbackEventHandler Members

        public string GetCallbackResult()
        {
           // throw new NotImplementedException();
            return "";
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
           // throw new NotImplementedException();
            if (eventArgument == "hide")
            {
                this.Display = "falseQS";
            }
            else
            {
                this.Display = "trueQS";
            }
        }

        #endregion

        public string GetFileName()
        {
            string s = HttpContext.Current.Request.Path;
            s = s.Replace("/", "").Replace(".", "").Replace("aspx", "");
            return s;

        }
    }
}
