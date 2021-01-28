using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Comfy.App.Web.Core.UC
{
    public enum ToolBarItem { Search, Clear, New, Edit, Delete, Approve, Export, Import, Help, Design }

    [ToolboxData("<{0}:ToolBar runat=\"server\"></{0}:ToolBar>")]
    public partial class ToolBar : System.Web.UI.UserControl
    {
        [System.ComponentModel.Category("Action")]
        public event EventHandler ExportClick;
        [System.ComponentModel.Category("Action")]
        public event EventHandler ImportClick;
        [System.ComponentModel.Category("Action")]
        public event EventHandler SearchClick;
        [System.ComponentModel.Category("Action")]
        public event EventHandler ClearClick;
        [System.ComponentModel.Category("Action")]
        public event EventHandler NewClick;
        [System.ComponentModel.Category("Action")]
        public event EventHandler EditClick;
        [System.ComponentModel.Category("Action")]
        public event EventHandler DeleteClick;
        [System.ComponentModel.Category("Action")]
        public event EventHandler ApproveClick;
        [System.ComponentModel.Category("Action")]
        public event EventHandler HelpClick;

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (SearchClick == null)
                GetItem(ToolBarItem.Search).NavigateUrl = GetUrl(ToolBarItem.Search);
            if (NewClick == null)
                GetItem(ToolBarItem.New).NavigateUrl = GetUrl(ToolBarItem.New);
            if (EditClick == null)
                GetItem(ToolBarItem.Edit).NavigateUrl = GetUrl(ToolBarItem.Edit);
            if (DeleteClick == null)
                GetItem(ToolBarItem.Delete).NavigateUrl = GetUrl(ToolBarItem.Delete);
            if (ApproveClick == null)
                GetItem(ToolBarItem.Approve).NavigateUrl = GetUrl(ToolBarItem.Approve);
            if (ClearClick == null)
                GetItem(ToolBarItem.Clear).NavigateUrl = GetUrl(ToolBarItem.Clear);
            if (ImportClick == null)
                GetItem(ToolBarItem.Import).NavigateUrl = GetUrl(ToolBarItem.Import);
            if (HelpClick == null)
                GetItem(ToolBarItem.Help).NavigateUrl = GetUrl(ToolBarItem.Help);
            if (ExportClick == null)
                GetItem(ToolBarItem.Export).NavigateUrl = GetUrl(ToolBarItem.Export);
        }

        string GetUrl(ToolBarItem item)
        {
            return "javascript:ToolBarMenuItemClick('" + item + "','" + this.ID + "');";
        }

        public MenuItem GetItem(ToolBarItem item)
        {
            switch (item)
            {
                case ToolBarItem.Delete:
                    return FindItem("刪除");
                case ToolBarItem.Approve:
                    return FindItem("審核");
                case ToolBarItem.Edit:
                    return FindItem("編輯");
                case ToolBarItem.New:
                    return FindItem("新增");
                case ToolBarItem.Search:
                    return FindItem("查找");
                case ToolBarItem.Export:
                    return FindItem("導出");
                case ToolBarItem.Import:
                    return FindItem("導入");
                case ToolBarItem.Help:
                    return FindItem("幫助");
                case ToolBarItem.Clear:
                    return FindItem("清除");
                default:
                    throw new NotSupportedException();
            }

        }



        public MenuItem FindItem(string itemText)
        {
            foreach (MenuItem item in ToolBar123.Items)
            {
                if (item.Text == itemText)
                {
                    return item;
                }
            }
            return null;
        }

        public void SetEnable(bool enabled, ToolBarItem item)
        {
            GetItem(item).Enabled = enabled;
        }

        public void SetVisible(ToolBarItem item)
        {
            ToolBar123.Items.Remove(GetItem(item));
        }

        public void ChangeText(ToolBarItem item, string newName)
        {
            MenuItem menuItem = GetItem(item);
            menuItem.Text = newName;
        }
        protected void Menu1_MenuItemClick(object sender, MenuEventArgs e)
        {

            if (ClearClick != null && e.Item.Text == "清除")
                ClearClick.Invoke(e.Item, new EventArgs());
            if (EditClick != null && e.Item.Text == "編輯")
                EditClick.Invoke(e.Item, new EventArgs());
            if (ExportClick != null && (e.Item.Text == "XLS" || e.Item.Text == "CSV"))
            {
                ExportClick.Invoke(e.Item, new EventArgs());
            }
            if (ImportClick != null && e.Item.Text == "導入")
                ImportClick.Invoke(e.Item, new EventArgs());
            if (SearchClick != null && e.Item.Text == "查找")
                SearchClick.Invoke(e.Item, new EventArgs());
            if (NewClick != null && e.Item.Text == "新增")
                NewClick.Invoke(e.Item, new EventArgs());
            if (DeleteClick != null && e.Item.Text == "刪除")
                DeleteClick.Invoke(e.Item, new EventArgs());
            if (ApproveClick != null && e.Item.Text == "審核")
                ApproveClick.Invoke(e.Item, new EventArgs());
            if (HelpClick != null && e.Item.Text == "幫助")
                HelpClick.Invoke(e.Item, new EventArgs());

        }

        //public ExportType GetExportType()
        //{
        //    Comfy.UI.Web.ASPxEditors.ASPxComboBox cbo =
        //        mnToolBar.Items.FindByName("ExportType").FindControl("cboExportType")
        //        as Comfy.UI.Web.ASPxEditors.ASPxComboBox;
        //    switch (cbo.Text)
        //    {
        //        case "XLS": return ExportType.XLS;
        //        case "CSV": return ExportType.CSV;

        //        default: return ExportType.XLS;
        //    }
        //}
    }
}