using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Comfy.UI.WebControls.WebGridView
{
   public class GridTextArea:AbsControl
    {
       public GridTextArea(string id, int columnSpan, int intWitch, Boolean isTime, Page page, Field Field, Boolean isSearch)
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
            TextBox tArea = new TextBox();
            if (!string.IsNullOrEmpty(Field.Check)&&!IsSearch)
            {
                tArea.Attributes.Add("check", Field.Check);
                tArea.Attributes.Add("cnname", string.IsNullOrEmpty(Field.Caption) ? Field.FieldName : Field.Caption);
            }
            tArea.TextMode = TextBoxMode.MultiLine;
            tArea.Rows = 4;
            tArea.ID = GetId(IsSearch);
            tArea.Width = (ColumnSpan * 132 + 100 * (ColumnSpan - 1));
            return tArea;
        }
    }
}
