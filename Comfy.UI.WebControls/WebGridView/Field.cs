using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Reflection;
using System.Web.Caching;
using System.Text.RegularExpressions;
using System.Data;

namespace Comfy.UI.WebControls.WebGridView
{
    public enum FieldType
    {
        Text = 0,
        CheckBox = 1,
        CheckBoxList = 2,
        TextArea = 3,
        Date = 4,
        ComboBox = 5,
        ButtonEdit = 6,
        Number = 7,
        Img = 8,
        Href = 9,
        CheckBoxOnGrid = 10
    }
    [ToolboxItem(false)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ParseChildren(true, "Items")] 
    [Bindable(true)]
    public class Field
    {

        public string FieldName { get; set; }
        public string Caption { get; set; }
        public string FRender { get; set; }
        public string DataSourceId { get; set; }
        public string TextField { get; set; }
        public string ValueField { get; set; }
        public FieldType FieldType { get; set; }
        public string FText { get; set; }
        public string NavigateUrlFormatString { get; set; }
        public int Width { get; set; }
        public string DateFormat { get; set; }
        public string ShowOnEditForm { get; set; }
        public string Visible { get; set; }
        public string ColumnSpan { get; set; }
        public string Check { get; set; }
        public string Patterns { get; set; }
        public string IsExport { get; set; }
        public bool ShowOnSearchForm { get; set; }
        // 添加是否需要的属性 by LYH 2014/2/25
        public bool required { get; set; }

        [PersistenceMode(PersistenceMode.InnerProperty),TemplateContainer(typeof(RepeaterItem)),Browsable(false)]
        public ITemplate CheckBoxListTemplate { get; set; }

        private Items items; //ComboBox的鍵值對
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [TypeConverter(typeof(CollectionConverter))]
        [Category("复杂属性")]
        [Description("复杂属性——内部默认嵌套形式")]
         public Items Items
        {
            get
            {
                if (this.items == null)
                {
                    this.items = new Items();
                }
                return this.items;
            }
        }

        public Field()
        {
        }



        public string GetRenderStr(string value,Dictionary<string,string> dic)
        {
            StringBuilder retStr = new StringBuilder("");
            switch (this.FieldType.ToString())
            {
                case "CheckBox":
                    if (value.Equals("True")||value.Equals("1"))
                        retStr.Append("<input  type='checkbox'checked='checked' onclick='return false;' />");
                    else
                        retStr.Append("<input  type='checkbox' onclick='return false;' />");
                    break;
                case "CheckBoxList":
                    string[] strS = value.Split(new char[]{','});
                    int i;
                    retStr.Append("<div style='height:17px;overflow:auto'>");
                    if (items!=null&&items.Count > 0)
                    {
                        foreach (Item item in Items)
                        {
                            i = 0;
                            foreach (string str in strS)
                            {

                                if (str == item.Value)
                                {
                                    retStr.Append(item.Text+"<input type='checkbox'  checked='checked' onclick='return false;'>");
                                    i++;
                                    break;
                                }

                            }
                            if(i==0)
                            {
                                retStr.Append(item.Text+"<input type='checkbox' onclick='return false;'>" );
                            }
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<string,string> keyValue in dic)
                        {
                            i = 0;
                            foreach (string str in strS)
                            {

                                if (str == keyValue.Value)
                                {
                                    retStr.Append("<input type='checkbox'  checked='checked' onclick='return false;'>" + keyValue.Key + "&nbsp;&nbsp;");
                                    i++;
                                    break;
                                }

                            }

                            if(i==0)
                            {
                                retStr.Append("<input type='checkbox' onclick='return false;'>" + keyValue.Key + "&nbsp;&nbsp;");
                            }
                        }
                    }
                    retStr.Append("</div>");
                    break;
                case "Number":
                    try
                    {
                        retStr.Append(string.Format("{0:N1}", Convert.ToDouble(value)));
                    }
                    catch (Exception e)
                    {
                        retStr.Append("不是數字格式");
                    }
                    break;
                case "Date":
                    try
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            retStr.Append("");
                        }
                        else
                        {
                            DateTime dt = DateTime.Parse(value);
                            if (dt == DateTime.MinValue)
                            {
                                retStr.Append("");
                            }
                            else
                            {
                                retStr.Append(dt.ToString(this.DateFormat.Replace("hh", "HH")));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        retStr.Append("時間格式有錯");
                    }
                    break;
                case "Img":
                    retStr.Append("<img src='" + value + "' />");
                    break;
                case "Href":
                    retStr.Append("<a href='" + value + "'  target='blank'>" + this.FText + "</a>");
                    break;
                default:
                    retStr.Append(value);
                    break;

            }
            return retStr.ToString();
        }

        public  object  FindControl(Page page,string id)
        {

            Control found = page.FindControl(id);

            if (found == null)
            {
                found = FindControlExtend(id, page.Controls);
            }

            if (page.Session[this.DataSourceId + "combo" + GetFileName(page)] == null)
            {
                object s = found as ObjectDataSource;
                if (s == null)
                {
                    s = found as SqlDataSource;
                    SqlDataSource sqlD = (SqlDataSource)s;
                    DataView dv = (DataView)sqlD.Select(DataSourceSelectArguments.Empty);
                    page.Session[this.DataSourceId + "combo" + GetFileName(page)] = dv;
                }
                else
                {
                    ObjectDataSource objD = (ObjectDataSource)s;
                    page.Session[this.DataSourceId + "combo" + GetFileName(page)] = objD.Select();
                }
            }
            return page.Session[this.DataSourceId + "combo" + GetFileName(page)];

        }


        public string GetFileName(Page page)
        {
            string s = page.Request.Path;
            s = s.Replace("/", "").Replace(".", "").Replace("aspx", "");
            return s;

        }

        private Control FindControlExtend(string id, ControlCollection controls)
        {
            int i;
            Control Found = null;
            for (i = 0; i < controls.Count; i++)
            {
                if (controls[i].ID == id)
                {
                    Found = controls[i];
                    break;
                }

                if (controls[i].Controls.Count > 0)
                {
                    Found = FindControlExtend(id, controls[i].Controls);
                    if (Found != null) break;
                }
            }
            return Found;
        }
    }
}
