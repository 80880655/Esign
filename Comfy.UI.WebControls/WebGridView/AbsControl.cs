using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

namespace Comfy.UI.WebControls.WebGridView
{
 public  abstract class AbsControl
    {
       protected string Id;
       protected int ColumnSpan;
       protected int IntWitch;
       protected Boolean IsTime;
       protected Page Page;
       protected Field Field;
       protected Boolean IsSearch;
       public abstract Control CreateControl();

       private Control FindControlExtend(string id, ControlCollection controls)
       {
           int i;
           Control Found = null;
           for (i = 0; i < controls.Count; i++)
           {
               if (controls[i].ID == id)
               {
                   Found = controls[i];
                   break;
               }

               if (controls[i].Controls.Count > 0)
               {
                   Found = FindControlExtend(id, controls[i].Controls);
                   if (Found != null) break;
               }
           }
           return Found;
       }

       protected object GetObjectByDataSource(string dataSourceId)
       {
           if (HttpContext.Current.Session[dataSourceId + "combo" + GetFileName()] == null)
           {
               object s = this.GetControl(dataSourceId) as ObjectDataSource;
               if (s == null)
               {
                   s = this.GetControl(dataSourceId) as SqlDataSource;
                   SqlDataSource sqlD = (SqlDataSource)s;
                   DataView dv = (DataView)sqlD.Select(DataSourceSelectArguments.Empty);
                   HttpContext.Current.Session[dataSourceId + "combo" + GetFileName()] = dv;
               }
               else
               {
                   ObjectDataSource objD = (ObjectDataSource)s;
                   HttpContext.Current.Session[dataSourceId + "combo" + GetFileName()] = objD.Select();
               }
           }
           return HttpContext.Current.Session[dataSourceId + "combo" + GetFileName()];
       }
       protected Control GetControl(string Id)
       {
           Control result = Page.FindControl(Id);
           if (result != null)
               return result;
           else
           {
             result =  FindControlExtend(Id, Page.Controls);
           }
           if (result != null)
           {
               return result;
           }
           return null;
       }
       protected string GetFileName()
       {
           string s = HttpContext.Current.Request.Path;
           s = s.Replace("/", "").Replace(".", "").Replace("aspx", "");
           return s;

       }

       protected string GetId(Boolean isSearch)
       {
           if (isSearch)
           {
               return this.Id + "Search" + Field.FieldName;
           }
           else
           {
               return this.Id + this.Id + Field.FieldName;
           }
       }

       protected Dictionary<string, string> GetValueByText(object ods, string textField, string valueField)
       {

           Dictionary<string, string> dic = new Dictionary<string, string>();
           if (ods == null)
               return dic;
           DataView dv = ods as DataView;
           if (dv != null)
           {
               DataTable table = dv.Table;
               DataRowCollection drc = table.Rows;
               foreach (DataRow r in drc)
               {
                   dic.Add(r[valueField].ToString(), r[textField].ToString());
               }
               return dic;
           }
           ICollection collection = ods as ICollection;
           foreach (object obj in collection)
           {
               object textFieldObj = obj.GetType().GetProperty(textField).GetValue(obj, null);
               object valueFieldObj = obj.GetType().GetProperty(valueField).GetValue(obj, null);
               string strTextField = textFieldObj == null ? null : textFieldObj.ToString();
               string strValueField = valueFieldObj == null ? null : valueFieldObj.ToString();
               if (!string.IsNullOrEmpty(strTextField))
               {
                   dic.Add(strValueField, strTextField);
               }
           }
           return dic;
       }
    }
}
