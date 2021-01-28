using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Comfy.UI.WebControls.WebGridView
{
    public class GridButtonEdit:AbsControl
    {
        public GridButtonEdit(string id, int columnSpan, int intWitch, Boolean isTime, Page page, Field Field,Boolean isSearch)
       {
           this.Id = id;
           this.ColumnSpan = columnSpan;
           this.IntWitch = intWitch;
           this.IsTime = isTime;
           this.Page = page;
           this.Field = Field;
           this.IsSearch = isSearch;

       }
        public override System.Web.UI.Control CreateControl()
        {
            HtmlGenericControl div = new HtmlGenericControl();
            WebButtonEdit.WebButtonEdit web = new WebButtonEdit.WebButtonEdit();
            if (!string.IsNullOrEmpty(Field.Check)&&!IsSearch)
            {
                web.Check = Field.Check;
                web.CnName = Field.Caption;
            }
            web.Width = (ColumnSpan * 132 + 100 * (ColumnSpan - 1) - IntWitch-25);
            web.ID = GetId(IsSearch);
            //modify on 2012.8.10 createChildren
          //  web.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(web.GetType(), "Comfy.UI.WebControls.WebButtonEdit.Image.edtEllipsis.png");
            web.ButtonClick = Field.NavigateUrlFormatString;
            Label lb = new Label();
            lb.Text = "<script type='text/javascript'> var " + web.ID + " = new WebButtonEdit('" + web.ID + "');</script>";
            div.Controls.Add(web);
            div.Controls.Add(lb);
            return div;
        }
    }
}
