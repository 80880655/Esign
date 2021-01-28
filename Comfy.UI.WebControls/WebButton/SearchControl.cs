using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Comfy.UI.WebControls.WebButton
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:SearchControl runat=server></{0}:SearchControl>")]
    public class SearchControl : CompositeControl
    {
        private Button btnSearch;
        private TextBox tbSearchText;

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            tbSearchText.RenderControl(output);
            btnSearch.RenderControl(output);
        }
        private static readonly object ButtonSearchClickObject = new object();
        public event SearchEventHandler ButtonSearchClick
        {
            add 
            {
                base.Events.AddHandler(ButtonSearchClickObject, value);
            }
            remove
            {
                base.Events.RemoveHandler(ButtonSearchClickObject, value);
            }
        }


        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            btnSearch = new Button();
            btnSearch.ID = "btn";
            btnSearch.Text = "搜索";
            btnSearch.Click += new EventHandler(btnSearch_Click);

            tbSearchText = new TextBox();
            tbSearchText.ID = "tb";
            this.Controls.Add(btnSearch);
            this.Controls.Add(tbSearchText);

        }


        protected virtual void OnButtonSearchClick(SearchEventArgs e)
        { 
           SearchEventHandler ButtonSearchClickHandler = (SearchEventHandler) Events[ButtonSearchClickObject];
           if (ButtonSearchClickHandler != null)
           {
               ButtonSearchClickHandler(this,e);
           }
        }

        void btnSearch_Click(object sender, EventArgs e)
        {
            SearchEventArgs args = new SearchEventArgs();
            args.SearchValue = this.Text;
            OnButtonSearchClick(args);
        }

    }

    public delegate void SearchEventHandler(object sender, SearchEventArgs e);
    public class SearchEventArgs : EventArgs
    {
        public SearchEventArgs()
        { }
        private string strSearchValue;
        public string SearchValue
        {
            get
            {
                return strSearchValue;
            }
            set
            {
                strSearchValue = value;
            }
        }
    }
}
