using Comfy.App.Core;
using Comfy.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Comfy.App.Web
{
    /// <summary>
    /// GetShrinkageRisk 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class GetShrinkageRisk : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetRishByQC(string qc)
        {
            try
            {
                if (qc == "")
                    return "";
                string[] array = qc.Split(',');

                string val = string.Empty;
                foreach (string str in array)
                {
                    if (str != "")
                        val += ",'" + str + "'";
                }
                if (qc == "")
                    return "";
                qc = val.Substring(1);
                if (qc == "")
                    return "";
                CustomSqlSection css = DataAccess.DefaultDB.CustomSql(" select nvl(riskrade,'')  from QCMAININFO where quality_code in(" + qc + ")  ");
                DataSet ds = css.ToDataSet();
                DataTable dt = ds.Tables[0];

                List<string> listGrade = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    qc = "," + dr[0].ToString();

                    if (dr[0].ToString() == "")
                        listGrade.Add("@");
                    else
                        listGrade.Add(dr[0].ToString());
                }
                var nameIndex = 0;
                string grade = "";
                for (int j = 0; j < listGrade.Count; j++)
                {
                    try
                    {

                        char c = Convert.ToChar(listGrade[j]);
                        char d = Convert.ToChar(listGrade[nameIndex]);
                        if ((int)c > (int)d)
                        {
                            nameIndex = j;
                        }
                    }
                    catch
                    {


                    }

                }
                grade = listGrade[nameIndex];
                if (grade == "@")
                    grade = "";

                return grade;
            }
            catch
            {
                return "";
            }
          
        }
    }
}
