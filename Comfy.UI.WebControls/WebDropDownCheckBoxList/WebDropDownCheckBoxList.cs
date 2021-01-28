using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;
using System.Drawing;

namespace Comfy.UI.WebControls.WebDropDownCheckBoxList
{
    [Serializable]
    [ToolboxBitmap(typeof(WebDropDownCheckBoxList), "Comfy.UI.WebControls.WebDropDownCheckBoxList.WebDropDownCheckBoxList.bmp")]
    [ToolboxData("<{0}:WebDropDownCheckBoxList id='WebDropDownCheckBoxList1' runat='server' width='120px' ShowSelectAllOption='true'></{0}:WebDropDownCheckBoxList>")]
    public class WebDropDownCheckBoxList : DropDownList
    {
        /// <summary>
        /// 下拉清單內容顯示形式
        /// </summary>
        [DefaultValue(typeof(DisplayMode), "Label")]
        public DisplayMode DisplayMode
        {
            get
            {
                return ViewState["DisplayMode"] == null ? DisplayMode.Label : (DisplayMode)ViewState["DisplayMode"];
            }
            set { ViewState["DisplayMode"] = value; }
        }

        /// <summary>
        /// 多選分隔符號 默認為 " , "
        /// </summary>
        [DefaultValue(",")]
        [Browsable(true)]
        [Category("property")]
        public string Splitor
        {
            get
            {
                return ViewState["Splitor"] == null ? "," : ViewState["Splitor"].ToString();
            }
            set { ViewState["Splitor"] = value; }
        }

        /// <summary>
        /// "全選"選項顯示的標籤文本
        /// </summary>
        [DefaultValue("全選")]
        [Browsable(true)]
        public string SelectAllOptionLabel
        {
            get
            {
                return ViewState["SelectAllOptionLabel"] == null ? "全選" : ViewState["SelectAllOptionLabel"].ToString();
            }
            set { ViewState["SelectAllOptionLabel"] = value; }
        }

        /// <summary>
        /// 選擇的文本
        /// </summary>
        [DefaultValue("")]
        [Browsable(true)]
        public string SelectedText
        {
            get
            {
                if (!DesignMode)
                { return this.txt.Text; }

                return base.SelectedItem.Text;
            }
            set
            {
                if (!DesignMode)
                { this.txt.Text = value; }
            }
        }

        /// <summary>
        /// 選擇的值
        /// </summary>
        [DefaultValue("")]
        [Browsable(true)]
        public override string SelectedValue
        {
            get
            {
                if (!DesignMode)
                { return hfValue.Value; }

                return base.SelectedValue;
            }
            set
            {
                if (!DesignMode)
                { hfValue.Value = value; }
                else
                { base.SelectedValue = value; }
            }
        }

        /// <summary>
        /// 選擇的值|文本
        /// </summary>
        [DefaultValue("")]
        [Browsable(true)]
        public string SelectedValueAndText
        {
            get
            {
                if (!DesignMode)
                { return this.hfValueText.Value; }

                return "";
            }
            set
            {
                if (!DesignMode)
                { this.hfValueText.Value = value; }
            }
        }

        /// <summary>
        /// 是否自動顯示"全選"選項
        /// </summary>
        [DefaultValue(false)]
        [Browsable(true)]
        public bool ShowSelectAllOption
        {
            get
            {
                return ViewState["ShowSelectAllOption"] == null ? false : (bool)ViewState["ShowSelectAllOption"];
            }
            set { ViewState["ShowSelectAllOption"] = value; }
        }

