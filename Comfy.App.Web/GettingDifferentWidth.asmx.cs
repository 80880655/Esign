using Comfy.App.Core;
using Comfy.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;

namespace Comfy.App.Web
{
    /// <summary>
    /// GettingDifferentWidth 的摘要说明
    /// 20190703 modify by linyob 收件人 修改为从STM 邮件配置读取
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class GettingDifferentWidth : System.Web.Services.WebService
    {


        #region  sqlServer调用定时任务usp_GettingDifferentWidthWebService

        // declare @ServiceUrl as varchar(1000) 
   //DECLARE @UrlAddress varchar(500)
  
 
   //set @UrlAddress = 'http://192.168.7.71/QuailtyCodeUAT/GettingDifferentWidth.asmx/GettingDifferentWidthTimer'
 
   //SET @ServiceUrl=@UrlAddress --如果有参数可以在此处拼入
  
 
   //--访问地址获取结果
   //Declare @Object as Int
   //Declare @ResponseText as Varchar(8000) --必须8000
                  
   //EXEC sp_OACreate 'MSXML2.XMLHTTP', @Object OUT; --创建OLE组件对象
   //Exec sp_OAMethod @Object, 'open', NULL, 'get',@ServiceUrl,'false' --打开链接，注意是get还是post
   //Exec sp_OAMethod @Object, 'send'
   //EXEC sp_OAMethod @Object, 'responseText', @ResponseText OUTPUT --输出参数
     
   //Select @ResponseText      --输出结果
   //Exec sp_OADestroy @Object
   //GO


        #endregion
        /// <summary>
        /// Tech-工艺-Qualitycode针筒门幅不一致报表从KMIS获取QC数据然后和Oracle数据进行对比 条件为：门幅相同，其他任一参数不同
        /// </summary>
        /// <returns></returns>
        [WebMethod (Description = "门幅一致，其他参数不一致")]
        public string GettingDifferentWidthTimer()
        {
            try
            {

                string sql = @"
           select distinct  r.PPO_No,pp.GK_No,pp.Quality_Code,r.Gauge,r.Diameter,r.Total_Needles,convert(int,m.Width)Width,convert(int,m.Max_width) Max_width,pp.Usage,r.Designer
             from ArtDB.dbo.rtRawInfo r with(nolock) 
             LEFT JOIN ArtDB.dbo.rtPdInfo AS rpi WITH (NOLOCK) ON rpi.Raw_NO = r.Raw_NO
              LEFT JOIN PlanningDB.dbo.pcPPOItem AS pp WITH (NOLOCK)  ON pp.GK_NO = rpi.GK_NO AND pp.PPO_NO = rpi.PPO_NO
              LEFT JOIN ArtDB.dbo.rtFabricMixProduct m WITH (NOLOCK)  on pp.GK_No=m.GK_NO
               where 1=1
              AND pp.ApproveTime>=DATEADD(DAY,-3,GETDATE()) 
              and not exists(select 1 from QUALITY_WidthCompare where ppo_no=r.ppo_no)
              and   r.cancel_date is null and r.groups='C' and r.Art_Type<>'M'
               and r.PPO_No like 'PKGK%' AND pp.Quality_Code IS NOT NULL
              
             ";
                CustomSqlSection cuSQL = DataAccess.CreateSqlServerDatabase().CustomSql(sql);
                DataSet ds = cuSQL.ToDataSet();
                DataTable dt = ds.Tables[0];

                List<string> emailList = new List<string>();
                emailList.Add("PPO_NO");
                emailList.Add("quality_code");
                emailList.Add("usage");
                emailList.Add("gk_no");
                emailList.Add("tech_user");
                emailList.Add("tech_Gauge");
                emailList.Add("Quality_Gauge");
                emailList.Add("tech_Diameter");
                emailList.Add("Quality_Diameter");
                emailList.Add("tech_Total_Needles");
                emailList.Add("Quality_Total_Needles");
                emailList.Add("tech_width");
                emailList.Add("Quality_width");
                DataTable dtEmail = new DataTable();

                if (dt.Rows.Count > 0)
                {
                    List<string> qcList = new List<string>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!qcList.Contains(dr["Quality_Code"].ToString()))
                            qcList.Add(dr["Quality_Code"].ToString());
                    }
                    StringBuilder oracle_sql = new StringBuilder();
                    foreach (string qc in qcList)
                    {
                        if (oracle_sql.ToString() == "")
                            oracle_sql.Append(" select '" + qc + "' qc from dual");
                        else
                            oracle_sql.Append(" union all select '" + qc + "' from dual");
                    }

                    string sql_oracle = "select  b.quality_code,gauge,diameter,total_needles,width,max_width from (" + oracle_sql.ToString() + ") a inner join Qcavailablewidth b on a.qc=b.quality_code ";
                    CustomSqlSection css_qcWidth = DataAccess.DefaultDB.CustomSql(sql_oracle);
                    DataSet ds_qcWidth = css_qcWidth.ToDataSet();
                    DataTable dt_qcWidth = ds_qcWidth.Tables[0];




                    foreach (string str in emailList)
                        dtEmail.Columns.Add(str);
                    foreach (DataRow dr in dt.Rows)
                    {
                        DataRow[] drwidth = dt_qcWidth.Select("Quality_Code='" + dr["Quality_code"].ToString() + "' and width='" + dr["width"].ToString() + "' and   max_width='" + dr["max_width"].ToString() + "' and ( gauge<>'" + dr["gauge"].ToString() + "' or   diameter<>'" + dr["diameter"].ToString() + "' or  total_needles<>'" + dr["total_needles"].ToString() + "' ) ");
                        if (drwidth.Length > 0)
                        {
                            foreach (DataRow drInsert in drwidth)
                            {
                                DataRow drEmail = dtEmail.NewRow();


                                drEmail["PPO_NO"] = dr["PPO_NO"].ToString();
                                drEmail["quality_code"] = dr["Quality_code"].ToString();
                                drEmail["usage"] = dr["Usage"].ToString();
                                drEmail["gk_no"] = dr["Gk_no"].ToString();
                                drEmail["tech_user"] = dr["Designer"].ToString();
                                drEmail["tech_Gauge"] = dr["Gauge"].ToString();
                                drEmail["Quality_Gauge"] = drInsert["gauge"].ToString();
                                drEmail["tech_Diameter"] = dr["Diameter"].ToString();
                                drEmail["Quality_Diameter"] = drInsert["diameter"].ToString();
                                drEmail["tech_Total_Needles"] = dr["Total_Needles"].ToString();
                                drEmail["Quality_Total_Needles"] = drInsert["total_needles"].ToString();
                                drEmail["tech_width"] = dr["Width"].ToString() + " " + dr["Max_width"].ToString();
                                drEmail["Quality_width"] = drInsert["width"].ToString() + " " + drInsert["max_width"].ToString();
                                dtEmail.Rows.Add(drEmail);

                            }
                        }
                    }


                    if (dtEmail.Rows.Count == 0)
                        return "FAIL";

                    string senderTo = GetEmailAddress();
                    string title = "针筒门幅不一致报表";
                    string Content ="针筒门幅不一致报表"+"</br>"+ GetMailContent(dtEmail, emailList);
                    string sqlStr = @"INSERT INTO  SEND_EMAIL_TRANS (SEQ_NO,MAIL_FROM,MAIL_TO,MAIL_CC,MAIL_SUBJECT,REPORT_URL,MAIL_CONTENT,MAIL_ATTACH,CREATE_DATE)
                   VALUES (SEQ_SEND_EMAIL_TRANS.NEXTVAL,@SDC,@MT,@BV,@SUB,'',@Content,'',SYSDATE)";
                    CustomSqlSection css = DataAccess.DefaultDB.CustomSql(sqlStr);
                    css.AddInputParameter("SDC", DbType.String, "escmadmin@esquel.com");
                    css.AddInputParameter("BV", DbType.String, "");
                    css.AddInputParameter("MT", DbType.String, senderTo);
                    css.AddInputParameter("SUB", DbType.String, title);
                    css.AddInputParameter("Content", DbType.String, Content);
                    int rt = css.ExecuteNonQuery();

                    if (rt > 0)
                    {
                        StringBuilder sqlInsert = new StringBuilder();
                        string remark = "";
                        foreach (DataRow dr in dtEmail.Rows)
                        {
                            remark = dr["quality_code"] + " " + dr["usage"].ToString() + " " + dr["gk_no"].ToString();
                            sqlInsert.Append("insert into PlanningDB.dbo.QUALITY_WidthCompare(Ppo_No,Remark,Create_date) values('" + dr["PPO_NO"].ToString() + "','" + remark + "',getdate())");
                        }
                        if (sqlInsert.ToString() != "")
                        {
                            CustomSqlSection execSQL = DataAccess.CreateSqlServerDatabase().CustomSql(sqlInsert.ToString());
                           execSQL.ExecuteNonQuery();
                            return "OK";
                        }
                    }


                }

                return "FAIL1";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }


