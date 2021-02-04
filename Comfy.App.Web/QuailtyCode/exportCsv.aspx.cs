using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Configuration;
using System.IO;
using YsYarnWHAutoRejection.CommonClass;
using System.Data.OracleClient;
using System.Configuration;
using System.Data.Common;
using Comfy.App.Core.QualityCode;
using Comfy.Data;
using Comfy.App.Core;
using System.Text.RegularExpressions;

namespace Comfy.App.Web
{
    public partial class exportCsv : System.Web.UI.Page
    {
        public static string a;

        protected void Page_Load(object sender, EventArgs e)
        {
            string url = HttpContext.Current.Request.RawUrl;
            int index = url.LastIndexOf("?") + 1;
            int length = url.Length - index;
            string i = url.Substring(index, length);

            if (index == 0)
            {
                a = "";
            }
            else
            {

                a = " and  " + Server.UrlDecode(i.ToString());

                /*  string c =  " and  "  + Server.UrlDecode(i.ToString());
                  if (c.Contains("Create_Date"))
                  {

                      int start = c.IndexOf("Create_Date") + 13;
                      int end = c.IndexOf("Create_Date") + 32;
                      string starttime = c.Substring(start + 1, end - start - 1);

                      int laststart = c.LastIndexOf("Create_Date") + 13;
                      int lastend = c.LastIndexOf("Create_Date") + 32;
                      string endtime = c.Substring(laststart + 1, lastend - laststart - 1);

                      if (starttime == endtime)
                      {
                          a = c.Replace("'" + endtime + "'", " to_date(' " + endtime + "','yyyy-mm-dd,hh24:mi:ss') ");
                      }
                      else
                      {
                          string b = c.Replace("'" + starttime + "'", "  to_date(' " + starttime + "','yyyy-mm-dd,hh24:mi:ss') ");
                          a = b.Replace("'" + endtime + "'", " to_date(' " + endtime + "','yyyy-mm-dd,hh24:mi:ss') ");
                      }
                  }
                  else
                  {
                      a = c;
                  }*/
            }

        }

        #region DataTable() 备份

        // 2021年1月19日17:17:25 备份
        //        public static DataTable DataTable()
        //        {

        //            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        //            OracleConnection con = new OracleConnection(connString);
        //            con.Open();
        //            string cmd = "SELECT    * " +
        //                          " FROM (SELECT ROWNUM as Items, E.* " +
        //                           "  FROM (SELECT DISTINCT A.* " +
        //                             "  FROM QCMainInfo A " +
        //                                   " LEFT JOIN QCConstructionDtl B " +
        //                                     "  ON A.Quality_Code = B.Quality_Code " +
        //                                  "  LEFT JOIN QCFinishDtl C " +
        //                                     "  ON A.Quality_COde = C.Quality_Code " +
        //                                   " LEFT JOIN QCCustomerLibrary D " +
        //                                     "  ON D.Quality_Code = A.Quality_Code " +
        //                                   " LEFT JOIN QCCustomerLibrary F " +
        //                                   "    ON F.Quality_Code=A.Quality_Code " +
        //                                     "  where 1=1   " + a + "  order by A.Quality_Code asc ) E) ";
        //            using (OracleDataAdapter oleAdper = new OracleDataAdapter(cmd, con))
        //            {
        //                DataTable result = new DataTable("table");
        //                oleAdper.Fill(result);
        //                if (result.Rows.Count >= 100)
        //                {
        //                    string cmds = @"
        //                       select  
        //                        distinct 
        //                        A.quality_code,A.status, A.replace_by, A.repeat, 

        //                        L.ApprovedFromSPPO,
        //                        L.ApprovedFromSPPO_Usage,


        //                        to_char(S.descriptions) as construction ,
        //                        to_char(K.finishing_name) as Finishing,A.pattern, (N.DESCRIPTION) as DYE_METHOD , A.bf_gmmm,
        //                        A.af_gmmm, A.shrinkage, A.shrinkage_testing_method, 
        //                        A.gmt_washing,to_char(J.customer_quality_id) as customer_quality_id,
        //                        to_char(H.AvailableWidth) as AvailableWidth,
        //                        A.remark,to_char(I.YarnInfo) as YarnInfo,A.layout, A.yarn_length, A.tapping_type ,(A.special_type ) as Heavy_Flat_Knit,
        //                        to_char(J.mill_comments) as mill_comments,A.measurement, A.sourcing, A.create_date, A.material_group,
        //                        A.analysis_no, A.ref_quality_code, A.creator, A.approve_date, A.approver, 
        //                        to_char(J.buyer_id) as buyer_id, to_char(J.brand) as Branks,
        //                        A.gk_no, A.qc_ref_ppo, A.qc_ref_gp, A.hf_ref_ppo, A.hf_ref_gp,M.customercomment
        //                        from QCMainInfo  A 
        //                        left join QCConstructionDtl B on A.Quality_Code = B.Quality_Code  
        //                        left join QCFinishDtl C on  A.Quality_COde = C.Quality_Code
        //                        left join QCCustomerLibrary D on  A.Quality_Code = D.Quality_Code
        //                        left join QCCustomerLibrary F on  A.Quality_Code = F.Quality_Code

        //   left join ( 
        //                         select * from ( select  Quality_Code , wm_concat(to_char(description)) as  descriptions   from   QCConstructionDtl   G 
        //                        left join  Pbknitconstruction S on S.construction=G.construction                     
        //                        group by Quality_Code )) S on A.Quality_Code=S.Quality_Code

        //                       -- left join QCConstructionDtl G on  A.Quality_Code = G.Quality_Code
        //                       -- left join  Pbknitconstruction M on M.construction=G.construction
        //                        left join PBKNITDYEMETHOD N on A.DYE_METHOD =N.DYE_METHOD

        //                        left join
        //                        (select * from (select quality_code, (wmsys.wm_concat(''||gauge||'G'||diameter||'Inch/'||total_needles||'N/'||width||'-'||max_width||'\'  )) as  AvailableWidth
        //                        from qcavailablewidth  where 1=1 group by quality_code) ) H 
        //                        on A.Quality_Code = H.Quality_Code
        //                        left join 
        //                        (select * from ( select quality_code,( wmsys.wm_concat( ''||yarn_count||'X'||threads||''||b.DESCRIPTION||''||yarn_ratio||'% '||yarn_component||'' ))as YarnInfo from qcyarndtl a left join PBKNITYARNTYPE b on a.YARN_TYPE=b.YARN_TYPE 
        //                        where 1=1 group by quality_code) ) I
        //                        on A.Quality_Code = I.Quality_Code
        //                        left join 
        //                        (select * from (select quality_code , (wmsys.wm_concat(buyer_id)) as buyer_id  ,  (wmsys.wm_concat(brand)) as brand,
        //                        (wmsys.wm_concat(customer_quality_id)) as customer_quality_id,(wmsys.wm_concat(mill_comments) ) as mill_comments  from QCCustomerLibrary  where 1=1 group by quality_code)) J
        //                        on A.Quality_Code=j.Quality_Code
        //                        left join 
        //                        (select * from (select quality_code, (wmsys.wm_concat(z.finishing_name )) as finishing_name 
        //                        from QCFinishDtl y,  pbknitfinish Z
        //                        where  y.finishing_code= z.finishing_code   group by quality_code)) K
        //                        on A.Quality_Code = K.Quality_Code
        //                        left join 
        //                        (

        // select   quality_code,

        //        ApprovedFromSPPO,ApprovedFromSPPO_Usage  

        //from 
        //     (

        //    --modify:gaofeng 2021/01/18  <2021-0001 QC System Enhancement from sales>  --begin

        //    --select  ROW_NUMBER() OVER(PARTITION BY quality_code ORDER BY  approve_date desc) rn,
        //     --quality_code, ApprovedFromSPPO from (SELECT distinct qm.quality_code,( pi.ppo_no|| '('||pi.FABRIC_TYPE_CD ||')' ) as ApprovedFromSPPO ,QCC.approve_date
        //     --FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm  
        //     --WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and qc.ppo_item_id=pi.ppo_item_id  
        //     --and pi.ppo_item_id=pic.ppo_item_id and pi.quality_code=qm.quality_code and  pic.fab_combo_id=fc.fab_combo_id   
        //     --and qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd  and QCC.STATUS='Approved' )s ) where rn=1 


        //    select  ROW_NUMBER() OVER(PARTITION BY quality_code ORDER BY  approve_date desc) rn,
        //     quality_code, 
        //        ApprovedFromSPPO,ApprovedFromSPPO_Usage 

        //    from (

        //        SELECT distinct qm.quality_code,
        //        --( pi.ppo_no|| '('||pi.FABRIC_TYPE_CD ||')' ) as ApprovedFromSPPO ,
        //        pi.ppo_no as ApprovedFromSPPO,
        //        pi.FABRIC_TYPE_CD as ApprovedFromSPPO_Usage,

