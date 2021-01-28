using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Reflection;
using System.Web.Caching;
using System.Text.RegularExpressions;

namespace Comfy.UI.WebControls.WebGridView
{

   [ToolboxItem(false)]
   [TypeConverter(typeof(ExpandableObjectConverter))]
   public class Item
    {
      public string Text { get; set; }
      public string Value { get; set; }
    }
}
