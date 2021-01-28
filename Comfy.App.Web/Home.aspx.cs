using System;

namespace Comfy.App.Web
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected bool IsImageVisible(object visible)
        {
            if (visible != null)
                return bool.Parse(visible.ToString());
            return true;
        }
    }
}
