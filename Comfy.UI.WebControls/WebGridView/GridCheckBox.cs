using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Comfy.UI.WebControls.WebGridView
{
   public class GridCheckBox : AbsControl
    {
       public GridCheckBox(string id, int columnSpan, int intWitch, Boolean isTime, Page page, Field Field, Boolean isSearch)
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
           CheckBox checkBox = new CheckBox();
           checkBox.ID = GetId(IsSearch);
           if (!string.IsNullOrEmpty(Field.Check)&&!IsSearch)
           {
               checkBox.Attributes.Add("check", Field.Check);
           }
           checkBox.Attributes.Add("cnname", string.IsNullOrEmpty(Field.Caption) ? Field.FieldName : Field.Caption);
           return checkBox;
       }
    }
}
