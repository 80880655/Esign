using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Comfy.UI.WebControls.WebGridView
{
  public  class GridCheckBoxList : AbsControl
    {
      public GridCheckBoxList(string id, int columnSpan, int intWitch, Boolean isTime, Page page, Field Field, Boolean isSearch)
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
            HtmlGenericControl divCh = new HtmlGenericControl("DIV");
            divCh.Attributes.Add("style", "overflow-x:hidden;overflow-y:auto;width:600px;height:35px;border:1px solid #76AEF0;background:#FFFFFF;");
            if (Field.Items.Count > 0)
            {
                int i = 0;
                foreach (Item item in Field.Items)
                {
                    HtmlGenericControl divChildren = new HtmlGenericControl("DIV");
                    HtmlGenericControl checkBoxList = new HtmlGenericControl("input");
                    checkBoxList.Attributes.Add("type", "checkbox");
                    checkBoxList.Attributes.Add("name", GetId(IsSearch));
                    checkBoxList.Attributes.Add("value", item.Value);
                    if (i == 0)
                    {

                        checkBoxList.ID = GetId(IsSearch);
                        if (!string.IsNullOrEmpty(Field.Check)&&!IsSearch)
                        {
                            checkBoxList.Attributes.Add("check", Field.Check);
                            checkBoxList.Attributes.Add("cnname", string.IsNullOrEmpty(Field.Caption) ? Field.FieldName : Field.Caption);
                        }
                    }
                    else
                    {
                        if (!IsSearch)
                        {
                            checkBoxList.ID = Id + "_" + Field.FieldName + i;
                        }
                        else
                        {
                            checkBoxList.ID = this.Id + "Search_" + Field.FieldName + i;
                        }
                    }
                    i++;
                    Label chLab = new Label();
                    Label labSpace = new Label();
                    labSpace.Text = "&nbsp;&nbsp;&nbsp;&nbsp;";
                    chLab.Text = item.Text;
                    divChildren.Controls.Add(chLab);
                    divChildren.Controls.Add(checkBoxList);
                    divChildren.Controls.Add(labSpace);
                    divCh.Controls.Add(divChildren);
                }
                return divCh;
            }
            else if (!string.IsNullOrEmpty(Field.DataSourceId))
            {
                Dictionary<string, string> dic;
                HtmlGenericControl divChildren = new HtmlGenericControl("DIV");
                object ods = this.GetObjectByDataSource(Field.DataSourceId);
                dic = GetValueByText(ods, Field.TextField, Field.ValueField);
                int i = 0;
                foreach (KeyValuePair<string, string> keyValue in dic)
                {
                    HtmlGenericControl checkBoxList = new HtmlGenericControl("input");
                    checkBoxList.Attributes.Add("type", "checkbox");
                    checkBoxList.Attributes.Add("name", GetId(IsSearch));
                    checkBoxList.Attributes.Add("value", keyValue.Value);
                    if (i == 0)
                    {
                        if (!string.IsNullOrEmpty(Field.Check)&&!IsSearch)
                        {
                            checkBoxList.Attributes.Add("check", Field.Check);
                            checkBoxList.Attributes.Add("cnname", string.IsNullOrEmpty(Field.Caption) ? Field.FieldName : Field.Caption);
                        }
                        checkBoxList.ID = GetId(IsSearch);
                    }
                    else
                    {
                        if (!IsSearch)
                            checkBoxList.ID = Id + "_" + Field.FieldName + i;
                        else
                            checkBoxList.ID = this.Id + "Search_" + Field.FieldName + i;
                    }
                    i++;
                    Label chLab = new Label();
                    chLab.Text = keyValue.Key;
                    divChildren.Controls.Add(chLab);
                    divChildren.Controls.Add(checkBoxList);
                    Label labSpace = new Label();
                    labSpace.Text = "&nbsp;&nbsp;&nbsp;&nbsp;";
                    divChildren.Controls.Add(labSpace);
                    divCh.Controls.Add(divChildren);
                }
                return divCh;

            }
            else
            {
                throw new Exception("CheckBoxList必須有DataList or DataSource");
            }
        }
    }
}
