using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using System.Web.SessionState;
using Comfy.Data;
using System.Data.Common;

namespace Comfy.App.Core.QualityCode
{
    public class CustomerManager
    {
        QcmaininfoManager qm = new QcmaininfoManager();
        public string GetCustomerInfo(string name)
        {
            string strRet = "";
            CustomSqlSection css;
            DataSet ds;
            DataRow[] dr = null;
            if (HttpContext.Current.Cache["CustomerInfo"] == null)
            {
                using (ds = DataAccess.DefaultDB.CustomSql("select CUSTOMER_CD||'<|>'||NAME as CU,CUSTOMER_CD,NAME from gen_customer").ToDataSet()) { };
                HttpContext.Current.Cache["CustomerInfo"] = ds;
            }
            else
            {
                ds = (DataSet)HttpContext.Current.Cache["CustomerInfo"];
            }
            /*   if (name != "null")
               {
                   css = DataAccess.DefaultDB.CustomSql("select CUSTOMER_CD||'<|>'||NAME as CU from gen_customer where (CUSTOMER_CD like (UPPER(@Name)||'%') or NAME like (UPPER(@Name)||'%') ) and rownum<51 order by NAME asc");
                   css.AddInputParameter("Name", DbType.String, name);
                   ds = css.ToDataSet();
               }
               else
               {
                    ds = DataAccess.DefaultDB.CustomSql("select CUSTOMER_CD||'<|>'||NAME as CU from gen_customer where rownum<51").ToDataSet();
               }*/
            DataTable dt = ds.Tables[0];
            if (name != "null")
            {
                dr = dt.Select("CUSTOMER_CD like '" + name + "%' or NAME like '" + name + "%'");
            }
            else
            {
                dr = dt.Select();
            }
            for (int i = 0; i < (dr.Length > 51 ? 51 : dr.Length); i++)
            {
                strRet = strRet + dr[i][0].ToString() + "<?>";
            }
            return strRet;
        }


           public string GetCustomer(string QC)
            {

                DataSet ds;
                string sql = "select Buyer_ID,count(*) as cou FROM QCCustomerLibrary where Quality_Code=@QC group by Buyer_ID";
                CustomSqlSection css = DataAccess.DefaultDB.CustomSql(sql);
                css.AddInputParameter("QC", DbType.String, QC);
                string QCAndCount="";
                using( ds = css.ToDataSet())
                {
                    DataTable dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        QCAndCount = dt.Rows[i]["Buyer_ID"].ToString() + ";" + dt.Rows[i]["cou"].ToString();

                    }
                }
                return QCAndCount;
            }

