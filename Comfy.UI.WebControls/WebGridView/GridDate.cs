using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Comfy.UI.WebControls.WebGridView
{
  public  class GridDate:AbsControl
    {
      public GridDate(string id, int columnSpan, int intWitch, Boolean isTime, Page page, Field Field, Boolean isSearch)
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
            //modify on 2012.8.10 createChildren
           // WebCalendar.WebCalendar wc = new WebCalendar.WebCalendar(this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "Comfy.UI.WebControls.WebCalendar.images.date.gif"));
            WebCalendar.WebCalendar wc = new WebCalendar.WebCalendar();
            if (!string.IsNullOrEmpty(Field.Check)&&!IsSearch)
            {
                wc.Check = Field.Check;
                wc.CnName = Field.Caption;
            }

            if (IsSearch)
            {
                if (!IsTime)
                {
                    wc.Width = (ColumnSpan * 132 + 100 * (ColumnSpan - 1) - IntWitch-2);
                    wc.ID = this.Id + "SearchBegin" + Field.FieldName;
                }
                else
                {
                    wc.Width = (ColumnSpan * 132 + 100 * (ColumnSpan - 1)-2);
                    wc.ID = this.Id + "SearchEnd" + Field.FieldName;
                }
            }
            else
            {

                wc.Width = (ColumnSpan * 132 + 100 * (ColumnSpan - 1) - IntWitch-2);
                wc.ID = this.Id + this.Id + Field.FieldName;
            }
            wc.DateFormat = Field.DateFormat;
            return wc;
        }
    }
}
