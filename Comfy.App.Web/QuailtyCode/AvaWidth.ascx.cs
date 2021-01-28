using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Comfy.App.Core.QualityCode;

namespace Comfy.App.Web.QuailtyCode
{
    public partial class AvaWidth : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        //获取页面的值并组合成MODEL
        public List<AvaWidthModel> GetAvaWidth()
        {
            List<AvaWidthModel> listWidthModel=new List<AvaWidthModel>();
            if(avaWidthValue.Value!=null)
            {
             string[] strs = avaWidthValue.Value.Split(new string[]{"<>"},StringSplitOptions.None);
             for (int i = 0; i < strs.Length - 1; i++)
             {
                 AvaWidthModel aVaWidthModel = new AvaWidthModel();
                 string[] strss = strs[i].Split(new string[] { ";" }, StringSplitOptions.None);
                 aVaWidthModel.Gauge = Convert.ToInt32(strss[0] == "" ? "0" : strss[0]);
                 aVaWidthModel.Diameter = Convert.ToInt32(strss[1] == "" ? "0" : strss[1]);
                 aVaWidthModel.TotalNeedles = Convert.ToInt32(strss[2] == "" ? "0" : strss[2]);
                 aVaWidthModel.Width = Convert.ToInt32(strss[3] == "" ? "0" : strss[3]);
                 aVaWidthModel.MaxWidth = Convert.ToInt32(strss[4] == "" ? "0" : strss[4]);
                 listWidthModel.Add(aVaWidthModel);
             }
            }
            return listWidthModel;
        }
    }
}