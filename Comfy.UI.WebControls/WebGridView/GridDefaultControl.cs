using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Comfy.UI.WebControls.WebGridView
{
    public class GridDefaultControl: AbsControl
    {
        public GridDefaultControl(string id, int columnSpan, int intWitch, Boolean isTime, Page page, Field Field, Boolean isSearch)
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
            TextBox tbox = new TextBox();
            tbox.ID = GetId(IsSearch);
            tbox.Width = (ColumnSpan * 132 + 100 * (ColumnSpan - 1) - IntWitch-3);
            if (!string.IsNullOrEmpty(Field.Check)&&!IsSearch)
            {
                tbox.Attributes.Add("check", Field.Check);
                tbox.Attributes.Add("cnname", string.IsNullOrEmpty(Field.Caption) ? Field.FieldName : Field.Caption);
            }
            return tbox;
        }
    }
}