        //     QCC.approve_date
        //     FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm  
        //     WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and qc.ppo_item_id=pi.ppo_item_id  
        //     and pi.ppo_item_id=pic.ppo_item_id and pi.quality_code=qm.quality_code and  pic.fab_combo_id=fc.fab_combo_id   
        //     and qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd  and QCC.STATUS='Approved' )s ) where rn=1 



        //    --modify:gaofeng 2021/01/18  <2021-0001 QC System Enhancement from sales>  --end


        //) L

        //on A.Quality_Code = L.Quality_Code

        //left join 
        //(  select * from 

        // (SELECT     ROW_NUMBER() OVER(PARTITION BY pi.quality_code ORDER BY  qcc.customer_comment ) rn, (qcc.customer_comment ) as customercomment ,pi.quality_code
        //FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm
        //WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and qc.ppo_item_id=pi.ppo_item_id and pi.ppo_item_id=pic.ppo_item_id and pi.quality_code=qm.quality_code and qm.Material_Group<>'Flat Knit Fabric' 
        //AND pic.fab_combo_id=fc.fab_combo_id and qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd
        //)s WHERE rn = 1  ) M

        //                          on A.Quality_Code = M.quality_code where 1=1
        //                            " + a + "  order by A.Quality_Code asc   ";


        //                    using (OracleDataAdapter oleAdpers = new OracleDataAdapter(cmds, con))
        //                    {
        //                        DataTable results = new DataTable("table");
        //                        oleAdpers.Fill(results);
        //                        results.Columns["QUALITY_CODE"].ColumnName = "QualityCode";
        //                        results.Columns["STATUS"].ColumnName = "Status";
        //                        results.Columns["REPLACE_BY"].ColumnName = "ReplaceBy";
        //                        results.Columns["REPEAT"].ColumnName = "Repeat";
        //                        results.Columns["APPROVEDFROMSPPO"].ColumnName = "ApprovedFromSPPO";


        //                        //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin
        //                        results.Columns["APPROVEDFROMSPPO_USAGE"].ColumnName = "ApprovedFromSPPO_Usage";

        //                        //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --end


        //                        results.Columns["CONSTRUCTION"].ColumnName = "Construction";
        //                        results.Columns["FINISHING"].ColumnName = "Finishing";
        //                        results.Columns["PATTERN"].ColumnName = "Pattern";
        //                        results.Columns["DYE_METHOD"].ColumnName = "DyeMethod";
        //                        results.Columns["BF_GMMM"].ColumnName = "BfGmmm";


        //                        results.Columns["AF_GMMM"].ColumnName = "AfGmmm";
        //                        results.Columns["SHRINKAGE"].ColumnName = "Shrinkage";
        //                        results.Columns["SHRINKAGE_TESTING_METHOD"].ColumnName = "ShrinkageTestingMethod";
        //                        results.Columns["GMT_WASHING"].ColumnName = "GmtWashing";
        //                        results.Columns["CUSTOMER_QUALITY_ID"].ColumnName = "CustomerQualityId";


        //                        results.Columns["AVAILABLEWIDTH"].ColumnName = "Available Width";
        //                        results.Columns["REMARK"].ColumnName = "Available Width Remark";
        //                        results.Columns["YARNINFO"].ColumnName = "YarnInfo";
        //                        results.Columns["LAYOUT"].ColumnName = "Layout";
        //                        results.Columns["YARN_LENGTH"].ColumnName = "YarnLength";

        //                        results.Columns["TAPPING_TYPE"].ColumnName = "TappingType";
        //                        results.Columns["HEAVY_FLAT_KNIT"].ColumnName = "Heavy_Flat_Knit";
        //                        results.Columns["MILL_COMMENTS"].ColumnName = "MillComments";
        //                        results.Columns["MEASUREMENT"].ColumnName = "Measurement";
        //                        results.Columns["SOURCING"].ColumnName = "Sourcing";


        //                        results.Columns["CREATE_DATE"].ColumnName = "CreateDate";
        //                        results.Columns["MATERIAL_GROUP"].ColumnName = "MaterialGroup";
        //                        results.Columns["ANALYSIS_NO"].ColumnName = "AnalysisNo";
        //                        results.Columns["REF_QUALITY_CODE"].ColumnName = "RefQualityCode";
        //                        results.Columns["CREATOR"].ColumnName = "Creator";


        //                        results.Columns["APPROVE_DATE"].ColumnName = "ApproveDate";
        //                        results.Columns["APPROVER"].ColumnName = "Approver";
        //                        results.Columns["BUYER_ID"].ColumnName = "BuyerIds";
        //                        results.Columns["BRANKS"].ColumnName = "Branks";
        //                        results.Columns["GK_NO"].ColumnName = "GK_NO";


        //                        results.Columns["QC_REF_PPO"].ColumnName = "QC_Ref_PPO";
        //                        results.Columns["QC_REF_GP"].ColumnName = "QC_Ref_GP";
        //                        results.Columns["HF_REF_PPO"].ColumnName = "HF_Ref_PPO";
        //                        results.Columns["HF_REF_GP"].ColumnName = "HF_Ref_GP";
        //                        results.Columns["CUSTOMERCOMMENT"].ColumnName = "CustomerComment";




        //                        return results;
        //                    }
        //                }

        //                else
        //                {
        //                    DataTable dtt = new DataTable();
        //                    dtt.Columns.Add("QualityCode", typeof(string));
        //                    dtt.Columns.Add("Status", typeof(string));
        //                    dtt.Columns.Add("ReplaceBy", typeof(string));
        //                    dtt.Columns.Add("Repeat", typeof(string));
        //                    dtt.Columns.Add("ApprovedFromSPPO", typeof(string));

        //                    //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin
        //                    dtt.Columns.Add("ApprovedFromSPPO_Usage", typeof(string));


        //                    //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --end


        //                    dtt.Columns.Add("Construction", typeof(string));
        //                    dtt.Columns.Add("Finishing", typeof(string));
        //                    dtt.Columns.Add("Pattern", typeof(string));
        //                    dtt.Columns.Add("DyeMethod", typeof(string));
        //                    dtt.Columns.Add("BfGmmm", typeof(string));
        //                    dtt.Columns.Add("AfGmmm", typeof(string));
        //                    dtt.Columns.Add("Shrinkage", typeof(string));
        //                    dtt.Columns.Add("ShrinkageTestingMethod", typeof(string));
        //                    dtt.Columns.Add("GmtWashing", typeof(string)); http://localhost:58826/Service References/
        //                    dtt.Columns.Add("CustomerQualityId", typeof(string));
        //                    dtt.Columns.Add("Available Width", typeof(string));
        //                    dtt.Columns.Add("Available Width Remark", typeof(string));
        //                    dtt.Columns.Add("YarnInfo", typeof(string));
        //                    dtt.Columns.Add("Layout", typeof(string));
        //                    dtt.Columns.Add("YarnLength", typeof(string));
        //                    dtt.Columns.Add("TappingType", typeof(string));
        //                    dtt.Columns.Add("Heavy_Flat_Knit", typeof(string));
        //                    dtt.Columns.Add("MillComments", typeof(string));
        //                    dtt.Columns.Add("Measurement", typeof(string));
        //                    dtt.Columns.Add("Sourcing", typeof(string));
        //                    dtt.Columns.Add("CreateDate", typeof(string));
        //                    dtt.Columns.Add("MaterialGroup", typeof(string));
        //                    dtt.Columns.Add("AnalysisNo", typeof(string));
        //                    dtt.Columns.Add("RefQualityCode", typeof(string));
        //                    dtt.Columns.Add("Creator", typeof(string));
        //                    dtt.Columns.Add("ApproveDate", typeof(string));
        //                    dtt.Columns.Add("Approver", typeof(string));
        //                    dtt.Columns.Add("BuyserIds", typeof(string));
        //                    dtt.Columns.Add("Branks", typeof(string));
        //                    dtt.Columns.Add("GK_NO", typeof(string));
        //                    dtt.Columns.Add("QC_Ref_PPO", typeof(string));
        //                    dtt.Columns.Add("QC_Ref_GP", typeof(string));
        //                    dtt.Columns.Add("HF_Ref_PPO", typeof(string));
        //                    dtt.Columns.Add("HF_Ref_GP", typeof(string));
        //                    dtt.Columns.Add("CustomerComment", typeof(string));