        TextBox txt = null;
        HiddenField hfValue = null;
        HiddenField hfValueText = null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!DesignMode)
            {
                CreateControls();
            }
        }

        protected void CreateControls()
        {
            txt = new TextBox();
            txt.ID = this.ClientID + "_txtMain";
            txt.ReadOnly = true;
            txt.Width = (Unit)(this.Width.Value - 20);
            txt.Style.Add(HtmlTextWriterStyle.Padding, "0");
            txt.Style.Add(HtmlTextWriterStyle.Margin, "0");
            txt.Height = this.Height;

            if (txt.Height.IsEmpty)
            { txt.Height = 15; }

            hfValueText = new HiddenField();
            hfValueText.ID = string.Format("{0}_{1}", this.ClientID, "selectItemValueText");
            hfValue = new HiddenField();
            hfValue.ID = string.Format("{0}_{1}", this.ClientID, "selectItemValue");

            txt.Enabled = this.Enabled;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!DesignMode)
            {
                DoRegisterScript();
            }
        }

        void DoRegisterScript()
        {
            ClientScriptManager sc = this.Page.Page.ClientScript;

            string scriptString = sc.GetWebResourceUrl(this.GetType(), "WebDropDownCheckBoxList.Resources.DropDownCheckBoxList.js");

            if (!sc.IsClientScriptIncludeRegistered("DropDownGridScriptKey"))
            { sc.RegisterClientScriptInclude(this.GetType(), "DropDownGridScriptKey", scriptString); }

            scriptString = Page.Form.Attributes["onclick"];
            if (string.IsNullOrEmpty(scriptString))
            { scriptString = string.Format("responseOnFormClick(event,'{0}');", this.ClientID); }
            else
            { scriptString = string.Format("{0} responseOnFormClick(event,'{1}');", scriptString, this.ClientID); }

            Page.Form.Attributes.Add("onclick", scriptString);

            this.Page.RegisterRequiresPostBack(this);
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                if (!DesignMode)
                {
                    return HtmlTextWriterTag.Table;
                }

                return base.TagKey;
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (!DesignMode)
            { AddCustomAttribute(writer); }
            else
            { base.AddAttributesToRender(writer); }
        }

        void AddCustomAttribute(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Width, this.Width.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID + "_displaytable");
            writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
            writer.AddStyleAttribute(HtmlTextWriterStyle.ZIndex, "1");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Left, "-5px");
        }

        void GetDivWidth()
        {
            int itemWidth = 0;
            int byteCount = 0;
            foreach (ListItem item in this.Items)
            {
                byteCount = System.Text.UnicodeEncoding.Default.GetByteCount(item.Text);
                if (byteCount > itemWidth)
                { itemWidth = byteCount; }
            }

            itemWidth = itemWidth * 8 + 20;
            itemWidth = itemWidth + 16; //加上checkbox 寬度

            if (itemWidth > this.Width.Value)
            { this.Width = itemWidth; }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                base.RenderContents(writer);
                return;
            }

            RenderCustomContent(writer);
        }

        void RenderCustomContent(HtmlTextWriter writer)
        {
            GetDivWidth();
            string divId = this.ClientID + "_div";
            string cmbDown = this.ClientID + "_imgDown";
            string cmbUp = this.ClientID + "_imgUp";

            // base.RenderContents(writer);
            ////控制項顯示主體
            writer.AddAttribute(HtmlTextWriterAttribute.Align, "left");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.AddStyleAttribute(HtmlTextWriterStyle.Cursor, "default");
            if (this.Enabled)
            { writer.AddAttribute(HtmlTextWriterAttribute.Onclick, string.Format("toggleDivShowState('{0}','{1}','{2}');", this.ClientID, Splitor, this.Items.Count)); }
            ////控制項主體文本框 
            txt.RenderControl(writer);

            writer.Write("<br/>");
            ////隱藏下拉麵板
            writer.AddAttribute(HtmlTextWriterAttribute.Id, divId);
            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
            writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "#ECECE3");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "absolute");
            writer.AddStyleAttribute(HtmlTextWriterStyle.ZIndex, "32766");
            writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, "Gray");
            writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "thin");
            writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "double");
            writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "top");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width.ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            ////呈現 item 列表
            ModifyRenderedCheckboxes(writer);
            writer.RenderEndTag();//end div

            writer.RenderEndTag();//end td
            ////下拉圖示
            writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.AddStyleAttribute(HtmlTextWriterStyle.Margin, "0");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "0");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Left, "-8px");
            writer.AddAttribute(HtmlTextWriterAttribute.Src, this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "WebDropDownCheckBoxList.Resources.cmb_down.jpg"));

            if (this.Enabled)
            { writer.AddAttribute(HtmlTextWriterAttribute.Onclick, string.Format("toggleDivShowState('{0}','{1}','{2}');", this.ClientID, Splitor, this.Items.Count)); }

            writer.AddAttribute(HtmlTextWriterAttribute.Id, cmbDown);
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();//end img
            writer.AddStyleAttribute(HtmlTextWriterStyle.Margin, "0");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "0");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Left, "-8px");
            writer.AddAttribute(HtmlTextWriterAttribute.Src, this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "WebDropDownCheckBoxList.Resources.cmb_up.jpg"));
            if (this.Enabled)
            { writer.AddAttribute(HtmlTextWriterAttribute.Onclick, string.Format("toggleDivShowState('{0}','{1}','{2}');", this.ClientID, Splitor, this.Items.Count)); }
            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, cmbUp);
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();//end img

            writer.RenderEndTag();//end td
            writer.RenderEndTag();//end tr 
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!DesignMode)
            {
                RenderBeginTag(writer);
                RenderContents(writer);
                RenderEndTag(writer);
            }
            else
            { base.Render(writer); }
        }

        protected void ModifyRenderedCheckboxes(HtmlTextWriter writer)
        {
            int index = 0;

            string spanId = "";
            string wapperId = "";
            string allChkId = "";

            writer.AddAttribute(HtmlTextWriterAttribute.Width, this.Width.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID + "_Optiontable");
            writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Left, "0px");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            #region 首選項
            if (ShowSelectAllOption)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                wapperId = string.Format("{0}_{1}", this.ClientID, "chkAllItemWapper");
                allChkId = string.Format("{0}_{1}", this.ClientID, "chkAllItemValue");
                spanId = string.Format("{0}_{1}", this.ClientID, "chkAllItemText");

                writer.AddAttribute("onmouseover", string.Format("setStyleOnMouseOver('{0}');", wapperId));
                writer.AddAttribute("onmouseout", string.Format("setStyleOnMouseOut('{0}');", wapperId));

                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, (this.Width.Value - 10).ToString());
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "20px");
                writer.AddAttribute(HtmlTextWriterAttribute.Id, wapperId);
                writer.AddStyleAttribute("cursor", "pointer");
                writer.AddStyleAttribute("cursor", "hand");
                writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "5px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, "5px");
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, string.Format("return clickChangeValueWhenCheckAllCheckBox(event,'{0}','{1}');", allChkId, this.ClientID));
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, allChkId);
                writer.AddStyleAttribute("cursor", "pointer");
                writer.AddStyleAttribute("cursor", "hand");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, "");
                writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingBottom, "5px");
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, string.Format("return mouseUpdateValueWhenSelectAllStateChanged(event,this,'{0}');", this.ClientID));
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();//end input

                writer.AddAttribute(HtmlTextWriterAttribute.Id, spanId);
                writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "small");
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, "5px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingBottom, "5px");
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, string.Format("return mouseUpdateValueWhenCheckAllCheckBox(event,'{0}','{1}');", allChkId, this.ClientID));
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write(SelectAllOptionLabel);
                writer.RenderEndTag();//end span

                writer.RenderEndTag();//td
                writer.RenderEndTag();//tr
            }
            #endregion

            #region 內容選項
            string chkId = "";
            foreach (ListItem item in this.Items)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                wapperId = string.Format("{0}_{1}{2}", this.ClientID, "chkItemWapper", index.ToString());
                chkId = string.Format("{0}_{1}{2}", this.ClientID, "chkItemValue", index.ToString());
                spanId = string.Format("{0}_{1}{2}", this.ClientID, "chkItemText", index.ToString());

                writer.AddAttribute("onmouseover", string.Format("setStyleOnMouseOver('{0}');", wapperId));
                writer.AddAttribute("onmouseout", string.Format("setStyleOnMouseOut('{0}');", wapperId));

                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, (this.Width.Value - 10).ToString());
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "20px");
                writer.AddAttribute(HtmlTextWriterAttribute.Id, wapperId);
                writer.AddStyleAttribute("cursor", "pointer");
                writer.AddStyleAttribute("cursor", "hand");
                writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "5px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, "5px");

                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, string.Format("return clickChangeValueWhenCheckItemCheckBox(event,'{0}','{1}','{2}','{3}');", chkId, spanId, allChkId, this.ClientID));
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, chkId);
                writer.AddStyleAttribute("cursor", "pointer");
                writer.AddStyleAttribute("cursor", "hand");
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, "5px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingBottom, "5px");
                writer.AddAttribute(HtmlTextWriterAttribute.Width, "16px");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, item.Value);
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, string.Format("return mouseUpdateValueWhenCheckItemStateChanged(event,'{0}','{1}','{2}','{3}');", chkId, spanId, allChkId, this.ClientID));
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();//end input

                writer.AddAttribute(HtmlTextWriterAttribute.Id, spanId);
                writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "small");
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, "5px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingBottom, "5px");
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, string.Format("return mouseUpdateValueWhenCheckItemCheckBox(event,'{0}','{1}','{2}','{3}');", chkId, spanId, allChkId, this.ClientID));
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                if (DisplayMode == Comfy.UI.WebControls.WebDropDownCheckBoxList.DisplayMode.Label)
                { writer.Write(item.Text); }
                else
                { writer.Write(item.Value); }

                writer.RenderEndTag();//end span

                writer.RenderEndTag();//td
                writer.RenderEndTag();//tr
                index++;
            }
            #endregion

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            hfValue.RenderControl(writer);
            hfValueText.RenderControl(writer);
            writer.RenderEndTag();//end td
            writer.RenderEndTag();//end tr
            writer.RenderEndTag();//end table
        }

        protected override bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            if (!DesignMode)
            {
                this.txt.Text = postCollection[txt.ID];
                this.hfValue.Value = postCollection[hfValue.ID];
                this.hfValueText.Value = postCollection[hfValueText.ID];
                return true;
            }

            return base.LoadPostData(postDataKey, postCollection);
        }
    }

    [Serializable]
    public enum DisplayMode
    {
        /// <summary>
        /// 顯示文本
        /// </summary>
        Label,
        /// <summary>
        /// 顯示值
        /// </summary>
        Value
    }
}


