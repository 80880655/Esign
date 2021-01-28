using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Comfy.App.Core.QualityCode;

namespace Comfy.App.Web.QuailtyCode
{
    public partial class FlagAttribute : System.Web.UI.UserControl
    {
        private List<YarnInfo> listInfo;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HttpContext.Current.Session.Timeout = 30;
            }

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
        //获取页面的值并组合成MODEL
        public CAttribute GetCAttributeValue()
        {
            CAttribute cAttribute = new CAttribute();
            cAttribute.Construction = construction.Text;
            cAttribute.HeavyCollar = heavyCollar.SelectedValue;
            cAttribute.Pttern = cmbPattern.SelectedValue;
            cAttribute.YarnLength = yarnLength.Text;
            cAttribute.Layout = Layout.Text;

            cAttribute.QC_Ref_PPO = txtQC_Ref_PPO.Text;
            cAttribute.QC_Ref_GP = DropQC_Ref_GP.SelectedValue;
            cAttribute.HF_Ref_PPO = txtHF_Ref_PPO.Text;
            cAttribute.HF_Ref_GP = DropHF_Ref_GP.SelectedValue;
            cAttribute.RF_Remart = TxtRemart.Text;

            cAttribute.ListFinishing = new List<string>();
            string sFinishTemp = finishValue.Value;
            if (sFinishTemp.Contains(";"))
            {
                string[] arrS = sFinishTemp.Split(new string[] { ";" }, StringSplitOptions.None);
                for (int i = 0; i < arrS.Length; i++)
                {
                    if (arrS[i] != "" && arrS[i] != null)
                    {
                        cAttribute.ListFinishing.Add(arrS[i]);
                    }
                }
            }
            return cAttribute;
        }
    }
}