        //                    string connStrings = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        //                    OracleConnection cons = new OracleConnection(connStrings);
        //                    cons.Open();
        //                    string Sql1 =
        //                      "SELECT    * " +
        //                      " FROM (SELECT ROWNUM as Items, E.* " +
        //                       "  FROM (SELECT DISTINCT A.* " +
        //                         "  FROM QCMainInfo A " +
        //                               " LEFT JOIN QCConstructionDtl B " +
        //                                 "  ON A.Quality_Code = B.Quality_Code " +
        //                              "  LEFT JOIN QCFinishDtl C " +
        //                                 "  ON A.Quality_COde = C.Quality_Code " +
        //                               " LEFT JOIN QCCustomerLibrary D " +
        //                                 "  ON D.Quality_Code = A.Quality_Code " +
        //                               " LEFT JOIN QCCustomerLibrary F " +
        //                               "    ON F.Quality_Code=A.Quality_Code " +
        //                                 "  where  1=1  @Condition   order by A.Quality_Code asc ) E) ";
        //                    PbknityarntypeManager yarnManager = new PbknityarntypeManager();//kingzhang for support 709786 对于YarnInfo字符串中的Yarn_Type信息替换为pbKnitYarnType.description
        //                    QcconstructiondtlManager qcm = new QcconstructiondtlManager();
        //                    QcfinishdtlManager qsm = new QcfinishdtlManager();
        //                    QccustomerlibraryManager qlm = new QccustomerlibraryManager();
        //                    QcconstructiondtlModelList qcml = new QcconstructiondtlModelList();
        //                    QcfinishdtlModelList qsml = new QcfinishdtlModelList();
        //                    QccustomerlibraryModelList qclm = new QccustomerlibraryModelList();
        //                    QcavailablewidthModelList qsaw = new QcavailablewidthModelList();
        //                    QcavailablewidthManager qcam = new QcavailablewidthManager();
        //                    QcyarndtlManager qym = new QcyarndtlManager();
        //                    QcyarndtlModelList qyml = new QcyarndtlModelList();
        //                    DataSet ds;
        //                    CustomSqlSection css;
        //                    DataTable dt;
        //                    QcmaininfoTable table = new QcmaininfoTable();
        //                    Sql1 = Sql1.Replace("@Condition", a.ToString());

        //                    CustomSqlSection csql1 = DataAccess.DefaultDB.CustomSql(Sql1);
        //                    using (SafeDataReader sdr = new SafeDataReader(csql1.ToDataReader()))
        //                    {

        //                        while (sdr.Read())
        //                        {
        //                            QcmaininfoModel m = new QcmaininfoModel();

        //                            m.QualityCode = sdr.GetString(table.QualityCode);
        //                            qcml = qcm.GetModelListOne(new QcconstructiondtlModel() { QualityCode = m.QualityCode });
        //                            for (int i = 0; i < qcml.Count; i++)
        //                            {
        //                                m.Construction = m.Construction + qcml[i].Construction + ";";
        //                            }

        //                            qsml = qsm.GetModelListOne(new QcfinishdtlModel() { QualityCode = m.QualityCode });
        //                            for (int i = 0; i < qsml.Count; i++)
        //                            {
        //                                m.Finishing = m.Finishing + qsml[i].FinishingCode + ";";
        //                            }
        //                            qclm = qlm.GetModelList(new QccustomerlibraryModel() { QualityCode = m.QualityCode });
        //                            for (int i = 0; i < qclm.Count; i++)
        //                            {
        //                                m.CustomerQualityId = m.CustomerQualityId + qclm[i].CustomerQualityId + ";";
        //                                m.BuyserIds = m.BuyserIds + qclm[i].BuyerId + ";";
        //                                m.Brank = m.Brank + qclm[i].Brand + ";";
        //                                m.MillComments = m.MillComments + qclm[i].MillComments + ";";
        //                            }
        //                            qsaw = qcam.GetModelList(new QcavailablewidthModel() { QualityCode = m.QualityCode });
        //                            for (int i = 0; i < qsaw.Count; i++)
        //                            {
        //                                if (i != qsaw.Count - 1)
        //                                    m.AvaWidth = m.AvaWidth + string.Format("{0}G{1}Inch/{2}N/{3}-{4}\"", qsaw[i].Gauge, qsaw[i].Diameter, qsaw[i].TotalNeedles, qsaw[i].Width, qsaw[i].MaxWidth) + "--";
        //                                else
        //                                    m.AvaWidth = m.AvaWidth + string.Format("{0}G{1}Inch/{2}N/{3}-{4}\"", qsaw[i].Gauge, qsaw[i].Diameter, qsaw[i].TotalNeedles, qsaw[i].Width, qsaw[i].MaxWidth);
        //                            }
        //                            qyml = qym.GetModelList(new QcyarndtlModel() { QualityCode = m.QualityCode });
        //                            for (int i = 0; i < qyml.Count; i++)
        //                            {
        //                                PbknityarntypeModelList yarnModel = yarnManager.GetModelList(new PbknityarntypeModel() { YarnType = qyml[i].YarnType });//kingzhang for support 709786 对于YarnInfo字符串中的Yarn_Type信息替换为pbKnitYarnType.description
        //                                if (i != qyml.Count - 1)
        //                                    m.YarnInfo = m.YarnInfo + string.Format("{0}X{1} {2} {3}% {4}", qyml[i].YarnCount, qyml[i].Threads, yarnModel.Count > 0 ? yarnModel[0].Description : qyml[i].YarnType, qyml[i].YarnRatio, qyml[i].YarnComponent) + "--";
        //                                else
        //                                    m.YarnInfo = m.YarnInfo + string.Format("{0}X{1} {2} {3}% {4}", qyml[i].YarnCount, qyml[i].Threads, yarnModel.Count > 0 ? yarnModel[0].Description : qyml[i].YarnType, qyml[i].YarnRatio, qyml[i].YarnComponent);
        //                            }



        //                            //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin

        //                            css = DataAccess.DefaultDB.CustomSql(@"
        //                               select  ppo_no as PPO , FABRIC_TYPE_CD   

        //                                from 
        //                                 (select  ROW_NUMBER() OVER(PARTITION BY quality_code ORDER BY  approve_date desc) rn,
        //                                 quality_code,ppo_no, FABRIC_TYPE_CD from (SELECT distinct qm.quality_code, pi.ppo_no ,pi.FABRIC_TYPE_CD  ,QCC.approve_date
        //                                 FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm  
        //                                 WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and qc.ppo_item_id=pi.ppo_item_id  
        //                                 and pi.ppo_item_id=pic.ppo_item_id and pi.quality_code=qm.quality_code and  pic.fab_combo_id=fc.fab_combo_id   
        //                                 and qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd  and QCC.STATUS='Approved' )s ) where rn=1 and quality_code=@QC

        //                            ");




        //                            //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --end


        //                            css.AddInputParameter("QC", DbType.String, m.QualityCode);
        //                            ds = css.ToDataSet();

        //                            dt = ds.Tables[0];
        //                            for (int i = 0; i < dt.Rows.Count; i++)
        //                            {
        //                                //m.ApprovedFromSPPO = m.ApprovedFromSPPO + dt.Rows[i]["PPO"].ToString() + "(" + dt.Rows[i]["FABRIC_TYPE_CD"].ToString() + ");";

        //                                //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin
        //                                m.ApprovedFromSPPO = m.ApprovedFromSPPO + dt.Rows[i]["PPO"].ToString();

        //                                m.ApprovedFromSPPO_Usage = m.ApprovedFromSPPO_Usage + dt.Rows[i]["FABRIC_TYPE_CD"].ToString();

        //                            };



        //                            //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --end

        //                            m.CreateDate = sdr.GetDateTime(table.CreateDate);
        //                            m.Creator = sdr.GetString(table.Creator);
        //                            m.ApproveDate = sdr.GetDateTime(table.ApproveDate);
        //                            m.Approver = sdr.GetString(table.Approver);
        //                            m.Status = sdr.GetString(table.Status);
        //                            if (m.Status == "NEW")
        //                                m.Status = "New";
        //                            m.Sourcing = sdr.GetString(table.Sourcing);
        //                            m.MaterialGroup = sdr.GetString(table.MaterialGroup);
        //                            m.AnalysisNo = sdr.GetString(table.AnalysisNo);
        //                            m.RefQualityCode = sdr.GetString(table.RefQualityCode);
        //                            m.Pattern = sdr.GetString(table.Pattern);
        //                            m.DyeMethod = sdr.GetString(table.DyeMethod);
        //                            m.ReplaceBy = sdr.GetString(table.ReplaceBy);
        //                            m.Repeat = sdr.GetString(table.REPEAT);
        //                            m.BfGmmm = sdr.GetInt32(table.BfGmmm);
        //                            m.AfGmmm = sdr.GetInt32(table.AfGmmm);
        //                            m.Shrinkage = sdr.GetString(table.Shrinkage);
        //                            m.ShrinkageTestingMethod = sdr.GetString(table.ShrinkageTestingMethod);
        //                            m.GmtWashing = sdr.GetString(table.GmtWashing);
        //                            m.Layout = sdr.GetString(table.Layout);
        //                            m.YarnLength = sdr.GetString(table.YarnLength);
        //                            m.TappingType = sdr.GetString(table.TappingType);
        //                            m.Measurement = sdr.GetString(table.Measurement);
        //                            m.Remark = sdr.GetString(table.Remark);
        //                            m.SpecialType = sdr.GetString(table.SpecialType);
        //                            m.LastUpdateTime = sdr.GetDateTime(table.LastUpdateTime);
        //                            m.LastUpdateBy = sdr.GetString(table.LastUpdateBy);

