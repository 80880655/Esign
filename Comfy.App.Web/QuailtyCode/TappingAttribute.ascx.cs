using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Comfy.App.Core.QualityCode;

namespace Comfy.App.Web.QuailtyCode
{
    public partial class TappingAttribute : System.Web.UI.UserControl
    {
        private List<YarnInfo> listInfo;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        //下拉框第一项增加一个空白选项
        protected void cmbDyeMethod_DataBound(object sender, EventArgs e)
        {
            ListItem myListItem = new ListItem();
            myListItem.Text = "";
            myListItem.Value = "";
            DropDownList dt = (DropDownList)sender;
            dt.Items.Insert(0, myListItem);
        }

        public CAttribute GetCAttributeValue()
        {
            CAttribute cAttribute = new CAttribute();
            cAttribute.YarnLength = yarnLength.Text;
            cAttribute.Size = size.Text;
            cAttribute.TappingType = tappingType.SelectedValue;
            cAttribute.Layout = Layout.Text;

            cAttribute.QC_Ref_PPO = txtQC_Ref_PPO.Text;
            cAttribute.QC_Ref_GP = DropQC_Ref_GP.SelectedValue;
            cAttribute.HF_Ref_PPO = txtHF_Ref_PPO.Text;
            cAttribute.HF_Ref_GP = DropHF_Ref_GP.SelectedValue;
            cAttribute.RF_Remart = TxtRemart.Text;


            return cAttribute;
        }
    }
}