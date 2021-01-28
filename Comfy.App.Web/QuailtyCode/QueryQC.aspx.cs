using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Comfy.App.Core.QualityCode;
using System.Text;
using System.Web.Services;
using System.Collections;

namespace Comfy.App.Web.QuailtyCode
{
    public partial class QueryQC : MastPage
    {
        string Condition = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnEdit.Visible = false;
                btnApprove.Visible = false;
                btnCreate.Visible = false;
                if (HttpContext.Current.Session["UserPower"] != null)
                {
                    List<string> sl = (List<string>)HttpContext.Current.Session["UserPower"];
                    if (sl.Contains("3"))
                    {
                        btnEdit.Visible = true;
                    }
                    if (sl.Contains("2"))
                    {
                        btnApprove.Visible = true;
                        btnCreate.Visible = true;
                    }
                }

                if (Request.QueryString["QC"] != null)
                {
                    ClientScript.RegisterClientScriptBlock(typeof(string), "js", "SearchQC('" +
                        Request.QueryString["QC"].ToString() + "');", true);
                }

            }
        }

        public QcmaininfoModelList GetModels(string QualityCode, string Status, string Sourcing, string CustomerCode,
            int BfGmmm, int AfGmmm, string MaterialGroup, string DyeMethod, string Pattern, string Finish, string Con, string Creator, string CustomerQualityId,
            string Sales, string SalesTeam, DateTime BeginCreateDate, DateTime EndCreateDate, int pageSize, int startPage, string orderByField)
        {

            QcmaininfoModelGen qmg = new QcmaininfoModelGen();
            qmg.QualityCode = QualityCode;
            qmg.Status = Status == "-1" ? null : Status;
            qmg.Sourcing = Sourcing == "-1" ? null : Sourcing;
            qmg.MaterialGroup = MaterialGroup == "-1" ? null : MaterialGroup;
            qmg.Pattern = Pattern == "-1" ? null : Pattern; ;
            qmg.DyeMethod = DyeMethod == "-1" ? null : DyeMethod;
            qmg.Finishing = Finish == "-1" ? null : Finish;
            qmg.Construction = Con == "-1" ? null : Con;
            qmg.AfGmmm = AfGmmm;
            qmg.BfGmmm = BfGmmm;
            qmg.CustomerQaulityId = CustomerQualityId;
            qmg.CreateDate = BeginCreateDate;
            qmg.CreateEndDate = EndCreateDate;
            qmg.Sales = Sales;

            /// 添加对Show My Own的判断显示 by LYH 2014/2/20
            if (Creator == null)
            {
                if (Convert.ToBoolean(HttpContext.Current.Session["ShowOwn"]))
                {
                    if (HttpContext.Current.Session["userId"] != null)
                    {
                        Creator = HttpContext.Current.Session["userId"].ToString();
                    }
                }
            }
            qmg.Creator = Creator;
            qmg.SalesTeam = SalesTeam == "-1" ? null : SalesTeam;
            qmg.PageSize = pageSize;
            qmg.StartPage = startPage;
            qmg.OrderByField = orderByField;
            qmg.CustomerCode = CustomerCode == "-1" ? null : CustomerCode;

            // edit by jack
            HttpContext.Current.Session["GetModelListCondition"] = GetModelListCondition(qmg);

            /*QcmaininfoModelList result = new QcmaininfoModelList();
            QcmaininfoModel list1 = new QcmaininfoModel();
            list1.QualityCode = "sdfsdf";
            result.Add(list1);
            return result;*/
            return qcMainManager.GetModelListOne(qmg);
        }

        public QccustomerlibraryModelList GetCModels(string QualityCode)
        {
            if (QualityCode == "" || QualityCode == null)
            {
                return new QccustomerlibraryModelList();
            }
            QccustomerlibraryModel model = new QccustomerlibraryModel();
            model.QualityCode = QualityCode;
            return qcCustomerManager.GetModelList(model);
        }

        protected void cboxShowOwn_CheckedChanged(object sender, EventArgs e)
        {
            // 根据是否选中Show My Own 按钮，再在查询QccustomerlibraryModel时判断是否进行筛选

            if (this.cboxShowOwn.Checked)
            {
                HttpContext.Current.Session["ShowOwn"] = true;
            }
            else
            {
                HttpContext.Current.Session["ShowOwn"] = false;
            }
        }

        //sunny 注销开始20170906  服务器控件导致父页面刷新，现在改为静态方法，避免父页面刷新
        /*protected void Unnamed1_Click(object sender, EventArgs e)
        { 
            string Condition = (string)HttpContext.Current.Session["GetModelListCondition"];
            if (Condition != "")
            {
                //add by jack 
                ClientScript.RegisterClientScriptBlock(typeof(string), "js", "RedirectURL('" +
                    Condition + "');", true);              
            }   
        }*/

        //sunny 注销结束20170906  服务器控件导致父页面刷新，现在改为静态方法，避免父页面刷新
         [WebMethod]
        public static Hashtable  Unnamed1_Click( )
        {
            Hashtable ht = new Hashtable();  
            string Condition = (string)HttpContext.Current.Session["GetModelListCondition"];
            if (Condition != "")
            {
                ht.Add("getcondition", Condition);
            }

            return ht;
        }


        [WebMethod]
        public static Hashtable Unnamed2_Click()
        {
            Hashtable ht = new Hashtable();
            string Condition = (string)HttpContext.Current.Session["GetModelListCondition"];
            if (Condition != "")
            {
                ht.Add("getcondition", Condition);
            }

            return ht;
        }




        public string GetModelListCondition(QcmaininfoModelGen model)
        {
            int flag = 0;

            // 添加查询条件 
            StringBuilder sb = new StringBuilder("");
            if (model.Pattern != "" && model.Pattern != null)
            {
                sb.Append("UPPER(A.Pattern)='" + model.Pattern.ToUpper() + "'");
                flag++;
            }
            if (model.CreateDate != null && model.CreateDate != DateTime.MinValue)
            {

              //sunny 注销开始  时间格式不对，需要重新转换
              //  if (flag == 0)
              //      sb.Append("A.Create_Date>='" + model.CreateDate.ToString() + "'");
              // else
                //    sb.Append(" and A.Create_Date>='" + model.CreateDate.ToString() + "'");  
              //sunny 注销结束

               if (flag == 0)
                   sb.Append("A.Create_Date>= to_date('" + model.CreateDate.ToString() + "','yyyy-mm-dd,hh24:mi:ss')");
                  else
                   sb.Append(" and A.Create_Date>= to_date('" + model.CreateDate.ToString() + "','yyyy-mm-dd,hh24:mi:ss')");
                flag++;

            }


            //Add by sunny 20171016当前系统对日期范围，只有起始时间，没有结束时间。

            if (model.CreateEndDate != null && model.CreateEndDate != DateTime.MinValue)
            {

                
                if (flag == 0)
                    sb.Append("A.Create_Date<= to_date('" + model.CreateEndDate.ToString() + "','yyyy-mm-dd,hh24:mi:ss')");
                else
                    sb.Append(" and A.Create_Date<= to_date('" + model.CreateEndDate.ToString() + "','yyyy-mm-dd,hh24:mi:ss')");
                flag++;

            }

            if (model.Creator != "" && model.Creator != null)
            {
                if (flag == 0)
                    sb.Append("upper(A.Creator)='" + model.Creator.ToUpper() + "'");
                else
                    sb.Append("and upper(A.Creator)='" + model.Creator.ToUpper() + "'");
                flag++;
            }
            if (model.BfGmmm != null && model.BfGmmm != 0)
            {
                if (flag == 0) sb.Append("A.BF_GMMM=" + model.BfGmmm.ToString());
                else
                    sb.Append(" and A.BF_GMMM=" + model.BfGmmm.ToString());
                flag++;
            }
            if (model.AfGmmm != null && model.AfGmmm != 0)
            {
                if (flag == 0)
                    sb.Append("A.AF_GMMM=" + model.AfGmmm.ToString());
                else
                    sb.Append(" and A.AF_GMMM=" + model.AfGmmm.ToString());
                flag++;
            }
            if (model.Status != "" && model.Status != null)
            {
                if (flag == 0)
                    sb.Append("UPPER(A.Status)='" + model.Status.ToUpper() + "'");
                else
                    sb.Append(" and UPPER(A.Status)='" + model.Status.ToUpper() + "'");
                flag++;
            }
            if (model.CustomerQaulityId != "" && model.CustomerQaulityId != null)
            {
                if (flag == 0)
                    sb.Append("UPPER(F.Customer_Quality_ID)='" + model.CustomerQaulityId.ToUpper() + "'");
                else
                    sb.Append(" and UPPER(F.Customer_Quality_ID)='" + model.CustomerQaulityId.ToUpper() + "'");
                flag++;
            }
            if (model.Sourcing != "" && model.Sourcing != null)
            {
                if (flag == 0)
                    sb.Append("UPPER(A.Sourcing)='" + model.Sourcing.ToUpper() + "'");
                else
                    sb.Append(" and UPPER(A.Sourcing)='" + model.Sourcing.ToUpper() + "'");
                flag++;
            }
            if (model.MaterialGroup != "" && model.MaterialGroup != null)
            {
                if (flag == 0)
                    sb.Append("UPPER(A.Material_Group)='" + model.MaterialGroup.ToUpper() + "'");
                else
                    sb.Append(" and UPPER(A.Material_Group)='" + model.MaterialGroup.ToUpper() + "'");
                flag++;
            }

            if (model.QualityCode != "" && model.QualityCode != null)
            {
                if (flag == 0)
                    sb.Append("UPPER(A.Quality_Code)='" + model.QualityCode.ToUpper() + "'");
                else
                    sb.Append(" and UPPER(A.Quality_Code)='" + model.QualityCode.ToUpper() + "'");
                flag++;
            }
            if (model.DyeMethod != "" && model.DyeMethod != null)
            {
                if (flag == 0)
                    sb.Append("UPPER(A.Dye_Method)='" + model.DyeMethod.ToUpper() + "'");
                else
                    sb.Append(" and UPPER(A.Dye_Method)='" + model.DyeMethod.ToUpper() + "'");
                flag++;
            }
            if (model.Sales != "" && model.Sales != null)
            {
                if (flag == 0)
                    sb.Append("upper(D.Sales)='" + model.Sales.ToUpper() + "'");
                else
                    sb.Append(" and upper(D.Sales)='" + model.Sales.ToUpper() + "'");
                flag++;
            }

            if (model.SalesTeam != "" && model.SalesTeam != null)
            {
                if (flag == 0)
                    sb.Append("UPPER(D.Sales_Group)='" + model.SalesTeam.ToUpper() + "'");
                else
                    sb.Append(" and UPPER(D.Sales_Group)='" + model.SalesTeam.ToUpper() + "'");
                flag++;
            }

            if (model.Construction != "" && model.Construction != null)
            {
                if (flag == 0)
                    sb.Append("UPPER(B.Construction)='" + model.Construction.ToUpper() + "'");
                else
                    sb.Append(" and UPPER(B.Construction)='" + model.Construction.ToUpper() + "'");
                flag++;
            }

            if (model.Finishing != "" && model.Finishing != null)
            {
                if (flag == 0)
                    sb.Append("UPPER(C.Finishing_Code)='" + model.Finishing.ToUpper() + "'");
                else
                    sb.Append(" and UPPER(C.Finishing_Code)='" + model.Finishing.ToUpper() + "'");
                flag++;
            }
            // 添加对customer条件的判断 by LYH 2014/2/25
            if (model.CustomerCode != "" && model.CustomerCode != null)
            {
                if (flag == 0)
                    sb.Append("UPPER(D.BUYER_ID) = '" + model.CustomerCode.ToUpper() + "'");
                else
                    sb.Append(" and UPPER(D.BUYER_ID) = '" + model.CustomerCode.ToUpper() + "'");
                flag++;
            }

            if (flag == 0)
                return "";

            return Server.UrlEncode(sb.ToString());


            /*if (tempSql.Contains("@CreateTime"))
            {
                csql1.AddInputParameter("CreateTime", DbType.DateTime, model.CreateDate);
                csql2.AddInputParameter("CreateTime", DbType.DateTime, model.CreateDate);
            }
            if (tempSql.Contains("@BfGmmm"))
            {
                csql1.AddInputParameter("BfGmmm", DbType.Int32, model.BfGmmm);
                csql2.AddInputParameter("BfGmmm", DbType.Int32, model.BfGmmm);
            }
            if (tempSql.Contains("@Pattern"))
            {
                csql1.AddInputParameter("Pattern", DbType.String, model.Pattern.ToUpper());
                csql2.AddInputParameter("Pattern", DbType.String, model.Pattern.ToUpper());
            }
            if (tempSql.Contains("@Status"))
            {
                csql1.AddInputParameter("Status", DbType.String, model.Status.ToUpper());
                csql2.AddInputParameter("Status", DbType.String, model.Status.ToUpper());
            }
            if (tempSql.Contains("@Sourcing"))
            {
                csql1.AddInputParameter("Sourcing", DbType.String, model.Sourcing.ToUpper());
                csql2.AddInputParameter("Sourcing", DbType.String, model.Sourcing.ToUpper());
            }
            if (tempSql.Contains("@CustomerQualityId"))
            {
                csql1.AddInputParameter("CustomerQualityId", DbType.String, model.CustomerQaulityId.ToUpper());
                csql2.AddInputParameter("CustomerQualityId", DbType.String, model.CustomerQaulityId.ToUpper());
            }
            if (tempSql.Contains("@MaterialGroup"))
            {
                csql1.AddInputParameter("MaterialGroup", DbType.String, model.MaterialGroup.ToUpper());
                csql2.AddInputParameter("MaterialGroup", DbType.String, model.MaterialGroup.ToUpper());
            }
            if (tempSql.Contains("@QualityCode"))
            {
                csql1.AddInputParameter("QualityCode", DbType.String, model.QualityCode.ToUpper());
                csql2.AddInputParameter("QualityCode", DbType.String, model.QualityCode.ToUpper());
            }
            if (tempSql.Contains("@Creator"))
            {
                csql1.AddInputParameter("Creator", DbType.String, model.Creator.ToUpper());
                csql2.AddInputParameter("Creator", DbType.String, model.Creator.ToUpper());
            }
            if (tempSql.Contains("@DyeMethod"))
            {
                csql1.AddInputParameter("DyeMethod", DbType.String, model.DyeMethod.ToUpper());
                csql2.AddInputParameter("DyeMethod", DbType.String, model.DyeMethod.ToUpper());
            }
            if (tempSql.Contains("@Salesed"))
            {
                csql1.AddInputParameter("Salesed", DbType.String, model.Sales.ToUpper());
                csql2.AddInputParameter("Salesed", DbType.String, model.Sales.ToUpper());
            }
            if (tempSql.Contains("@SalesTeam"))
            {
                csql1.AddInputParameter("SalesTeam", DbType.String, model.SalesTeam.ToUpper());
                csql2.AddInputParameter("SalesTeam", DbType.String, model.SalesTeam.ToUpper());
            }
            if (tempSql.Contains("@Construction"))
            {
                csql1.AddInputParameter("Construction", DbType.String, model.Construction.ToUpper());
                csql2.AddInputParameter("Construction", DbType.String, model.Construction.ToUpper());
            }
            if (tempSql.Contains("@Finishing"))
            {
                csql1.AddInputParameter("Finishing", DbType.String, model.Finishing.ToUpper());
                csql2.AddInputParameter("Finishing", DbType.String, model.Finishing.ToUpper());
            }
            // 添加对customer条件的判断 by LYH 2014/2/25
            if (tempSql.Contains("@CustomerCode"))
            {
                csql1.AddInputParameter("CustomerCode", DbType.String, model.CustomerCode.ToUpper());
                csql2.AddInputParameter("CustomerCode", DbType.String, model.CustomerCode.ToUpper());
            }*/
        }
    }
}