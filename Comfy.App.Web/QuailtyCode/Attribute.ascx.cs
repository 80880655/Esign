using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Comfy.App.Core.QualityCode;
using System.Data;


namespace Comfy.App.Web.QuailtyCode
{
    public partial class Attribute : System.Web.UI.UserControl
    {
        private List<YarnInfo> listInfo;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HttpContext.Current.Session.Timeout = 60;
               // cmbDyeMethod.SelectedIndex = cmbDyeMethod.Items.Count;
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

            cAttribute.Construction = txtFabricType.Text;
            cAttribute.AWWeight = Convert.ToInt32(txtAWWidth.Text == "" ? "0" : txtAWWidth.Text);
            cAttribute.BWWeight = Convert.ToInt32(txtBWWidth.Text == "" ? "0" : txtBWWidth.Text);
            cAttribute.DyeMehtod = cmbDyeMethod.SelectedValue;
            cAttribute.GMTWash = txtGMTWash.Text;
            cAttribute.Pttern = cmbPattern.SelectedValue;
            cAttribute.Layout = Layout.Text;
            cAttribute.Shringkage = txtShrinkage.Text + "X" + txtOneShrinkage.Text;
            cAttribute.TestMethod = cmbTextMethod.SelectedValue;

            cAttribute.QC_Ref_PPO=txtQC_Ref_PPO.Text;
            cAttribute.QC_Ref_GP = DropQC_Ref_GP.SelectedValue;
            cAttribute.HF_Ref_PPO=txtHF_Ref_PPO.Text;
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

            //QC记录重复后注释字段赋值          ---修改人：郑舟
            string dye_method=cAttribute.DyeMehtod;
            string yarn_count="";
            string yarn_type="";
            string yarn_ration="";
            int weight_bw=cAttribute.BWWeight;
            int weight_aw = cAttribute.AWWeight;
            string construction=cAttribute.Construction;
            string pattern=cAttribute.Pttern;
            string finishing="";
            string shrinkage=cAttribute.Shringkage;
            string shrinkage_test_method=cAttribute.TestMethod;
            string gmt_washing=cAttribute.GMTWash;
            string laout=cAttribute.Layout;
            //DataTable dt_notIncludePatternAndFinishing=new QcmaininfoManager().GetSameQC(txt

            //ASPxListBox aspBC = (ASPxListBox)dxConstruction.FindControl("constructionLB");
            //for (int i = 0; i < aspBC.Items.Count; i++)
            //{
            //    if (aspBC.Items[i].Selected)
            //    {
            //        cAttribute.Construction = cAttribute.Construction + aspBC.Items[i].Value.ToString() + ",";
            //    }
            //}
             /* for (int i = 0; i < cbFinishing.Items.Count; i++)
                {
                    if (cbFinishing.Items[i].Selected)
                    {
                        cAttribute.ListFinishing.Add(cbFinishing.Items[i].Value);
                    }
                }*/
                return cAttribute;
        }

    }

}