        //                            m.GK_NO = sdr.GetString(table.GK_NO);
        //                            m.QC_Ref_PPO = sdr.GetString(table.QC_Ref_PPO);
        //                            m.HF_Ref_PPO = sdr.GetString(table.HF_Ref_PPO);
        //                            m.QC_Ref_GP = sdr.GetString(table.QC_Ref_GP);
        //                            m.HF_Ref_GP = sdr.GetString(table.HF_Ref_GP);
        //                            m.CustomerComment = GetCustomerComment(m.QualityCode);


        //                            dtt.Rows.Add(m.QualityCode, m.Status, m.ReplaceBy, m.Repeat,


        //                                //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin


        //                                m.ApprovedFromSPPO,
        //                                m.ApprovedFromSPPO_Usage,

        //                                         //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --end


        //                                         m.Construction, m.Finishing, m.Pattern, m.DyeMethod, m.BfGmmm,
        //                                         m.AfGmmm, m.Shrinkage, m.ShrinkageTestingMethod, m.GmtWashing, m.CustomerQualityId,
        //                                         m.AvaWidth, m.Remark, m.YarnInfo, m.Layout, m.YarnLength,
        //                                         m.TappingType, m.SpecialType, m.MillComments, m.Measurement, m.Sourcing,
        //                                         m.CreateDate, m.MaterialGroup, m.AnalysisNo, m.RefQualityCode, m.Creator,
        //                                         m.ApproveDate, m.Approver, m.BuyserIds, m.Brank, m.GK_NO,
        //                                         m.QC_Ref_PPO, m.QC_Ref_GP, m.HF_Ref_PPO, m.HF_Ref_GP, m.CustomerComment
        //                                         );

        //                        }
        //                    }
        //                    return dtt;
        //                }

        //            }
        //        }


        #endregion DataTable() 备份


