using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Comfy.UI.WebControls.WebGridView
{
   public class GridComboBox : AbsControl
    {
       public GridComboBox(string id, int columnSpan, int intWitch, Boolean isTime, Page page, Field Field, Boolean isSearch)
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
            DropDownList ddl = new DropDownList();
            ddl.ID = GetId(IsSearch);
            if (string.IsNullOrEmpty(Field.DataSourceId))
            {
                if (Field.Items.Count > 0)
                {
                    foreach (Item item in Field.Items)
                    {
                        ddl.Items.Add(new ListItem(item.Text, item.Value));
                    }
                    ddl.Items.Insert(0, new ListItem("", "-1"));
                }
            }
            else
            {
                ddl.DataSource = this.GetObjectByDataSource(Field.DataSourceId);
                ddl.DataTextField = Field.TextField;
                ddl.DataValueField = Field.ValueField;
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("", "-1"));
            }
            if (!string.IsNullOrEmpty(Field.Check)&&!IsSearch)
            {
                ddl.Attributes.Add("check", Field.Check);
                ddl.Attributes.Add("cnname", string.IsNullOrEmpty(Field.Caption) ? Field.FieldName : Field.Caption);
            }
            ddl.Width = (ColumnSpan * 132 + 100 * (ColumnSpan - 1) - IntWitch);
            return ddl;
        }
    }
}
