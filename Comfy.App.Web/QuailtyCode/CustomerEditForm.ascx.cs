using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Comfy.App.Core.QualityCode;

namespace Comfy.App.Web.QuailtyCode
{
    public partial class CustomerEditForm : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public QccustomerlibraryModelList GetCustomer(string QualityCode)
        {
            QccustomerlibraryManager manager = new QccustomerlibraryManager();
            return  manager.GetModelListOne(QualityCode);
        }

        public void UpdateCustomer(QccustomerlibraryModel model)
        {
            QccustomerlibraryManager manager = new QccustomerlibraryManager();
            manager.UpdateModel(model,null);
        }


    }
}