           public string GetCustomerName(string QC)
           {

               DataSet ds;
               string sql = "select A.QUALITY_CODE,B.CUSTOMER_CD,B.NAME from QCCustomerLibrary a left join gen_customer b on A.BUYER_ID=B.CUSTOMER_CD where a.quality_code=@QC";
               CustomSqlSection css = DataAccess.DefaultDB.CustomSql(sql);
               css.AddInputParameter("QC", DbType.String, QC);
               using (ds = css.ToDataSet())
               {
                   DataTable dt = ds.Tables[0];
                   if (dt.Rows.Count > 0)
                   {
                       return dt.Rows[0]["NAME"].ToString();
                   }
                   else
                   {
                       return string.Empty;
                   }
               }
           }
            /// <summary>
            /// 通过 Quality_Code 与PPO_NO 获取GK_NO       by mengjw  K15076 功能项3   最近需要改动的地方
            /// </summary>
            /// <param name="strQuality_Code"></param>
            /// <param name="strPPO_NO"></param>
            /// <returns></returns>
           public string GetGK_NO(string strQuality_Code,string strPPO_NO)
           {
               string strResult = "";
               DataSet ds;
               string sqlOracle = "  select * from QCMainInfo where quality_code='"+strQuality_Code+"'";
               CustomSqlSection cssOracle = DataAccess.CreateDatabase().CustomSql(sqlOracle);
               using (ds = cssOracle.ToDataSet())
               {
                   DataTable dt = ds.Tables[0];
                   if (dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0]["GK_NO"].ToString().Trim()))
                   {
                       strResult = dt.Rows[0]["GK_NO"].ToString();
                       return strResult;
                   }
                
               }

               string sql = "  select top 1 GK_NO " +
                 "  from PlanningDB.dbo.pcPPOItem a with(NOLOCK)  " +
                "  where a.PPO_No='" + strPPO_NO + "' and a.Quality_Code='" + strQuality_Code + "' and a.Combo_ID>0  and EXISTS (select 1 from PlanningDB.dbo.pcPPOLot WITH(NOLOCK) WHERE pcPPOLot.PPO_Item_ID=a.PPO_Item_ID and Lot_Status<>'CANCEL') ";  

               CustomSqlSection css = DataAccess.CreateSqlServerDatabase().CustomSql(sql);
               //css.AddInputParameter("QC", DbType.String, QC);
             
               using (ds = css.ToDataSet())
               {
                   DataTable dt = ds.Tables[0];
                   for (int i = 0; i < dt.Rows.Count; i++)
                   {
                       strResult = dt.Rows[i]["GK_NO"].ToString();

                   }
               }
               return strResult;
           }
           /// <summary>
           /// 通过 PPO_NO 获取fabric_type_cd   
           /// </summary>
           /// <param name="strQuality_Code"></param>
           /// <param name="strPPO_NO"></param>
           /// <returns></returns>
           public string GetFabric_type_cd(string strppo_no)
           {
               string strResult = "";
               //kingzhang for support 742387 增加内部试样单的GP数据获取
                   DataSet ds;
                   string sql = " SELECT DISTINCT usage from planningdb..pcppoitem where PPO_No ='" + strppo_no + "'";                 
                   CustomSqlSection css = DataAccess.CreateSqlServerDatabase().CustomSql(sql);
                   //css.AddInputParameter("QC", DbType.String, QC);  
                   using (ds = css.ToDataSet())
                   {
                       DataTable dt = ds.Tables[0];
                       for (int i = 0; i < dt.Rows.Count; i++)
                       {
                           strResult += "|" + dt.Rows[i]["usage"].ToString();

                       }
                   }

                   //DataSet ds;
                   //string sql = "  select fabric_type_cd from ppo_item where ppo_no='" + strppo_no + "'  and status<>'C'  ";
               
                   //CustomSqlSection css = DataAccess.DefaultDB.CustomSql(sql);
                   ////css.AddInputParameter("QC", DbType.String, QC);  
                   //using (ds = css.ToDataSet())
                   //{
                   //    DataTable dt = ds.Tables[0];
                   //    for (int i = 0; i < dt.Rows.Count; i++)
                   //    {
                   //        strResult +="|"+ dt.Rows[i]["fabric_type_cd"].ToString();

                   //    }
                   //}
               return strResult;
           }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strppo_no"></param>
        /// <returns>0 为通过 ，1为不通过</returns>
           public int IsEffective(string strppo_no)
           {
               //kingzhang for support 742387 更改GP数据获取
               int iResult = 1;
                   DataSet ds;
                   string sql = " SELECT  * from planningdb..pcppoitem where PPO_No ='" + strppo_no + "'";

                   CustomSqlSection css = DataAccess.CreateSqlServerDatabase().CustomSql(sql);
                   using (ds = css.ToDataSet())
                   {
                       DataTable dt = ds.Tables[0];
                       if (dt.Rows.Count > 0)
                       { iResult = 0; }
                       else
                       { iResult = 1; }
                   }
     
                   //DataSet ds;
                   //string sql = "  select * from PPO_HD where Status <>'c' and PPO_NO='" + strppo_no + "' ";

                   //CustomSqlSection css = DataAccess.DefaultDB.CustomSql(sql);
                   ////css.AddInputParameter("QC", DbType.String, QC);
                  
                   //using (ds = css.ToDataSet())
                   //{
                   //    DataTable dt = ds.Tables[0];
                   //    if (dt.Rows.Count > 0)
                   //    { iResult = 0; }
                   //    else
                   //    { iResult = 1; }

                   //}
               return iResult;
           }
        

            //QC用在SUBMIT的大货或者样板PPO里时，不能修改QC里的资料及不能UPAPPROVE 该QC
            /// <summary>
            /// 判断是否可以UPAPPROVE
            /// </summary>
            ///  /// <param name="strQuality_Code"></param>
            /// <returns></returns>
           public bool ISUnApprove(string strQuality_Code, out string qcStr)
           {

               DataSet ds;
               string sql = "  SELECT DISTINCT  PH.PPO_NO,PH.ORDER_TYPE,PI.QUALITY_CODE FROM PPO_HD PH " +
               "  INNER JOIN PPO_ITEM PI ON PH.PPO_NO=PI.PPO_NO  " +
               "   WHERE PH.NEW_KPPO_FLAG ='Y' " +
               "  AND PH.STATUS<>'C' " +
               "  AND PI.STATUS<>'C' " +
               "  and  PI.QUALITY_CODE='" + strQuality_Code + "' " +
               "  AND PH.Status in ('R','L2') AND PH.ORDER_TYPE='KNBU' ";

               CustomSqlSection css = DataAccess.DefaultDB.CustomSql(sql);
               bool bResult = false;
               using (ds = css.ToDataSet())
               {
                   DataTable dt = ds.Tables[0];
                   StringBuilder sb = new StringBuilder();
                   if (dt.Rows.Count > 0)
                   {
                       for (int i = 0; i < dt.Rows.Count; i++)
                       {
                           sb.Append(dt.Rows[i]["PPO_NO"] + "\n");
                       }
                       bResult = false;
                   }
                   else
                   {
                       bResult = true;
                   }
                   qcStr = sb.ToString();

               }
               return bResult;
           }


        public void GetUserPower(string userId)
        {
         /*   string sql = "select Description1   FROM ums_functions f, ums_role_functions rf, ums_role_users ru " +
              "WHERE f.function_id = rf.function_id AND rf.role_id = ru.role_id AND f.module_id = 49020 and user_Id=@UserId ";
            CustomSqlSection css = DataAccess.DefaultDB.CustomSql(sql);
            css.AddInputParameter("UserId", DbType.String, userId);
            List<string> listCount = new List<string>();
            using (DataSet ds = css.ToDataSet())
            {
                DataTable dt = ds.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Description1"].ToString() == "CreateQualityCode" && !listCount.Contains("1"))
                    {
                        listCount.Add("1");
                    }
                    if (dt.Rows[i]["Description1"].ToString() == "ApprovalQualityCode" && !listCount.Contains("2"))
                    {
                        listCount.Add("2");
                    }
                    if (dt.Rows[i]["Description1"].ToString() == "MaintainQualityCode" && !listCount.Contains("3"))
                    {
                        listCount.Add("3");
                    }
                    if (dt.Rows[i]["Description1"].ToString() == "SearchQualityCode" && !listCount.Contains("4"))
                    {
                        listCount.Add("4");
                    }
                }
            }*/
            List<string> listCount = new List<string>();
            listCount.Add("1");
            listCount.Add("2");
            listCount.Add("3");
            listCount.Add("4");
            HttpContext.Current.Session["UserPower"] = listCount;
        }
        /// <summary>
        /// 返回userId查出的userId,Department_id,name
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserInfoByUserId(object userId)
        {
            string strRet = "";
            DataSet ds;
            CustomSqlSection css;
            if (userId == null || userId.ToString() == "null")
            {
                userId = HttpContext.Current.Session["userId"];
            }
            if (userId != null)
            {
                css = DataAccess.DefaultDB.CustomSql("select USER_ID||'<|>'||DEPARTMENT_ID||'<|>'||Name as CU from GEN_USERS where USER_ID = @userId");
                css.AddInputParameter("userId", DbType.String, userId.ToString());
                ds = css.ToDataSet();
            }
            else
            {
                return "";
            }
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                strRet = dt.Rows[0][0].ToString();
            }

            return strRet;
        }
        public string GetConstruction(string s)
        {
            DataSet ds;
            string strRet = "";
            string[] stemps;
            CustomSqlSection css;
            if (s.Contains(","))
            {
                stemps = s.Split(new string[] { "," }, StringSplitOptions.None);
                //  stemp = "(Construction='" + stemps[0] + "' or Construction='" + stemps[1] + "')";
                css = DataAccess.DefaultDB.CustomSql("select Description as CU from pbKnitConstruction where IS_Active='Y' and (Construction=@con1 or Construction=@con2 )");
                css.AddInputParameter("con1", DbType.String, stemps[0]);
                css.AddInputParameter("con2", DbType.String, stemps[1]);
            }
            else
            {
                css = DataAccess.DefaultDB.CustomSql("select Description as CU from pbKnitConstruction where IS_Active='Y' and Construction=@con1");
                css.AddInputParameter("con1", DbType.String, s);
                //  stemp = "(Construction='" + s + "')";
            }
            //  css = DataAccess.DefaultDB.CustomSql("select Description as CU from pbKnitConstruction where IS_Active='Y' and @Construction");
            // css.AddInputParameter("Construction", DbType.String, stemp);
            ds = css.ToDataSet();
            DataTable dt = ds.Tables[0];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strRet = strRet + dt.Rows[i][0].ToString() + "<?>";
            }
            return strRet;
        }

        /// <summary>
        /// 修改查询布料为全部匹配方式    by LYH 2014/2/18
        /// 获取布料，各布料以<|>衔接其名称和描述，以<?>衔接不同布料
        /// </summary>
        /// <param name="name">页面用户输入的筛选字符串</param>
        /// <returns></returns>
        public string GetConstructionInfo(string name)
        {
            string strRet = "";
            DataSet ds;
            DataRow[] dr;
            if (HttpContext.Current.Cache["ConstructionInfo"] == null)
            {
                using (ds = DataAccess.DefaultDB.CustomSql("select Construction||'<|>'||Description as CU,Description from pbKnitConstruction where IS_Active='Y' order by Description asc").ToDataSet()) { }
                HttpContext.Current.Cache["ConstructionInfo"] = ds;
            }
            else
            {
                ds = (DataSet)HttpContext.Current.Cache["ConstructionInfo"];
            }
            DataTable dt = ds.Tables[0];
            if (name != "null")
            {
                dr = dt.Select("Description like '%" + name + "%'");

            }
            else
            {
                dr = dt.Select();
            }
            for (int i = 0; i < (dr.Length > 51 ? 51 : dr.Length); i++)
            {
                strRet = strRet + dr[i][0].ToString() + "<?>";
            }
            return strRet;
        }
        /// <summary>
        /// 获取Finish的信息


        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetFinishInfo(string name)
        {
            string strRet = "";
            DataSet ds;
            //  CustomSqlSection css;
            if (HttpContext.Current.Cache["FinishInfo"] == null)
            {
                using (ds = DataAccess.DefaultDB.CustomSql("select Finishing_Code||'<|>'||Finishing_Name as CU,Finishing_Name from pbKnitFinish where Washing_Flag='N' and IS_Active='Y' order by Description asc").ToDataSet()) { }
                HttpContext.Current.Cache["FinishInfo"] = ds;
            }
            else
            {
                ds = (DataSet)HttpContext.Current.Cache["FinishInfo"];
            }
            DataTable dt = ds.Tables[0];
            DataRow[] dr = null;
            if (name != "null")
            {
                dr = dt.Select("Finishing_Name like '%" + name + "%'");
            }
            else
            {
                dr = dt.Select();
            }
            for (int i = 0; i < dr.Length; i++)
            {
                strRet = strRet + dr[i][0].ToString() + "<?>";
            }
            return strRet;
        }

        /// <summary>
        /// 根据组别找到它下面的用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetUserIdByDepartment(string name)
        {
            string strRet = "";
            DataSet ds;
            CustomSqlSection css;
            if (name != "null")
            {
                css = DataAccess.DefaultDB.CustomSql("select USER_ID||'<|>'||Name as CU from GEN_USERS where Active='Y' and DEPARTMENT_ID = @Name order by Name asc");
                css.AddInputParameter("Name", DbType.String, name);
                ds = css.ToDataSet();
            }
            else
            {
                return "";
            }
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strRet = strRet + dt.Rows[i][0].ToString() + "<?>";
            }
            return strRet;
        }


        public string UserCanEdit(string salesTeam)
        {
            DataSet ds = null;
            CustomSqlSection css;
            string userId;
            userId = HttpContext.Current.Session["userId"].ToString();

            if (userId != null)
            {
                css = DataAccess.DefaultDB.CustomSql("select DEPARTMENT_ID from GEN_USERS where USER_ID = @userId");
                css.AddInputParameter("userId", DbType.String, userId.ToString());
                ds = css.ToDataSet();
            }
            else
            {
                return "0";
            }
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString() == "GEK" || dt.Rows[i][0].ToString() == salesTeam)
                {
                    return "1";
                }
            }
            return "0";
        }


        /*  public void UpdateViewFlag(string yOrN,string ppono,string qc,string ftc,string cc ,DbTransaction tran)
          {

              string sql = "update ppo_qccomment a  set  view_flag=@vf where " +
                    "exists( " +
                    "select 1   FROM ppo_hd ph,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_item_combo pic " +
                    "WHERE  ph.ppo_no=pi.ppo_no and qc.ppo_item_id=pi.ppo_item_id and pi.ppo_item_id=pic.ppo_item_id  and " +
                    "pic.fab_combo_id=fc.fab_combo_id and qc.ppo_qc_id=a.ppo_qc_id and PI.PPO_NO=@ppono and PI.QUALITY_CODE=@qc " +
                    "and PI.FABRIC_TYPE_CD=@ftc and FC.COMBO_CODE=@cc)";
              CustomSqlSection css = DataAccess.DefaultDB.CustomSql(sql);
              if (tran != null)
              {
                  css.SetTransaction(tran);
              }
              css.AddInputParameter("vf",DbType.String,yOrN);
              css.AddInputParameter("ppono", DbType.String, ppono);
              css.AddInputParameter("qc", DbType.String, qc);
              css.AddInputParameter("ftc", DbType.String, ftc);
              css.AddInputParameter("cc", DbType.String, cc);
              css.ExecuteNonQuery();

          }*/
        public void UpdateViewFlag(string yOrN, string ppono, string qc, string ftc, string cc, string Iden, DbTransaction tran)
        {
            /*
            string sql = @"update ppo_qccomment a  set  a.view_flag=@vf where Iden in  (SELECT qcc.Iden 
               FROM  ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic 
               WHERE fc.fab_combo_id = qcc.fab_combo_id and pi.ppo_item_id=pic.ppo_item_id  and
              pic.fab_combo_id=fc.fab_combo_id and qcc.view_flag='N' and pi.quality_code=@QC and  pi.ppo_NO in (
                SELECT  pi.ppo_no
               FROM  ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic 
               WHERE fc.fab_combo_id = qcc.fab_combo_id and pi.ppo_item_id=pic.ppo_item_id  and
               pic.fab_combo_id=fc.fab_combo_id and qcc.view_flag='N' and qcc.iden=@Iden
              )
              and  pi.fabric_type_cd in (
                  SELECT  pi.fabric_type_cd
               FROM  ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic 
                 WHERE fc.fab_combo_id = qcc.fab_combo_id and pi.ppo_item_id=pic.ppo_item_id  and
                 pic.fab_combo_id=fc.fab_combo_id and qcc.view_flag='N' and qcc.iden=@IdenT
              ))";*/
            string sql = @"update ppo_qccomment a  set  a.view_flag=@vf where Iden in  (
              SELECT  qcc.Iden 
              FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc,
               ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm 
              WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and
               qc.ppo_item_id=pi.ppo_item_id and pi.ppo_item_id=pic.ppo_item_id and 
               pi.quality_code=qm.quality_code  and    pic.fab_combo_id=fc.fab_combo_id and 
               qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd and 
               qcc.view_flag='N' and pi.Quality_Code=@QC       
                  )";//and ph.ppo_no=@PPO
            CustomSqlSection css = DataAccess.DefaultDB.CustomSql(sql);
            if (tran != null)
            {
                css.SetTransaction(tran);
            }
            css.AddInputParameter("vf", DbType.String, yOrN);
            css.AddInputParameter("QC", DbType.String, qc);
          //  css.AddInputParameter("PPO", DbType.String, ppono);
          //  css.AddInputParameter("IdenT", DbType.Int32, Iden);
            css.ExecuteNonQuery();

        }


        public string HasPPO(string QC)
        {
            String sql = "select count(*) " +
              " FROM PPO_HD a INNER JOIN PPO_Item b ON a.PPO_NO = b.PPO_NO " +
              " WHERE b.QUALITY_CODE = @QC AND a.status = 'L2' AND b.status <> 'C'";
            CustomSqlSection css;
            css = DataAccess.DefaultDB.CustomSql(sql);
            css.AddInputParameter("QC", DbType.String, QC);
            if (css.ToScalar<Int32>() > 0)
            {
                return "1";
            }
            return "0";
        }

        /// <summary>
        /// 判断QualityCode是否能被UnApprove
        /// </summary>
        /// <param name="QC"></param>
        /// <returns></returns>
        public bool CanUnApprove(string QC, string reason, string creator, DbTransaction tran)
        {
            List<FabricCodeModel> listFabric = GetFabricCodeListOne(QC);

            string PPOS="";
            for (int i = 0; i < listFabric.Count; i++)
            {
                if (!listFabric[i].PPONO.Contains("KSF") && !listFabric[i].PPONO.Contains("GK"))  //去除样板单和FE单

                {
                    PPOS = PPOS + listFabric[i].PPONO + ",";
                }
            }

            //判断是否有排单

            if (PPOS != "")
            {
                string sql = " select count(*) from PlanningDB.dbo.pcPPOItem a with(nolock) inner join PlanningDB.dbo.pcPPoLot b with(nolock) " +
                      " on a.PPO_Item_ID=b.PPO_Item_ID where a.PPO_No in (select  No from SystemDB.dbo.UDF_ConvertStrToTable(@PPOS)) and b.Job_No is not null and a.Combo_ID>0 and b.Lot_Status<>'CANCEL'";

                CustomSqlSection css;
                css = DataAccess.CreateSqlServerDatabase().CustomSql(sql);
                css.AddInputParameter("PPOS", DbType.String, PPOS);
                int count = css.ToScalar<Int32>();
                if (count != 0)
                {
                    return false;
                }

            }
            SendEmain(listFabric, creator, QC, reason,tran,"unapproved");
            //写日志

            QcmaininfologManager logmanager = new QcmaininfologManager();
            QcmaininfologModel logModel = new QcmaininfologModel();
            logModel.QualityCode = QC;
            logModel.EditReason = reason;
            logModel.EditorContent = "UnApprove QualityCode";
            logModel.EditorTime = System.DateTime.Now;
            logModel.Editor = HttpContext.Current.Session["userId"].ToString();
            logModel.Status = "Approved";
            logmanager.AddModel(logModel,tran);

            return true;


        }


        public void ReApprove(string QC, string creator, DbTransaction tran)
        {

            string sql = "select count(*)  from QCMainInfoLog where Editor_Content='UnApprove QualityCode' and Quality_Code=@QC";
            CustomSqlSection css;
            css = DataAccess.DefaultDB.CustomSql(sql);
            css.AddInputParameter("QC", DbType.String, QC);
            int count = css.ToScalar<Int32>();
            if (count == 0)
            {
                return;
            }

            List<FabricCodeModel> listFabric = GetFabricCodeListOne(QC);
            SendEmain(listFabric, creator, QC, "", tran, "re-approved");
            //写日志

            QcmaininfologManager logmanager = new QcmaininfologManager();
            QcmaininfologModel logModel = new QcmaininfologModel();
            logModel.QualityCode = QC;
            logModel.EditorContent = "ReApprove QualityCode";
            logModel.EditorTime = System.DateTime.Now;
            logModel.Editor = HttpContext.Current.Session["userId"].ToString();
            logModel.Status = "New";
            logmanager.AddModel(logModel, tran);      
        }

        public List<FabricCodeModel> GetFabricCodeListOne(string QC)
        {
            List<FabricCodeModel> modellist = new List<FabricCodeModel>();
            FabricCodeModel model = new FabricCodeModel();
          /*  String sql = "select distinct d.Fabric_Code,h.NAME as CREATE_USER_ID,h.EMAIL,f.APPROVE_DATE,a.CREATE_USER_GRP_ID,a.PPO_NO,g.NAME as Customer_CD,B.QUALITY_CODE,B.FABRIC_TYPE_CD,d.Combo_Name,F.Status,F.CUSTOMER_COMMENT,F.VIEW_FLAG " +
                        "from PPO_HD a " +
                        "inner join PPO_Item b on a.PPO_NO=b.PPO_NO " +
                        "inner join PPO_Item_Combo c on b.PPO_Item_ID=c.PPO_Item_ID " +
                        "inner join Fab_Combo d on c.Fab_Combo_ID=d.Fab_Combo_ID " +
                        "inner join PPO_QCMainInfo e on b.PPO_Item_ID=e.PPO_Item_ID " +
                        "inner join gen_customer g on g.customer_cd=a.customer_cd " +
                        "left join GEN_USERS h on h.User_ID=a.CREATE_USER_ID " +
                        "left join PPO_QCComment f on e.PPO_QC_ID=F.PPO_QC_ID " +
                        "where b.QUALITY_CODE=@QC";*/
            string sql = @"SELECT ph.customer_cd,h.Name as CREATE_USER_ID,FC.FABRIC_CODE,
                          h.EMAIL,QCC.APPROVE_DATE,PH.CREATE_USER_GRP_ID,PH.PPO_NO,
                          C.NAME as Customer_CD,PI.QUALITY_CODE,PI.FABRIC_TYPE_CD,FC.COMBO_NAME,QCC.STATUS,QCC.CUSTOMER_COMMENT,QCC.VIEW_FLAG
                          FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm ,
                          GEN_USERS h   
                          WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and qc.ppo_item_id=pi.ppo_item_id
                          and pi.ppo_item_id=pic.ppo_item_id and pi.quality_code=qm.quality_code  and
                          pic.fab_combo_id=fc.fab_combo_id and qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd and h.User_ID=ph.CREATE_USER_ID  and qcc.view_flag='Y'
                          and qm.Quality_code=@QC";
            CustomSqlSection css;
            css = DataAccess.DefaultDB.CustomSql(sql);
            css.AddInputParameter("QC", DbType.String, QC);
            DataSet ds = css.ToDataSet();
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                model = new FabricCodeModel();
                model.userId = dt.Rows[i]["CREATE_USER_ID"].ToString();
                model.userGroup = dt.Rows[i]["CREATE_USER_GRP_ID"].ToString(); ;
                model.CustomerId = dt.Rows[i]["Customer_CD"].ToString();
                model.PPONO = dt.Rows[i]["PPO_NO"].ToString();
                model.FabricCode = dt.Rows[i]["Fabric_Code"].ToString();
                model.Creator = dt.Rows[i]["CREATE_USER_ID"].ToString();
                model.CreatorEmail = dt.Rows[i]["EMAIL"].ToString();
                model.QualityCode = dt.Rows[i]["QUALITY_CODE"].ToString();
                model.FabricPart = dt.Rows[i]["FABRIC_TYPE_CD"].ToString();
                model.Status = dt.Rows[i]["Status"].ToString();
                model.ComboName = dt.Rows[i]["Combo_Name"].ToString();
                model.CustomerComment = dt.Rows[i]["CUSTOMER_COMMENT"].ToString();
                model.ViewFlag = dt.Rows[i]["VIEW_FLAG"].ToString();
                modellist.Add(model);
            }
            return modellist;

        }


        public List<FabricCodeModel> GetFabricCodeList(string MG, string PPONO, string QualityCode, string CustomerId)
        {
            List<FabricCodeModel> modellist = new List<FabricCodeModel>();
            if (MG == "null")
            {
                return modellist;
            }
            else if (MG == "FlatKnit")
            {
                MG = "Flat Knit Fabric";
            }
            FabricCodeModel model = new FabricCodeModel();
            CustomSqlSection css;
            //model.QualityCode = "C1400009";
            //model.CustomerId = "17250";
            //model.PPONO = "PKGK03AA012729";
            //modellist.Add(model);
            //model = new FabricCodeModel();
            //model.QualityCode = "C1400010";
            //model.CustomerId = "17096";
            //model.PPONO = "PKGK03AA012729";
            //modellist.Add(model);
            DataSet ds;

            string SQL = "";

            if (!string.IsNullOrEmpty(QualityCode) && QualityCode != "null")
            {
                SQL = "SELECT ph.customer_cd,c.Name as CustomerName,pi.ppo_no, pi.quality_code, pi.fabric_type_cd AS fabric_part," +
                  "fc.fabric_code, fc.combo_code, fc.combo_name," +
                  "qcc.status AS sample_approve, qcc.customer_comment, pi.fabric_width," +
                  "qcc.last_modi_date, qcc.last_modi_user_id,qcc.view_flag,qcc.Iden " +
                  "FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm  " +
                  "WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and qc.ppo_item_id=pi.ppo_item_id and pi.ppo_item_id=pic.ppo_item_id and pi.quality_code=qm.quality_code and qm.Material_Group=@MG and " +
                  " pic.fab_combo_id=fc.fab_combo_id and qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd and qcc.view_flag='N'  " +
                  " And ph.status<>'C' And qc.status<>'C' And pi.status<>'C' And qcc.status='Approved' And pic.status<>'C'  and qm.status ='New'  ";//by mengjw 原来qcc.status<>'c' 
            }
            else
            {
                SQL = "SELECT ph.customer_cd,c.Name as CustomerName,pi.ppo_no, pi.quality_code, pi.fabric_type_cd AS fabric_part," +
                 "fc.fabric_code, fc.combo_code, fc.combo_name," +
                 "qcc.status AS sample_approve, qcc.customer_comment, pi.fabric_width," +
                 "qcc.last_modi_date, qcc.last_modi_user_id,qcc.view_flag,qcc.Iden " +
                 "FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm  " +
                 "WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and qc.ppo_item_id=pi.ppo_item_id and pi.ppo_item_id=pic.ppo_item_id and pi.quality_code=qm.quality_code and qm.Material_Group=@MG and " +
                 " pic.fab_combo_id=fc.fab_combo_id and qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd and qcc.view_flag='N'  " +
                 " And ph.status<>'C' And qc.status<>'C' And pi.status<>'C' And qcc.status='Approved' And pic.status<>'C' And qcc.approve_date > add_months(sysdate,-3)  and qm.status ='New'   ";//by mengjw 原来qcc.status<>'c'   
            }

            if (!string.IsNullOrEmpty(PPONO) && PPONO != "null")
            {
                SQL = SQL + " and pi.ppo_no like '%"+PPONO+"%' ";
            }
            if (!string.IsNullOrEmpty(QualityCode) && QualityCode != "null")
            {
                SQL = SQL + " and pi.quality_code like '%" + QualityCode + "%' ";
            }
            if (!string.IsNullOrEmpty(CustomerId) && CustomerId != "null")
            {
                SQL = SQL + " and ph.customer_cd = '" + CustomerId + "' ";
            }

            SQL = SQL + "ORDER BY pi.quality_code desc";
            css = DataAccess.DefaultDB.CustomSql(SQL);
            css.AddInputParameter("MG", DbType.String, MG);
            ds = css.ToDataSet();

            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                model = new FabricCodeModel();
                model.CustomerId = dt.Rows[i]["customer_cd"].ToString();
                model.CustomerName = dt.Rows[i]["CustomerName"].ToString();
                model.PPONO = dt.Rows[i]["ppo_no"].ToString();
                model.QualityCode = dt.Rows[i]["quality_code"].ToString();
                model.FabricPart = dt.Rows[i]["fabric_part"].ToString();
                model.FabricCode = dt.Rows[i]["fabric_code"].ToString();
                model.ComboCode = dt.Rows[i]["combo_code"].ToString();
                model.ComboName = dt.Rows[i]["combo_name"].ToString();
                model.SampleApprove = dt.Rows[i]["sample_approve"].ToString();
                model.CustomerComment = dt.Rows[i]["customer_comment"].ToString();
                model.FabricWidth = dt.Rows[i]["fabric_width"].ToString();
                //   model.LastModiDate = dt.Rows[i]["last_modi_date"] == null ? null : (DateTime)dt.Rows[i]["last_modi_date"];
                model.LastModiUserId = dt.Rows[i]["last_modi_user_id"].ToString();
                model.ViewFlag = dt.Rows[i]["view_flag"].ToString();
                model.Iden = dt.Rows[i]["Iden"].ToString();
                modellist.Add(model);
            }
            return modellist;
        }

        public string GetGekComment(string Qc, string customerId)
        {
            CustomSqlSection css = DataAccess.DefaultDB.CustomSql("select MILL_COMMENTS from QCCustomerLibrary where Quality_COde=@Qc and BUYER_ID=@customerId");
            css.AddInputParameter("Qc", DbType.String, Qc);
            css.AddInputParameter("customerId", DbType.String, customerId);
            string sd = css.ToScalar<string>();
            return (sd == null ? "" : sd);
        }

        public string GetGekCommentAndCustomerQualityId_ByCode(string Qc)
        {
            CustomSqlSection css = DataAccess.DefaultDB.CustomSql("select MILL_COMMENTS,CUSTOMER_QUALITY_ID from QCCustomerLibrary where Quality_COde=@Qc");
            css.AddInputParameter("Qc", DbType.String, Qc);
            //string sd = css.ToScalar<string>();
            string sd = css.ToDataSet().Tables[0].Rows[0][0].ToString() +"$"+ css.ToDataSet().Tables[0].Rows[0][1].ToString();
            return (sd == null ? "" : sd);
        }

        public string GetHeavyCollar(string Param)
        {
            string[] sq = Param.Split(new string[] { "," }, StringSplitOptions.None);
            string Qc=sq[0];
            string PPO=sq[1];
            string FactoryPart = sq[2];
            CustomSqlSection css = DataAccess.DefaultDB.CustomSql("SELECT A.HEAVY_FLAT_KNIT||','||A.SPECIAL_TYPE FROM PPO_QCMAININFO A INNER JOIN PPO_ITEM B ON A.PPO_NO=B.PPO_NO AND A.PPO_ITEM_ID=B.PPO_ITEM_ID WHERE A.PPO_NO=@PPO AND B.QUALITY_CODE=@QC AND B.FABRIC_TYPE_CD=@Part AND B.STATUS<>'C' AND A.STATUS<>'C' AND Rownum =1");
            css.AddInputParameter("QC", DbType.String, Qc);
            css.AddInputParameter("PPO", DbType.String, PPO);
            css.AddInputParameter("Part", DbType.String, FactoryPart);
            string sd = css.ToScalar<string>();
            if (!string.IsNullOrEmpty(sd))
            {
                sd = sd.ToUpper();
                string[] sds = sd.Split(',');
                if (sds[0] == "Y")
                    return "Y";
                else
                    return sds[1];
            }
            return "";
        }


        public string GetHeavyCollarOne(string Qc)
        {
            CustomSqlSection css = DataAccess.DefaultDB.CustomSql("select special_type from qcmaininfo where Quality_Code=@QC");
            css.AddInputParameter("QC", DbType.String, Qc);
            string sd = css.ToScalar<string>();
            return (sd == null ? "" : sd);
        }

        public string GetGKInfo(string ppoAndQc)
        {
            string[] sq = ppoAndQc.Split(new string[] { "," }, StringSplitOptions.None);
            string strRet = "";
            string sqlStr = "SELECT distinct d.Batch_No,d.Test_No " +
                         "from planningdb.dbo.pcPPOItem a WITH(nolock) " +
                         "inner Join PlanningDB..PCPPoLot B with(nolock) On A.PPo_Item_ID=B.PPo_Item_ID " +
                         "inner Join semiproductdb..fprequest C with(nolock) On A.Gk_No=C.Gk_No And B.Job_No=C.Job_No " +
                         "inner Join QualityDB..phyWebTest D with(nolock) On C.Batch_No=D.Batch_No " +
                         "Where a.PPO_NO=@PPO and a.Quality_Code=@QC and D.Web_Flag='Y'";
            CustomSqlSection css = DataAccess.CreateSqlServerDatabase().CustomSql(sqlStr);
            css.AddInputParameter("PPO", DbType.String, sq[0]);
            css.AddInputParameter("QC", DbType.String, sq[1]);
            DataSet ds = css.ToDataSet();
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strRet = strRet + sq[0] + "<?>" + dt.Rows[i][0].ToString() + "<?>" + dt.Rows[i][1].ToString() + "<*>";
            }
            if (strRet == "")
            {
                strRet = "<?><?>";
            }
            return strRet;
        }

        public DataSet GetInfoFromKmis(string allParam)
        {
            string[] st = allParam.Split(new string[] { "," }, StringSplitOptions.None);
            string PPONO = st[1];
            string QC = st[0]; string Material_Group = st[2];
            CustomSqlSection cuSQL = DataAccess.CreateSqlServerDatabase().CustomSql("Exec planningDB.dbo.Usp_GetQualityResult @PPO,@QC,@MG,@result");
            cuSQL.AddInputParameter("PPO", DbType.String, PPONO);
            cuSQL.AddInputParameter("QC", DbType.String, QC);
            cuSQL.AddInputParameter("MG", DbType.String, Material_Group);
            cuSQL.AddInputParameter("result", DbType.String, "reslutStr");
            DataSet ds = cuSQL.ToDataSet();

            return ds;
        }
        //根据分析号返回工艺信息



        public List<User> GetUserInfo(string userId)
        {
            CustomSqlSection css = DataAccess.DefaultDB.CustomSql("select EMAIL,NAME from GEN_USERS where user_id=@UserId");
            css.AddInputParameter("UserId", DbType.String, userId);
            DataSet ds = css.ToDataSet();
            DataTable dt = ds.Tables[0];
            List<User> lu = new List<User>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                User user = new User();
                user.Name = dt.Rows[i][1].ToString();
                user.Email = dt.Rows[i][0].ToString();
                lu.Add(user);
            }
            return lu;
        }

        public DataSet GetInfoByGKNo(string NO)
        {
            CustomSqlSection cuSQL = DataAccess.CreateSqlServerDatabase().CustomSql("Exec planningDB.dbo.USP_GetQualityCodeInfoByGkNo @NO");
            cuSQL.AddInputParameter("NO", DbType.String, NO);
            DataSet ds = cuSQL.ToDataSet();
            return ds;
        }
        public DataSet GetInfoByAnalysisNo(string NO)
        {
            CustomSqlSection cuSQL = DataAccess.CreateSqlServerDatabase().CustomSql("Exec planningDB.dbo.USP_GetInfoByAnalysisNo @NO");
            cuSQL.AddInputParameter("NO", DbType.String, NO);
            DataSet ds = cuSQL.ToDataSet();
            return ds;
        }


        /// <summary>
        /// UnApprove QC发邮件

        /// </summary>
        /// <param name="QC"></param>
        /// <param name="reason"></param>
        /// <param name="flag"></param>
        public void SendEmain(List<FabricCodeModel> listFCM, string Creator, string QC, string reason, DbTransaction tran,string reOrUn)
        {
            string senderTo = "";
            string sendContent = "";
            for (int i = 0; i < listFCM.Count; i++)
            {
                if (!senderTo.Contains(listFCM[i].CreatorEmail))
                    senderTo = senderTo + listFCM[i].CreatorEmail + ";";
                sendContent = sendContent + listFCM[i].CustomerId + "  ,  " + listFCM[i].PPONO + "  ,  " + listFCM[i].Creator + "  ,  " + listFCM[i].ApproveTime.ToString("yyyy-MM-dd")
                    + listFCM[i].Status + "  ,  " + listFCM[i].QualityCode + "  ,  " + listFCM[i].FabricCode + "  ,  " + listFCM[i].ComboName + "\r\n";
            }
            QcmaininfoModelList qmm = qm.GetModelList(new QcmaininfoModel() { QualityCode = QC });

            List<User> lu = GetUserInfo(HttpContext.Current.Session["userId"].ToString());
            senderTo = senderTo + (lu.Count > 0 ? (lu[0].Email + ";") : "");
            string SNAME = (lu.Count > 0 ? lu[0].Name : "");

            lu = GetUserInfo(Creator);
            senderTo = senderTo + (lu.Count > 0 ? (lu[0].Email + ";") : "");

            lu = GetUserInfo(qmm[0].Creator);
            senderTo = senderTo + (lu.Count > 0 ? (lu[0].Email + ";") : "");

            if (QC.Substring(0, 1) == "C")
            {
                senderTo = senderTo + "zhangx@esquel.com;xuqiang@esquel.com;chenqy@esquel.com;baitm@esquel.com;ShiYH@esquel.com;ZhangMiaM@esquel.com;xiedq@esquel.com";
            }
            else
            {
                senderTo = senderTo + "chengh@esquel.com;wangliang@esquel.com;jiaww@esquel.com;songta@esquel.com;fengl@esquel.com;PanWSh@esquel.com;YuLind@esquel.com";  
            }

            string title = "Quality " + QC + " had been " + reOrUn + " by " + SNAME;
            string Content = @"Pls note Quality " + QC + " had been " + reOrUn + " by " + SNAME + "\r\n";
            if (reason != "")
                Content = Content + "UnApprove reason:" + reason + "\r\n";
             Content=Content+"Original Creator: " + (lu.Count > 0 ? (lu[0].Name) : "") + "\r\n";
            string sqlStr = @"INSERT INTO  SEND_EMAIL_TRANS (SEQ_NO,MAIL_FROM,MAIL_TO,MAIL_CC,MAIL_SUBJECT,REPORT_URL,MAIL_CONTENT,MAIL_ATTACH,CREATE_DATE)
                VALUES (SEQ_SEND_EMAIL_TRANS.NEXTVAL,@SDC,@MT,@BV,@SUB,'',@Content,'',SYSDATE)";
            CustomSqlSection css = DataAccess.DefaultDB.CustomSql(sqlStr);
            css.SetTransaction(tran);
            css.AddInputParameter("SDC", DbType.String, "escmadmin@esquel.com");
            css.AddInputParameter("BV", DbType.String, "eescmadmin@esquel.com");
            css.AddInputParameter("MT", DbType.String, senderTo);
            css.AddInputParameter("SUB", DbType.String, title);
            css.AddInputParameter("Content", DbType.String, Content);
            css.ExecuteNonQuery();
        }


        /// <summary>
        /// 修改QC发邮件，写日志

        /// </summary>
        /// <param name="QC"></param>
        /// <param name="reason"></param>
        /// <param name="flag"></param>
        public void SendEmain(string QC, string reason,string flag)
        {

            List<FabricCodeModel> listFCM = new List<FabricCodeModel>();
            listFCM = GetFabricCodeListOne(QC);
            string senderTo = "";
            string sendContent = "";
            for (int i = 0; i < listFCM.Count; i++)
            {
                if (!senderTo.Contains(listFCM[i].CreatorEmail))
                    senderTo = senderTo + listFCM[i].CreatorEmail + ";";
                sendContent = sendContent + listFCM[i].CustomerId + "  ,  " + listFCM[i].PPONO + "  ,  " + listFCM[i].Creator + "  ,  " + listFCM[i].ApproveTime.ToString("yyyy-MM-dd")
                    + listFCM[i].Status + "  ,  " + listFCM[i].QualityCode + "  ,  " + listFCM[i].FabricCode + "  ,  " + listFCM[i].ComboName + "\r\n";
            }

            List<User> lu = GetUserInfo(HttpContext.Current.Session["userId"].ToString());
            senderTo = senderTo + (lu.Count > 0 ? (lu[0].Email + ";") : "");
            string SNAME = (lu.Count > 0 ? lu[0].Name : "");

            QcmaininfoModelList qmm = qm.GetModelList(new QcmaininfoModel() { QualityCode = QC });

            //写日志

            QcmaininfologManager logmanager = new QcmaininfologManager();
            QcmaininfologModel logModel = new QcmaininfologModel();
            logModel.QualityCode = QC;
            logModel.EditReason = reason;
            logModel.EditorTime = System.DateTime.Now;
            logModel.Editor = HttpContext.Current.Session["userId"].ToString();
            logModel.Status = qmm[0].Status;
            logmanager.AddModel(logModel);

            lu = GetUserInfo(qmm[0].Creator);
            senderTo = senderTo + (lu.Count > 0 ? (lu[0].Email + ";") : "");

            string title = "Quality " + QC + " has been revised. Pls note your order will be affected.";
            string Content = @"Quality Code " + QC + @" has been " + (flag =="1"?"revised":(flag=="2"?"disabled":"deleted"))+ " revised by " + SNAME + "\r\n" +
                            "Revision reason:" + reason + "\r\n" +
                            "Original Creator: " + (lu.Count > 0 ? (lu[0].Name) : "") + "\r\n";
            if (flag == "1" || flag == "2") //1、2代表已经下单。1代表编辑、2代表Disabled、3代表删除，还没有下单
            {
                Content=Content+"Below PPOs include this Quality Code. The revision will be updated to the PPOs as well.\r\n" + sendContent;
            }

            string sqlStr = @"INSERT INTO  SEND_EMAIL_TRANS (SEQ_NO,MAIL_FROM,MAIL_TO,MAIL_CC,MAIL_SUBJECT,REPORT_URL,MAIL_CONTENT,MAIL_ATTACH,CREATE_DATE)
                VALUES (SEQ_SEND_EMAIL_TRANS.NEXTVAL,@SDC,@MT,@BV,@SUB,'',@Content,'',SYSDATE)";
            CustomSqlSection css = DataAccess.DefaultDB.CustomSql(sqlStr);
            css.AddInputParameter("SDC", DbType.String, "escmadmin@esquel.com");
            css.AddInputParameter("BV", DbType.String, "eescmadmin@esquel.com");
            css.AddInputParameter("MT", DbType.String, senderTo);
            css.AddInputParameter("SUB", DbType.String, title);
            css.AddInputParameter("Content", DbType.String, Content);
            css.ExecuteNonQuery();
        }


        //add by linyob 20190102 自动带入质量标准.  根据订单号和用途获取已经审核的最新记录
        public DataTable GetPhyWebTest(string allParam)
        {
            string[] st = allParam.Split(new string[] { "," }, StringSplitOptions.None);
            string qc = st[0];
            string ppo_no = st[1];
            string usage = st[2];

            string sql = "";
            //    " select 1 ty, Item_Name,Gk_Stand1,Gk_Stand2 from QualityDB..PhyWebTest_TD with (nolock) where Test_No ";
            //sql += " =(select  top 1 a.Test_No from QualityDB..phytesthdr a with (nolock) inner join  QualityDB..PhyWebTest b  with (nolock) on a.test_no=b.Test_No ";
            //sql += " where  Ppo_List like  '%'+@ppo_no1+'%' and a.Usage=@usage1 and b.FN_Verify_Time is not null and b.TD_Verify_Time is not null  order by a.IDEN desc)";
            //sql += " and Item_Name in ('Slanting','S-bowing','Weight Tolerance','Repeat Tolerance','Repeat')  and (Gk_Stand1<>'' or Gk_Stand2<>'')";
            //sql += " and exists (select 1 from  PlanningDB.dbo.pcPPOItem with (nolock) where ppo_no=@ppo_no2 and quality_code=@qc1) ";


            

            //历史记录
            //sql += " union all select 2 ty, Item_Name,Gk_Stand1,Gk_Stand2 from QualityDB..PhyWebTest_TD with (nolock) where Test_No ";
            //sql += " =(select  top 1 a.Test_No from HistoryData..phytesthdrbk a with (nolock) inner join  HistoryData..phyWebTestBK b with (nolock) on a.test_no=b.Test_No ";
            //sql += " where  Ppo_List like  '%'+@ppo_no3+'%' and a.Usage=@usage2 and b.FN_Verify_Time is not null and b.TD_Verify_Time is not null  order by a.IDEN desc)";
            //sql += " and Item_Name in ('Slanting','S-bowing','Weight Tolerance','Repeat Tolerance','Repeat')  and (Gk_Stand1<>'' or Gk_Stand2<>'')";
            //参考PTM  物测系统-上网数据修改 历史数据太卡
            sql = @"
           Declare @Test_no varchar(30);
              select top 1 @Test_no=Test_no from (
        --Select  D.Iden,D.Test_No    FROM PlanningDB.dbo.PPO A WITH (NoLock)   INNER Join HistoryData.dbo.PdBatchFabric C With (NoLock) On A.Gk_No=C.Gk_No And A.Job_No=C.Job_No    INNER Join HistoryData.dbo.phyTestHdrBK D With (NoLock) On C.Batch_No=D.Batch_No   INNER JOIN HistoryData.dbo.PhyTestHeaderbk E WITH (NOLOCK) ON D.Test_No=E.Test_No    INNER JOIN QualityDB.dbo.Phyitemlist F WITH (NOLOCK) ON E.Item_Code = F.Item_Code  inner join HistoryData..PhyWebTestbk G  WITH (NOLOCK) on d.Test_No=G.Test_No   Where A.PPo_No =@ppo_no1  And A.Usage =@Usage1  AND ( F.Item_Name LIKE '%FABRIC WEIGHT%' OR F.Item_Name LIKE '%FABRIC WIDTH%' OR  F.Item_Name LIKE '%DIMENSIONAL STABILITY%'  OR F.Item_Name LIKE '%BURSTING STRENGTH%' OR F.Item_Name LIKE '%PILLING RESISTANCE%'  ) and  G.FN_Verify_Time is not null and G.TD_Verify_Time is not null 
         -- UNION  Select  D.Iden,D.Test_No   FROM PlanningDB.dbo.PPO A WITH (NoLock)     INNER Join PlanningDb.dbo.pcOutBatchDetail C With (NoLock) On A.Gk_No=C.Gk_No And SUBSTRING(A.Job_No,1,8)=SUBSTRING(C.Job_No,1,8)    INNER Join HistoryData.dbo.phyTestHdrBK D With (NoLock) On C.Self_BatchNo=D.Batch_No    INNER JOIN HistoryData.dbo.PhyTestHeaderbk E WITH (NOLOCK) ON D.Test_No=E.Test_No    INNER JOIN QualityDB.dbo.Phyitemlist F WITH (NOLOCK) ON E.Item_Code = F.Item_Code    inner join HistoryData..PhyWebTestbk G  WITH (NOLOCK) on d.Test_No=G.Test_No    Where A.PPo_No = @ppo_no2  And A.Usage =@Usage2 AND ( F.Item_Name LIKE '%FABRIC WEIGHT%' OR F.Item_Name LIKE '%FABRIC WIDTH%' OR  F.Item_Name LIKE '%DIMENSIONAL STABILITY%'  OR F.Item_Name LIKE '%BURSTING STRENGTH%' OR F.Item_Name LIKE '%PILLING RESISTANCE%'  )  and  G.FN_Verify_Time is not null and G.TD_Verify_Time is not null   
       --  UNION 
       Select  D.Iden,D.Test_No  FROM PlanningDB.dbo.PPO A With (NoLock)     INNER JOIN Finishingdb..PdBatchFabric C With (NoLock) On A.Gk_No=C.Gk_No And A.Job_No=C.Job_No    INNER JOIN QualityDB..phyTestHdr D With (NoLock) On C.Batch_No=D.Batch_No      INNER JOIN QualityDB..phytestheader E WITH (NOLOCK) ON D.Test_No=E.Test_No     INNER JOIN QualityDB..Phyitemlist F WITH (NOLOCK) ON E.Item_Code = F.Item_Code inner join QualityDB..PhyWebTest G  WITH (NOLOCK) on d.Test_No=G.Test_No    WHERE  A.PPo_No =@ppo_no3   And A.Usage =@Usage3 AND ( F.Item_Name LIKE '%FABRIC WEIGHT%' OR F.Item_Name LIKE '%FABRIC WIDTH%' OR  F.Item_Name LIKE '%DIMENSIONAL STABILITY%'  OR F.Item_Name LIKE '%BURSTING STRENGTH%' OR F.Item_Name LIKE '%PILLING RESISTANCE%'  )   and  G.FN_Verify_Time is not null and G.TD_Verify_Time is not null  
        UNION   Select   D.Iden,D.Test_No   FROM PlanningDB.dbo.PPO A With (NoLock)   INNER Join PlanningDb..pcOutBatchDetail C With (NoLock) On A.Gk_No=C.Gk_No And SUBSTRING(A.Job_No,1,8)=SUBSTRING(C.Job_No,1,8)   INNER Join QualityDB..phyTestHdr D With (NoLock) On C.Self_BatchNo=D.Batch_No  INNER JOIN QualityDB..phytestheader E WITH (NOLOCK) ON D.Test_No=E.Test_No    INNER JOIN QualityDB..Phyitemlist F WITH (NOLOCK) ON E.Item_Code = F.Item_Code  inner join QualityDB..PhyWebTest G  WITH (NOLOCK) on d.Test_No=G.Test_No   WHERE A.PPo_No =@ppo_no4  And A.Usage =@Usage4 AND ( F.Item_Name LIKE '%FABRIC WEIGHT%' OR F.Item_Name LIKE '%FABRIC WIDTH%' OR  F.Item_Name LIKE '%DIMENSIONAL STABILITY%'  OR F.Item_Name LIKE '%BURSTING STRENGTH%' OR F.Item_Name LIKE '%PILLING RESISTANCE%'  )  and  G.FN_Verify_Time is not null and G.TD_Verify_Time is not null  
        ) t order by Iden desc


         select  test_no,   Item_Name,Gk_Stand1,Gk_Stand2 from QualityDB..PhyWebTest_TD with (nolock) where Test_No  
         =@Test_no
        and Item_Name in ('Slanting','S-bowing','Weight Tolerance','Repeat Tolerance','Repeat') 
         and (Gk_Stand1<>'' or Gk_Stand2<>'') 
           ";

            CustomSqlSection css = DataAccess.CreateSqlServerDatabase().CustomSql(sql);
            css.AddInputParameter("ppo_no1", DbType.String, ppo_no);
            css.AddInputParameter("ppo_no2", DbType.String, ppo_no);
            css.AddInputParameter("ppo_no3", DbType.String, ppo_no);
            css.AddInputParameter("ppo_no4", DbType.String, ppo_no);
            css.AddInputParameter("Usage1", DbType.String, usage);
            css.AddInputParameter("Usage2", DbType.String, usage);
            css.AddInputParameter("Usage3", DbType.String, usage);
            css.AddInputParameter("Usage4", DbType.String, usage);
            //css.AddInputParameter("ppo_no3", DbType.String, ppo_no);
            //css.AddInputParameter("usage2", DbType.String, usage);
            //css.AddInputParameter("ppo_no4", DbType.String, ppo_no);
            //css.AddInputParameter("qc2", DbType.String, qc);

            //        select  a.Test_No from HistoryData..phytesthdrbk a with (nolock)  
            // where  Ppo_List like  '%KSF18KB0000102%'  执行超时无法查询，所以取消历史记录查询
            DataSet ds = css.ToDataSet();
            DataTable dt = ds.Tables[0];
            return dt;
        }
    }
}
