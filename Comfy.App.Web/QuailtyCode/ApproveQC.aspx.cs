using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Comfy.App.Core.QualityCode;
using System.Data.Common;
using Comfy.App.Core;
using Comfy.App.Web.WebReference3;
using System.Text;
using System.Configuration;
using System.Data.OracleClient;
using System.Net;
using System.IO;

namespace Comfy.App.Web.QuailtyCode
{
    public partial class ApproveQC : MastPage
    {
        Attribute attribute = new Attribute();
        FlagAttribute flatAttribute = new FlagAttribute();
        TappingAttribute tapAttribute = new TappingAttribute();
        AvaWidth avaWidth = new AvaWidth();
        TextBox GekComments = new TextBox();
        QcmaininfoManager mainManager = new QcmaininfoManager();
        PbknityarntypeManager yarnManager = new PbknityarntypeManager();
        protected override string SessionName
        {
            get
            {
                //不同的流程返回不同的SessionName，避免多个页面共用一个Sessin而导致数据共享的问题
                return "ApproveSession";
            }

        }

        public string GetYarnInfo(string a)
        {
            List<YarnInfo> listYarn = new List<YarnInfo>();
            if (HttpContext.Current.Session[SessionName] != null)
            {
                string retStr = "";
                string des = "";
                listYarn = HttpContext.Current.Session[SessionName] == null ? new List<YarnInfo>() : (List<YarnInfo>)HttpContext.Current.Session[SessionName];
                foreach (YarnInfo yi in listYarn)
                {
                    PbknityarntypeModelList yarnModel = yarnManager.GetModelList(new PbknityarntypeModel() { YarnType = yi.YarnType });
                    if (yarnModel.Count > 0)
                        des = yarnModel[0].Description;
                    else
                        des = "";

                    retStr = retStr + yi.YarnType + "," + yi.YarnCount + "," + yi.Threads + ((des.ToLower().Contains("heather") || des.ToLower().Contains("htr")) ? "" : ("," + yi.Radio.ToString())) + ";";
                }
                return retStr;
            }
            else
            {
                return "";
            }
        }
        private static string strMGType;

        public static string StrMGType
        {
            get { return strMGType; }
            set { strMGType = value; }
        }
        public static QcmaininfoModelList qcMainLists { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HttpContext.Current.Session[SessionName] = null;
                if (HttpContext.Current.Session["UserPower"] != null)
                {
                    List<string> sl = (List<string>)HttpContext.Current.Session["UserPower"];
                    if (!sl.Contains("2"))
                    {
                        Response.Write("<script language='JavaScript'>window.location='NoPower.aspx';</script>");
                        return;
                    }
                }
                else
                {
                    Response.Write("<script language='JavaScript'>window.location='NoPower.aspx';</script>");
                    return;
                }
            }
            //根据横机、圆机或带子而调用不同的属性控件，因为它们的属性不一样。

            GekComments.Width = 433;
            GekComments.Height = 90;
            GekComments.ClientIDMode = ClientIDMode.Static;
            GekComments.ID = "GekComments";
            GekComments.TextMode = TextBoxMode.MultiLine;
            GekComments.Attributes.Add("style", "border:1px solid #FFA200");
          //  GekComments.BorderColor = System.Drawing.Color.FromArgb(255,162,0);
            string strMG = Request.QueryString["MG"] == null ? "Fabric" : Request.QueryString["MG"].ToString();
            StrMGType = strMG;
            if (strMG == "Fabric")//圆机
            {
                ViewState["MGType"] = "Fabric";
                attribute = (Attribute)Page.LoadControl("Attribute.ascx");
                avaWidth = (AvaWidth)Page.LoadControl("AvaWidth.ascx");
                this.CFTAttribute.Controls.Add(attribute);
                this.AvaPanel.Controls.Add(avaWidth);
                this.gridCustomerPanel.Controls.Add(GekComments);
            }
            else if (strMG == "FlatKnit" || strMG == "Flat Knit Fabric") //横机
            {
                strMG = "FlatKnit";
                ViewState["MGType"] = "FlatKnit";
                GekComments.Height = 45;
                GekComments.Width = 487;
                SalesComments.Height = 158;
                avaRemark.Visible = false;
                flatAttribute = (FlagAttribute)Page.LoadControl("FlagAttribute.ascx");
                this.CFTAttribute.Controls.Add(flatAttribute);
                this.AvaPanel.Controls.Add(GekComments);
                this.labAW.Text = "GEK Comments";
                this.labGKC.Visible = false;
            }
            else if (strMG == "Tapping") //带子
            {
                ViewState["MGType"] = "Tapping";
                GekComments.Height = 76;
                GekComments.Width = 487;
                avaRemark.Visible = false;
                tapAttribute = (TappingAttribute)Page.LoadControl("TappingAttribute.ascx");
                this.CFTAttribute.Controls.Add(tapAttribute);
                this.AvaPanel.Controls.Add(GekComments);
                this.labAW.Text = "GEK Comments";
                this.labGKC.Visible = false;
            }
            qcMainLists = new QcmaininfoManager().GetModelList(new QcmaininfoModel() { QualityCode = Request.QueryString["QC"] });
            //  ClientScript.RegisterClientScriptBlock(typeof(string), "js", "setGMType('" + strMG + "');", true);
            if (Request.QueryString["QC"] != null)
            {
                //如果是"另存为"的情况（即页面地址中QC的参数不为NUll），还需要初始化界面，初始化的方式：调用前台的setGMType 进行初始化。
                if (Request.QueryString["customerId"] != null)
                {
                    //kingzhang for support 735443 初始加载时传递Sales comment加载出来
                    string CustomerComment = new QcmaininfoManager().GetCustomerComment(Request.QueryString["QC"]);
                    ClientScript.RegisterClientScriptBlock(typeof(string), "js", "setGMType('" + strMG + "','" + Request.QueryString["QC"].ToString() + "','" + Request.QueryString["customerId"].ToString() + "','" + CustomerComment + "');", true);
                }
                else
                {
                    ClientScript.RegisterClientScriptBlock(typeof(string), "js", "setGMType('" + strMG + "','" + Request.QueryString["QC"].ToString() + "','','');", true);
                }
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(typeof(string), "js", "setGMType('" + strMG + "','','','');", true);
            }

        }

