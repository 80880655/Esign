using System;
using System.Collections.Generic;
using Comfy.App.Core.Navigation;
using Comfy.App.Web.Core;
using System.Data;
using System.Text;
using Geekees.Common.Controls;

namespace Comfy.App.Web.AppCore.Navigation
{
    [System.ComponentModel.DataObject]
    public partial class Default : BasePage
    {
        IAppNavItemManager manager = CreateRefObj<IAppNavItemManager>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GenerateTree();
            }
        }

        protected void mainToolBar_OnExportClick(object sender, EventArgs e)
        {
        }

        protected string GetVisible(object visible)
        {
            return ((NavItemVisible)Convert.ToInt32(visible)).ToString();
        }

        public override Module ModuleID
        {
            get { return Module.AppNavigation; }
        }


        protected List<int> GetRoleIkeys(object itemIkey)
        {
            if (itemIkey == null) return null;
            return manager.GetRoleIkeyList(Convert.ToInt32(itemIkey));
        }

        void RemoveCache()
        {
            CacheManager.NavItemCache = null;
        }

        #region DataObjectMethod

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public AppNavItemModelList GetModels(string id, int group, int active)
        {
            if (!CanRead) return null;
            try
            {
                return manager.GetModels(new AppNavItemCriteria() { ItemId = id, GroupIkey = group, Active= active });
            }
            catch (Exception exc)
            {
               // throw GetError(exc);
                throw exc;
            }
        }

        public System.Data.DataSet GetModelsDataSet(string id, int group, int active)
        {
            if(!CanRead) return null;
            try
            {
                return manager.GetModelsDataSet(new AppNavItemCriteria() { ItemId = id, GroupIkey = group, Active = active });
            }
            catch (Exception exc)
            {
               // throw GetError(exc);
               throw exc;
            }
        }

        public string GetNotNullString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return "";
            }
            return s;
        }

        public string GetVisible(int i)
        {
            if (i == 0)
                return "";
            if (i == 1)
                return "Web";
            if (i == 2)
                return "Win";
            if (i == 4)
                return "PDA";
            if (i == 3)
                return "Web,Win";
            if (i == 5)
                return "Web,PDA";
            if (i == 6)
                return "Win,PDA";
            if (i == 7)
                return "Web,Win,PDA";
            return "";

        }
        public string GetModelStr(string ikey)
        {
            StringBuilder resultStr = new StringBuilder("");
            AppNavItemModel model =  manager.GetOneModel(new AppNavItemCriteria() { Ikey= Convert.ToInt32(ikey) });
            if (model != null)
            {
                resultStr.Append(GetNotNullString(model.ItemId) + "<*>");
                resultStr.Append(GetNotNullString(model.Text) + "<*>");
                resultStr.Append(GetNotNullString(model.ToolTip) + "<*>");
                resultStr.Append(GetNotNullString(model.NavigateUrl) + "<*>");
                resultStr.Append(GetNotNullString(model.Target) + "<*>");
                resultStr.Append(GetVisible(model.Visible) + "<*>");
                resultStr.Append(model.VisibleIndex + "<*>");
                resultStr.Append(GetNotNullString(model.ImageUrl) + "<*>");
                resultStr.Append(GetModuleName(model.ModuleId) + "<*>");
                resultStr.Append(GetNotNullString(model.AssemblyName) + "<*>");
                resultStr.Append(GetNotNullString(model.ClassName) + "<*>");
                resultStr.Append((model.Active?"有效":"無效") + "<*>");
                resultStr.Append(GetNotNullString(model.RoleIds) + "<*>");
                resultStr.Append(GetNotNullString(manager.GetUser(model.Creator)) + "<*>");
                resultStr.Append(model.CreateTime==DateTime.MinValue?"0":model.CreateTime.ToString() + "<*>");
                resultStr.Append(GetNotNullString(manager.GetUser(model.Updator)) + "<*>");
                resultStr.Append(model.UpdateTime == DateTime.MinValue ? "0" : model.UpdateTime.ToString());

                return resultStr.ToString();

            }
            return "NoModel";
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        public void AddModel(AppNavItemModel model)
        {
            try
            {
                return;
                model.Creator = AppUser.Ikey;
                manager.AddModel(model);
                RemoveCache();
            }
            catch (Exception exc)
            {
               // throw GetError(exc);
            }
        }

        public void ChangeGroupAndNot(string nodeId,string parentNodeId)
        {
            int node = Convert.ToInt32(nodeId);
            int parentNode;
            if (string.IsNullOrEmpty(parentNodeId) || parentNodeId.ToLower() == "null")
                parentNode = 0;
            else
                parentNode = Convert.ToInt32(parentNodeId);
            manager.ChangeNode(node, parentNode);

        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Update)]
        public void UpdateModel(AppNavItemModel model)
        {
            try
            {
                model.Updator = AppUser.Ikey;
                manager.UpdateModel(model);
            }
            catch (Exception exc)
            {
               // throw GetError(exc);
            }
        }

        public void DeleteModel(string ikey)
        {
            try
            {
                AppNavItemModel model = new AppNavItemModel();
                model.Ikey = Convert.ToInt32(ikey);
                manager.DeleteModel(model);
            }
            catch (Exception exc)
            {
               // throw GetError(exc);
            }
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public AppNavItemModelList GetGroups()
        {
            try
            {
                AppNavItemModelList list = manager.GetGroupList();
                foreach(AppNavItemModel m in list)
                    m.ImageUrl = "~/Images/" + m.ImageUrl;
                return list;
            }
            catch (Exception exc)
            {
                //throw GetError(exc);
                throw exc;
            }
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public List<KeyValuePair<string, string>> GetImages()
        {
            try
            {
                return NavManager.GetImages();
            }
            catch (Exception exc)
            {
               // throw GetError(exc);
                throw exc;
            }
        }

        public string GetModuleName(int ikey)
        {
            Comfy.Data.KeyTextList module = GetModuleId();
            foreach (Comfy.Data.KeyTextPair pair in module)
            {
                if (pair.Key == ikey)
                    return pair.Text;
            }
            return "";
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public Comfy.Data.KeyTextList GetModuleId()
        {
            try
            {
                return NavManager.GetPageID();
            }
            catch (Exception exc)
            {
                //throw GetError(exc);
                throw exc;
            }
        }
        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }
        private void InitializeComponent()
        {
            this.astvMyTree.ContextMenu.MenuItems.Add(new ASContextMenuItem("新增", "AddNav(" + this.astvMyTree.ContextMenuClientID + ".getSelectedItem().parentNode.getAttribute('treeNodeValue')" + ");return false;", "otherevent"));
            this.astvMyTree.ContextMenu.MenuItems.Add(new ASContextMenuItem("編輯", "EditNav(" + this.astvMyTree.ContextMenuClientID + ".getSelectedItem().parentNode.getAttribute('treeNodeValue')" + ");return false;", "otherevent"));
            this.astvMyTree.ContextMenu.MenuItems.Add(new ASContextMenuItem("刪除", "DeleteNav(" + this.astvMyTree.ContextMenuClientID + ".getSelectedItem().parentNode.getAttribute('treeNodeValue')" + ");return false;", "otherevent"));
            this.astvMyTree.ContextMenu.MenuItems.Add(new ASContextMenuItem("新增根節點", "AddRoot();return false;", "otherevent"));
        }

        public void GenerateTree()
        {
            ASTreeViewTheme macOS = new ASTreeViewTheme();
            macOS.BasePath = "../../Plugin/AsTreeView/astreeview/themes/macOS/";
            macOS.CssFile = "macOS.css";
            this.astvMyTree.Theme = macOS;
            this.astvMyTree.EnableTreeLines = false;
            this.astvMyTree.EnableRightToLeftRender = false;

            System.Data.DataSet navList = this.GetModelsDataSet("", 0, -1);


            ASTreeViewDataTableColumnDescriptor descripter = new ASTreeViewDataTableColumnDescriptor("ItemID"
                , "Ikey"
                , "ParentIkey");

            this.astvMyTree.DataSourceDescriptor = descripter;
            this.astvMyTree.DataSource = navList.Tables[0];
            this.astvMyTree.DataBind();

        }

        #endregion
    }
}