        public string GetEmailAddress()
        {

            string sql = @"
                     select top 1 Mail_Address from fabricstoredb.dbo.stMailList where USER_NAME='GettingDifferentWidth'   ";
            CustomSqlSection cuSQLEmailAddress = DataAccess.CreateSqlServerDatabase().CustomSql(sql);
            object obj = cuSQLEmailAddress.ToScalar();
            if (obj == null)
                return "zhangx@esquel.com;huangyl@esquel.com;zhengca@esquel.com;chenqy@esquel.com;WangTT@esquel.com;ShiYH@esquel.com;TanYueC@esquel.com;huyf@esquel.com;escmadmin@esquel.com;";
            return obj.ToString();

        }


        private string GetMailContent(DataTable dt, List<string> tilte)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<table  border='1' cellspacing='0'><tr>");
            foreach (string str in tilte)
            {
                result.Append("<td>");
                result.Append(str);
                result.Append("</td>");

            }
            result.Append("</tr>");

            foreach (DataRow dr in dt.Rows)
            {
                result.Append("<tr>");

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    result.Append("<td>");
                    result.Append(dr[i].ToString());
                    result.Append("</td>");
                }
                
                result.Append("</tr>");
            }
            result.Append("</table>");
            return result.ToString();

        }

     
    }

}