        public static DataTable DataTable()
        {

            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            OracleConnection con = new OracleConnection(connString);
            con.Open();
            string cmd = "SELECT    * " +
                          " FROM (SELECT ROWNUM as Items, E.* " +
                           "  FROM (SELECT DISTINCT A.* " +
                             "  FROM QCMainInfo A " +
                                   " LEFT JOIN QCConstructionDtl B " +
                                     "  ON A.Quality_Code = B.Quality_Code " +
                                  "  LEFT JOIN QCFinishDtl C " +
                                     "  ON A.Quality_COde = C.Quality_Code " +
                                   " LEFT JOIN QCCustomerLibrary D " +
                                     "  ON D.Quality_Code = A.Quality_Code " +
                                   " LEFT JOIN QCCustomerLibrary F " +
                                   "    ON F.Quality_Code=A.Quality_Code " +
                                     "  where 1=1   " + a + "  order by A.Quality_Code asc ) E) ";
            using (OracleDataAdapter oleAdper = new OracleDataAdapter(cmd, con))
            {
                DataTable result = new DataTable("table");
                oleAdper.Fill(result);
                if (result.Rows.Count >= 100)
                {
                    string cmds = @"
                       select  

                        distinct 

                            (A.quality_code ||'-'|| A.status)as QC_Key,
                            A.quality_code,
                            A.status, 
                            L.ApprovedFromSPPO,
                            L.ApprovedFromSPPO_Usage,

                            ---to_char(S.descriptions) as construction ,
                            replace(translate(to_char(S.descriptions),chr(13)||chr(10),';'),',','/') as construction,

                            ---to_char(K.finishing_name) as Finishing,
                            replace(translate(to_char(K.finishing_name),chr(13)||chr(10),';'),',','/') as Finishing,


                            A.pattern,
                            (N.DESCRIPTION) as DYE_METHOD ,
                            A.bf_gmmm,
                            A.af_gmmm,
                            A.shrinkage,
                            A.shrinkage_testing_method,
                            A.gmt_washing,


                            ---to_char(H.AvailableWidth) as AvailableWidth,
                            replace(translate(to_char(H.AvailableWidth),chr(13)||chr(10),';'),',',' ') as AvailableWidth,


                            ---A.remark,
                            replace(translate(to_char(A.remark),chr(13)||chr(10),';'),',',' ') as remark,


                            ---to_char(I.YarnInfo) as YarnInfo,
                            replace(translate(to_char(I.YarnInfo),chr(13)||chr(10),';'),',','/') as YarnInfo,

                            
                            -- to_char(J.mill_comments) as mill_comments,
                            replace(translate(to_char(J.mill_comments),chr(13)||chr(10),';'),',',' ')as mill_comments,

                            A.measurement,
                            A.tapping_type ,
                            to_char(J.customer_quality_id) as customer_quality_id

                            -- A.replace_by, 
                            -- A.repeat, 
                            -- A.layout, A.yarn_length, (A.special_type ) as Heavy_Flat_Knit,
                            --  A.sourcing, A.create_date, A.material_group,
                            -- A.analysis_no, A.ref_quality_code, A.creator, A.approve_date, A.approver, 
                            -- to_char(J.buyer_id) as buyer_id, to_char(J.brand) as Branks,
                            -- A.gk_no, A.qc_ref_ppo, A.qc_ref_gp, A.hf_ref_ppo, A.hf_ref_gp,M.customercomment

                        from QCMainInfo  A 
                        left join QCConstructionDtl B on A.Quality_Code = B.Quality_Code  
                        left join QCFinishDtl C on  A.Quality_COde = C.Quality_Code
                        left join QCCustomerLibrary D on  A.Quality_Code = D.Quality_Code
                        left join QCCustomerLibrary F on  A.Quality_Code = F.Quality_Code

   left join ( 
                         select * from ( select  Quality_Code , wm_concat(to_char(description)) as  descriptions   from   QCConstructionDtl   G 
                        left join  Pbknitconstruction S on S.construction=G.construction                     
                        group by Quality_Code )) S on A.Quality_Code=S.Quality_Code

                       -- left join QCConstructionDtl G on  A.Quality_Code = G.Quality_Code
                       -- left join  Pbknitconstruction M on M.construction=G.construction
                        left join PBKNITDYEMETHOD N on A.DYE_METHOD =N.DYE_METHOD

                        left join
                        (select * from (select quality_code, (wmsys.wm_concat(''||gauge||'G'||diameter||'Inch/'||total_needles||'N/'||width||'-'||max_width||'\'  )) as  AvailableWidth
                        from qcavailablewidth  where 1=1 group by quality_code) ) H 
                        on A.Quality_Code = H.Quality_Code
                        left join 
                        (select * from ( select quality_code,( wmsys.wm_concat( ''||yarn_count||'X'||threads||''||b.DESCRIPTION||''||yarn_ratio||'% '||yarn_component||'' ))as YarnInfo from qcyarndtl a left join PBKNITYARNTYPE b on a.YARN_TYPE=b.YARN_TYPE 
                        where 1=1 group by quality_code) ) I
                        on A.Quality_Code = I.Quality_Code
                        left join 
                        (select * from (select quality_code , (wmsys.wm_concat(buyer_id)) as buyer_id  ,  (wmsys.wm_concat(brand)) as brand,
                        (wmsys.wm_concat(customer_quality_id)) as customer_quality_id,(wmsys.wm_concat(mill_comments) ) as mill_comments  from QCCustomerLibrary  where 1=1 group by quality_code)) J
                        on A.Quality_Code=j.Quality_Code
                        left join 
                        (select * from (select quality_code, (wmsys.wm_concat(z.finishing_name )) as finishing_name 
                        from QCFinishDtl y,  pbknitfinish Z
                        where  y.finishing_code= z.finishing_code   group by quality_code)) K
                        on A.Quality_Code = K.Quality_Code
                        left join 
                        (

 select   quality_code,

        ApprovedFromSPPO,ApprovedFromSPPO_Usage  

from 
     (
    
    --modify:gaofeng 2021/01/18  <2021-0001 QC System Enhancement from sales>  --begin

    --select  ROW_NUMBER() OVER(PARTITION BY quality_code ORDER BY  approve_date desc) rn,
     --quality_code, ApprovedFromSPPO from (SELECT distinct qm.quality_code,( pi.ppo_no|| '('||pi.FABRIC_TYPE_CD ||')' ) as ApprovedFromSPPO ,QCC.approve_date
     --FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm  
     --WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and qc.ppo_item_id=pi.ppo_item_id  
     --and pi.ppo_item_id=pic.ppo_item_id and pi.quality_code=qm.quality_code and  pic.fab_combo_id=fc.fab_combo_id   
     --and qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd  and QCC.STATUS='Approved' )s ) where rn=1 


    select  ROW_NUMBER() OVER(PARTITION BY quality_code ORDER BY  approve_date desc) rn,
     quality_code, 
        ApprovedFromSPPO,ApprovedFromSPPO_Usage 

    from (

        SELECT distinct qm.quality_code,
        --( pi.ppo_no|| '('||pi.FABRIC_TYPE_CD ||')' ) as ApprovedFromSPPO ,
        pi.ppo_no as ApprovedFromSPPO,
        pi.FABRIC_TYPE_CD as ApprovedFromSPPO_Usage,

     QCC.approve_date
     FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm  
     WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and qc.ppo_item_id=pi.ppo_item_id  
     and pi.ppo_item_id=pic.ppo_item_id and pi.quality_code=qm.quality_code and  pic.fab_combo_id=fc.fab_combo_id   
     and qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd  and QCC.STATUS='Approved' )s ) where rn=1 



    --modify:gaofeng 2021/01/18  <2021-0001 QC System Enhancement from sales>  --end


) L

on A.Quality_Code = L.Quality_Code
                      
left join 
(  select * from 
 
 (SELECT     ROW_NUMBER() OVER(PARTITION BY pi.quality_code ORDER BY  qcc.customer_comment ) rn, (qcc.customer_comment ) as customercomment ,pi.quality_code
FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm
WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and qc.ppo_item_id=pi.ppo_item_id and pi.ppo_item_id=pic.ppo_item_id and pi.quality_code=qm.quality_code and qm.Material_Group<>'Flat Knit Fabric' 
AND pic.fab_combo_id=fc.fab_combo_id and qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd
)s WHERE rn = 1  ) M

                          on A.Quality_Code = M.quality_code where 1=1
                            " + a + "  order by A.Quality_Code asc   ";


                    using (OracleDataAdapter oleAdpers = new OracleDataAdapter(cmds, con))
                    {
                        DataTable results = new DataTable("table");

                        //2021年2月4日13:45:32 需要处理 results.Columns["MILL_COMMENTS"] 的回车/空格的情况

                        //if (results.Columns["MILL_COMMENTS"]!="") {
                        //    results.Columns["MILL_COMMENTS"].MaxLength = 8000;
                        //}


                        oleAdpers.Fill(results);

                        // gaofeng 2021年2月3日16:59:15 总共21个字段
                        results.Columns["QC_Key"].ColumnName = "QC_Key";
                        results.Columns["QUALITY_CODE"].ColumnName = "Quality Code";
                        results.Columns["STATUS"].ColumnName = "Status";
                        //results.Columns["APPROVEDFROMSPPO"].ColumnName = "ApprovedFromSPPO";
                        results.Columns["APPROVEDFROMSPPO"].ColumnName = "Related To SPPO";
                        //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin
                        //results.Columns["APPROVEDFROMSPPO_USAGE"].ColumnName = "ApprovedFromSPPO_Usage";
                        results.Columns["APPROVEDFROMSPPO_USAGE"].ColumnName = "Related To SPPO Usage";
                        //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --end
                        results.Columns["CONSTRUCTION"].ColumnName = "Construction";
                        results.Columns["FINISHING"].ColumnName = "Fabric Finishing";
                        results.Columns["PATTERN"].ColumnName = "Pattern";
                        results.Columns["DYE_METHOD"].ColumnName = "Dye Method";

                        results.Columns["BF_GMMM"].ColumnName = "Weight(B/W)";
                        results.Columns["AF_GMMM"].ColumnName = "Weight(A/W)";

                        results.Columns["SHRINKAGE"].ColumnName = "Shrinkage";
                        results.Columns["SHRINKAGE_TESTING_METHOD"].ColumnName = "Shrinkage Test Method";
                        results.Columns["GMT_WASHING"].ColumnName = "GMT Washing (Y or N)";

                        results.Columns["AVAILABLEWIDTH"].ColumnName = "Fabric Width";
                        results.Columns["REMARK"].ColumnName = "Fabric Width Remark";

                        results.Columns["YARNINFO"].ColumnName = "Yarn Info";


                        results.Columns["MILL_COMMENTS"].ColumnName = "Quality Code Comment";


                        results.Columns["MEASUREMENT"].ColumnName = "Size";
                        results.Columns["TAPPING_TYPE"].ColumnName = "Tapping Type";
                        results.Columns["CUSTOMER_QUALITY_ID"].ColumnName = "Customer Quality ID";


                        //results.Columns["REPLACE_BY"].ColumnName = "ReplaceBy";
                        //results.Columns["REPEAT"].ColumnName = "Repeat";

                        //results.Columns["LAYOUT"].ColumnName = "Layout";
                        //results.Columns["YARN_LENGTH"].ColumnName = "YarnLength";
                        //results.Columns["HEAVY_FLAT_KNIT"].ColumnName = "Heavy_Flat_Knit";

                        return results;
                    }
                }

                else
                {
                    DataTable dtt = new DataTable();
                    dtt.Columns.Add("QC Key", typeof(string));
                    dtt.Columns.Add("Quality Code", typeof(string));
                    dtt.Columns.Add("Status", typeof(string));
                    //dtt.Columns.Add("ReplaceBy", typeof(string));
                    //dtt.Columns.Add("Repeat", typeof(string));

                    //dtt.Columns.Add("ApprovedFromSPPO", typeof(string));
                    dtt.Columns.Add("Related To SPPO", typeof(string));

                    //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin
                    //dtt.Columns.Add("ApprovedFromSPPO-Usage", typeof(string));
                    dtt.Columns.Add("Related To SPPO Usage", typeof(string));

                    //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --end
                    dtt.Columns.Add("Construction", typeof(string));
                    dtt.Columns.Add("Fabric Finishing", typeof(string));
                    dtt.Columns.Add("Pattern", typeof(string));
                    dtt.Columns.Add("Dye Method", typeof(string));
                    //dtt.Columns.Add("BfGmmm", typeof(string));
                    dtt.Columns.Add("Weight(B/W)", typeof(string));

                    //dtt.Columns.Add("AfGmmm", typeof(string));
                    dtt.Columns.Add("Weight(A/W)", typeof(string));

                    dtt.Columns.Add("Shrinkage", typeof(string));
                    dtt.Columns.Add("Shrinkage Test Method", typeof(string));
                    dtt.Columns.Add("GMT Washing (Y or N)", typeof(string)); http://localhost:58826/Service References/

                    //dtt.Columns.Add("Available Width", typeof(string));
                    dtt.Columns.Add("Fabric Width", typeof(string));

                    //dtt.Columns.Add("Available Width Remark", typeof(string));
                    dtt.Columns.Add("Fabric Width Remark", typeof(string));

                    dtt.Columns.Add("Yarn Info", typeof(string));

                    //dtt.Columns.Add("MillComments", typeof(string));
                    dtt.Columns.Add("Quality Code Comment", typeof(string));

                    //dtt.Columns.Add("Measurement", typeof(string));
                    dtt.Columns.Add("Size", typeof(string));

                    //dtt.Columns.Add("Layout", typeof(string));
                    //dtt.Columns.Add("YarnLength", typeof(string));
                    dtt.Columns.Add("Tapping Type", typeof(string));

                    //dtt.Columns.Add("CustomerQualityId", typeof(string));
                    dtt.Columns.Add("Customer Quality ID", typeof(string));

                    string connStrings = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                    OracleConnection cons = new OracleConnection(connStrings);
                    cons.Open();
                    string Sql1 =
                      "SELECT    * " +
                      " FROM (SELECT ROWNUM as Items, E.* " +
                       "  FROM (SELECT DISTINCT A.* " +
                         "  FROM QCMainInfo A " +
                               " LEFT JOIN QCConstructionDtl B " +
                                 "  ON A.Quality_Code = B.Quality_Code " +
                              "  LEFT JOIN QCFinishDtl C " +
                                 "  ON A.Quality_COde = C.Quality_Code " +
                               " LEFT JOIN QCCustomerLibrary D " +
                                 "  ON D.Quality_Code = A.Quality_Code " +
                               " LEFT JOIN QCCustomerLibrary F " +
                               "    ON F.Quality_Code=A.Quality_Code " +
                                 "  where  1=1  @Condition   order by A.Quality_Code asc ) E) ";
                    PbknityarntypeManager yarnManager = new PbknityarntypeManager();//kingzhang for support 709786 对于YarnInfo字符串中的Yarn_Type信息替换为pbKnitYarnType.description
                    QcconstructiondtlManager qcm = new QcconstructiondtlManager();
                    QcfinishdtlManager qsm = new QcfinishdtlManager();
                    QccustomerlibraryManager qlm = new QccustomerlibraryManager();
                    QcconstructiondtlModelList qcml = new QcconstructiondtlModelList();
                    QcfinishdtlModelList qsml = new QcfinishdtlModelList();
                    QccustomerlibraryModelList qclm = new QccustomerlibraryModelList();
                    QcavailablewidthModelList qsaw = new QcavailablewidthModelList();
                    QcavailablewidthManager qcam = new QcavailablewidthManager();
                    QcyarndtlManager qym = new QcyarndtlManager();
                    QcyarndtlModelList qyml = new QcyarndtlModelList();
                    DataSet ds;
                    CustomSqlSection css;
                    DataTable dt;
                    QcmaininfoTable table = new QcmaininfoTable();
                    Sql1 = Sql1.Replace("@Condition", a.ToString());

                    CustomSqlSection csql1 = DataAccess.DefaultDB.CustomSql(Sql1);
                    using (SafeDataReader sdr = new SafeDataReader(csql1.ToDataReader()))
                    {

                        while (sdr.Read())
                        {
                            QcmaininfoModel m = new QcmaininfoModel();

                            m.QualityCode = sdr.GetString(table.QualityCode);
                            qcml = qcm.GetModelListOne(new QcconstructiondtlModel() { QualityCode = m.QualityCode });
                            for (int i = 0; i < qcml.Count; i++)
                            {
                                m.Construction = m.Construction + qcml[i].Construction + ";";
                            }

                            qsml = qsm.GetModelListOne(new QcfinishdtlModel() { QualityCode = m.QualityCode });
                            for (int i = 0; i < qsml.Count; i++)
                            {
                                m.Finishing = m.Finishing + qsml[i].FinishingCode + ";";
                            }
                            qclm = qlm.GetModelList(new QccustomerlibraryModel() { QualityCode = m.QualityCode });
                            for (int i = 0; i < qclm.Count; i++)
                            {
                                m.CustomerQualityId = m.CustomerQualityId + qclm[i].CustomerQualityId + ";";
                                m.BuyserIds = m.BuyserIds + qclm[i].BuyerId + ";";
                                m.Brank = m.Brank + qclm[i].Brand + ";";
                                m.MillComments = m.MillComments + qclm[i].MillComments + ";";
                            }
                            qsaw = qcam.GetModelList(new QcavailablewidthModel() { QualityCode = m.QualityCode });
                            for (int i = 0; i < qsaw.Count; i++)
                            {
                                if (i != qsaw.Count - 1)
                                    m.AvaWidth = m.AvaWidth + string.Format("{0}G{1}Inch/{2}N/{3}-{4}\"", qsaw[i].Gauge, qsaw[i].Diameter, qsaw[i].TotalNeedles, qsaw[i].Width, qsaw[i].MaxWidth) + "--";
                                else
                                    m.AvaWidth = m.AvaWidth + string.Format("{0}G{1}Inch/{2}N/{3}-{4}\"", qsaw[i].Gauge, qsaw[i].Diameter, qsaw[i].TotalNeedles, qsaw[i].Width, qsaw[i].MaxWidth);
                            }
                            qyml = qym.GetModelList(new QcyarndtlModel() { QualityCode = m.QualityCode });
                            for (int i = 0; i < qyml.Count; i++)
                            {
                                PbknityarntypeModelList yarnModel = yarnManager.GetModelList(new PbknityarntypeModel() { YarnType = qyml[i].YarnType });//kingzhang for support 709786 对于YarnInfo字符串中的Yarn_Type信息替换为pbKnitYarnType.description
                                if (i != qyml.Count - 1)
                                    m.YarnInfo = m.YarnInfo + string.Format("{0}X{1} {2} {3}% {4}", qyml[i].YarnCount, qyml[i].Threads, yarnModel.Count > 0 ? yarnModel[0].Description : qyml[i].YarnType, qyml[i].YarnRatio, qyml[i].YarnComponent) + "--";
                                else
                                    m.YarnInfo = m.YarnInfo + string.Format("{0}X{1} {2} {3}% {4}", qyml[i].YarnCount, qyml[i].Threads, yarnModel.Count > 0 ? yarnModel[0].Description : qyml[i].YarnType, qyml[i].YarnRatio, qyml[i].YarnComponent);
                            }



                            //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin

                            css = DataAccess.DefaultDB.CustomSql(@"
                               select  ppo_no as PPO , FABRIC_TYPE_CD   
                                
                                from 
                                 (select  ROW_NUMBER() OVER(PARTITION BY quality_code ORDER BY  approve_date desc) rn,
                                 quality_code,ppo_no, FABRIC_TYPE_CD from (SELECT distinct qm.quality_code, pi.ppo_no ,pi.FABRIC_TYPE_CD  ,QCC.approve_date
                                 FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm  
                                 WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and qc.ppo_item_id=pi.ppo_item_id  
                                 and pi.ppo_item_id=pic.ppo_item_id and pi.quality_code=qm.quality_code and  pic.fab_combo_id=fc.fab_combo_id   
                                 and qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd  and QCC.STATUS='Approved' )s ) where rn=1 and quality_code=@QC

                            ");




                            //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --end


                            css.AddInputParameter("QC", DbType.String, m.QualityCode);
                            ds = css.ToDataSet();

                            dt = ds.Tables[0];
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                //m.ApprovedFromSPPO = m.ApprovedFromSPPO + dt.Rows[i]["PPO"].ToString() + "(" + dt.Rows[i]["FABRIC_TYPE_CD"].ToString() + ");";

                                //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin



                                m.ApprovedFromSPPO = m.ApprovedFromSPPO + dt.Rows[i]["PPO"].ToString();

                                m.ApprovedFromSPPO_Usage = m.ApprovedFromSPPO_Usage + dt.Rows[i]["FABRIC_TYPE_CD"].ToString();

                            };



                            //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --end

                            m.CreateDate = sdr.GetDateTime(table.CreateDate);
                            m.Creator = sdr.GetString(table.Creator);
                            m.ApproveDate = sdr.GetDateTime(table.ApproveDate);
                            m.Approver = sdr.GetString(table.Approver);
                            m.Status = sdr.GetString(table.Status);
                            if (m.Status == "NEW")
                                m.Status = "New";

                            m.Sourcing = sdr.GetString(table.Sourcing);
                            m.QC_Key = m.QualityCode + "-" + m.Status.ToString();

                            m.MaterialGroup = sdr.GetString(table.MaterialGroup);
                            m.AnalysisNo = sdr.GetString(table.AnalysisNo);
                            m.RefQualityCode = sdr.GetString(table.RefQualityCode);
                            m.Pattern = sdr.GetString(table.Pattern);
                            m.DyeMethod = sdr.GetString(table.DyeMethod);
                            m.ReplaceBy = sdr.GetString(table.ReplaceBy);

                            m.Repeat = sdr.GetString(table.REPEAT);


                            m.BfGmmm = sdr.GetInt32(table.BfGmmm);
                            m.AfGmmm = sdr.GetInt32(table.AfGmmm);

                            m.Shrinkage = sdr.GetString(table.Shrinkage);
                            m.ShrinkageTestingMethod = sdr.GetString(table.ShrinkageTestingMethod);

                            m.GmtWashing = sdr.GetString(table.GmtWashing);

                            m.Layout = sdr.GetString(table.Layout);
                            m.YarnLength = sdr.GetString(table.YarnLength);

                            m.TappingType = sdr.GetString(table.TappingType);

                            m.Measurement = sdr.GetString(table.Measurement);

                            m.Remark = sdr.GetString(table.Remark);

                            m.SpecialType = sdr.GetString(table.SpecialType);

                            m.LastUpdateTime = sdr.GetDateTime(table.LastUpdateTime);
                            m.LastUpdateBy = sdr.GetString(table.LastUpdateBy);

                            m.GK_NO = sdr.GetString(table.GK_NO);
                            m.QC_Ref_PPO = sdr.GetString(table.QC_Ref_PPO);
                            m.HF_Ref_PPO = sdr.GetString(table.HF_Ref_PPO);
                            m.QC_Ref_GP = sdr.GetString(table.QC_Ref_GP);
                            m.HF_Ref_GP = sdr.GetString(table.HF_Ref_GP);
                            m.CustomerComment = GetCustomerComment(m.QualityCode);


                            //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin

                            if (m.MillComments != null)
                            {
                                if (m.MillComments.Length > 0)
                                {
                                    m.MillComments = Regex.Replace(m.MillComments, @"[/n/r]", "");
                                };
                            }
                            else
                            {
                                m.MillComments = null;
                            }



                            dtt.Rows.Add(

                                m.QC_Key,
                                m.QualityCode,
                                m.Status,
                                //m.ReplaceBy, m.Repeat,

                                m.ApprovedFromSPPO,
                                m.ApprovedFromSPPO_Usage,
                                m.Construction,
                                m.Finishing,
                                m.Pattern,
                                m.DyeMethod,
                                m.BfGmmm,
                                m.AfGmmm,
                                m.Shrinkage,
                                m.ShrinkageTestingMethod,
                                m.GmtWashing,
                                //m.CustomerQualityId,
                                m.AvaWidth,
                                m.Remark,
                                m.YarnInfo,
                                //m.Layout, m.YarnLength,
                                //m.SpecialType, 


                                //m.MillComments,

                                m.MillComments,



                            m.Measurement,
                                m.TappingType,
                                m.CustomerQualityId

                            //m.Sourcing,
                            //m.CreateDate, m.MaterialGroup, m.AnalysisNo, m.RefQualityCode, m.Creator,
                            //m.ApproveDate, m.Approver, m.BuyserIds, m.Brank, m.GK_NO,
                            //m.QC_Ref_PPO, m.QC_Ref_GP, m.HF_Ref_PPO, m.HF_Ref_GP, m.CustomerComment
                            );

                            //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --end

                        }
                    }
                    return dtt;
                }

            }
        }



        //public static DataTable DataTable()
        //{
        //    string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        //    OracleConnection con = new OracleConnection(connString);
        //    con.Open();
        //    string cmd = "SELECT    * " +
        //                  " FROM (SELECT ROWNUM as Items, E.* " +
        //                   "  FROM (SELECT DISTINCT A.* " +
        //                     "  FROM QCMainInfo A " +
        //                           " LEFT JOIN QCConstructionDtl B " +
        //                             "  ON A.Quality_Code = B.Quality_Code " +
        //                          "  LEFT JOIN QCFinishDtl C " +
        //                             "  ON A.Quality_COde = C.Quality_Code " +
        //                           " LEFT JOIN QCCustomerLibrary D " +
        //                             "  ON D.Quality_Code = A.Quality_Code " +
        //                           " LEFT JOIN QCCustomerLibrary F " +
        //                           "    ON F.Quality_Code=A.Quality_Code " +
        //                             "  where 1=1   " + a + "  order by A.Quality_Code asc ) E) ";
        //    using (OracleDataAdapter oleAdper = new OracleDataAdapter(cmd, con))
        //    {
        //        //DataTable result = new DataTable("table");
        //        //oleAdper.Fill(result);

        //        DataTable dtt = new DataTable();
        //        dtt.Columns.Add("QC Key", typeof(string));
        //        dtt.Columns.Add("Quality Code", typeof(string));
        //        dtt.Columns.Add("Status", typeof(string));
        //        dtt.Columns.Add("Related To SPPO", typeof(string));
        //        dtt.Columns.Add("Related To SPPO Usage", typeof(string));
        //        dtt.Columns.Add("Construction", typeof(string));
        //        dtt.Columns.Add("Fabric Finishing", typeof(string));
        //        dtt.Columns.Add("Pattern", typeof(string));
        //        dtt.Columns.Add("Dye Method", typeof(string));
        //        dtt.Columns.Add("Weight(B/W)", typeof(string));
        //        dtt.Columns.Add("Weight(A/W)", typeof(string));

        //        dtt.Columns.Add("Shrinkage", typeof(string));
        //        dtt.Columns.Add("Shrinkage Test Method", typeof(string));
        //        dtt.Columns.Add("GMT Washing (Y or N)", typeof(string)); http://localhost:58826/Service References/
        //        dtt.Columns.Add("Fabric Width", typeof(string));
        //        dtt.Columns.Add("Fabric Width Remark", typeof(string));
        //        dtt.Columns.Add("Yarn Info", typeof(string));
        //        dtt.Columns.Add("Quality Code Comment", typeof(string));
        //        dtt.Columns.Add("Size", typeof(string));
        //        dtt.Columns.Add("Tapping Type", typeof(string));
        //        dtt.Columns.Add("Customer Quality ID", typeof(string));

        //        string connStrings = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        //        OracleConnection cons = new OracleConnection(connStrings);
        //        cons.Open();
        //        string Sql1 =
        //          "SELECT    * " +
        //          " FROM (SELECT ROWNUM as Items, E.* " +
        //           "  FROM (SELECT DISTINCT A.* " +
        //             "  FROM QCMainInfo A " +
        //                   " LEFT JOIN QCConstructionDtl B " +
        //                     "  ON A.Quality_Code = B.Quality_Code " +
        //                  "  LEFT JOIN QCFinishDtl C " +
        //                     "  ON A.Quality_COde = C.Quality_Code " +
        //                   " LEFT JOIN QCCustomerLibrary D " +
        //                     "  ON D.Quality_Code = A.Quality_Code " +
        //                   " LEFT JOIN QCCustomerLibrary F " +
        //                   "    ON F.Quality_Code=A.Quality_Code " +
        //                     "  where  1=1  @Condition   order by A.Quality_Code asc ) E) ";
        //        PbknityarntypeManager yarnManager = new PbknityarntypeManager();//kingzhang for support 709786 对于YarnInfo字符串中的Yarn_Type信息替换为pbKnitYarnType.description
        //        QcconstructiondtlManager qcm = new QcconstructiondtlManager();
        //        QcfinishdtlManager qsm = new QcfinishdtlManager();
        //        QccustomerlibraryManager qlm = new QccustomerlibraryManager();
        //        QcconstructiondtlModelList qcml = new QcconstructiondtlModelList();
        //        QcfinishdtlModelList qsml = new QcfinishdtlModelList();
        //        QccustomerlibraryModelList qclm = new QccustomerlibraryModelList();
        //        QcavailablewidthModelList qsaw = new QcavailablewidthModelList();
        //        QcavailablewidthManager qcam = new QcavailablewidthManager();
        //        QcyarndtlManager qym = new QcyarndtlManager();
        //        QcyarndtlModelList qyml = new QcyarndtlModelList();
        //        DataSet ds;
        //        CustomSqlSection css;
        //        DataTable dt;
        //        QcmaininfoTable table = new QcmaininfoTable();
        //        Sql1 = Sql1.Replace("@Condition", a.ToString());

        //        CustomSqlSection csql1 = DataAccess.DefaultDB.CustomSql(Sql1);
        //        using (SafeDataReader sdr = new SafeDataReader(csql1.ToDataReader())) {
        //            QcmaininfoModel m = new QcmaininfoModel();

        //            m.QualityCode = sdr.GetString(table.QualityCode);
        //            qcml = qcm.GetModelListOne(new QcconstructiondtlModel() { QualityCode = m.QualityCode });
        //            for (int i = 0; i < qcml.Count; i++)
        //            {
        //                m.Construction = m.Construction + qcml[i].Construction + ";";
        //            }

        //            qsml = qsm.GetModelListOne(new QcfinishdtlModel() { QualityCode = m.QualityCode });
        //            for (int i = 0; i < qsml.Count; i++)
        //            {
        //                m.Finishing = m.Finishing + qsml[i].FinishingCode + ";";
        //            }
        //            qclm = qlm.GetModelList(new QccustomerlibraryModel() { QualityCode = m.QualityCode });
        //            for (int i = 0; i < qclm.Count; i++)
        //            {
        //                m.CustomerQualityId = m.CustomerQualityId + qclm[i].CustomerQualityId + ";";
        //                m.BuyserIds = m.BuyserIds + qclm[i].BuyerId + ";";
        //                m.Brank = m.Brank + qclm[i].Brand + ";";
        //                m.MillComments = m.MillComments + qclm[i].MillComments + ";";
        //            }
        //            qsaw = qcam.GetModelList(new QcavailablewidthModel() { QualityCode = m.QualityCode });
        //            for (int i = 0; i < qsaw.Count; i++)
        //            {
        //                if (i != qsaw.Count - 1)
        //                    m.AvaWidth = m.AvaWidth + string.Format("{0}G{1}Inch/{2}N/{3}-{4}\"", qsaw[i].Gauge, qsaw[i].Diameter, qsaw[i].TotalNeedles, qsaw[i].Width, qsaw[i].MaxWidth) + "--";
        //                else
        //                    m.AvaWidth = m.AvaWidth + string.Format("{0}G{1}Inch/{2}N/{3}-{4}\"", qsaw[i].Gauge, qsaw[i].Diameter, qsaw[i].TotalNeedles, qsaw[i].Width, qsaw[i].MaxWidth);
        //            }
        //            qyml = qym.GetModelList(new QcyarndtlModel() { QualityCode = m.QualityCode });
        //            for (int i = 0; i < qyml.Count; i++)
        //            {
        //                PbknityarntypeModelList yarnModel = yarnManager.GetModelList(new PbknityarntypeModel() { YarnType = qyml[i].YarnType });//kingzhang for support 709786 对于YarnInfo字符串中的Yarn_Type信息替换为pbKnitYarnType.description
        //                if (i != qyml.Count - 1)
        //                    m.YarnInfo = m.YarnInfo + string.Format("{0}X{1} {2} {3}% {4}", qyml[i].YarnCount, qyml[i].Threads, yarnModel.Count > 0 ? yarnModel[0].Description : qyml[i].YarnType, qyml[i].YarnRatio, qyml[i].YarnComponent) + "--";
        //                else
        //                    m.YarnInfo = m.YarnInfo + string.Format("{0}X{1} {2} {3}% {4}", qyml[i].YarnCount, qyml[i].Threads, yarnModel.Count > 0 ? yarnModel[0].Description : qyml[i].YarnType, qyml[i].YarnRatio, qyml[i].YarnComponent);
        //            }



        //            //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin

        //            css = DataAccess.DefaultDB.CustomSql(@"
        //                       select  ppo_no as PPO , FABRIC_TYPE_CD   

        //                        from 
        //                         (select  ROW_NUMBER() OVER(PARTITION BY quality_code ORDER BY  approve_date desc) rn,
        //                         quality_code,ppo_no, FABRIC_TYPE_CD from (SELECT distinct qm.quality_code, pi.ppo_no ,pi.FABRIC_TYPE_CD  ,QCC.approve_date
        //                         FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm  
        //                         WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and qc.ppo_item_id=pi.ppo_item_id  
        //                         and pi.ppo_item_id=pic.ppo_item_id and pi.quality_code=qm.quality_code and  pic.fab_combo_id=fc.fab_combo_id   
        //                         and qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd  and QCC.STATUS='Approved' )s ) where rn=1 and quality_code=@QC

        //                    ");




        //            //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --end


        //            css.AddInputParameter("QC", DbType.String, m.QualityCode);
        //            ds = css.ToDataSet();

        //            dt = ds.Tables[0];
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                //m.ApprovedFromSPPO = m.ApprovedFromSPPO + dt.Rows[i]["PPO"].ToString() + "(" + dt.Rows[i]["FABRIC_TYPE_CD"].ToString() + ");";

        //                //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin



        //                m.ApprovedFromSPPO = m.ApprovedFromSPPO + dt.Rows[i]["PPO"].ToString();

        //                m.ApprovedFromSPPO_Usage = m.ApprovedFromSPPO_Usage + dt.Rows[i]["FABRIC_TYPE_CD"].ToString();

        //            };



        //            //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --end

        //            m.CreateDate = sdr.GetDateTime(table.CreateDate);
        //            m.Creator = sdr.GetString(table.Creator);
        //            m.ApproveDate = sdr.GetDateTime(table.ApproveDate);
        //            m.Approver = sdr.GetString(table.Approver);
        //            m.Status = sdr.GetString(table.Status);
        //            if (m.Status == "NEW")
        //                m.Status = "New";

        //            m.Sourcing = sdr.GetString(table.Sourcing);
        //            m.QC_Key = m.QualityCode + "-" + m.Status.ToString();

        //            m.MaterialGroup = sdr.GetString(table.MaterialGroup);
        //            m.AnalysisNo = sdr.GetString(table.AnalysisNo);
        //            m.RefQualityCode = sdr.GetString(table.RefQualityCode);
        //            m.Pattern = sdr.GetString(table.Pattern);
        //            m.DyeMethod = sdr.GetString(table.DyeMethod);
        //            m.ReplaceBy = sdr.GetString(table.ReplaceBy);

        //            m.Repeat = sdr.GetString(table.REPEAT);


        //            m.BfGmmm = sdr.GetInt32(table.BfGmmm);
        //            m.AfGmmm = sdr.GetInt32(table.AfGmmm);

        //            m.Shrinkage = sdr.GetString(table.Shrinkage);
        //            m.ShrinkageTestingMethod = sdr.GetString(table.ShrinkageTestingMethod);

        //            m.GmtWashing = sdr.GetString(table.GmtWashing);

        //            m.Layout = sdr.GetString(table.Layout);
        //            m.YarnLength = sdr.GetString(table.YarnLength);

        //            m.TappingType = sdr.GetString(table.TappingType);

        //            m.Measurement = sdr.GetString(table.Measurement);

        //            m.Remark = sdr.GetString(table.Remark);
        //            if (m.Remark != null)
        //            {
        //                if (m.Remark.Length > 0)
        //                {
        //                    m.Remark = Regex.Replace(m.Remark, @"[/n/r]", "");
        //                }

        //            }
        //            else {
        //                m.Remark = "";
        //            }

        //            m.SpecialType = sdr.GetString(table.SpecialType);

        //            m.LastUpdateTime = sdr.GetDateTime(table.LastUpdateTime);
        //            m.LastUpdateBy = sdr.GetString(table.LastUpdateBy);

        //            m.GK_NO = sdr.GetString(table.GK_NO);
        //            m.QC_Ref_PPO = sdr.GetString(table.QC_Ref_PPO);
        //            m.HF_Ref_PPO = sdr.GetString(table.HF_Ref_PPO);
        //            m.QC_Ref_GP = sdr.GetString(table.QC_Ref_GP);
        //            m.HF_Ref_GP = sdr.GetString(table.HF_Ref_GP);
        //            m.CustomerComment = GetCustomerComment(m.QualityCode);
        //            if (m.CustomerComment != null)
        //            {
        //                if (m.CustomerComment.Length > 0)
        //                {
        //                    m.CustomerComment = Regex.Replace(m.CustomerComment, @"[/n/r]", "");
        //                };
        //            }
        //            else
        //            {
        //                m.CustomerComment = "";
        //            }


        //            //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin

        //            if (m.MillComments != null)
        //            {
        //                if (m.MillComments.Length > 0)
        //                {
        //                    m.MillComments = Regex.Replace(m.MillComments, @"[/n/r]", "");
        //                };
        //            }
        //            else
        //            {
        //                m.MillComments = "";
        //            }



        //            dtt.Rows.Add(

        //                m.QC_Key,
        //                m.QualityCode,
        //                m.Status,
        //                //m.ReplaceBy, m.Repeat,

        //                m.ApprovedFromSPPO,
        //                m.ApprovedFromSPPO_Usage,
        //                m.Construction,
        //                m.Finishing,
        //                m.Pattern,
        //                m.DyeMethod,
        //                m.BfGmmm,
        //                m.AfGmmm,
        //                m.Shrinkage,
        //                m.ShrinkageTestingMethod,
        //                m.GmtWashing,
        //                //m.CustomerQualityId,
        //                m.AvaWidth,
        //                m.Remark,
        //                m.YarnInfo,
        //                m.MillComments,
        //                m.Measurement,
        //                m.TappingType,
        //                m.CustomerQualityId

        //            );
        //        }
        //        return dtt;
        //    }  
        //}





        //Add by sunny 2017 0904  summar ma 要去search页面添加销售的评价
        /// <param name="strQC"></param>
        /// <returns>通过QC获得销售评价</returns>
        public static string GetCustomerComment(string QC)
        {

            string sd = "";
            if (QC != "")
            {
                CustomSqlSection css = DataAccess.DefaultDB.CustomSql(@"SELECT  qcc.customer_comment
FROM ppo_hd ph,gen_customer c,ppo_qcmaininfo qc, ppo_item pi, fab_combo fc, ppo_qccomment qcc,ppo_item_combo pic,QCMainInfo qm
WHERE ph.ppo_no=pi.ppo_no and  fc.fab_combo_id = qcc.fab_combo_id and qc.ppo_item_id=pi.ppo_item_id and pi.ppo_item_id=pic.ppo_item_id and pi.quality_code=qm.quality_code and qm.Material_Group<>'Flat Knit Fabric' 
AND pic.fab_combo_id=fc.fab_combo_id and qc.ppo_qc_id=qcc.ppo_qc_id and ph.customer_cd=c.customer_cd
and pi.quality_code= @Qc ");
                css.AddInputParameter("Qc", DbType.String, QC);

                DataTable dt = css.ToDataSet().Tables[0];
                string i = "";

                if (dt.Rows.Count > 0)
                {
                    i = css.ToDataSet().Tables[0].Rows[0][0].ToString();
                }

                if (i != "")
                {
                    sd = i;
                }
            }
            return (sd);
        }






        private string DataSetToString(DataSet ds)
        {
            return ds.GetXml();
        }



        //modify:gaofeng 2021 / 01 / 18 < 2021 - 0001 QC System Enhancement from sales > --begin
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        protected void DownLoadExcel(System.Data.DataTable dt, string fileName, int sheetIndex = 0)
        {
            try
            {
                if (fileName == "QC export file template")
                {

                    //string sourceFile = Server.MapPath("~/TemparyFile/QC export file template.csv");
                    //string destFileName = "QC export file template" + Guid.NewGuid() + ".csv";
                    //string destFilePath = Server.MapPath("~/TemparyFile/") + destFileName;
                    //File.Copy(sourceFile, destFilePath);
                    //ExcelHandle.DataTableToExcel(dt, destFilePath);



                    string sourceFile = Server.MapPath("~/TemparyFile/QC export file template.csv");
                    string destFileName = "QC export file template" + Guid.NewGuid() + ".csv";
                    string destFilePath = Server.MapPath("~/TemparyFile/") + destFileName;

                    Comfy.Data.CSVHelper.WriteCSV(destFilePath, dt);


                    //File.Copy(sourceFile, destFilePath);
                    //ExcelHandle.DataTableToExcel(dt, destFilePath);




                    //ExcelHandle.WriteCSV(destFilePath, dt);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "", "downloadFile('" + destFileName + "','" + fileName + "');", true);

                }
                else
                {
                    string sourceFile = Server.MapPath("~/TemparyFile/ExcelTemplateTwo.xls");
                    string destFileName = "ExcelTemplate" + Guid.NewGuid() + ".xls";
                    string destFilePath = Server.MapPath("~/TemparyFile/") + destFileName;
                    File.Copy(sourceFile, destFilePath);
                    ExcelHandle.DataTableToExcel(dt, destFilePath);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "", "downloadFile('" + destFileName + "','" + fileName + "');", true);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// CSV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        
        protected void BtnExport_Click1(object sender, EventArgs e)
        {
            //gaofeng 2021年1月20日16:59:28

            DataTable table = DataTable();
            DownLoadExcel(table, "QC export file template");


        }


        /// <param name="e"></param>
        protected void BtnExport_Click(object sender, EventArgs e)
        {
            DataTable table = DataTable();
            DownLoadExcel(table, "NextTwoMonthsScrapped");
        }


    }
}