        public string GetSourcing(string QC)
        {
          QcmaininfoModelList mainList= mainManager.GetModelList(new QcmaininfoModel() { QualityCode = QC });
          if (mainList != null && mainList.Count > 0)
          {
              return mainList[0].Sourcing;
          }
          return "";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //分圆机、横机和带子进行不同信息的更新
/*
            if (ViewState["MGType"].ToString() == "Fabric")
            {
                UpdatedFabric();
            }
            else if (ViewState["MGType"].ToString() == "FlatKnit")
            {
                UpdatedFlat();
            }
            else if (ViewState["MGType"].ToString() == "Tapping")
            {
                UpdatedTapping();
            }*/
        }

        ////将Comment表的ViewFlag为C
        public string ShutDown(string QC)
        {
            try
            {
               string[] paramss = QC.Split(new string[]{",mn"},StringSplitOptions.None);
                CustomerManager cm  =new CustomerManager();
                cm.UpdateViewFlag("C",paramss[1],paramss[0],paramss[2],paramss[3],paramss[4],null);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "1";
        }

        public string ShutDownAndDisabled(string QC)
        {
            DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
            try
            {
                QcmaininfoModel main = new QcmaininfoModel();
                main.QualityCode = QC.Split(new string[] { ",mn" }, StringSplitOptions.None)[0];
                main.Status = "Disabled";
                main.LastUpdateBy = HttpContext.Current.Session["UserId"].ToString();
                main.LastUpdateTime = System.DateTime.Now;
                main.ApproveDate = System.DateTime.Now;
                main.Approver = HttpContext.Current.Session["UserId"].ToString();
                qcMainManager.ApproveAndShowDown(main, tran);

                string[] paramss = QC.Split(new string[] { ",mn" }, StringSplitOptions.None);
                CustomerManager cm = new CustomerManager();
                cm.UpdateViewFlag("C", paramss[1], paramss[0], paramss[2], paramss[3], paramss[4], tran);
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ex.Message;
            }
            return "1";
        }


        public string UnApprove(string QC)
        {
            DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
            try
            {
                string[] paramss = QC.Split(new string[] { ",mn" }, StringSplitOptions.None);
                string qcStr = "";
                //将状态变为New
                QcmaininfoModel main = new QcmaininfoModel();
                main.QualityCode = paramss[0];
                QcmaininfoModelList mfml =  qcMainManager.GetModelList(main);

                CustomerManager cm = new CustomerManager();
                //by mengjw 2015-08-04
                if (!cm.ISUnApprove(paramss[0],out qcStr))
                {
                    return "0" + qcStr;
                }

                if (mfml.Count == 0 || mfml[0].Status!="Approved")
                {
                    return "2";
                }
                if (cm.CanUnApprove(paramss[0], paramss[1], main.Creator, tran))
                {

                    //更新状态为New
                    main.LastUpdateBy = HttpContext.Current.Session["UserId"].ToString();
                    main.LastUpdateTime = System.DateTime.Now;
                    main.ApproveDate = System.DateTime.Now;
                    main.Approver = HttpContext.Current.Session["UserId"].ToString();
                    main.Status = "New";
                    main.Riskrade = "";
                    qcMainManager.ApproveAndShowDown(main, tran);
                    tran.Commit();
                }
                else
                {
                    return "3";//已经排单，不允许UnApprove
                }

            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ex.Message;
            }
            return "1";     
        }

        public string Approve(string QC)
        {
            DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
            try
            {
                 CustomerManager cm = new CustomerManager();

                string[] paramss = QC.Split(new string[] { ",mn" }, StringSplitOptions.None);
                //将状态变为Approved,同时写Comment表的ViewFlag为Y
                QcmaininfoModel main = new QcmaininfoModel();
                main.QualityCode = QC.Split(new string[] { ",mn" }, StringSplitOptions.None)[0];
                main.Status = "Approved";
                main.LastUpdateBy = HttpContext.Current.Session["UserId"].ToString();
                main.LastUpdateTime = System.DateTime.Now;
                main.ApproveDate = System.DateTime.Now;
                main.ReplaceBy = paramss[5];
                main.Approver = HttpContext.Current.Session["UserId"].ToString();

             
                //Add by sunny 2017 0720 Repeat 审批后改为第一次审批的色号
                if (paramss[6] != "")
                {
                    var Repeats = new QcmaininfoManager().GetSameApproveQC(paramss[6]);
                    main.Repeat = Repeats.ToString();
                }
                else
                {
                    main.Repeat = "";
                }

                string strGK_NO = cm.GetGK_NO(paramss[0], paramss[1]);
                main.GK_NO = strGK_NO;

               //20180823 linyob add FN-不洗水单缩率风险品种信息提醒SRF
                int getsuccess = 0;

                QcmaininfoModelList qcMainModelList = qcMainManager.GetModelList(new QcmaininfoModel() { QualityCode = main.QualityCode });
                if (qcMainModelList.Count == 0)
                {
                    return "没有找到该QC#数据";
                }
                QcmaininfoModel qcMainModel = qcMainModelList[0];

                List<YarnInfo> ListYarn = new List<YarnInfo>();
                if (HttpContext.Current.Session[SessionName] != null)
                {
                    ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
                }


                QcyarndtlModelList qcYarnList = CreateYarnDtl(ListYarn, main.QualityCode);


                string riskGrade = GetShrinkageRisk(qcMainModel, qcYarnList, ref getsuccess);
                main.Riskrade = riskGrade;
               //end
                qcMainManager.ApproveAndShowDown(main, tran);

                 
               // string[] paramss = QC.Split(new string[] { ",mn" }, StringSplitOptions.None);
               
                cm.UpdateViewFlag("Y", paramss[1], paramss[0], paramss[2], paramss[3],paramss[4] ,tran);


                cm.ReApprove(paramss[0], main.Creator, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ex.Message;
            }
            return "1";
        }
        /// <summary>
        /// kingzhang for support 参考编辑界面调用ESCM接口
        /// </summary>
        public string UpdateESCM_Ref(string data)
        {
            string strMG = StrMGType;       
            if (strMG == "Fabric")
            {
                try
                {
                    string[] strArrayStr = data.Replace("<>", "#").Split('#');
                    string qc = strArrayStr[0];
                    string remark_New = strArrayStr[15] == "null" ? "" : strArrayStr[15];
                    string HF_Ref_GP_New = strArrayStr[14] == "null" ? "" : strArrayStr[14];
                    string HF_Ref_PPO_New = strArrayStr[13] == "null" ? "" : strArrayStr[13];
                    string QC_Ref_GP_New = strArrayStr[12] == "null" ? "" : strArrayStr[12];
                    string QC_Ref_PPO_New = strArrayStr[11] == "null" ? "" : strArrayStr[11];                

                    string HF_Ref_GP_Old = qcMainLists[0].HF_Ref_GP;
                    string HF_Ref_PPO_Old = qcMainLists[0].HF_Ref_PPO;
                    string QC_Ref_GP_Old = qcMainLists[0].QC_Ref_GP;
                    string QC_Ref_PPO_Old = qcMainLists[0].QC_Ref_PPO;               
                    string remark_Olds = qcMainLists[0].RF_Remark;
                    if (HF_Ref_GP_New != HF_Ref_GP_Old || HF_Ref_PPO_New != HF_Ref_PPO_Old || QC_Ref_GP_New != QC_Ref_GP_Old || QC_Ref_PPO_New != QC_Ref_PPO_Old || remark_New != remark_Olds)
                    {
                        //创建和数据库的连接
                        string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                        OracleConnection oraCon = new OracleConnection(connString);
                        //打开连接
                        oraCon.Open();
                        OracleCommand oraCmd = new OracleCommand();
                        //新建一个事务对象的实例
                        OracleTransaction oraTact = oraCon.BeginTransaction();
                        oraCmd.Connection = oraCon;
                        //绑定事务对象到命令
                        oraCmd.Transaction = oraTact;
                        try
                        {
                            //将一个表的满足某条件的行的指定的列插入到另一个表
                            oraCmd.CommandText = "INSERT INTO ESCMOWNER.QCMAININFO_SYNC_LOG(SYNC_LOG_ID,QUALITY_CODE,SYNC_TO_ESCM_FLAG,CREATE_USER_ID,CREATE_DATE) SELECT QCMAININFO_SYNC_LOG_SEQ.NEXTVAL,'" + qc.Trim().ToString() + "','N','" + HttpContext.Current.Session["UserId"].ToString() + "',SYSDATE FROM DUAL";
                            int r = oraCmd.ExecuteNonQuery();

                            r = 10;
                            //没有错误，执行提交命令
                            oraTact.Commit();
                        }
                        catch (Exception ex)
                        {
                            //出现错误，执行回滚命令
                            oraTact.Rollback();
                            //弹出窗口显示错误
                            Response.Write("<script>alert('" + ex.Message + "')</script>");
                        }
                        finally
                        {
                            //关闭连接
                            oraCon.Close();
                        }

                        //插入数据库完毕后做调用webserver
                        // webservice调用地址
                        //string url = "http://192.168.27.80/YPD_DEV/YPDWebService.asmx?op=SyncQualityHandfeelReference";  //测试地址
                        string url = "http://192.168.7.187/YPD/YPDWebService.asmx?op=SyncQualityHandfeelReference";   //正式地址 

                        // SOAP格式内容，参数为：http://www.what21.com
                        StringBuilder param = new StringBuilder();
                        param.Append("<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\" > ");
                        param.Append("<soap12:Body>");
                        param.Append("<SyncQualityHandfeelReference xmlns=\"http://tempuri.org/\">");
                        param.Append("<qualityCodes>" + qc.Trim().ToString() + "</qualityCodes>");
                        param.Append("<userId>" + HttpContext.Current.Session["UserId"].ToString() + "</userId>");
                        param.Append("</SyncQualityHandfeelReference>");
                        param.Append("</soap12:Body>");
                        param.Append("</soap12:Envelope>");

                        try
                        {
                            // 创建HttpWebRequest对象
                            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
                            // 设置POST调用方法
                            httpRequest.Method = "POST";
                            // 设置HTTP头ContentType
                            //httpRequest.ContentType = "application/soap+xml;charset=UTF-8;action=\"http://192.168.27.80/YPD_DEV/YPDWebService.asmx?op=SyncQualityHandfeelReference\"";  //测试地址
                            httpRequest.ContentType = "application/soap+xml;charset=UTF-8;action=\"http://192.168.7.187/YPD/YPDWebService.asmx?op=SyncQualityHandfeelReference\"";   //正式地址 
                            // 设置HTTP头SOAPAction的值
                            httpRequest.Headers.Add("SOAPAction", "urn:world");
                            // 调用内容
                            byte[] bytes = Encoding.UTF8.GetBytes(param.ToString());
                            // 设置HTTP头内容的长度
                            httpRequest.ContentLength = param.ToString().Length;
                            using (Stream reqStream = httpRequest.GetRequestStream())
                            {
                                reqStream.Write(bytes, 0, bytes.Length);
                                reqStream.Flush();
                            }
                            // HttpWebRequest发起调用
                            using (HttpWebResponse myResponse = (HttpWebResponse)httpRequest.GetResponse())
                            {
                                // StreamReader对象
                                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                                // 返回结果
                                string responseString = sr.ReadToEnd();
                                Console.WriteLine("调用结果" + responseString);
                            }
                        }
                        catch (Exception ex)
                        {
                            //弹出窗口显示错误
                            Response.Write("<script>alert('" + ex.Message + "')</script>");
                        }

                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('" + ex.Message + "')</script>");
                }
                //added by hejianh 2020-04-20 对接系统质量&手感参办自动更新
            }
            return "1";
        }
        //调用WebService

        public string CallWS_AX(string Params)
        {
            try
            {
                if (Params.Contains("E"))
                {
                    return "The code is not internal, don't need to call AX";
                }

                QccustomerlibraryManager cumanager = new QccustomerlibraryManager();
                QcmaininfoManager qcMainManager = new QcmaininfoManager();
                QcconstructiondtlManager qcConManager = new QcconstructiondtlManager();
                QcyarndtlManager yarnManager = new QcyarndtlManager();
                QcfinishdtlManager finishManager = new QcfinishdtlManager();
                QcavailablewidthManager avaManager = new QcavailablewidthManager();


                QcmaininfoModelList qcMainModelList = qcMainManager.GetModelList(new QcmaininfoModel() { QualityCode = Params });
                if (qcMainModelList.Count == 0)
                {
                    return "没有找到该QC#数据";
                }
                QcmaininfoModel qcMainModel = qcMainModelList[0];

                if (qcMainModel.DyeMethod == "YD")
                {
                    return "DyeMethod is YD,don't need to Call AX";
                }

                if (qcMainModel.Status != "Approved")
                {
                    return "The status is not Approved,don't need to Call AX";
                }

                QcconstructiondtlModelList qcConstructionList = qcConManager.GetModelList(new QcconstructiondtlModel() { QualityCode = Params });
                QcyarndtlModelList qcYarnList = yarnManager.GetModelList(new QcyarndtlModel() { QualityCode = Params });
                QcfinishdtlModelList qcFinfishing = finishManager.GetModelList(new QcfinishdtlModel() { QualityCode = Params });
                QcavailablewidthModelList qcAvaWidthList = avaManager.GetModelList(new QcavailablewidthModel() { QualityCode = Params });
                QccustomerlibraryModelList customerList = cumanager.GetModelList(new QccustomerlibraryModel() { QualityCode = qcMainModel.QualityCode });


                WebReference3.TEX_AxdInventTable data = new WebReference3.TEX_AxdInventTable();

                //Customer

                List<WebReference3.TEX_AxdQCCustomerLibrary> customer = new List<TEX_AxdQCCustomerLibrary>();
                for (int i = 0; i < customerList.Count; i++)
                {
                    customer.Add(new TEX_AxdQCCustomerLibrary()
                    {
                        BrandId = customerList[i].Brand,
                        CustBuyerId = customerList[i].BuyerId,
                        CustMillComments = customerList[i].MillComments,
                        CustQualityId = customerList[i].CustomerQualityId,
                        CustSalesGroup = customerList[i].SalesGroup,
                        CustSalesId = customerList[i].Sales,
                       IsFirstOwnerStr = (customerList[i].IsFirstOwner == "Y" ? "Yes" :"No"),
                       
                    // IsFirstOwnerStr="YES",
                        ItemId = customerList[i].QualityCode


                    });
                }
                if (customer.Count > 0)
                    data.CustomerLibrary = customer.ToArray();

                //Finish
                List<WebReference3.TEX_AxdQCFinishDetails> finish = new List<WebReference3.TEX_AxdQCFinishDetails>();
                for (int i = 0; i < qcFinfishing.Count; i++)
                {
                    finish.Add(new WebReference3.TEX_AxdQCFinishDetails() { FinishingCode = qcFinfishing[i].FinishingCode, ItemId = qcFinfishing[i].QualityCode });
                }

                if(finish.Count>0)
                   data.FinishDetails = finish.ToArray();
                //Yarn
                List<WebReference3.TEX_AxdQCYarnDetails> yarnD = new List<WebReference3.TEX_AxdQCYarnDetails>();
                for (int i = 0; i < qcYarnList.Count; i++)
                {
                    yarnD.Add(new TEX_AxdQCYarnDetails()
                    {
                        YarnRatio = qcYarnList[i].YarnRatio,
                        ItemId = qcYarnList[i].QualityCode,
                        YarnThreads = qcYarnList[i].Threads,
                        YarnTypeId = qcYarnList[i].YarnType,
                        YarnCountId = qcYarnList[i].YarnCount,
                        YarnCompositionId = qcYarnList[i].YarnComponent,
                        YarnDensity = qcYarnList[i].YarnDensity

                    });
                }
                if (yarnD.Count > 0)
                    data.YarnDetails = yarnD.ToArray();
                //Construction
                List<WebReference3.TEX_AxdQCConstructionDetails> ConList = new List<WebReference3.TEX_AxdQCConstructionDetails>();
                for (int i = 0; i < qcConstructionList.Count; i++)
                {
                    ConList.Add(new TEX_AxdQCConstructionDetails() { ConstructionId = qcConstructionList[i].Construction, ItemId = qcConstructionList[i].QualityCode });

                }
                if (ConList.Count > 0)
                    data.ConstructionDetails = ConList.ToArray();

                //AvaWith
                if (qcAvaWidthList.Count > 0)
                { 
                     TEX_AxdQCAvailableWidth[] avaList = new TEX_AxdQCAvailableWidth[qcAvaWidthList.Count];
                     TEX_AxdQCAvailableWidth aw = null;
                     for (int i = 0; i < qcAvaWidthList.Count; i++)
                     {
                         aw = new TEX_AxdQCAvailableWidth();
                         aw.ItemId = qcAvaWidthList[i].QualityCode;
                         aw.Gauge = qcAvaWidthList[i].Gauge;
                         aw.Diameter = qcAvaWidthList[i].Diameter;
                         aw.TotalNeedles = qcAvaWidthList[i].TotalNeedles;
                         aw.Width = qcAvaWidthList[i].Width;
                         aw.MaxWidth = qcAvaWidthList[i].MaxWidth;
                         avaList.SetValue(aw, i);
                     }
                     data.AvailableWidth = avaList;
                }
                /*
                List<WebReference3.TEX_AxdQCAvailableWidth> avaList = new List<WebReference3.TEX_AxdQCAvailableWidth>();
                
                for (int i = 0; i < qcAvaWidthList.Count; i++)
                {
                    avaList.Add(new TEX_AxdQCAvailableWidth()
                    {
                        ItemId = qcAvaWidthList[i].QualityCode,
                        Gauge = qcAvaWidthList[i].Gauge,
                        Diameter = qcAvaWidthList[i].Diameter,
                        TotalNeedles = qcAvaWidthList[i].TotalNeedles,
                        Width = qcAvaWidthList[i].Width,
                        MaxWidth = qcAvaWidthList[i].MaxWidth

                    });

                }
                if (avaList.Count > 0)
                    data.AvailableWidth = avaList.ToArray();*/

                data.ItemId = qcMainModel.QualityCode;
                data.PatternId = qcMainModel.Pattern;
                data.DyeMethodId = qcMainModel.DyeMethod;
                data.GramWeightBeforeWash = qcMainModel.BfGmmm;
                data.GramWeightAfterWash = qcMainModel.AfGmmm;
                data.Shrinkage = qcMainModel.Shrinkage;
                data.InventStatus =TEX_InventStatus.Approved;
                data.ShrinkTestMethod = qcMainModel.ShrinkageTestingMethod;
               // data.IsGarmentWash = (qcMainModel.GmtWashing == "Y" ? NoYes.Yes : NoYes.No);
                data.IsGarmentWashStr = (qcMainModel.GmtWashing == "Y" ? "Yes" : "No");
                data.IsGarmentWashStr = "Yes";
                data.InventLayout = qcMainModel.Layout;
                data.Remark = qcMainModel.Remark;
                data.ApprovalDateTime = qcMainModel.ApproveDate;
                data.Approver = qcMainModel.Approver;
                data.InventSourcing = qcMainModel.Sourcing;
                data.MaterialGroup = qcMainModel.MaterialGroup;
                data.KnitAnalysisNum = qcMainModel.AnalysisNo;
                data.RefQualityCode = qcMainModel.RefQualityCode;
                data.TappingType = qcMainModel.TappingType;
                data.TappingMeasurement = qcMainModel.Measurement;
                data.YarnLength = qcMainModel.YarnLength;
                data.SpecialType = qcMainModel.SpecialType;


                WebReference3.portTypeClient qc = new WebReference3.portTypeClient();

               
                if (qc.TransQCOp(data))
                    return "Sucess";
                else
                    return "Fail";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //更新圆机信息
        public string  UpdatedFabric(string Params)
        {

        
            DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
            string[] strsParam = Params.Split(new string[] { "<>" }, StringSplitOptions.None);
            List<YarnInfo> ListYarn=new List<YarnInfo>() ;
            if (HttpContext.Current.Session[SessionName] != null)
            {
                ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
            }

            QcmaininfoModel qcMainModel = CreateMain(strsParam);
            QcconstructiondtlModelList qcConstructionList = CreateConstruction(strsParam[1],strsParam[0]);
            QcyarndtlModelList qcYarnList = CreateYarnDtl(ListYarn,strsParam[0]);
            QcfinishdtlModelList qcFinfishing = CreateFinishing(strsParam[9],strsParam[0]);
            QcavailablewidthModelList qcAvaWidthList = CreateAvaWidth(strsParam[16],strsParam[0]);//11 变16
            QccustomerlibraryModel qcCustomer = CreateCustomer(strsParam[0],strsParam[17],strsParam[18]);// 12 变17 13 变18 

           // return;
            try
            {
                qcMainManager.UpdateModel(qcMainModel, tran);

                qcConstructionManager.DeleteModel(new QcconstructiondtlModel() { QualityCode = strsParam[0] }, tran);
                qcConstructionManager.AddModels(qcConstructionList, tran);

                qcYarnManager.DeleteModel(new QcyarndtlModel() { QualityCode = strsParam[0] }, tran);
                qcYarnManager.AddModels(qcYarnList, tran);

                qcFinishManager.DeleteModel(new QcfinishdtlModel() { QualityCode = strsParam[0] }, tran);
                qcFinishManager.AddModels(qcFinfishing, tran);

                qcAvaWidthManager.DeleteModel(new QcavailablewidthModel() { QualityCode = strsParam[0] }, tran);
                qcAvaWidthManager.AddModels(qcAvaWidthList, tran);

                qcCustomerManager.UpdateModelOne(qcCustomer, tran);
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ex.Message;
            }

            return "";       
        }
        //调用WebService Of 横机
        public string CallWSOfFlat(string Params)
        {
            string[] strsParam = Params.Split(new string[] { "<>" }, StringSplitOptions.None);
            // CAttribute cAttribute = flatAttribute.GetCAttributeValue();
            List<YarnInfo> ListYarn = new List<YarnInfo>();
            if (HttpContext.Current.Session[SessionName] != null)
            {
                ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
            }

            QcmaininfoModel qcMainModel = CreateFlatMain(strsParam);
            QcfinishdtlModelList qcFinfishing = CreateFinishing(strsParam[4], strsParam[0]);
            QcconstructiondtlModelList qcConstructionList = CreateConstruction(strsParam[1], strsParam[0]);
            QcyarndtlModelList qcYarnList = CreateYarnDtl(ListYarn, strsParam[0]);
            QccustomerlibraryModel qcCustomer = CreateCustomer(strsParam[0], strsParam[7], strsParam[8]);
            return "";
        }

        //更新横机信息
        public string UpdatedFlat(string Params)
        {
          
            string[] strsParam = Params.Split(new string[] { "<>" }, StringSplitOptions.None);
            DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
           // CAttribute cAttribute = flatAttribute.GetCAttributeValue();
            List<YarnInfo> ListYarn = new List<YarnInfo>();
            if (HttpContext.Current.Session[SessionName] != null)
            {
                ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
            }

            QcmaininfoModel qcMainModel = CreateFlatMain(strsParam);
            QcfinishdtlModelList qcFinfishing = CreateFinishing(strsParam[4],strsParam[0]);
            QcconstructiondtlModelList qcConstructionList = CreateConstruction(strsParam[1],strsParam[0]);
            QcyarndtlModelList qcYarnList = CreateYarnDtl(ListYarn,strsParam[0]);
            QccustomerlibraryModel qcCustomer = CreateCustomer(strsParam[0],strsParam[12],strsParam[13]);

         

            try
            {
                qcMainManager.UpdateModelFlat(qcMainModel, tran);

                qcFinishManager.DeleteModel(new QcfinishdtlModel() { QualityCode = strsParam[0] }, tran);
                qcFinishManager.AddModels(qcFinfishing, tran);

                qcConstructionManager.DeleteModel(new QcconstructiondtlModel() { QualityCode = strsParam[0] }, tran);
                qcConstructionManager.AddModels(qcConstructionList, tran);

                qcYarnManager.DeleteModel(new QcyarndtlModel() { QualityCode = strsParam[0] }, tran);
                qcYarnManager.AddModels(qcYarnList, tran);


                qcCustomerManager.UpdateModelOne(qcCustomer, tran);
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ex.Message;
            }
            return "";
        }

        //调用WebService of Tapping
        public string CallWSOfTapping(string Params)
        {
            string[] strsParam = Params.Split(new string[] { "<>" }, StringSplitOptions.None);
            List<YarnInfo> ListYarn = new List<YarnInfo>();
            if (HttpContext.Current.Session[SessionName] != null)
            {
                ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
            }

            QcmaininfoModel qcMainModel = CreateTappingMain(strsParam);
            QcyarndtlModelList qcYarnList = CreateYarnDtl(ListYarn, strsParam[0]);
            QccustomerlibraryModel qcCustomer = CreateCustomer(strsParam[0], strsParam[5], strsParam[6]);

            return "";
        }
        //更新带子信息
        public string UpdatedTapping(string Params)
        {

         

            string[] strsParam = Params.Split(new string[] { "<>" }, StringSplitOptions.None);
            DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
          //  CAttribute cAttribute = tapAttribute.GetCAttributeValue();
            List<YarnInfo> ListYarn = new List<YarnInfo>();
            if (HttpContext.Current.Session[SessionName] != null)
            {
                ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
            }

            QcmaininfoModel qcMainModel = CreateTappingMain(strsParam);
            QcyarndtlModelList qcYarnList = CreateYarnDtl(ListYarn,strsParam[0]);
            QccustomerlibraryModel qcCustomer = CreateCustomer(strsParam[0],strsParam[10],strsParam[11]);

           
          
            try
            {
                qcMainManager.UpdateModelTapping(qcMainModel, tran);

                qcYarnManager.DeleteModel(new QcyarndtlModel() { QualityCode = strsParam[0] }, tran);
                qcYarnManager.AddModels(qcYarnList, tran);


                qcCustomerManager.UpdateModelOne(qcCustomer, tran);
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ex.Message;
            }
            return "";
          //  Response.Write("<script language='JavaScript'>alert('Successfully updated！');window.location='ApproveQC.aspx?MG=Tapping';</script>");
        }

        //根据页面信息产生各个Model
        public QcmaininfoModel CreateMain(string[]  strParams)
        {
            QcmaininfoModel qcMainModel = new QcmaininfoModel();
            qcMainModel.QualityCode = strParams[0];
            qcMainModel.LastUpdateTime = System.DateTime.Now;
            qcMainModel.Pattern = strParams[5];
            qcMainModel.DyeMethod = strParams[4];
            qcMainModel.BfGmmm = Convert.ToInt32(strParams[2] == "" ? "0" : strParams[2]);
            qcMainModel.AfGmmm = Convert.ToInt32(strParams[3] == "" ? "0" : strParams[3]);
            qcMainModel.Shrinkage = strParams[8];
            qcMainModel.GmtWashing = strParams[7];
            qcMainModel.Layout = strParams[10];

            //todo by mengjw
            //kingzhang for support 735101 PPO为空时，GP数据也为空
            //begin
            qcMainModel.QC_Ref_PPO = strParams[11];
            if (!string.IsNullOrEmpty(qcMainModel.QC_Ref_PPO))
                qcMainModel.QC_Ref_GP = strParams[12] == "null" ? "" : strParams[12];
            else
                qcMainModel.QC_Ref_GP = "";

            qcMainModel.HF_Ref_PPO = strParams[13];
            if (!string.IsNullOrEmpty(qcMainModel.HF_Ref_PPO))
                qcMainModel.HF_Ref_GP = strParams[14] == "null" ? "" : strParams[14];
            else
                qcMainModel.HF_Ref_GP = "";
            //end
            qcMainModel.RF_Remark = strParams[15] == "null" ? "" : strParams[15];

            qcMainModel.Remark = strParams[19];//old 16
            qcMainModel.ShrinkageTestingMethod = strParams[6];
            return qcMainModel;
        }


        public QcmaininfoModel CreateFlatMain(string[] Params)
        {
            QcmaininfoModel qcMainModel = new QcmaininfoModel();
            qcMainModel.QualityCode = Params[0];//
            qcMainModel.LastUpdateTime = System.DateTime.Now;
            qcMainModel.Pattern = Params[2];
            qcMainModel.Layout = Params[6];
            qcMainModel.SpecialType = Params[5];
            qcMainModel.YarnLength = Params[3];

            //todo by mengjw
            //kingzhang for support 735101 PPO为空时，GP数据也为空
            //begin
            qcMainModel.QC_Ref_PPO = Params[7];
            if (!string.IsNullOrEmpty(qcMainModel.QC_Ref_PPO))
                qcMainModel.QC_Ref_GP = Params[8] == "null" ? "" : Params[8];
            else
                qcMainModel.QC_Ref_GP = "";

            qcMainModel.HF_Ref_PPO = Params[9];
            if (!string.IsNullOrEmpty(qcMainModel.HF_Ref_PPO))
                qcMainModel.HF_Ref_GP = Params[10] == "null" ? "" : Params[10];
            else
                qcMainModel.HF_Ref_GP = "";
            //end
            qcMainModel.RF_Remark = Params[11] == "null" ? "" : Params[11];
            return qcMainModel;
        }


        public QcmaininfoModel CreateTappingMain(string[] Params)
        {
            QcmaininfoModel qcMainModel = new QcmaininfoModel();
            qcMainModel.QualityCode = Params[0];//
            qcMainModel.LastUpdateTime = System.DateTime.Now;
            qcMainModel.Measurement = Params[2];
            qcMainModel.YarnLength = Params[1];
            qcMainModel.Layout = Params[4];
            qcMainModel.TappingType = Params[3];

            //todo by mengjw
            //kingzhang for support 735101 PPO为空时，GP数据也为空
            //begin
            qcMainModel.QC_Ref_PPO = Params[5];
            if (!string.IsNullOrEmpty(qcMainModel.QC_Ref_PPO))
                qcMainModel.QC_Ref_GP = Params[6] == "null" ? "" : Params[6];
            else
                qcMainModel.QC_Ref_GP = "";

            qcMainModel.HF_Ref_PPO = Params[7];
            if (!string.IsNullOrEmpty(qcMainModel.HF_Ref_PPO))
                qcMainModel.HF_Ref_GP = Params[8] == "null" ? "" : Params[8];
            else
                qcMainModel.HF_Ref_GP = "";
            //end
            qcMainModel.RF_Remark = Params[9] == "null" ? "" : Params[9];
            return qcMainModel;
        }
        public QcconstructiondtlModelList CreateConstruction(string Construction,string QC)
        {
            QcconstructiondtlModelList qcConstructionModelList = new QcconstructiondtlModelList();
            if (Construction != "")
            {
                string[] strTemp = Construction.Split(new string[] { "," }, StringSplitOptions.None);
                for (int i = 0; i < strTemp.Length; i++)
                {
                    if (strTemp[i] != "")
                    {
                        QcconstructiondtlModel qcConstructionModel = new QcconstructiondtlModel();
                        qcConstructionModel.Construction = strTemp[i];
                        qcConstructionModel.QualityCode = QC;
                        qcConstructionModelList.Add(qcConstructionModel);
                    }
                }

            }
            return qcConstructionModelList;
        }

        public QcyarndtlModelList CreateYarnDtl(List<YarnInfo> ListYarn,string QC)
        {
            QcyarndtlModelList qcYarnList = new QcyarndtlModelList();
            for (int i = 0; i < ListYarn.Count; i++)
            {
                QcyarndtlModel qcYarnModel = new QcyarndtlModel();
                qcYarnModel.YarnRatio = ListYarn[i].Radio == 0 ? 100 : ListYarn[i].Radio;
                qcYarnModel.Threads = ListYarn[i].Threads;
                qcYarnModel.YarnType = ListYarn[i].YarnType;
                qcYarnModel.YarnCount = ListYarn[i].YarnCount;
                qcYarnModel.WarpWeft = ListYarn[i].WarpWeft;
                qcYarnModel.YarnDensity = ListYarn[i].YarnDensity;
                qcYarnModel.YarnComponent = ListYarn[i].YarnComponent;
                qcYarnModel.QualityCode = QC;
                qcYarnList.Add(qcYarnModel);

            }
            return qcYarnList;
        }

        public QcfinishdtlModelList CreateFinishing(string Finish,string QC)
        {
            QcfinishdtlModelList qcfinishingList = new QcfinishdtlModelList();
            if (string.IsNullOrEmpty(Finish))
            {
                return qcfinishingList;
            }
            string[] strTemp = Finish.Split(new string[] { ";" }, StringSplitOptions.None);
            for (int i = 0; i < strTemp.Length; i++)
            {
                if (string.IsNullOrEmpty(strTemp[i]))
                {
                    continue;
                }
                QcfinishdtlModel qcFinishModel = new QcfinishdtlModel();
                qcFinishModel.QualityCode = QC;
                qcFinishModel.FinishingCode = strTemp[i];
                qcfinishingList.Add(qcFinishModel);
            }
            return qcfinishingList;
        }

        public QcavailablewidthModelList CreateAvaWidth(string strAva,string QC)
        {
            QcavailablewidthModelList qcAvaWidthList = new QcavailablewidthModelList();
            if (string.IsNullOrEmpty(strAva))
            {
                return qcAvaWidthList;
            }

            string[] strP = strAva.Split(new string[] { "()" }, StringSplitOptions.None);

            for (int i = 0; i < strP.Length; i++)
            {
                if (string.IsNullOrEmpty(strP[i]))
                {
                    continue;
                }
                string[] strss = strP[i].Split(new string[] { ";" }, StringSplitOptions.None);
                QcavailablewidthModel qcAvaModel = new QcavailablewidthModel();
                qcAvaModel.Gauge = Convert.ToInt32(strss[0] == "" ? "0" : strss[0]);
                qcAvaModel.Diameter = Convert.ToInt32(strss[1] == "" ? "0" : strss[1]);
                qcAvaModel.TotalNeedles = Convert.ToInt32(strss[2] == "" ? "0" : strss[2]);
                qcAvaModel.Width = Convert.ToInt32(strss[3] == "" ? "0" : strss[3]);
                qcAvaModel.MaxWidth = Convert.ToInt32(strss[4] == "" ? "0" : strss[4]);
                qcAvaModel.QualityCode = QC;
                qcAvaModel.UpdatedBy = HttpContext.Current.Session["UserId"].ToString();
                qcAvaModel.UpdatedTime = System.DateTime.Now;
                qcAvaWidthList.Add(qcAvaModel);
            }
            return qcAvaWidthList;
        }
        //获取客户信息
        public QccustomerlibraryModel CreateCustomer(string QC,string BI,string GC)
        {
            QccustomerlibraryModel qcCustomerModel = new QccustomerlibraryModel();
            qcCustomerModel.QualityCode = QC;
            qcCustomerModel.BuyerId = BI;
            qcCustomerModel.MillComments = GC;
            return qcCustomerModel;
        }
        //kingzhang for support  740959 根据QC获取是否可以审批
        public string GetApproveStaus(string qc) {
            string ApproveStaus = new QcmaininfoManager().GetApproveStaus(qc);
            return ApproveStaus;
        }
        //获取GEKComment
        public string GetGekComment(string codeAndCustomerId)
        {
            string result = "";
            CustomerManager cm = new CustomerManager();
            string[] strs = codeAndCustomerId.Split(new string[] { "," }, StringSplitOptions.None);

            result = cm.GetGekComment(strs[0], strs[1]);
            if (!string.IsNullOrEmpty(result))
            {
                if (result.Substring(result.Length - 1, 1) != "\n")
                {
                    result = result + "\n";
                }
            }
            return result;
        }

        //获取GEKComment by quality code --------add by zheng zhou
        public string GetGekCommentAndCustomerQualityId_ByCode(string code)
        {
            string result = "";
            CustomerManager cm = new CustomerManager();
            result = cm.GetGekCommentAndCustomerQualityId_ByCode(code);

            return result;
        }

        //根据QualityCode查询QC_Ref_PPO信息和HF_Ref_PPO信息--------add by zheng zhou 2016-8-11
        public string GetRefByQC(string strQCandPPONO)
        {
            string result = "";
            string[] strs = strQCandPPONO.Split(new string[]{","}, StringSplitOptions.None);
            QcmaininfoManager qm = new QcmaininfoManager();
            result = qm.GetRefByQC(strs[0], strs[1]);

            return result;  
        }



        //Add by sunny  Edit编译保存时判断ratio是否符合逻辑规则
        public string VerificationRatioSave(string Params)
        {
            List<YarnInfo> ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
            decimal Ratio = 0;
            for (int i = 0; i < ListYarn.Count; i++)
            {
                if (ListYarn[i].Radio.ToString() == "")
                {

                    return "FalseOne";

                }
                else
                {
                    Ratio = Ratio + decimal.Parse(ListYarn[i].Radio.ToString());
                }
            }
            if (Ratio != 100)
            {
                return "FalseTwo";
            }

            return "";


        }




        private string GetShrinkageRisk(QcmaininfoModel qcmain,QcyarndtlModelList yarn,ref int success )
        {

            try
            {
                if (qcmain.QualityCode == null || qcmain.QualityCode == "")
                {
                    success = 1;
                    return "";
                }
                if (qcmain.QualityCode.Substring(0, 1) != "C")
                {
                    success = 1;
                    return "";
                }

                QcmaininfoManager qm = new QcmaininfoManager();
                string wash= qcmain.GmtWashing==null?"": qcmain.GmtWashing;
                string rtn = qm.GetShrinkageRisk(qcmain.QualityCode, qcmain.Shrinkage, qcmain.ShrinkageTestingMethod, yarn, wash, ref success);
               
                return rtn;
            }
            catch(Exception ex) {
               
                success = 1;
                return ex.Message;
            }
        }
      
    }
}