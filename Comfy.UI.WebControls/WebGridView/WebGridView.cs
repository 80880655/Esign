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
using System.Collections.Specialized;
using System.Data;
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Js.jquery1.4.js", "application/x-javascript", PerformSubstitution = true)]
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Js.WebGrid.js", "application/x-javascript", PerformSubstitution = true)]
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Js.form.checker.js", "application/x-javascript", PerformSubstitution = true)]
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Js.jquery.move.js", "application/x-javascript", PerformSubstitution = true)]
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Css.css.css", "text/css", PerformSubstitution = true)]
//[assembly: WebResource("Comfy.UI.WebControls.WebCalendar.images.date.gif", "image/gif")]
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Images.gvheaderbg.gif", "image/gif")]
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Images.footerbg.gif", "image/gif")]
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Images.Loading.gif", "image/gif")]
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Images.formHead.gif", "image/gif")]
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Images.pcCloseButton.png", "image/png")]
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Images.pNext.png", "image/png")]
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Images.pPrev.png", "image/png")]
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Images.pNextDisabled.gif", "image/gif")]
[assembly: WebResource("Comfy.UI.WebControls.WebGridView.Images.pPreDisabled.gif", "image/gif")]
//[assembly: WebResource("Comfy.UI.WebControls.WebCalendar.js.calendar.js", "application/x-javascript", PerformSubstitution = true)]
//[assembly: WebResource("Comfy.UI.WebControls.WebButtonEdit.Image.edtEllipsis.png", "image/png")]
//[assembly: WebResource("Comfy.UI.WebControls.WebButtonEdit.Js.ButtonEdit.js", "application/x-javascript", PerformSubstitution = true)]
namespace Comfy.UI.WebControls.WebGridView
{
    [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
    [PersistChildren(false)]
    [ParseChildren(true, "Fields")]
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ServerControl1 runat=server></{0}:ServerControl1>")]
    public class WebGridView : WebControl, ICallbackEventHandler
    {
        Table table = new Table();
        private Fields fields; //要顯示的字段
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [TypeConverter(typeof(CollectionConverter))]
        [Category("复杂属性")]
        [Description("复杂属性——内部默认嵌套形式")]
        public Fields Fields
        {
            get
            {
                if (this.fields == null)
                {
                    this.fields = new Fields();
                }
                return this.fields;
            }
        }
        /// <summary>
        /// 查詢條件
        /// </summary>
        private string CheckCondition
        {
            get
            {
                if (Page == null || !Page.IsPostBack)
                {
                    HttpContext.Current.Session[this.ID + "CheckCondition" + GetFileName()] = null;
                    return string.Empty;
                }
                String s = (String)HttpContext.Current.Session[this.ID + "CheckCondition" + GetFileName()];
                return ((s == null) ? string.Empty : s);
            }
            set
            {
                HttpContext.Current.Session[this.ID + "CheckCondition" + GetFileName()] = value;
            }
        }

        private string ErrorMessage
        {
            get
            {

                String s = (String)HttpContext.Current.Session[this.ID + "ErrorMessage" + GetFileName()];
                return s;
            }
            set
            {
                HttpContext.Current.Session[this.ID + "ErrorMessage" + GetFileName()] = value;
            }
        }

        /// <summary>
        /// 是否是分頁查詢

        /// </summary>
        private Boolean IsSplitSearch { get; set; }

        /// <summary>
        /// 保存選中的行對應的model
        /// </summary>
        private object SelectModel
        {
            get
            {
                object s = HttpContext.Current.Session[this.ID + "SelectModel" + GetFileName()];
                return s;
            }
            set
            {
                HttpContext.Current.Session[this.ID + "SelectModel" + GetFileName()] = value;
            }
        }
        /// <summary>
        /// 排序的字段

        /// </summary>
        private string OrderByField
        {
            get
            {
                if (Page == null || !Page.IsPostBack)
                {
                    HttpContext.Current.Session[this.ID + "OrderByField" + GetFileName()] = null;
                    return string.Empty;
                }
                String s = (String)HttpContext.Current.Session[this.ID + "OrderByField" + GetFileName()];
                return ((s == null) ? string.Empty : s);
            }
            set
            {
                HttpContext.Current.Session[this.ID + "OrderByField" + GetFileName()] = value;
            }
        }
        /// <summary>
        /// 用於標誌用戶輸入的頁數是否正確

        /// </summary>
        private string PageSizeFlag { get; set; }

        private string TempPageSize { get; set; }
        /// <summary>
        /// 一頁數據的總數
        /// </summary>
        public string PageSize
        {
            get
            {
                if (Page == null || !Page.IsPostBack)
                {
                    HttpContext.Current.Session[this.ID + "PageSize" + GetFileName()] = null;
                    return TempPageSize;
                }
                object o = HttpContext.Current.Session[this.ID + "PageSize" + GetFileName()];
                return ((o == null) ? "20" : o.ToString());
            }
            set
            {
                TempPageSize = value;
                if (HttpContext.Current.Session[this.ID + "PageSize" + GetFileName()] == null)
                {
                    HttpContext.Current.Session[this.ID + "PageSize" + GetFileName()] = value;
                }
                if (value.ToString().Contains("**"))
                {
                    HttpContext.Current.Session[this.ID + "PageSize" + GetFileName()] = value.ToString().Substring(0, value.ToString().Length - 2);
                }
            }

        }
        /// <summary>
        /// 當前的頁面

        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string StartPage
        {
            get
            {
                if (Page == null || !Page.IsPostBack)
                {
                    HttpContext.Current.Session[this.ID + "StartPage" + GetFileName()] = null;
                    return "1";
                }
                return HttpContext.Current.Session[this.ID + "StartPage" + GetFileName()] == null ? "1" : (string)HttpContext.Current.Session[this.ID + "StartPage" + GetFileName()];
            }
            set
            {
                HttpContext.Current.Session[this.ID + "StartPage" + GetFileName()] = value;
            }

        }

        public Boolean CreateAddPanel { get; set; }  //是否產生編輯面板

        private string Action { get; set; }  //頁面動作

        public string OnDbClick { get; set; }  //雙擊

        public string OnClick { get; set; } //單擊

        public Boolean HasSequence { get; set; } //是否加多序號

        public Boolean HasNoOrder { get; set; }

        public Boolean HasCheckBox { get; set; }//是否增加勾選框


        public Boolean CreateSearchPanel { get; set; }

        private object SelectCollectioin  //保存當前頁面的modelList
        {
            get { return HttpContext.Current.Session[this.ID + "SelectDataOfGrid" + GetFileName()]; }
            set
            {
                HttpContext.Current.Session[this.ID + "SelectDataOfGrid" + GetFileName()] = value;
            }
        }

        private string AddNewStr { get; set; }   //保存新增的數據


        private string UpdateOldStr { get; set; } //保存要更新的數據

        private string FieldAndRow { get; set; }   //頁面傳過來的字段和第幾條數據的信息


        private string SelectedIndex { get; set; } //選中的所有行例如：1；2；3

        private string DeleteValue { get; set; }  //主鍵值



        /// <summary>
        /// 數據的總條數
        /// </summary>
        private int GridRowCount
        {
            get
            {
                object o = HttpContext.Current.Session[this.ID + "GridRowCount"];
                return ((o == null) ? -2 : Convert.ToInt32(o));
            }
            set
            {
                HttpContext.Current.Session[this.ID + "GridRowCount"] = value;
            }
        }

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
        /// <summary>
        /// 頁面的列數，也就是字段總數

        /// </summary>
        private int ColumnCount
        {
            get
            {
                return ((ViewState["ColumnCount"] == null) ? 0 : (int)ViewState["ColumnCount"]);
            }
            set
            {
                ViewState["ColumnCount"] = value;
            }
        }
        /// <summary>
        /// 主鍵
        /// </summary>
        public string KeyFieldName
        {
            get
            {
                return ((ViewState["KeyFieldName"] == null) ? string.Empty : (string)ViewState["KeyFieldName"]);
            }
            set
            {
                ViewState["KeyFieldName"] = value;
            }

        }
        /// <summary>
        /// ObjectDatasourceId
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string DataSourceId
        {
            get
            {
                String s = (String)ViewState["DataSourceId"];
                return ((s == null) ? string.Empty : s);
            }
            set
            {
                ViewState["DataSourceId"] = value;
            }

        }

        protected override void OnPreRender(EventArgs e)
        {

            string strRefrence = Page.ClientScript.GetCallbackEventReference(this, "arg", this.ID + ".ReceiveDataFromServer", "context", false);

            string strCallBack = "function " + this.ID + "_CallBackToTheServer(arg, context) {" + strRefrence + "};";

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.ID + "_CallBackToTheServer", strCallBack, true); //注册JS函数CallBackToTheServer

            this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);

            base.OnPreRender(e);
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

        void Page_PreRenderComplete(object sender, EventArgs e)
        {
            string cssUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Css.css.css"); //獲得css的資源路徑


            this.RegisterCssFile(cssUrl);  //向頁面注css文件

            Page.ClientScript.RegisterClientScriptResource(this.GetType(), "Comfy.UI.WebControls.WebGridView.Js.jquery1.4.js");//向頁面注入js
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), "Comfy.UI.WebControls.WebGridView.Js.jquery.move.js");
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), "Comfy.UI.WebControls.WebGridView.Js.WebGrid.js");
            Page.ClientScript.RegisterClientScriptResource(this.GetType(), "Comfy.UI.WebControls.WebGridView.Js.form.checker.js");
            //modify on 2012.8.10 createChildren
            //Page.ClientScript.RegisterClientScriptResource(new WebCalendar.WebCalendar().GetType(), "Comfy.UI.WebControls.WebCalendar.js.calendar.js");
            //Page.ClientScript.RegisterClientScriptResource(new WebButtonEdit.WebButtonEdit().GetType(), "Comfy.UI.WebControls.WebButtonEdit.Js.ButtonEdit.js");
            string gridJs = TextJs.Js;
            gridJs = gridJs.Replace("$tableId$", this.ID).Replace("$addForm$", this.CreateAddPanel.ToString().ToLower()).Replace("$searchForm$", this.CreateSearchPanel.ToString().ToLower());
            Page.ClientScript.RegisterClientScriptBlock(typeof(string), this.ID + "GridJs", gridJs);
            
            ObjectDataSource s = this.GetControl(this.DataSourceId) as ObjectDataSource;

            if (s == null)
            {
                s = this.FindControlExtend(this.DataSourceId, Page.Controls) as ObjectDataSource;
            }

            //ParameterCollection pc = s.SelectParameters;
            //Parameter[] pt = new Parameter[pc.Count];
            //pc.CopyTo(pt, 0);
            //ControlParameter p = (ControlParameter)pt[0];
            //string sid = p.ControlID;
            //TextBox tb =  (TextBox)this.FindControl(sid);


            s.Selecting += new ObjectDataSourceSelectingEventHandler(InputParameter);  //傳參數

            object o = s.Select();

            if (o != null)
            {
                /************當不是分頁查詢的時候就要再所有的查詢結果中篩選到當前頁的記錄************/
                if (!this.IsSplitSearch)
                {
                    ICollection collection = o as ICollection;
                    IEnumerator en = collection.GetEnumerator();
                    List<object> listObj = new List<object>();
                    int k = 0;
                    while (en.MoveNext())
                    {
                        int maxInt = Convert.ToInt32(this.StartPage) * Convert.ToInt32(this.PageSize);

                        if (k >= (Convert.ToInt32(this.StartPage) - 1) * Convert.ToInt32(this.PageSize) && k < maxInt)
                        {
                            listObj.Add(en.Current);
                        }
                        k++;
                        if (k > maxInt)
                        {
                            break;
                        }
                    }

                    o = listObj;
                    this.GridRowCount = collection.Count;

                }
            }
            else
            {
                this.GridRowCount = 0;
            }

            /********************/
            SelectCollectioin = o;
            this.CreateTable(o);
        }

        /// <summary>
        /// 刪除model
        /// </summary>
        /// <param name="ods"></param>
        private void DeleteRowByKey(ObjectDataSource ods, string[] deleteKeys, object[] objs)
        {
            IDataSource currtentD = ods as IDataSource;
            DataSourceView view = currtentD.GetView("");
            for (int i = 0; i < objs.Length; i++)
            {
                OrderedDictionary keys = new OrderedDictionary();
                for (int j = 0; j < deleteKeys.Length; j++)
                {
                    if (!string.IsNullOrEmpty(deleteKeys[j]))
                    {
                        keys.Add(deleteKeys[j], objs[i].GetType().GetProperty(deleteKeys[j]).GetValue(objs[i], null));
                    }
                }

                view.Delete(keys, null, new DataSourceViewOperationCallback(HandleDataSourceViewDeleteOperationCallback));

            }
        }


        /// <summary>
        /// 刪除model
        /// </summary>
        /// <param name="ods"></param>
        private void DeleteRowByKey(ObjectDataSource ods, string ikey)
        {
            IDataSource currtentD = ods as IDataSource;
            DataSourceView view = currtentD.GetView("");
            OrderedDictionary keys = new OrderedDictionary();
            keys.Add(this.KeyFieldName, ikey);
            view.Delete(keys, null, new DataSourceViewOperationCallback(HandleDataSourceViewDeleteOperationCallback));
        }

        /// <summary>
        /// 增加model
        /// </summary>
        /// <param name="ods"></param>
        private void AddNewModel(ObjectDataSource ods)
        {
            IDataSource currtentD = ods as IDataSource;
            DataSourceView view = currtentD.GetView("");

            string[] fieldAndVal = this.AddNewStr.Split(new string[] { "<>;" }, StringSplitOptions.None);
            OrderedDictionary keys = new OrderedDictionary();

            for (int i = 0; i < fieldAndVal.Length; i++)
            {
                if (fieldAndVal[i].Contains("<>:"))
                {
                    string[] fVS = fieldAndVal[i].Split(new string[] { "<>:" }, StringSplitOptions.None);
                    if (!string.IsNullOrEmpty(fVS[1]))
                    {
                        if (!keys.Contains(fVS[0]))
                        {
                            keys.Add(fVS[0], fVS[1]);
                        }
                    }
                }
            }
            try
            {
                view.Insert(keys, new DataSourceViewOperationCallback(HandleDataSourceViewDeleteOperationCallback));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 更新model
        /// </summary>
        /// <param name="ods"></param>
        private void UpdateModel(ObjectDataSource ods)
        {
            IDataSource currtentD = ods as IDataSource;
            DataSourceView view = currtentD.GetView("");

            string[] fieldAndVal = this.UpdateOldStr.Split(new string[] { "<>;" }, StringSplitOptions.None);
            OrderedDictionary keysNew = new OrderedDictionary();

            for (int i = 0; i < fieldAndVal.Length; i++)
            {
                if (fieldAndVal[i].Contains("<>:"))
                {
                    string[] fVS = fieldAndVal[i].Split(new string[] { "<>:" }, StringSplitOptions.None);
                    if (!keysNew.Contains(fVS[0]))
                    {
                        keysNew.Add(fVS[0], fVS[1]);
                    }

                }
            }
            view.Update(null, keysNew, null, new DataSourceViewOperationCallback(HandleDataSourceViewDeleteOperationCallback));
        }
        protected virtual bool HandleDataSourceViewDeleteOperationCallback(int affectedRecords, Exception e)
        {

            if (e != null && e.InnerException != null)
            {
                ErrorMessage = e.InnerException.Message;
            }
            else
            {
                ErrorMessage = null;
            }
            return true;
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


        protected override void Render(HtmlTextWriter writer)
        {
            //writer.AddAttribute(HtmlTextWriterAttribute.Style, "height:100%;width:100%;overflow:auto;");
            //writer.RenderBeginTag(HtmlTextWriterTag.Div);


            if (this.Height != 0 && this.Width != 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Style, "overflow: auto;height:" + this.Height + "px;width:" + this.Width + "px");
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Style, "overflow: auto;height:79%;width:100%;");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "gridBodyDiv");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ID + "Div");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            table.RenderControl(writer);
            writer.RenderEndTag();


            string imgUrl = "";
            if (Page != null)
            {
                imgUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.footerbg.gif");
            }
            if (this.Width != 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "gridFootDiv");
                writer.AddAttribute(HtmlTextWriterAttribute.Id,this.ID+"gridFootDiv");
                writer.AddAttribute(HtmlTextWriterAttribute.Style, "width:" + this.Width + "px;height:39px;background:url(" + imgUrl + ") repeat-x;padding:0;margin:0;");
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "gridFootDivNoWidth");
                writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ID + "gridFootDiv");
                writer.AddAttribute(HtmlTextWriterAttribute.Style, "width:100%;height:39px;background:url(" + imgUrl + ") repeat-x;padding:0;margin:0;");
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            this.CreateTableFooter().RenderControl(writer);
            writer.RenderEndTag();
            //modify on 2012.8.10 createChildren
            //if (this.CreateAddPanel)  //是否自動添加新增、編輯面板

            //{
            //    writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ID + "DivF");
            //    writer.AddAttribute(HtmlTextWriterAttribute.Class, "blockDiv");
            //    writer.RenderBeginTag(HtmlTextWriterTag.Div);
            //    this.CreateDivHead().RenderControl(writer);
            //    this.AddFormHtml().RenderControl(writer);
            //    writer.RenderEndTag();
            //}

            //if (this.CreateSearchPanel) //是否添加查詢面板
            //{
            //    writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ID + "SearchDiv");
            //    writer.AddAttribute(HtmlTextWriterAttribute.Class, "blockDiv");
            //    writer.RenderBeginTag(HtmlTextWriterTag.Div);
            //    this.CreateSearchDivHead().RenderControl(writer);
            //    this.AddSearchFormHtml().RenderControl(writer);
            //    writer.RenderEndTag();
            //}
            base.Render(writer);
        }
        //modify on 2012.8.10 createChildren
        protected override void CreateChildControls()
        {
            if (this.CreateSearchPanel) //是否添加查詢面板
            {
                HtmlGenericControl div = new HtmlGenericControl();
                div.Attributes.Add("Id", this.ID + "SearchDiv");
           //     div.ID = this.ID + "SearchDiv";
                div.Attributes.Add("class", "blockDiv");
                div.Controls.Add(this.CreateSearchDivHead());
                div.Controls.Add(this.AddSearchFormHtml());
                this.Controls.Add(div);
            }
            if (this.CreateAddPanel)  //是否自動添加新增、編輯面板

            {
                HtmlGenericControl divF = new HtmlGenericControl();
                divF.Attributes.Add("Id", this.ID + "DivF");
               // divF.ID = this.ID + "DivF";
                divF.Attributes.Add("class", "blockDiv");
                divF.Controls.Add(this.CreateDivHead());
                divF.Controls.Add(this.AddFormHtml());
                this.Controls.Add(divF);
            }
        }

        /// <summary>
        /// 根據id獲取objectdatasource
        /// </summary>
        /// <param name="dsId"></param>
        /// <returns></returns>
        private Control GetControl(string Id)
        {
            Control result = Page.FindControl(Id);
            if (result == null && Page.Master!=null)
                result = Page.Master.FindControl(Id);
            if (result != null)
                return result;
            return FindControlExtend(Id, Page.Controls);
          //  return null;
        }
        /// <summary>
        /// 生成Grid的頭部

        /// </summary>
        /// <param name="table"></param>
        /// <param name="obj"></param>
        private void CreateTableHeader(ref Table table)
        {
            int j = 0;
            string imgUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.gvheaderbg.gif");
            TableRow trHeader = new TableRow();
            trHeader.ID = this.ID + "GridHeard";
            trHeader.Attributes.Add("class", "tt");

            if (this.HasCheckBox)//增加checkbox
            {
                TableCell tc = new TableCell();
                // tc.Attributes.Add("class", "mtt");
                tc.Attributes.Add("align", "center");
                tc.Attributes.Add("style", "height:26px;width:20px;background:url(" + imgUrl + ") repeat-x;");

                CheckBox checkBox = new CheckBox();
                checkBox.Attributes.Add("onclick", this.ID + ".CheckboxRow(this)");
                checkBox.ID = this.ID + "__ChexkBox";
                tc.Controls.Add(checkBox);
                trHeader.Cells.Add(tc);

            }
            if (this.HasSequence)   //增加序列
            {
                TableCell tc = new TableCell();
                //   tc.Attributes.Add("class", "mtt");
                tc.Attributes.Add("style", "height:26px;background:url(" + imgUrl + ") repeat-x;");
                tc.Attributes.Add("align", "center");
                trHeader.Cells.Add(tc);
            }

            foreach (Field field in this.Fields)
            {

                if (field.Visible == "false")
                    continue;
                j++;
                TableCell tc = new TableCell();
                if (!string.IsNullOrEmpty(field.FieldName))
                {
                    tc.ID = this.ID + field.FieldName + "*orderAsc";
                }
                tc.Attributes.Add("style", "width:" + (field.Width == 0 ? "145px" : (field.Width + "px")) + ";height:26px;background:url(" + imgUrl + ") repeat-x;");
                // tc.Attributes.Add("class", "mtt");
                tc.Attributes.Add("class", "tt");
                tc.Attributes.Add("align", "center");
                if ((field.FieldType.ToString() != "Href" && field.FieldType.ToString() != "CheckBoxOnGrid") && !this.HasNoOrder) //不是model的字段不增加排序功能
                {
                    tc.Attributes.Add("onclick", this.ID + ".OrderBy(this)");
                    tc.Attributes.Add("onmouseover", this.ID + ".ChangeMouseStyle(this)");
                }

                Label lb = new Label();
                //  lb.Attributes.Add("style", "font-weight:300;");
                if (string.IsNullOrEmpty(field.Caption))
                {
                    lb.Text = field.FieldName;
                }
                else
                {
                    lb.Text = field.Caption;
                }
                tc.Controls.Add(lb);
                trHeader.Cells.Add(tc);

            }
            ColumnCount = j;
            if (this.HasSequence)  //如果有序列就加多一列

            {
                ColumnCount++;
            }
            if (this.HasCheckBox)  //如果有勾選框就加多一列

            {
                ColumnCount++;
            }

            table.Rows.Add(trHeader);
        }

        private void ClearSession()
        {
            foreach (Field field in this.Fields)  //遍曆model的所有屬性

            {
                if (!string.IsNullOrEmpty(field.DataSourceId))
                {
                    HttpContext.Current.Session[GetFileName() + field.DataSourceId] = null;
                    HttpContext.Current.Session[field.DataSourceId + "combo" + GetFileName()] = null;
                }
            }
        }


        /// <summary>
        /// 根據數據源生產grid
        /// </summary>
        /// <param name="ods"></param>
        private void CreateTable(object ods)
        {

            ClearSession();
            table.ID = this.ID;
            //設置table 屬性

            //  table.Attributes.Add("ondrag", "return false;");
            table.Attributes.Add("align", "center");
            table.Attributes.Add("style", "font-size:12pt;width:" + GetFieldWidth() + "px;float:left;background-color:#0080C0");
            table.Attributes.Add("cellpadding", "1");
            table.Attributes.Add("cellspacing", "1");

            ////table.Attributes.Add("onmousedown", "MoveTableUtil.init(this)");  //table的邊框可以拖動

            ////table.Attributes.Add("onmouseup", "MoveTableUtil.end(this)");
            ////table.Attributes.Add("onmousemove", "MoveTableUtil.drag(this)");

            //獲取數據源信息

            ICollection collection = ods as ICollection;

            this.CreateTableHeader(ref table);  //產生表頭

            if (collection != null && collection.Count != 0)  //有數據

            {
                int i = 0;
                int temp = 0;
                foreach (object obj in collection)
                {
                    TableRow trContent = new TableRow();

                    if (this.HasCheckBox)//增加checkbox
                    {
                        TableCell tc = new TableCell();
                        CheckBox checkBox = new CheckBox();
                        tc.Attributes.Add("class", "tt");
                        tc.Attributes.Add("align", "center");
                        tc.Attributes.Add("style", "width:20px");
                        checkBox.ID = this.ID + "ChexkBox" + (((Convert.ToInt32(this.StartPage) - 1) * Convert.ToInt32(this.PageSize)) + i + 1);
                        tc.Controls.Add(checkBox);
                        trContent.Cells.Add(tc);

                    }

                    if (this.HasSequence)//增加序列
                    {
                        TableCell tc = new TableCell();
                        Label lb = new Label();
                        tc.Attributes.Add("class", "tt");
                        tc.Attributes.Add("align", "center");
                        tc.Attributes.Add("style", "width:20px");
                        lb.Text = (((Convert.ToInt32(this.StartPage) - 1) * Convert.ToInt32(this.PageSize)) + i + 1) + "";
                        tc.Controls.Add(lb);
                        trContent.Cells.Add(tc);
                    }

                    trContent.Attributes.Add("height", "25");
                    if (!string.IsNullOrEmpty(this.OnClick))
                    {
                        trContent.Attributes.Add("onclick", "javascript:" + this.ID + ".changeActiveRow(this);" + this.OnClick); //增加單擊事件
                    }
                    else
                    {
                        trContent.Attributes.Add("onclick", this.ID + ".changeActiveRow(this)");
                    }

                    if (!string.IsNullOrEmpty(this.OnDbClick))   //增加雙擊事件
                    {
                        trContent.Attributes.Add("ondblclick", this.OnDbClick);
                    }
                    if (i % 2 == 0)  //控制相鄰兩行數據的背景顏色不一樣

                    {
                        trContent.Attributes.Add("bgColor", "#ffffff");
                    }
                    else
                    {
                        trContent.Attributes.Add("bgColor", "#e5f1ff");
                    }
                    if (temp == 0)
                    {
                        foreach (System.Reflection.PropertyInfo modelInfo in obj.GetType().GetProperties())
                        {
                            if (modelInfo.Name.Equals("GridRowCount"))
                            {
                                if (this.IsSplitSearch)
                                {
                                    GridRowCount = (int)modelInfo.GetValue(obj, null);  //獲取總條數

                                    break;
                                }
                            }
                        }
                    }
                    temp++;
                    foreach (Field field in this.Fields)  //遍曆model的所有屬性

                    {
                        if (field.Visible == "false")
                            continue;
                        Boolean bFlag = false;  //判斷Field 是否是model的屬性，如果是就根據model的屬性值來顯示，如果不是就根據Field的設定值來顯示
                        TableCell tc = new TableCell();
                        string cellStr = "";
                        if (field.FieldName.Contains(";"))
                        {
                            string tqStr = "";
                            foreach (string ss in field.FieldName.Split(new char[] { ';' }))
                            {
                                string tempStrStr = GetValueByVlaue(field, ss, obj);
                                tqStr += (tempStrStr == null ? "" : tempStrStr + "<*M*>");
                            }
                            cellStr = tqStr.Substring(0, tqStr.Length - 5);
                        }
                        else
                        {
                            string tempStrStr = GetValueByVlaue(field, field.FieldName, obj);
                            if (tempStrStr == null)
                            {
                                bFlag = true;
                            }
                            else
                            {
                                cellStr = tempStrStr;
                            }
                        }

                        if (!bFlag)
                        {
                            tc.Attributes.Add("style", "width:" + (field.Width == 0 ? "145px" : field.Width + "px"));
                            if (!string.IsNullOrEmpty(field.FRender))
                            {
                                tc.Attributes.Add("Render", field.FRender);
                            }
                            Label lb = new Label();
                            lb.Text = cellStr;
                            tc.Attributes.Add("class", "tt");
                            tc.Attributes.Add("align", "center");
                            tc.Controls.Add(lb);
                        }
                        if (bFlag)
                        {
                            if (field.FieldType.ToString() == "CheckBoxOnGrid")
                            {
                                int tempN = (((Convert.ToInt32(this.StartPage) - 1) * Convert.ToInt32(this.PageSize)) + i + 1);
                                tc.Attributes.Add("style", "width:" + (field.Width == 0 ? "145px" : field.Width + "px"));
                                Label ckLable = new Label();
                                ckLable.Text = "<input id='" + this.ID + "ChexkBoxOnGrid" + tempN + "' type='checkbox' name='" + this.ID + "ChexkBoxOnGrid" + tempN + "' onclick='javascript:" + this.ID + ".changeActiveRow($(this).parent().parent().parent().get(0));" + this.ID + "CheckOnClick(this);' valueTemp='" + tempN + "' />";
                                tc.Attributes.Add("class", "tt");
                                tc.Attributes.Add("align", "center");
                                tc.Controls.Add(ckLable);
                                trContent.Cells.Add(tc);
                            }
                            else
                            {
                                tc.Attributes.Add("style", "width:" + (field.Width == 0 ? "145px" : field.Width + "px"));
                                HtmlGenericControl A = new HtmlGenericControl("A");
                                A.Attributes.Add("href", field.NavigateUrlFormatString);
                                A.InnerText = field.FText;

                                tc.Attributes.Add("class", "tt");
                                tc.Attributes.Add("align", "center");
                                tc.Controls.Add(A);
                                trContent.Cells.Add(tc);
                            }
                        }
                        trContent.Cells.Add(tc);
                    }
                    i++;
                    table.Rows.Add(trContent);

                }
            }
            else
            {
                GridRowCount = 0;
            }

            // this.CreateTableFooter(ref table);

        }


        private string GetValueByVlaue(Field field, string Name, object obj)
        {
            foreach (System.Reflection.PropertyInfo modelInfo in obj.GetType().GetProperties())
            {
                if (Name == modelInfo.Name)
                {
                    object valueObject = modelInfo.GetValue(obj, null);
                    return this.GetLabelText(field, valueObject == null ? string.Empty : valueObject.ToString());
                }
            }
            return null;
        }
        /// <summary>
        /// 根據數據源Id取得數據源

        /// </summary>
        /// <param name="dataSourceId"></param>
        /// <returns></returns>
        private object GetObjectByDataSource(string dataSourceId)
        {
            if (HttpContext.Current.Session[dataSourceId + "combo" + GetFileName()] == null)
            {
                object s = this.GetControl(dataSourceId) as ObjectDataSource;
                if (s == null)
                {
                    s = this.GetControl(dataSourceId) as SqlDataSource;
                    SqlDataSource sqlD = (SqlDataSource)s;
                    DataView dv = (DataView)sqlD.Select(DataSourceSelectArguments.Empty);
                    HttpContext.Current.Session[dataSourceId + "combo" + GetFileName()] = dv;
                }
                else
                {
                    ObjectDataSource objD = (ObjectDataSource)s;
                    HttpContext.Current.Session[dataSourceId + "combo" + GetFileName()] = objD.Select();
                }
            }
            return HttpContext.Current.Session[dataSourceId + "combo" + GetFileName()];
        }

        /// <summary>
        /// 提取ods的textField和valueField字段並組合成dictionary
        /// </summary>
        /// <param name="ods"></param>
        /// <param name="textField"></param>
        /// <param name="valueField"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetValueByText(object ods, string textField, string valueField)
        {

            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (ods == null)
                return dic;
            DataView dv = ods as DataView;
            if (dv != null)
            {
                DataTable table = dv.Table;
                DataRowCollection drc = table.Rows;
                foreach (DataRow r in drc)
                {
                    dic.Add(r[valueField].ToString(), r[textField].ToString());
                }
                return dic;
            }
            ICollection collection = ods as ICollection;
            foreach (object obj in collection)
            {
                object textFieldObj = obj.GetType().GetProperty(textField).GetValue(obj, null);
                object valueFieldObj = obj.GetType().GetProperty(valueField).GetValue(obj, null);
                string strTextField = textFieldObj == null ? null : textFieldObj.ToString();
                string strValueField = valueFieldObj == null ? null : valueFieldObj.ToString();
                if (!string.IsNullOrEmpty(strTextField))
                {
                    dic.Add(strValueField, strTextField);
                }
            }
            return dic;
        }

        /// <summary>
        /// 根據Items生成Dictionary
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetDicByItems(Field field)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Items items = field.Items;
            foreach (Item item in items)
            {
                dic.Add(item.Value, item.Text);
            }
            return dic;
        }


        /// <summary>
        /// 如果Field設置了objectdatasource，那麼顯示的內容根據objectdatasource顯示
        /// </summary>
        /// <param name="field"></param>
        /// <param name="lb"></param>
        /// <param name="text"></param>
        public string GetLabelText(Field field, string text)
        {
            if (!string.IsNullOrEmpty(field.DataSourceId) && !string.IsNullOrEmpty(field.TextField) && !string.IsNullOrEmpty(field.ValueField) || field.Items.Count > 0)
            {
                Dictionary<string, string> dic = null;

                if (string.IsNullOrEmpty(field.DataSourceId) && field.Items.Count > 0)   //以ITEMS作為ComboBox的數據源
                {
                    dic = GetDicByItems(field);
                }
                else   //以DataSourceId作為ComboBox的數據源
                {
                    object dicObject = HttpContext.Current.Session[GetFileName() + field.DataSourceId];
                    if (dicObject != null)
                    {
                        dic = (Dictionary<string, string>)dicObject;
                    }
                    else
                    {
                        object ods = this.GetObjectByDataSource(field.DataSourceId);
                        dic = this.GetValueByText(ods, field.TextField, field.ValueField);
                        HttpContext.Current.Session[GetFileName() + field.DataSourceId] = dic;
                    }
                }
                string s = "";
                if (dic.ContainsKey(text))
                {
                    s = dic[text];
                }
                else
                {
                    return field.GetRenderStr(text, dic);
                }
                if (!string.IsNullOrEmpty(s))
                {
                    return field.GetRenderStr(s, dic);
                }
                else
                {
                    return field.GetRenderStr(text, dic);
                }
            }
            else
            {
                return field.GetRenderStr(text, null);
            }

        }

        /// <summary>
        /// 向ObjectDataSource傳參
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InputParameter(object sender, ObjectDataSourceSelectingEventArgs e)
        {


            ObjectDataSourceView ods = (ObjectDataSourceView)sender;
            ParameterCollection pc = ods.SelectParameters;
            int flag = 0;
            foreach (Parameter p in pc)  //判斷是否分頁查詢
            {
                if (flag == 3)
                    break;
                if (p.Name.Equals("startPage") || p.Name.Equals("pageSize") || p.Name.Equals("orderByField"))
                {
                    flag++;
                }
            }

            if (flag == 3)  //是否帶有startPage、pageSize、orderByField參數,即判斷是否分頁查詢

            {
                this.IsSplitSearch = true;
                e.InputParameters["startPage"] = this.StartPage;
                e.InputParameters["pageSize"] = this.PageSize;

                if (!string.IsNullOrEmpty(this.OrderByField))
                {
                    e.InputParameters["orderByField"] = this.OrderByField.Substring(this.ID.Length, this.OrderByField.Length - this.ID.Length);
                }
                else
                {
                    e.InputParameters["orderByField"] = null;
                }

            }
            else
            {
                this.IsSplitSearch = false;
            }
            string[] conditions = CheckCondition.Split(new string[] { "<>;" }, StringSplitOptions.None);
            foreach (Parameter p in pc)  //匹配查詢條件
            {
                object fp = e.InputParameters[p.Name];
                object fs = fp;
                if (p.Name.Equals("startPage") || p.Name.Equals("pageSize") || p.Name.Equals("orderByField"))
                {
                    continue;
                }
                //     Boolean paramFlag = true;
                for (int i = 0; i < conditions.Length; i++)
                {
                    string[] nameAndCon = conditions[i].Split(new string[] { "<>:" }, StringSplitOptions.None);
                    if (nameAndCon.Length != 2)
                        continue;
                    if (nameAndCon[0].Equals(p.Name))
                    {
                        if (string.IsNullOrEmpty(nameAndCon[1]) || nameAndCon[1] == "null")
                        {
                            e.InputParameters[p.Name] = null;
                        }
                        else
                        {
                            if (p.Type.ToString().ToLower().Contains("int"))
                            {
                                if (nameAndCon[1].ToLower() == "true")
                                {
                                    e.InputParameters[p.Name] = 1;
                                }
                                else if (nameAndCon[1].ToLower() == "false")
                                {
                                    e.InputParameters[p.Name] = 0;
                                }
                                else
                                {
                                    e.InputParameters[p.Name] = nameAndCon[1];
                                }
                                continue;
                            }
                            e.InputParameters[p.Name] = nameAndCon[1];
                        }
                        // paramFlag = false;
                        break;
                    }
                }
                //if (paramFlag)
                //{
                //    if (Page.IsPostBack)
                //    {
                //        object fb = e.InputParameters[p.Name];
                //        string s = fb.ToString();
                //        e.InputParameters[p.Name] = null;
                //    }
                //}
            }
        }



        /// <summary>
        /// 向ObjectDataSource傳參
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ExportInputParameter(object sender, ObjectDataSourceSelectingEventArgs e)
        {

            ObjectDataSourceView ods = (ObjectDataSourceView)sender;
            ParameterCollection pc = ods.SelectParameters;
            int flag = 0;
            foreach (Parameter p in pc)  //判斷是否分頁查詢
            {
                if (flag == 3)
                    break;
                if (p.Name.Equals("startPage") || p.Name.Equals("pageSize") || p.Name.Equals("orderByField"))
                {
                    flag++;
                }
            }

            if (flag == 3)  //是否帶有startPage、pageSize、orderByField參數,即判斷是否分頁查詢

            {
                this.IsSplitSearch = true;
                e.InputParameters["startPage"] = 1;
                e.InputParameters["pageSize"] = this.GridRowCount;

                if (!string.IsNullOrEmpty(this.OrderByField))
                {
                    e.InputParameters["orderByField"] = this.OrderByField.Substring(this.ID.Length, this.OrderByField.Length - this.ID.Length);
                }
                else
                {
                    e.InputParameters["orderByField"] = null;
                }

            }
            else
            {
                this.IsSplitSearch = false;
            }
            string[] conditions = CheckCondition.Split(new string[] { "<>;" }, StringSplitOptions.None);
            foreach (Parameter p in pc)  //匹配查詢條件
            {
                object fp = e.InputParameters[p.Name];
                object fs = fp;
                if (p.Name.Equals("startPage") || p.Name.Equals("pageSize") || p.Name.Equals("orderByField"))
                {
                    continue;
                }

                for (int i = 0; i < conditions.Length; i++)
                {
                    string[] nameAndCon = conditions[i].Split(new string[] { "<>:" }, StringSplitOptions.None);
                    if (nameAndCon.Length != 2)
                        continue;
                    if (nameAndCon[0].Equals(p.Name))
                    {
                        if (string.IsNullOrEmpty(nameAndCon[1]) || nameAndCon[1] == "null")
                        {
                            e.InputParameters[p.Name] = null;
                        }
                        else
                        {
                            if (p.Type.ToString().ToLower().Contains("int"))
                            {
                                if (nameAndCon[1].ToLower() == "true")
                                {
                                    e.InputParameters[p.Name] = 1;
                                }
                                else if (nameAndCon[1].ToLower() == "false")
                                {
                                    e.InputParameters[p.Name] = 0;
                                }
                                else
                                {
                                    e.InputParameters[p.Name] = nameAndCon[1];
                                }
                                continue;
                            }
                            e.InputParameters[p.Name] = nameAndCon[1];
                        }
                        // paramFlag = false;
                        break;
                    }
                }

            }
        }



        #region ICallbackEventHandler Members

        public string GetCallbackResult()
        {
            //  object  ocs = Page.Session;
            if (this.Action.Equals("getGridData"))  //獲取grid選中行的值

            {
                ICollection selectCollection = SelectCollectioin as ICollection;

                if (selectCollection == null)
                {
                    return "ExError" + "<>页面太久没有操作，请重新打开该页面";
                }

                object[] allObj = new object[selectCollection.Count];
                selectCollection.CopyTo(allObj, 0);
                return this.GetFieldData(allObj);
            }

            if (this.Action.Equals("getSelectedValue"))  //獲取checkbox選中的值

            {
                ICollection selectCollection = SelectCollectioin as ICollection;
                if (selectCollection == null)
                {
                    return "ExError" + "<>页面太久没有操作，请重新打开该页面";
                }
                object[] allObj = new object[selectCollection.Count];
                selectCollection.CopyTo(allObj, 0);
                return this.GetSelectedValue(allObj);
            }

            if (this.Action.Equals("witchPage"))  //顯示第幾頁

            {
                if (this.PageSizeFlag == "witchError")
                    return "error";
            }
            ObjectDataSource s = this.GetControl(this.DataSourceId) as ObjectDataSource;
            if (s == null)
            {
                s = this.FindControlExtend(this.DataSourceId, Page.Controls) as ObjectDataSource;
            }

            if (this.Action.Equals("deleteRow")) //刪除
            {
                ICollection selectCollection = SelectCollectioin as ICollection;
                if (selectCollection == null)
                {
                    return "ExError" + "<>页面太久没有操作，请重新打开该页面";
                }
                object[] allObj = new object[selectCollection.Count];
                selectCollection.CopyTo(allObj, 0);
                object[] selectObj = this.GetSelectedObject(allObj, this.DeleteValue);

                if (this.DeleteValue.Contains("<"))
                {
                    string[] deleteKeys = this.DeleteValue.Split(new char[] { ':' })[1].Split(new char[] { '<' })[1].Split(new char[] { ';' });
                    this.DeleteRowByKey(s, deleteKeys, selectObj);
                }
                else
                {
                    string deleteKey = this.DeleteValue.Split(new char[] { ':' })[1];
                    this.DeleteRowByKey(s, deleteKey);
                }
                return "";
            }

            if (this.Action.Equals("howPage"))  //每頁顯示多少條

            {
                if (this.PageSizeFlag == "error")
                {
                    return "error";
                }
            }

            if (this.Action.Equals("addNew"))
            {
                this.AddNewModel(s);
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    return "ExError" + "<>" + ErrorMessage;
                }
                return "";
            }

            if (this.Action.Equals("updateOld"))
            {
                this.UpdateModel(s);
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    return "ExError" + "<>" + ErrorMessage;
                }
                return "";
            }
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                string tempErrorMessage = ErrorMessage;
                ErrorMessage = null;
                return "ExError" + "<>" + tempErrorMessage;
            }
            ParameterCollection pc = s.SelectParameters;
            s.Selecting += new ObjectDataSourceSelectingEventHandler(InputParameter);  //傳參數


            object ob = null;
            try
            {
                ob = s.Select();
            }
            catch (Exception ex)
            {
                return "ExError" + "<>" + ex.Message;
            }

            /**********當不是分頁查詢的時候就要再所有的查詢結果中篩選到當前頁的記錄**************/
            if (!this.IsSplitSearch)
            {
                ICollection collectionO = ob as ICollection;

                IEnumerator en = collectionO.GetEnumerator();

                List<object> listObj = new List<object>();
                int k = 0;
                while (en.MoveNext())
                {
                    int maxInt = Convert.ToInt32(this.StartPage) * Convert.ToInt32(this.PageSize);

                    if (k >= (Convert.ToInt32(this.StartPage) - 1) * Convert.ToInt32(this.PageSize) && k < maxInt)
                    {
                        listObj.Add(en.Current);
                    }
                    k++;
                    if (k > maxInt)
                    {
                        break;
                    }
                }

                ob = listObj;
                this.GridRowCount = collectionO.Count;

            }

            /*******************/

            SelectCollectioin = ob;  //SelectCollectioin時刻保持最新查找出來的數據
            StringBuilder builder = new StringBuilder("");  //拼接html的builder
            StringBuilder builderContent = new StringBuilder("");
            ICollection collection = ob as ICollection;

            if (collection != null)
            {
                int i = 0;
                string strOnClick = "";
                string strOnDbClick = "";
                if (!string.IsNullOrEmpty(this.OnClick))  //點擊
                {
                    strOnClick = this.OnClick;
                }
                if (!string.IsNullOrEmpty(this.OnDbClick))  //雙擊
                {
                    strOnDbClick = this.OnDbClick;
                }
                if (collection.Count == 0)  //沒有數據
                {
                    this.GridRowCount = 0;
                }
                int temp = 0;
                foreach (object obj in collection)
                {

                    if (i % 2 == 0) //相隔兩行顯示不同的顏色

                    {
                        builderContent.Append("<tr height='25' bgColor='#ffffff' onclick='javascript:" + this.ID + ".changeActiveRow(this);" + strOnClick + "' ondblclick='" + this.OnDbClick + "' >");
                    }
                    else
                    {
                        builderContent.Append("<tr height='25' bgColor='#e5f1ff' onclick='javascript:" + this.ID + ".changeActiveRow(this);" + strOnClick + "' ondblclick='" + this.OnDbClick + "'>");
                    }


                    if (this.HasCheckBox)  //增加checkbox
                    {
                        builderContent.Append(" <td class='tt' align='center' style='width:20px'><input id='" + this.ID + "ChexkBox" + (((Convert.ToInt32(this.StartPage) - 1) * Convert.ToInt32(this.PageSize)) + i + 1) + "' type='checkbox' name='" + this.ID + "ChexkBox" + (((Convert.ToInt32(this.StartPage) - 1) * Convert.ToInt32(this.PageSize)) + i + 1) + "'/></td> ");
                    }

                    if (this.HasSequence)
                    {
                        builderContent.Append("<td class='tt' align='center' style='width:20px'><span>" + (((Convert.ToInt32(this.StartPage) - 1) * Convert.ToInt32(this.PageSize)) + i + 1) + "</span></td>");  //序列
                    }
                    if (temp == 0)
                    {
                        foreach (System.Reflection.PropertyInfo modelInfo in obj.GetType().GetProperties())
                        {
                            if (modelInfo.Name.Equals("GridRowCount"))
                            {
                                if (this.IsSplitSearch)
                                {
                                    GridRowCount = (int)modelInfo.GetValue(obj, null);  //獲取總條數

                                    break;
                                }
                            }
                        }
                    }
                    temp++;
                    foreach (Field field in Fields)  //遍曆model的所有屬性

                    {
                        if (field.Visible == "false")
                            continue;

                        Boolean bFlag = false;
                        string tempStrSS = "";
                        string cellStr = "";
                        if (field.FieldName.Contains(";"))
                        {
                            string tqStr = "";
                            foreach (string ss in field.FieldName.Split(new char[] { ';' }))
                            {
                                string tempStrStr = GetValueByVlaue(field, ss, obj);
                                tqStr += (tempStrStr == null ? "" : tempStrStr + "<*M*>");
                            }
                            tempStrSS = tqStr.Substring(0, tqStr.Length - 5);
                        }
                        else
                        {
                            string tqStr = GetValueByVlaue(field, field.FieldName, obj);
                            if (tqStr == null)
                            {
                                bFlag = true;
                            }
                            else
                            {
                                tempStrSS = tqStr;
                            }
                        }
                        if (!string.IsNullOrEmpty(field.FRender) && !bFlag)
                        {
                            cellStr = "<td class='tt' align='center' style='width:" + (field.Width == 0 ? "145px" : field.Width + "px") + "' Render='" + field.FRender + "'><span>" + tempStrSS + "</span></td>";  //內容
                        }
                        if (string.IsNullOrEmpty(field.FRender) && !bFlag)
                        {
                            cellStr = "<td class='tt' style='width:" + (field.Width == 0 ? "145px" : field.Width + "px") + "' align='center'><span>" + tempStrSS + "</span></td>";  //內容
                        }
                        if (bFlag)
                        {
                            if (field.FieldType.ToString() == "CheckBoxOnGrid")
                            {
                                int tempN = (((Convert.ToInt32(this.StartPage) - 1) * Convert.ToInt32(this.PageSize)) + i + 1);
                                cellStr = "<td class='tt' align='center' style='width:" + (field.Width == 0 ? "145px" : field.Width + "px") + "'><span><input id='" + this.ID + "ChexkBoxOnGrid" + tempN + "' type='checkbox' name='" + this.ID + "ChexkBoxOnGrid" + tempN + "' onclick='javascript:" + this.ID + ".changeActiveRow($(this).parent().parent().parent().get(0));" + this.ID + "CheckOnClick(this);' valueTemp='" + tempN + "' /></span></td>";
                            }
                            else
                            {
                                cellStr = "<td class='tt' align='center' style='width:" + (field.Width == 0 ? "145px" : field.Width + "px") + "'><A href=\"" + field.NavigateUrlFormatString + "\">" + field.FText + "</A></td>";  //

                            }
                        }
                        builderContent.Append(cellStr);

                    }
                    i++;
                    builderContent.Append("</tr>");

                }

            }

            string strDisableB = "";
            string strDisableN = "";

            string imgPre = "";
            string imgNext = "";
            if (Convert.ToInt32(this.StartPage) == 1)   //第一頁的時候，“上一頁”就不允許點擊

            {
                strDisableB = "disabled=disabled";
                imgPre = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.pPreDisabled.gif");
            }
            else
            {
                imgPre = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.pPrev.png");
            }
            if (Convert.ToInt32(this.StartPage) * Convert.ToInt32(this.PageSize) >= this.GridRowCount)//最後一頁的時候，“下一頁”就不允許點擊

            {
                strDisableN = "disabled=disabled";
                imgNext = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.pNextDisabled.gif");
            }
            else
            {
                imgNext = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.pNext.png");
            }
            builder.Append("<tr id='" + this.ID + "trFooter'><td colspan='" + this.ColumnCount + "' align='left'> <table><tr><td><span id='" + this.ID + "pageMessage'>");
            builder.Append("Page " + this.StartPage+" of " + GetPageCount() + " &nbsp;"+"("+this.GridRowCount+" Items)</span></td><td>");
            builder.Append("<div style='background:url(\"" + imgPre + "\");height:19px;width:19px;cursor:pointer;' " + strDisableB + " onclick=\"javascript:var " + this.ID + "pageMessage = $('#" + this.ID + "pageMessage').html();" + this.ID + ".LoadingPattern();" + this.ID + ".GetServerTime(" + this.ID + "pageMessage+'*before');return false;\" ></div></td><td>&nbsp;</td><td>");
            builder.Append("<input type='text' value='" + this.StartPage + "' id='" + this.ID + "WitchPage' style='width:20px;height:15px;text-align:center;' onchange='javascript:" + this.ID + ".LoadingPattern();" + this.ID + ".SetWitchPage()'/></td><td>&nbsp;</td><td>");
            builder.Append("<div style='background:url(\"" + imgNext + "\");height:19px;width:19px;cursor:pointer;' " + strDisableN + "  onclick=\"javascript:var " + this.ID + "pageMessage = $('#" + this.ID + "pageMessage').html();" + this.ID + ".LoadingPattern();" + this.ID + ".GetServerTime(" + this.ID + "pageMessage+'*next');return false;\"></div></td><td>");
            builder.Append("<span>&nbsp;&nbsp;&nbsp;PageItems:&nbsp;</span></td><td><input type='text' value='" + this.PageSize + "' id='" + this.ID + "HowPage' onchange='javascript:" + this.ID + ".LoadingPattern();" + this.ID + ".SetHowPage();' style='width:20px;height:15px;text-align:center;' /></td><td>&nbsp;</td></tr></table></td></tr>");

            builder.Append("<**>");  //grid內容和底部信息的分割標誌

            return builder.Append(builderContent).ToString();

        }
        /// <summary>
        /// 獲得在grid中選中的行的字段值

        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string GetFieldData(object[] obj)
        {
            int k = 0;
            string returnStr = "";
            k = Convert.ToInt32(this.FieldAndRow.Split(new char[] { ':' })[1]);  //獲得選中的行
            object selectObj = obj[k];
            SelectModel = selectObj;
            foreach (System.Reflection.PropertyInfo modelInfo in selectObj.GetType().GetProperties())  //遍曆model的所有屬性

            {

                if (modelInfo.PropertyType.Name == "DateTime")  //時間格式，如果是yyyy-MM-dd 就返回10位，否則返回包括HH:mm:ss
                {
                    foreach (Field field in Fields)
                    {
                        if (field.FieldName == modelInfo.Name)
                        {

                            object tempObj = modelInfo.GetValue(selectObj, null);
                            try
                            {
                                DateTime dt = DateTime.Parse(tempObj.ToString());
                                if (dt == DateTime.MinValue)
                                {
                                    returnStr += "'" + modelInfo.Name.Replace("\\", "\\\\").Replace("'", @"\'").Replace("\0", "").Replace("\r", "").Replace("\n", "") + "':'" + string.Empty + "',";
                                }
                                else
                                {
                                    returnStr += "'" + modelInfo.Name.Replace("\\", "\\\\").Replace("'", @"\'").Replace("\0", "").Replace("\r", "").Replace("\n", "") + "':'" + dt.ToString(string.IsNullOrEmpty(field.DateFormat) ? "yyyy-MM-dd HH:mm:ss" : field.DateFormat.Replace("hh", "HH")) + "',";
                                }
                            }
                            catch (Exception e)
                            {
                                returnStr += "'" + modelInfo.Name.Replace("\\", "\\\\").Replace("'", @"\'").Replace("\0", "").Replace("\r", "").Replace("\n", "") + "':'" + string.Empty + "',";
                            }
                            break;
                        }
                    }
                }
                else
                {
                    object o = modelInfo.GetValue(selectObj, null);
                    returnStr += "'" + modelInfo.Name.Replace("\\", "\\\\").Replace("'", @"\'").Replace("\0", "").Replace("\r", "").Replace("\n", "") + "':'" + (o == null ? string.Empty : o.ToString().Replace("\r", "").Replace("\n", "").Replace("\0", "").Replace("\\", "\\\\").Replace("'", @"\'")) + "',";
                }

            }

            returnStr = returnStr.Substring(0, returnStr.Length - 1);
            return returnStr;

        }

        /// <summary>
        /// 獲得在grid中checkbox選中的值

        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string GetSelectedValue(object[] obj)
        {
            string returnStr = "";
            string[] strK = this.SelectedIndex.Split(new char[] { ':' })[1].Trim().Split(new char[] { ';' });  //獲得選中的行
            for (int i = 0; i < strK.Length - 1; i++)
            {
                object selectObj = obj[Convert.ToInt32(strK[i])];
                foreach (System.Reflection.PropertyInfo modelInfo in selectObj.GetType().GetProperties())  //遍曆model的所有屬性

                {

                    object o = modelInfo.GetValue(selectObj, null); //此處還沒有更新到v4當中，必須更新

                    returnStr += "'" + modelInfo.Name.Replace("\\", "\\\\").Replace("'", @"\'").Replace("\0", "").Replace("\r", "").Replace("\n", "") + "':'" + (o == null ? string.Empty : o.ToString().Replace("\r", "").Replace("\n", "").Replace("\0", "").Replace("\\", "\\\\").Replace("'", @"\'")) + "',";
                    //  returnStr += "'" + modelInfo.Name.Replace("\\", "\\\\").Replace("'", @"\'") + "':'" + modelInfo.GetValue(selectObj, null).ToString().Replace("\\", "\\\\").Replace("'", @"\'") + "',";

                }

                returnStr = returnStr.Substring(0, returnStr.Length - 1);
                if (i != strK.Length - 2)
                {
                    returnStr += "<sPl>;";
                }
            }

            return returnStr;

        }


        private object[] GetSelectedObject(object[] obj, string s)
        {
            string[] s1 = s.Split(new char[] { ':' })[1].Split(new char[] { '<' })[0].Trim().Split(new char[] { ';' });
            object[] retO = new object[s1.Length - 1];
            for (int i = 0; i < s1.Length - 1; i++)
            {
                retO[i] = obj[Convert.ToInt32(s1[i])];

            }
            return retO;
        }

        /// <summary>
        /// 客戶端js首先調用的函數

        /// </summary>
        /// <param name="eventArgument"></param>
        public void RaiseCallbackEvent(string eventArgument)
        {
            ErrorMessage = "";
            if (eventArgument.Contains("getGridData"))
            {
                this.Action = "getGridData"; //獲取一行值

                this.FieldAndRow = eventArgument;
                return;
            }

            if (eventArgument.Contains("getSelectedValue"))
            {
                this.Action = "getSelectedValue"; //獲取多行值

                this.SelectedIndex = eventArgument;
                return;
            }

            if (eventArgument.Contains("check"))  //查找,頁數為第一頁

            {
                this.StartPage = "1";
                this.Action = "check";  //設置頁面js傳過來的工作
                // string[] checkCond = Regex.Split(eventArgument, "<*>");
                string[] checkCond = eventArgument.Split(new string[] { "<*>" }, StringSplitOptions.None);
                if (checkCond.Length != 2)
                {
                    CheckCondition = "";
                }
                else
                {
                    CheckCondition = checkCond[1];
                }
                return;
            }

            if (eventArgument.Contains("deleteRow")) //刪除某行
            {
                this.Action = "deleteRow";
                this.DeleteValue = eventArgument;
                return;
            }

            if (eventArgument.Contains("witchPage")) //第幾頁

            {
                this.Action = "witchPage";
                int wP;
                string[] strI = eventArgument.Split(new char[] { ':' });
                try
                {
                    wP = Convert.ToInt32(strI[1]);
                }
                catch (Exception e)   //輸入的頁數不合法
                {
                    //  this.StartPage = -2;
                    this.PageSizeFlag = "witchError";
                    return;
                }
                if (wP < 1 || wP > this.GetPageCount())  //輸入的頁數小於1或者大於總頁數也不合法
                {
                    //  this.StartPage = -2;
                    this.PageSizeFlag = "witchError";
                    return;
                }
                this.StartPage = wP + "";
                return;
            }

            if (eventArgument.Contains("howPage")) //一頁顯示多少條
            {
                string[] strI = eventArgument.Split(new char[] { ':' });
                this.Action = "howPage";
                int pageTemp = 0;
                try
                {
                    pageTemp = Convert.ToInt32(strI[1]);
                }
                catch
                {
                    this.PageSizeFlag = "error";  //輸入有誤
                    return;
                }
                if (pageTemp < 1 || pageTemp > 20)
                {
                    this.PageSizeFlag = "error";
                    return;
                }
                this.StartPage = "1";
                this.PageSize = strI[1] + "**";
                return;
            }

            if (eventArgument.Contains("orderBy"))
            {
                this.Action = "orderBy";
                this.OrderByField = eventArgument.Split(new char[] { ':' })[1];
                return;
            }

            if (eventArgument.Contains("addNew"))
            {
                this.Action = "addNew";
                this.AddNewStr = eventArgument.Substring(7, eventArgument.Length - 7);
                return;
            }
            if (eventArgument.Contains("updateOld"))
            {
                this.Action = "updateOld";
                this.UpdateOldStr = eventArgument.Substring(10, eventArgument.Length - 10);
                return;
            }
            SetNextPage(eventArgument);  //點擊下一頁，頁數就加1；點擊上一頁，頁數就減1

        }
        #endregion
        /// <summary>
        /// 設置下一頁是第幾頁

        /// </summary>
        /// <param name="eventArgument"></param>
        private void SetNextPage(string eventArgument)
        {
            this.Action = "nextPage"; //設置頁面js傳過來的工作
            string[] strTemp = eventArgument.Split(new char[] { '*' });
            string strTempCut = strTemp[0].Trim();
            int intPage = Convert.ToInt32((strTempCut.Substring(strTempCut.IndexOf("Page") + 4, strTempCut.LastIndexOf("of") - strTempCut.IndexOf("Page") - 4)).Trim());  //獲得頁面當前是第幾頁

            if (strTemp[1].Equals("refresh"))  //刷新當前頁

            {
                this.StartPage = intPage + "";
                return;
            }

            //    this.GridRowCount this.PageSize

            if (strTemp[1].Equals("next"))
            {
                int tempI = this.GridRowCount / Convert.ToInt32(this.PageSize);
                int tempY = this.GridRowCount % Convert.ToInt32(this.PageSize);
                if (tempY > 0)
                {
                    tempI++;
                }
                if (intPage + 1 > tempI)
                {
                    this.StartPage = intPage + "";
                }
                else
                {
                    this.StartPage = (intPage + 1) + "";  //點擊下一頁，頁數就加1    
                }
            }
            else
            {
                if (intPage > 1)
                {
                    this.StartPage = (intPage - 1) + "";  //點擊上一頁，頁數就減1
                }
                else
                {
                    this.StartPage = "1";
                }
            }
        }

        /// <summary>
        /// 獲取所有的欄位的寬度總和

        /// </summary>
        /// <returns></returns>
        private int GetFieldWidth()
        {
            int inResult = 0;
            if (this.HasCheckBox)
            {
                inResult += 20;
            }
            if (this.HasSequence)
            {
                inResult += 20;
            }
            foreach (Field field in this.Fields)
            {
                if (field.Width == 0)
                {
                    inResult += 145;
                }
                else
                {
                    inResult += field.Width;
                }
            }
            return inResult;

        }

        /// <summary>
        /// 創建grid的選頁欄
        /// </summary>
        /// <returns></returns>
        private Table CreateTableFooter()
        {

            Table table = new Table();
            table.ID = this.ID + "FooterTable";


            string strDisableB = "";
            string strDisableN = "";

            string imgPre = "";
            string imgNext = "";
            if (Convert.ToInt32(this.StartPage) == 1)   //第一頁的時候，“上一頁”就不允許點擊

            {
                strDisableB = "disabled=disabled";
                imgPre = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.pPreDisabled.gif");
            }
            else
            {
                imgPre = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.pPrev.png");
            }
            if (Convert.ToInt32(this.StartPage) * Convert.ToInt32(this.PageSize) >= this.GridRowCount)//最後一頁的時候，“下一頁”就不允許點擊

            {
                strDisableN = "disabled=disabled";
                imgNext = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.pNextDisabled.gif");
            }
            else
            {
                imgNext = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.pNext.png");
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("<table><tr><td><span id='" + this.ID + "pageMessage'>");
            builder.Append("Page " + this.StartPage + " of " + GetPageCount() + " &nbsp;&nbsp;" + "(" + this.GridRowCount + " Items)</span></td><td>");
            builder.Append("<div style='background:url(\"" + imgPre + "\");height:19px;width:19px;cursor:pointer;' " + strDisableB + " onclick=\"javascript:var " + this.ID + "pageMessage = $('#" + this.ID + "pageMessage').html();" + this.ID + ".LoadingPattern();" + this.ID + ".GetServerTime(" + this.ID + "pageMessage+'*before');return false;\" ></div></td><td>&nbsp;</td><td>");
            builder.Append("<input type='text' value='" + this.StartPage + "' id='" + this.ID + "WitchPage' style='width:20px;height:15px;text-align:center;' onchange='javascript:" + this.ID + ".LoadingPattern();" + this.ID + ".SetWitchPage()'/></td><td>&nbsp;</td><td>");
            builder.Append("<div style='background:url(\"" + imgNext + "\");height:19px;width:19px;cursor:pointer;' " + strDisableN + "  onclick=\"javascript:var " + this.ID + "pageMessage = $('#" + this.ID + "pageMessage').html();" + this.ID + ".LoadingPattern();" + this.ID + ".GetServerTime(" + this.ID + "pageMessage+'*next');return false;\"></div></td><td>");
            builder.Append("<span>&nbsp;&nbsp;&nbsp;PageItems:&nbsp;</span></td><td><input type='text' value='" + this.PageSize + "' id='" + this.ID + "HowPage' onchange='javascript:" + this.ID + ".LoadingPattern();" + this.ID + ".SetHowPage();' style='width:20px;height:15px;text-align:center;' /></td><td>&nbsp;</td></tr></table>");


            Label tfootLable = new Label();
            tfootLable.Text = builder.ToString();

            //table的空tr，方便回傳的時候jquery替換他的兄弟tr
            TableRow trFooter1 = new TableRow();
            trFooter1.ID = this.ID + "trFooter1";
            trFooter1.Attributes.Add("style", "height:3px");
            TableCell tcFooter1 = new TableCell();
            trFooter1.Cells.Add(tcFooter1);
            table.Rows.Add(trFooter1);

            TableRow trFooter = new TableRow();
            trFooter.ID = this.ID + "trFooter";
            TableCell tcFooter = new TableCell();
            tcFooter.Attributes.Add("align", "left");

            tcFooter.Controls.Add(tfootLable);
            trFooter.Cells.Add(tcFooter);
            table.Rows.Add(trFooter);
            return table;

        }
        /// <summary>
        /// 計算總頁數

        /// </summary>
        /// <returns></returns>
        private int GetPageCount()
        {
            int inRe = this.GridRowCount / Convert.ToInt32(this.PageSize);
            if (this.GridRowCount % Convert.ToInt32(this.PageSize) != 0)
            {
                inRe++;
            }
            return inRe;
        }
        /// <summary>
        /// 產生添加、修改面板

        /// </summary>
        /// <returns></returns>
        private Table AddFormHtml()
        {
            Table table = new Table();
            table.Width = 699;
            table.ID = this.ID + "AddForm";
            //空行

            TableRow trBlank1 = new TableRow();
            trBlank1.Attributes.Add("style", "height:20px");

            TableCell tdBalek1 = new TableCell();
            tdBalek1.ID = this.ID + "SearchStayTd";
            tdBalek1.Attributes.Add("style", "width:100px");

            TableCell tdBalek2 = new TableCell();
            tdBalek2.Attributes.Add("style", "width:132px");

            TableCell tdBalek3 = new TableCell();
            tdBalek3.Attributes.Add("style", "width:100px");

            TableCell tdBalek4 = new TableCell();
            tdBalek4.Attributes.Add("style", "width:132px");

            TableCell tdBalek5 = new TableCell();
            tdBalek5.Attributes.Add("style", "width:100px");

            TableCell tdBalek6 = new TableCell();
            tdBalek6.Attributes.Add("style", "width:132px");

            trBlank1.Cells.Add(tdBalek1);
            trBlank1.Cells.Add(tdBalek2);
            trBlank1.Cells.Add(tdBalek3);
            trBlank1.Cells.Add(tdBalek4);
            trBlank1.Cells.Add(tdBalek5);
            trBlank1.Cells.Add(tdBalek6);

            table.Rows.Add(trBlank1);


            TableRow tr = null;
            int intFlag = 0;
            foreach (Field field in Fields)
            {
                int ColumnSpan;
                if (!string.IsNullOrEmpty(field.ColumnSpan))
                {
                    ColumnSpan = Convert.ToInt32(field.ColumnSpan) > 3 ? 3 : Convert.ToInt32(field.ColumnSpan);
                }
                else
                {
                    ColumnSpan = 1;
                }
                if (field.FieldType.ToString() == "TextArea")
                {
                    ColumnSpan = 3;
                }
                if (field.FieldType.ToString() == "CheckBoxOnGrid" || field.ShowOnEditForm == "false")  //過濾掉不需要現在在編輯框的控件
                    continue;
                if (intFlag == 0 || ColumnSpan == 3 || (intFlag + ColumnSpan) > 3)  //控制換行
                {
                    tr = new TableRow();
                    tr.Attributes.Add("style", "height:25px");
                    intFlag = 0;
                }
                intFlag += ColumnSpan;
                int intWitch = ((intFlag == 3 && String.IsNullOrEmpty(field.ColumnSpan) || field.ColumnSpan == "1") ? 8 : 0);
                TableCell tdL = new TableCell();
                tdL.Attributes.Add("align", "right"); //Jianbo Update 2012-7-3
                tdL.Attributes.Add("style", "width:100px");
                Label lb = new Label();
                lb.Text = string.IsNullOrEmpty(field.Caption) ? field.FieldName : field.Caption;
                tdL.Controls.Add(lb);

                TableCell tdC = new TableCell();

                //  Control tbox = CreateControlByFieldType(field, ColumnSpan, intWitch);

                Control tbox = GridControlCreator.CreateControl(this.ID, ColumnSpan, intWitch, false, this.Page, field, false).CreateControl();

                tdC.Attributes.Add("colspan", (ColumnSpan * 2 - 1) + "");

                tdC.Attributes.Add("style", "width:" + (ColumnSpan * 132 + 100 * (ColumnSpan - 1)) + "px");
                tdC.Controls.Add(tbox);
                tr.Cells.Add(tdL);
                tr.Cells.Add(tdC);

                table.Rows.Add(tr);


            }
            //空行
            TableRow trBlank = new TableRow();
            TableCell tdBalek = new TableCell();
            TextBox tboxId = new TextBox();
            tboxId.ID = this.ID + this.ID + KeyFieldName;
            tdBalek.Controls.Add(tboxId);
            tdBalek.Attributes.Add("style", "display:none");
            trBlank.Attributes.Add("style", "height:32px");
            trBlank.Cells.Add(tdBalek);

            //增加按鈕
            TableRow trAction = new TableRow();
            trAction.Attributes.Add("Id", this.ID + "trAdd");
         //   trAction.ID = this.ID + "trAdd";
            TableCell tdAction = new TableCell();
            tdAction.Attributes.Add("colspan", "6");
            tdAction.Attributes.Add("align", "right");

            Button bSaveNotClose = new Button();
            bSaveNotClose.Attributes.Add("Id", this.ID + "bSNC");
          //  bSaveNotClose.ID = this.ID + "bSNC";

            bSaveNotClose.OnClientClick = "javascript:" + this.ID + ".AddNewAndClose();return false;";
            bSaveNotClose.Text = "Save and close";

            Button bSaveAndClose = new Button();
           // bSaveAndClose.ID = this.ID + "bSAC";
            bSaveAndClose.Attributes.Add("Id", this.ID + "bSAC");
            bSaveAndClose.OnClientClick = "javascript:" + this.ID + ".AddNew(); return false;";
            bSaveAndClose.Text = "Save";


            Button bCancel = new Button();
            bCancel.Attributes.Add("Id", this.ID + "bC");
           // bCancel.ID = this.ID + "bC";
            bCancel.Text = "Cancel";
            bCancel.OnClientClick = "javascript:" + this.ID + ".Unblock();return false;";

            Label l1 = new Label();
            Label l2 = new Label();
            l1.Text = "&nbsp;&nbsp;&nbsp;";
            l2.Text = "&nbsp;&nbsp;&nbsp;";

            tdAction.Controls.Add(bSaveNotClose);
            tdAction.Controls.Add(l1);
            tdAction.Controls.Add(bSaveAndClose);
            tdAction.Controls.Add(l2);
            tdAction.Controls.Add(bCancel);
            trAction.Cells.Add(tdAction);



            //更新按鈕

            TableRow trActionU = new TableRow();
            trActionU.Attributes.Add("Id", this.ID + "trUpdate");
           // trActionU.ID = this.ID + "trUpdate";
            trActionU.Attributes.Add("style", "display:none");
            TableCell tdActionU = new TableCell();
            tdActionU.Attributes.Add("colspan", "3");
            tdActionU.Attributes.Add("align", "right");


            TableCell tdActionNP = new TableCell();
            tdActionNP.Attributes.Add("colspan", "3");
            tdActionNP.Attributes.Add("align", "center");
            tdActionNP.Attributes.Add("style", "padding:0 0 0 200px;");



            // Button bNext = new Button();
            HtmlGenericControl bNext = new HtmlGenericControl("div");
            bNext.ID = this.ID + "bNext";
            bNext.Attributes.Add("style", "cursor:pointer;width:19px;height:19px;background: url('" + Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.pNext.png") + "')");
            //  divHead.Attributes.Add("style", "height: 27px; background: url('" + Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebPopupControl.Images.formHead.gif") + "') repeat-x;");
            //  bNext.OnClientClick = "javascript:" + this.ID + ".NextData();return false;";
            bNext.Attributes.Add("onclick", this.ID + ".NextData()");
            // bNext.Text = "下一條";

            //  Button bPre = new Button();
            HtmlGenericControl bPre = new HtmlGenericControl("div");
            bPre.ID = this.ID + "bPre";
            bPre.Attributes.Add("style", "cursor:pointer;width:19px;height:19px;background: url('" + Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.pPrev.png") + "')");
            // bPre.Text = "上一條";
            // bPre.OnClientClick = "javascript:" + this.ID + ".PreData(); return false;";
            bPre.Attributes.Add("onclick", this.ID + ".PreData()");

            Table npTable = new Table();
            TableRow nprow = new TableRow();
            TableCell ndivcell = new TableCell();
            TableCell pdivcell = new TableCell();
            ndivcell.Controls.Add(bNext);
            pdivcell.Controls.Add(bPre);
            nprow.Cells.Add(pdivcell);
            nprow.Cells.Add(ndivcell);
            npTable.Rows.Add(nprow);

            Button bUpdate = new Button();
            bUpdate.ID = this.ID + "bUpdate";
            bUpdate.OnClientClick = "javascript:" + this.ID + ".Update();return false;";
            bUpdate.Text = "Update and close";

            Button bUpdateClose = new Button();
            bUpdateClose.ID = this.ID + "bUpdateClose";
            bUpdateClose.OnClientClick = "javascript:" + this.ID + ".UpdateClose();return false;";
            bUpdateClose.Text = "Update";

            Button bCancel1 = new Button();
            bCancel1.ID = this.ID + "bC1";
            bCancel1.Text = "Cancel";
            bCancel1.OnClientClick = "javascript:" + this.ID + ".Unblock(); return false;";

            Label l3 = new Label();
            l3.Text = "&nbsp;&nbsp;&nbsp;";

            Label l4 = new Label();
            l4.Text = "&nbsp;&nbsp;&nbsp;";

            // tdActionNP.Controls.Add(bPre);
            // tdActionNP.Controls.Add(bNext);

            tdActionNP.Controls.Add(npTable);

            tdActionU.Controls.Add(bUpdate);
            tdActionU.Controls.Add(l4);
            tdActionU.Controls.Add(bUpdateClose);
            tdActionU.Controls.Add(l3);
            tdActionU.Controls.Add(bCancel1);
            trActionU.Cells.Add(tdActionNP);
            trActionU.Cells.Add(tdActionU);




            table.Rows.Add(trBlank);
            table.Rows.Add(trAction);
            table.Rows.Add(trActionU);
            return table;
        }




        /// <summary>
        /// 產生查詢面板
        /// </summary>
        /// <returns></returns>
        private Table AddSearchFormHtml()
        {
            Table table = new Table();
            table.Width = 699;
            table.ID = this.ID + "AddSearchForm";
            //空行
            TableRow trBlank1 = new TableRow();
            trBlank1.Attributes.Add("style", "height:20px");

            TableCell tdBalek1 = new TableCell();
            tdBalek1.ID = this.ID + "SearchStayTdSearch";
            tdBalek1.Attributes.Add("style", "width:100px");

            TableCell tdBalek2 = new TableCell();
            tdBalek2.Attributes.Add("style", "width:132px");

            TableCell tdBalek3 = new TableCell();
            tdBalek3.Attributes.Add("style", "width:100px");

            TableCell tdBalek4 = new TableCell();
            tdBalek4.Attributes.Add("style", "width:132px");

            TableCell tdBalek5 = new TableCell();
            tdBalek5.Attributes.Add("style", "width:100px");

            TableCell tdBalek6 = new TableCell();
            tdBalek6.Attributes.Add("style", "width:132px");

            trBlank1.Cells.Add(tdBalek1);
            trBlank1.Cells.Add(tdBalek2);
            trBlank1.Cells.Add(tdBalek3);
            trBlank1.Cells.Add(tdBalek4);
            trBlank1.Cells.Add(tdBalek5);
            trBlank1.Cells.Add(tdBalek6);

            table.Rows.Add(trBlank1);


            TableRow tr = null;
            int intFlag = 0;
            foreach (Field field in Fields)
            {
                if (!field.ShowOnSearchForm)  //過濾掉不需要呈現在查詢面板的控件

                    continue;
                int ColumnSpan;
                if (!string.IsNullOrEmpty(field.ColumnSpan))
                {
                    ColumnSpan = Convert.ToInt32(field.ColumnSpan) > 3 ? 3 : Convert.ToInt32(field.ColumnSpan);
                }
                else
                {
                    ColumnSpan = 1;
                }
                if (field.FieldType.ToString() == "TextArea")
                {
                    ColumnSpan = 3;
                }
                if (intFlag == 0 || ColumnSpan == 3 || (intFlag + ColumnSpan) > 3)  //控制換行
                {
                    tr = new TableRow();
                    tr.Attributes.Add("style", "height:25px");
                    intFlag = 0;
                }
                intFlag += ColumnSpan;
                int intWitch = ((intFlag == 3 && String.IsNullOrEmpty(field.ColumnSpan) || field.ColumnSpan == "1") ? 8 : 0);
                TableCell tdL = new TableCell();
                tdL.Attributes.Add("align", "center");
                tdL.Attributes.Add("style", "width:100px");
                Label lb = new Label();
                lb.Text = string.IsNullOrEmpty(field.Caption) ? field.FieldName : field.Caption;
                tdL.Controls.Add(lb);

                TableCell tdC = new TableCell();

                // Control tbox = CreateSearchControlByFieldType(field, ColumnSpan, intWitch, false);
                Control tbox = GridControlCreator.CreateControl(this.ID, ColumnSpan, intWitch, false, this.Page, field, true).CreateControl();
                tdC.Attributes.Add("colspan", (ColumnSpan * 2 - 1) + "");

                tdC.Attributes.Add("style", "width:" + (ColumnSpan * 132 + 100 * (ColumnSpan - 1)) + "px");
                tdC.Controls.Add(tbox);
                tr.Cells.Add(tdL);
                tr.Cells.Add(tdC);

                table.Rows.Add(tr);


                if (field.FieldType.ToString() == "Date")  //如果是時間格式就要多創建一個時間控件。

                {
                    if ((intFlag + 1) > 3)  //控制換行
                    {
                        tr = new TableRow();
                        tr.Attributes.Add("style", "height:25px");
                        intFlag = 0;
                    }
                    intFlag += 1;
                    int intWitch1 = (intFlag == 3 ? 2 : 0);
                    TableCell tdL1 = new TableCell();
                    tdL1.Attributes.Add("align", "center");
                    tdL1.Attributes.Add("style", "width:100px");
                    Label lb1 = new Label();
                    lb1.Text = "~";
                    tdL1.Controls.Add(lb1);

                    TableCell tdC1 = new TableCell();

                    // Control tbox1 = CreateSearchControlByFieldType(field, 1, intWitch, true);
                    Control tbox1 = GridControlCreator.CreateControl(this.ID, 1, intWitch, true, this.Page, field, true).CreateControl();

                    tdC1.Attributes.Add("colspan", (1 * 2 - 1) + "");

                    tdC1.Attributes.Add("style", "width:" + (1 * 133 + 100 * (1 - 1) + intWitch) + "px");
                    tdC1.Controls.Add(tbox1);
                    tr.Cells.Add(tdL1);
                    tr.Cells.Add(tdC1);

                    table.Rows.Add(tr);
                }
            }

            //空行
            TableRow trBlank = new TableRow();
            TableCell tdBalek = new TableCell();
            trBlank.Attributes.Add("style", "height:32px");
            trBlank.Cells.Add(tdBalek);

            //增加按鈕
            TableRow trAction = new TableRow();
            trAction.ID = this.ID + "trSearch";
            TableCell tdAction = new TableCell();
            tdAction.Attributes.Add("colspan", "6");
            tdAction.Attributes.Add("align", "right");

            Button btnSearch = new Button();
            btnSearch.ID = this.ID + "btnSearch";
            btnSearch.OnClientClick = "javascript:" + this.ID + ".PanelSearch();return false;";
            btnSearch.Text = "Search";


            Button bCancel = new Button();
            bCancel.ID = this.ID + "btnSearchCanel";
            bCancel.Text = "Cancle";
            bCancel.OnClientClick = "javascript:" + this.ID + ".SearchUnblock();return false;";

            Label l1 = new Label();
            l1.Text = "&nbsp;&nbsp;&nbsp;";

            tdAction.Controls.Add(btnSearch);
            tdAction.Controls.Add(l1);
            tdAction.Controls.Add(bCancel);
            trAction.Cells.Add(tdAction);

            table.Rows.Add(trBlank);
            table.Rows.Add(trAction);
            return table;
        }

        private Control CreateDivHead()
        {

            HtmlGenericControl HeaderDivD = new HtmlGenericControl("div");
            string imgUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.formHead.gif");
            HeaderDivD.Attributes.Add("style", "width:700px;height:27px;background: url('" + imgUrl + "') repeat-x;");
            HtmlGenericControl headDiv = new HtmlGenericControl("div");
            headDiv.Attributes.Add("Id", this.ID + "DivHeader");
           // headDiv.ID = this.ID + "DivHeader";
            headDiv.Attributes.Add("style", "float: left;background: url('" + imgUrl + "') repeat-x;");
            headDiv.Attributes.Add("class", "blockDivHeader");
            headDiv.Attributes.Add("align", "left");

            Image img = new Image();
            img.Attributes.Add("style", "float: right; margin: 2px 2px 0 0;cursor: pointer;");
            img.ID = this.ID + "closeImg";
            img.ImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.pcCloseButton.png");
            img.Attributes.Add("onclick", "javascript:" + this.ID + ".Unblock(); return false;");

            HeaderDivD.Controls.Add(headDiv);
            HeaderDivD.Controls.Add(img);
            return HeaderDivD;

        }


        private Control CreateSearchDivHead()
        {

            HtmlGenericControl HeaderDivD = new HtmlGenericControl("div");

            string imgUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.formHead.gif");

            HeaderDivD.Attributes.Add("style", "width:700px;height:27px;background: url('" + imgUrl + "') repeat-x;");

            HtmlGenericControl headDiv = new HtmlGenericControl("div");
            headDiv.ID = this.ID + "SearchDivHeader";
            headDiv.Attributes.Add("style", "background: url('" + imgUrl + "') repeat-x;float:left;");
            headDiv.Attributes.Add("class", "blockDivHeader");
            headDiv.Attributes.Add("align", "left");
            headDiv.InnerHtml = "<div style='padding:8px 0 0 2px;font-weight:bolder;'>【Search Panel】</div>";

            Image img = new Image();
            img.Attributes.Add("style", "float: right; margin: 2px 2px 0 0;cursor: pointer;");
            img.ID = this.ID + "closeImgSearch";
            img.ImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebGridView.Images.pcCloseButton.png");
            img.Attributes.Add("onclick", "javascript:" + this.ID + ".SearchUnblock();return false;");

            HeaderDivD.Controls.Add(headDiv);
            HeaderDivD.Controls.Add(img);

            return HeaderDivD;

        }
        /// <summary>
        /// 將數據導出EXCEL
        /// </summary>
        public void ExportXls()
        {
            ObjectDataSource s = this.GetControl(this.DataSourceId) as ObjectDataSource;

            if (s == null)
            {
                s = this.FindControlExtend(this.DataSourceId, Page.Controls) as ObjectDataSource;
            }

            ParameterCollection pc = s.SelectParameters;
            s.Selecting += new ObjectDataSourceSelectingEventHandler(ExportInputParameter);  //傳參數

            object ob = s.Select();

            Fields fields = Fields;

            for (int i = (fields.Count - 1); i >= 0; i--)  //移除不需要導出的字段
            {
                if (fields[i].FieldType.ToString() == "Href" || fields[i].FieldType.ToString() == "CheckBoxOnGrid" || fields[i].IsExport == "false" || (fields[i].Visible == "false" && fields[i].IsExport != "true"))
                {
                    fields.Remove(fields[i]);
                }

            }
            new WebGridExporter().ExportToXls(ob, fields, this);

        }

        private string GetFileName()
        {
            string s = HttpContext.Current.Request.Path;
            s = s.Replace("/", "").Replace(".", "").Replace("aspx", "");
            return s;

        }

    }

}
