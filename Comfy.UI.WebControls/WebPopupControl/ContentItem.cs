using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;

namespace Comfy.UI.WebControls.WebPopupControl
{
    [ToolboxItem(false)]
    public class ContentItem: CompositeControl
    {
        protected override void OnDataBinding(EventArgs e)
        {
            this.EnsureChildControls();//确定是否包含子控件,否则创建
            base.OnDataBinding(e);
        }

    }
}
