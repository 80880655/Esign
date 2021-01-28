using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Comfy.App.Web.QuailtyCode
{
    public partial class GetAjaxData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        //为Attribute页面上面得yran_type去获取相关的数据，然后显示回去
        [WebMethod]
        public static String GetYranContent(string yranType)
        {

            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlServer"].ToString()))
            {
                conn.Open();
                String sql = "select Yarn_Content from [SystemDB].dbo.pbYarnTypeContentList WHERE Yarn_Content!=''";
                if (yranType!="") {
                    sql = sql + " AND Yarn_Type = '" + yranType + "'";
                }
                sql = sql + " Group by Yarn_Content";
                //创建命令对象，指定要执行sql语句与连接对象conn
                SqlCommand cmd = new SqlCommand(sql, conn);
                //执行查询返回结果集
                SqlDataReader sdr = cmd.ExecuteReader();
                //将返回数据生成json
                ReturnJson jsonStr = new ReturnJson();
                return jsonStr.ToJson(sdr);
            }

        }
    }
  
}