using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Comfy.App.Core.QualityCode;
using System.Data.Common;
using Comfy.App.Core;
using System.Text;

namespace Comfy.App.Web.QuailtyCode
{
    public partial class CreateQC : MastPage
    {
        string qcCode=null;
        Attribute attribute = new Attribute();
        FlagAttribute flatAttribute = new FlagAttribute();
        TappingAttribute tapAttribute = new TappingAttribute();
        AvaWidth avaWidth = new AvaWidth();
        CustomerManager cuManager = new CustomerManager();
        QcmaininfoManager qmManager = new QcmaininfoManager();
        PbknityarntypeManager yarnManager = new PbknityarntypeManager();
        QcconstructiondtlManager qcm = new QcconstructiondtlManager();
        QcfinishdtlManager qsm = new QcfinishdtlManager();
        QcconstructiondtlModelList qcml = new QcconstructiondtlModelList();
        QcfinishdtlModelList qsml = new QcfinishdtlModelList();
        QcyarndtlManager qym = new QcyarndtlManager();
        QcyarndtlModelList qyml = new QcyarndtlModelList();

        public string strQC
        {
            get {
                if (Session["strQC"]!=null)
                {
                    return Session["strQC"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                HttpContext.Current.Session[SessionName] = null;
                string isEscmCreate = Request.QueryString["ESCM"] == null ? "" : Request.QueryString["ESCM"].ToString();
                string isCheck = Request.QueryString["Search"] == null ? "" : Request.QueryString["Search"].ToString();
                if (isEscmCreate == "1" && Request.QueryString["userId"] != null)
                {
                    HttpContext.Current.Session.Timeout = 60;
                    HttpContext.Current.Session["UserId"] = Request.QueryString["userId"].ToString();
                    btnSaveQC.Enabled = false;
                    btnEditQC.Enabled = false;
                    cancel.Enabled = false;
                    btnQcSearch.Enabled = false;
                    if (Request.QueryString["CustomerId"] != null)
                    {
                        tchange.Value = Request.QueryString["CustomerId"];
                        customer.Text = Request.QueryString["CustomerId"];
                    }

                }
                 else if(isCheck=="1")
                {
                    divButton.Visible = false;
                    rowSearch.Visible = false;
                 }

                else
                {

                    if (HttpContext.Current.Session["UserPower"] != null)
                    {
                        List<string> sl = (List<string>)HttpContext.Current.Session["UserPower"];
                        if (!sl.Contains("1"))
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
            }

            //根据横机、圆机或带子而调用不同的属性控件，因为它们的属性不一样。

            string strMG=Request.QueryString["MG"]==null?"Fabric":Request.QueryString["MG"].ToString();
            if (Request.QueryString["MG"] == null && Request.QueryString["QC"] != null)
            {
                if (Request.QueryString["QC"].ToString().Contains("C"))
                {
                    strMG = "Fabric";
                }
                else if (Request.QueryString["QC"].ToString().Contains("F"))
                {
                    strMG = "FlatKnit";
                }
                else
                {
                    strMG = "Tapping";
                }
            }
            if (strMG=="Fabric")  //圆机
            {
                ViewState["MGType"] = "Fabric";
                attribute = (Attribute)Page.LoadControl("Attribute.ascx");
                avaWidth = (AvaWidth)Page.LoadControl("AvaWidth.ascx");
                this.attrPanel.Controls.Add(attribute);
                this.avaWidthPanel.Controls.Add(avaWidth);
            }
            else if (strMG == "FlatKnit") //横机
            {
                ViewState["MGType"] = "FlatKnit";
                flatAttribute = (FlagAttribute)Page.LoadControl("FlagAttribute.ascx");
                this.attrPanel.Controls.Add(flatAttribute);
                this.avaRemark.Visible = false;
                this.Label5.Visible = false;
                this.GEKComments.Height = 360;
            }
            else if(strMG=="Tapping") //带子
            {
                ViewState["MGType"] = "Tapping";
                tapAttribute = (TappingAttribute)Page.LoadControl("TappingAttribute.ascx");
                this.attrPanel.Controls.Add(tapAttribute);
                this.Label5.Visible = false;
                this.avaRemark.Visible = false;
                this.GEKComments.Height = 235;
            }
            object obj = Request.QueryString["AQC"];




            if (Request.QueryString["QC"] != null)
            {
                //如果是"另存为"的情况（即页面地址中QC的参数不为NUll），还需要初始化界面，初始化的方式：调用前台的setGMType 进行初始化。


                if (Request.QueryString["ESCM"] != null && Request.QueryString["ESCM"] == "1")
                {
                    ClientScript.RegisterClientScriptBlock(typeof(string), "js", "setCookie('" +
                           Request.QueryString["QC"].ToString()+ "');", true);
                    return;
                }
                ClientScript.RegisterClientScriptBlock(typeof(string), "js", "setGMType('" + strMG + "','" +
                    Request.QueryString["QC"].ToString() + "','" + Request.QueryString["customerId"].ToString()
                    + "','" + (obj == null ? "" : obj.ToString()) + "');", true);
            }
            else
            {
                //添加 查询GK_NO 代码 by mengjw 2015-08-04
            
                //QcmaininfoModelList maininfoModel = qcMainManager.GetModelListOne(new QcmaininfoModelGen() { QualityCode = tQC.Text });
                //if (maininfoModel.Count > 0)
                //{ 
                //    txtGkNo.Text = maininfoModel[0].GK_NO; 
                //}

                // edit by jack 2017-5-8 单号312116 QC号规则加判断条件
                if (obj != null)
                {
                    QcmaininfoModelList maininfoModel = qcMainManager.GetModelList(new QcmaininfoModel() { QualityCode = obj.ToString() });
                    if (maininfoModel.Count > 0)
                    {
                        if (obj.ToString().Substring(0, 1) == "C" && maininfoModel[0].GK_NO.ToString()!="")
                        {
                            if (maininfoModel[0].GK_NO.Substring(0, 1) == "B" || (maininfoModel[0].GK_NO.Substring(0, 1) == "S" && maininfoModel[0].GK_NO.Substring(4, 1) == "B"))
                            {
                                //
                            }
                            else
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "message", "<script>alert('请检查用途是否有误！');</script>");
                            }
                        }

                        if (obj.ToString().Substring(0, 1) == "F" && maininfoModel[0].GK_NO.ToString() != "")
                        {
                            if (maininfoModel[0].GK_NO.Substring(0, 1) == "C" || (maininfoModel[0].GK_NO.Substring(0, 1) == "S" && maininfoModel[0].GK_NO.Substring(4, 1) == "C"))
                            {
                                //
                            }
                            else
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "message", "<script>alert('请检查用途是否有误！');</script>");
                                return;
                            }
                        }
                    }
                }




                ClientScript.RegisterClientScriptBlock(typeof(string), "js", "setGMType('" + strMG + "','','','" +
                    (obj == null ? "" : obj.ToString()) + "');", true);
            }

            /*
            if (HttpContext.Current.Session["UserId"] != null)
            {
                string userId = HttpContext.Current.Session["UserId"].ToString();
                string userInfo = new CustomerManager().GetUserInfoByUserId(userId);
                string[] userInfos = userInfo.Split(',');
                ListItem item = null;
                if (userInfos.Length > 1)
                {
                    item = this.SalesTeam.Items.FindByValue(userInfos[1]);
                    if (item != null)
                    {
                        this.SalesTeam.SelectedItem.Text = item.Text;
                        this.SalesTeam.SelectedItem.Value = item.Value;
                    }
                    item = this.Sales.Items.FindByText(userInfos[0]);
                    if (item != null)
                    {
                        this.Sales.Value = userId;
                        this.Sales.DataTextField = userInfos[0];
                    }
                }
            }
             * */
        }

        protected override string SessionName
        {
            get
            {
                return "CreateSession";
            }
        }

        protected void create_Click(object sender, EventArgs e)
        {

          //  return;
            //分圆机、横机和带子进行不同信息的保存，同时产生不同的QuailtyCode
            if (Sourcing.SelectedValue == "External")
            {
                qcCode = QcUtil.GetQualityCode("External");
            }
            if (ViewState["MGType"].ToString() == "Fabric")
            {
                if (qcCode == null)
                {
                    qcCode = QcUtil.GetQualityCode("Fabric");
                }
                AddFabric();
            }
            else if (ViewState["MGType"].ToString() == "FlatKnit")
            {
                if (qcCode == null)
                {
                    qcCode = QcUtil.GetQualityCode("Flat Knit");
                }
                AddFlat();
            }
            else if (ViewState["MGType"].ToString() == "Tapping")
            {
                if (qcCode == null)
                {
                    qcCode = QcUtil.GetQualityCode("Tapping");
                }
                AddTapping();
            }

        }


        public string GetCustomerInfo(string qcAndById)
        {
            string[] sT=qcAndById.Split(new string[] { ",Qb," },StringSplitOptions.None);
           qcCustomerList= qcCustomerManager.GetModelListOne(sT[0], sT[1]);
           if (qcCustomerList.Count != 0)
           {
               return qcCustomerList[0].BuyerId + "<?>" + qcCustomerList[0].SalesGroup + "<?>" + qcCustomerList[0].Sales +
                   "<?>" + qcCustomerList[0].CustomerQualityId + "<?>" + qcCustomerList[0].Brand + "<?>" + qcCustomerList[0].MillComments;
           }
           return "";
        }

        public string GetYarnInfo(string a)
        {
            List<YarnInfo> listYarn = new List<YarnInfo>();
            if (HttpContext.Current.Session[SessionName] != null)
            {
                string retStr="";
                string des = "";
                listYarn = HttpContext.Current.Session[SessionName] == null ? new List<YarnInfo>() : (List<YarnInfo>)HttpContext.Current.Session[SessionName];
                foreach(YarnInfo yi in listYarn)
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


        public void AddFabric()  //圆机的保存函数

        {
            DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
            CAttribute cAttribute = attribute.GetCAttributeValue();
            List<AvaWidthModel> listAWM = avaWidth.GetAvaWidth();
            if (HttpContext.Current.Session[SessionName] != null)
            {
                cAttribute.ListYarn = HttpContext.Current.Session[SessionName] == null ? new List<YarnInfo>() : (List<YarnInfo>)HttpContext.Current.Session[SessionName];
            }
            cAttribute.REPEAT = new QcmaininfoManager().GetSameQCRemark(strQC);
            QcmaininfoModel qcMainModel = CreateMain(cAttribute);
            qcMainModel.GK_NO = txtGkNo.Text;

            QcconstructiondtlModelList qcConstructionList = CreateConstruction(cAttribute);

            //Add by sunny 2017/1025对ratio进行判断
         /*   if (VerificationRatio(cAttribute) == 1)
            {
                return;
            }*/

            QcyarndtlModelList qcYarnList = CreateYarnDtl(cAttribute);
            QcfinishdtlModelList qcFinfishing = CreateFinishing(cAttribute);
            QcavailablewidthModelList qcAvaWidthList = CreateAvaWidth(listAWM);
            QccustomerlibraryModel qcCustomer = CreateCustomer();

            try
            {
                qcMainManager.AddModel(qcMainModel, tran);
                qcConstructionManager.AddModels(qcConstructionList, tran);
                qcYarnManager.AddModels(qcYarnList, tran);
                qcFinishManager.AddModels(qcFinfishing, tran);
                qcAvaWidthManager.AddModels(qcAvaWidthList, tran);
                qcCustomerManager.AddModel(qcCustomer, tran);
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Response.Write("<script language='JavaScript'>alert('" + ex.Message.Replace("'", " ").Replace("\"", " ").Replace("\n", " ").Replace("\r", " ") + "');</script>");
                return;
            }
            if (Request.QueryString["ESCM"] != null && Request.QueryString["ESCM"] == "1")
            {
                Response.Write("<script language='JavaScript'>window.location='CreateQC.aspx?ESCM=1&userId=" + Request.QueryString["userId"].ToString()+ "&MG=Fabric&QC=" + qcCode + "';</script>");
                return;
            }
            Response.Write("<script language='JavaScript'>window.location='CreateQC.aspx?MG=Fabric&AQC="+qcCode+"';</script>");      
        }


        public string GetTheSaveQC(string st,string strFrom)
        {
            QcmaininfoModelList mainList = new QcmaininfoModelList();
            string[] sts = st.Split(new string[] { "<>" }, StringSplitOptions.None);
            int Flat;
            string strQC = "";

            //20190730 kingzhang for support 597477 判断GM为Y时，AW不能为（0和空）
            //begin
            if (sts[8].Trim() != "" && sts[8].Trim()=="Y")
            {
                if (sts[4].Trim() == "" || sts[4].Trim() == "0")
                {
                    //return "当GMTWash选择Y时，AWWidth不能为0或者空,请重新填写！";
                    return "AWWidth";
                }
            }
            //end

            if (sts[0] == "Fabric")
            {
                if (sts[2].Trim() == "" && sts[3].Trim() == "" && sts[4].Trim() == "" && sts[5].Trim() == "" && sts[6].Trim() == "" && sts[7].Trim() == "" && sts[8].Trim() == "" &&
                    sts[9].Trim() == "X" && sts[10].Trim() == "" && sts[11].Trim() == "")
                {
                    return "";
                }
              
                QcmaininfoModel model = new QcmaininfoModel()
                {
                    MaterialGroup = sts[0].Trim(),
                    Sourcing = sts[1].Trim(),
                    BfGmmm = (Int32.Parse(sts[3].Trim() == "" ? "0" : sts[3])),
                    AfGmmm = (Int32.Parse(sts[4].Trim() == "" ? "0" : sts[4])),
                    DyeMethod = sts[5].Trim(),
                    Pattern = sts[6].Trim(),
                    ShrinkageTestingMethod = sts[7].Trim(),
                    GmtWashing = sts[8].Trim(),
                    Shrinkage = sts[9].Trim(),
                    //GK_NO = sts[11].Trim()//gk_no
                };
                if (strFrom == "Approve")
                {
                    model.Status = "Approved";
                }
                mainList = qmManager.GetModelListByAttC(model);
                if (mainList.Count > 0)
                {
                    foreach (QcmaininfoModel m in mainList)
                    {
                        Flat = 0;
                        qcml = qcm.GetModelList(new QcconstructiondtlModel() { QualityCode = m.QualityCode });
                        if ((qcml.Count == sts[2].Split(';').Count()&&sts[2]!=""))
                        {
                            for (int i = 0; i < qcml.Count; i++)
                            {
                                if (!sts[2].Contains(qcml[i].Construction))
                                {
                                    Flat = 1;
                                    break;
                                }
                            }
                        }
                        if ((qcml.Count != sts[2].Split(';').Length && sts[2].Trim() != "") || (sts[2].Trim() == "" && qcml.Count != 0))
                        {
                            Flat = 1;
                        }
                        if (Flat == 1)
                            continue;

                        qsml = qsm.GetModelListTwo(new QcfinishdtlModel() { QualityCode = m.QualityCode });
                        if ((qsml.Count == sts[10].Split(';').Count()&&sts[10]!=""))
                        {
                            for (int i = 0; i < qsml.Count; i++)
                            {
                                if (!sts[10].Contains(qsml[i].FinishingCode))
                                {
                                    Flat = 1;
                                    break;
                                }
                            }
                        }
                        if ((qsml.Count != sts[10].Split(';').Length && sts[10].Trim() != "") || (sts[10].Trim() == "" && qsml.Count != 0))
                        {
                            Flat = 1;
                        }
                        if (Flat == 1)
                            continue;

                        qyml = qym.GetModelList(new QcyarndtlModel() { QualityCode = m.QualityCode });
                        if ((qyml.Count == sts[11].Split(';').Count()&&sts[11]!=""))
                        {
                            string des = "";
                            string strYarn="";
                            //Add by 2017 0616 
                            string strYarns = "";
                            for (int i = 0; i < qyml.Count; i++)
                            {
                                
                                PbknityarntypeModelList yarnModel = yarnManager.GetModelList(new PbknityarntypeModel() { YarnType = qyml[i].YarnType });
                                if (yarnModel.Count > 0)
                                    des = yarnModel[0].Description;
                                else
                                    des = "";

                                strYarn = qyml[i].YarnType + "," + qyml[i].YarnCount + "," + qyml[i].Threads + ((des.ToLower().Contains("heather") || des.ToLower().Contains("htr")) ? "" : ("," + qyml[i].YarnRatio.ToString()));
                                //Add by sunny 2017 0616
                                strYarns = strYarns + ';' + strYarn;

                                if (!sts[11].Contains(strYarn))
                                {
                                    Flat = 1;
                                    break;
                                }
                            }

                            //Add by sunny 2017 0616
                            string yarninfo = strYarns.Substring(1);
                            string[] stsyarn = sts[11].Split(new string[] { ";" }, StringSplitOptions.None);
                            for (int i = 0; i < stsyarn.Count(); i++)
                            {
                                if (!yarninfo.Contains(stsyarn[i]))
                                {
                                    Flat = 1;
                                    break;
                                }
                                
                            }




                        }
                        if ((qyml.Count != sts[11].Split(';').Length && sts[11].Trim() != "") || (sts[11].Trim() == "" && qyml.Count != 0))
                        {
                            Flat = 1;
                        }
                        if (Flat == 1)
                            continue;

                        strQC = strQC + m.QualityCode + ",";
                    }

                }
            }
            else if (sts[0] == "FlatKnit")
            {
                if (sts[2].Trim() == "" && sts[3].Trim() == "" && sts[4].Trim() == "" && sts[5].Trim() == "" && sts[6].Trim() == "")
                {
                    return "";
                }
                QcmaininfoModel model = new QcmaininfoModel()
                {
                    MaterialGroup = "Flat Knit Fabric",
                    Sourcing = sts[1].Trim(),
                    Pattern = sts[3].Trim(),
                    YarnLength=sts[4].Trim()
                    //GK_NO=txtGkNo.Text
                };
                if (strFrom == "Approve")
                {
                    model.Status = "Approved";
                }
                mainList = qmManager.GetModelListByAttF(model);
                if (mainList.Count > 0)
                {
                    foreach (QcmaininfoModel m in mainList)
                    {
                        Flat = 0;
                        qcml = qcm.GetModelList(new QcconstructiondtlModel() { QualityCode = m.QualityCode });
                        if ((qcml.Count == sts[2].Split(';').Length&&sts[2].Trim()!=""))
                        {
                            for (int i = 0; i < qcml.Count; i++)
                            {
                                if (!sts[2].Contains(qcml[i].Construction))
                                {
                                    Flat = 1;
                                    break;
                                }
                            }
                        }
                        if ((qcml.Count != sts[2].Split(';').Length && sts[2].Trim() != "")||(sts[2].Trim()=="" && qcml.Count!=0))
                        {
                            Flat = 1;
                        }
                        
                        if (Flat == 1)
                            continue;

                        qsml = qsm.GetModelListTwo(new QcfinishdtlModel() { QualityCode = m.QualityCode });
                        if ((qsml.Count == sts[5].Split(';').Count()&&sts[5]!=""))
                        {
                            for (int i = 0; i < qsml.Count; i++)
                            {
                                if (!sts[5].Contains(qsml[i].FinishingCode))
                                {
                                    Flat = 1;
                                    break;
                                }
                            }
                        }
                        if ((qsml.Count != sts[5].Split(';').Length && sts[5].Trim() != "") || (sts[5].Trim() == "" && qsml.Count != 0))
                        {
                            Flat = 1;
                        }
                        if (Flat == 1)
                            continue;

                        qyml = qym.GetModelList(new QcyarndtlModel() { QualityCode = m.QualityCode });
                        if ((qyml.Count == sts[6].Split(';').Count()&&sts[6]!=""))
                        {
                            string des = "";
                            string strYarn = "";
                            for (int i = 0; i < qyml.Count; i++)
                            {

                                PbknityarntypeModelList yarnModel = yarnManager.GetModelList(new PbknityarntypeModel() { YarnType = qyml[i].YarnType });
                                if (yarnModel.Count > 0)
                                    des = yarnModel[0].Description;
                                else
                                    des = "";

                                strYarn = qyml[i].YarnType + "," + qyml[i].YarnCount + "," + qyml[i].Threads + ((des.ToLower().Contains("heather") || des.ToLower().Contains("htr")) ? "" : ("," + qyml[i].YarnRatio.ToString()));
                                if (!sts[6].Contains(strYarn))
                                {
                                    Flat = 1;
                                    break;
                                }
                            }
                        }
                        if ((qyml.Count != sts[6].Split(';').Length && sts[6].Trim() != "") || (sts[6].Trim() == "" && qyml.Count != 0))
                        {
                            Flat = 1;
                        }
                        if (Flat == 1)
                            continue;

                        strQC = strQC + m.QualityCode + ",";
                    }

                }
            }
            else if (sts[0] == "Tapping")
            {
                if (sts[2].Trim() == "" && sts[3].Trim() == "" && sts[4].Trim() == "" && sts[5].Trim() == "")
                {
                    return "";
                }
                QcmaininfoModel model = new QcmaininfoModel()
                {
                    MaterialGroup = sts[0].Trim(),
                    Sourcing = sts[1].Trim(),
                    YarnLength =sts[2].Trim(),
                    Measurement=sts[3].Trim(),
                    TappingType =sts[4].Trim()
                };
                if (strFrom == "Approve")
                {
                    model.Status = "Approved";
                }
                mainList = qmManager.GetModelListByAttT(model);
                if (mainList.Count > 0)
                {
                    foreach (QcmaininfoModel m in mainList)
                    {
                        Flat = 0;
                        qyml = qym.GetModelList(new QcyarndtlModel() { QualityCode = m.QualityCode });
                        if ((qyml.Count == sts[5].Split(';').Count()&&sts[5]!=""))
                        {
                            string des = "";
                            string strYarn = "";
                            for (int i = 0; i < qyml.Count; i++)
                            {

                                PbknityarntypeModelList yarnModel = yarnManager.GetModelList(new PbknityarntypeModel() { YarnType = qyml[i].YarnType });
                                if (yarnModel.Count > 0)
                                    des = yarnModel[0].Description;
                                else
                                    des = "";

                                strYarn = qyml[i].YarnType + "," + qyml[i].YarnCount + "," + qyml[i].Threads + ((des.ToLower().Contains("heather") || des.ToLower().Contains("htr")) ? "" : ("," + qyml[i].YarnRatio.ToString()));
                                if (!sts[5].Contains(strYarn))
                                {
                                    Flat = 1;
                                    break;
                                }
                            }
                        }
                        if ((qyml.Count != sts[5].Split(';').Length && sts[5].Trim() != "") || (sts[5].Trim() == "" && qyml.Count != 0))
                        {
                            Flat = 1;
                        }
                        if (Flat == 1)
                            continue;

                        strQC = strQC + m.QualityCode + ",";
                    }

                }
            
            }
            Session["strQC"] = strQC;
            return strQC;
        }

        public string GetCustomer(string qcStr)
        {
            if(string.IsNullOrEmpty(qcStr))
            {
                return string.Empty;
            }

            if (qcStr.Substring(qcStr.Length - 1, 1) == ",")
            {
                qcStr = qcStr.Substring(0, qcStr.Length - 1);
            }

            string[] qcArray = qcStr.Split(',');
            StringBuilder sb = new StringBuilder();

            foreach (string qc in qcArray)
            {
                sb.Append(cuManager.GetCustomerName(qc)+"|");
            }

            return sb.ToString();
        }

        public void AddFlat() //横机的保存函数

        {

            DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
            CAttribute cAttribute =   flatAttribute.GetCAttributeValue();
            if (HttpContext.Current.Session[SessionName] != null)
            {
                cAttribute.ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
            }
            cAttribute.REPEAT = new QcmaininfoManager().GetSameQCRemark(strQC);
            QcmaininfoModel qcMainModel = CreateFlagMain(cAttribute);
            qcMainModel.GK_NO = txtGkNo.Text;
            QcconstructiondtlModelList qcConstructionList = CreateConstruction(cAttribute);

            //Add by sunny 20171025 对ratio进行判断。
           /* if (VerificationRatio(cAttribute) == 1)
            {
                return;
            }*/
            QcyarndtlModelList qcYarnList = CreateYarnDtl(cAttribute);
            QcfinishdtlModelList qcFinfishing = CreateFinishing(cAttribute);
            QccustomerlibraryModel qcCustomer = CreateCustomer();

            try
            {
                qcMainManager.AddModel(qcMainModel, tran);
                qcConstructionManager.AddModels(qcConstructionList, tran);
                qcYarnManager.AddModels(qcYarnList, tran);
                qcFinishManager.AddModels(qcFinfishing, tran);
                qcCustomerManager.AddModel(qcCustomer, tran);
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Response.Write("<script language='JavaScript'>alert('" + ex.Message.Replace("'", " ").Replace("\"", " ").Replace("\n", " ").Replace("\r", " ") + "');</script>");
                return;
            }
            if (Request.QueryString["ESCM"] != null && Request.QueryString["ESCM"] == "1")
            {
                Response.Write("<script language='JavaScript'>window.location='CreateQC.aspx?ESCM=1&userId=" + Request.QueryString["userId"].ToString() + "&MG=Fabric&QC=" + qcCode + "';</script>");
                return;
            }
          //  Response.Write("<script language='JavaScript'>alert('保存成功！');window.location='CreateQC.aspx?MG=FlatKnit';</script>");
            Response.Write("<script language='JavaScript'>window.location='CreateQC.aspx?MG=FlatKnit&AQC=" + qcCode + "';</script>"); 
        }

        public string GetCustomerInfoOne(string qc)
        {
          QccustomerlibraryModelList qcl= qcCustomerManager.GetModelListTwo(qc);
           if(qcl.Count>0)
           {
               return qcl[0].Iden + "<|>" + qcl[0].BuyerId + "<|>" + qcl[0].BuyerName + "<|>" + qcl[0].Sales + "<|>" + qcl[0].SalesName + "<|>" + qcl[0].SalesGroup +
                  "<|>" + qcl[0].CustomerQualityId + "<|>" + qcl[0].Brand+"<|>"+qcl[0].MillComments;
           }
           return "No";
        }

        public void AddTapping() //带子的保存函数

        {

            DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
            CAttribute cAttribute = tapAttribute.GetCAttributeValue();
            if (HttpContext.Current.Session[SessionName] != null)
            {
                cAttribute.ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
            }
            cAttribute.REPEAT = new QcmaininfoManager().GetSameQCRemark(strQC);
            QcmaininfoModel qcMainModel = CreateTappingMain(cAttribute);
            qcMainModel.GK_NO = txtGkNo.Text;


            //Add by sunny 20171025 对ratio进行判断。
           /* if (VerificationRatio(cAttribute) == 1)
            {
                return;
            }*/
            QcyarndtlModelList qcYarnList = CreateYarnDtl(cAttribute);
            QccustomerlibraryModel qcCustomer = CreateCustomer();

            try
            {
                qcMainManager.AddModel(qcMainModel, tran);
                qcYarnManager.AddModels(qcYarnList, tran);
                qcCustomerManager.AddModel(qcCustomer, tran);
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Response.Write("<script language='JavaScript'>alert('" + ex.Message.Replace("'", " ").Replace("\"", " ").Replace("\n", " ").Replace("\r", " ") + "');</script>");
                return;
            }
            if (Request.QueryString["ESCM"] != null && Request.QueryString["ESCM"] == "1")
            {
                Response.Write("<script language='JavaScript'>window.location='CreateQC.aspx?ESCM=1&userId=" + Request.QueryString["userId"].ToString() + "&MG=Fabric&QC=" + qcCode + "';</script>");
                return;
            }
            Response.Write("<script language='JavaScript'>window.location='CreateQC.aspx?MG=Tapping&AQC=" + qcCode + "';</script>"); 
            // Response.Write("<script language='JavaScript'>alert('Save sucess！The QualityCode is:"+qcCode+"');window.location='CreateQC.aspx?MG=Tapping';</script>");
        }

        //根据页面信息产生各个Model
        private QcmaininfoModel CreateMain(CAttribute cat) //圆机主表信息
        {
            QcmaininfoModel qcMainModel = new QcmaininfoModel();
            qcMainModel.QualityCode = qcCode;//
            qcMainModel.Creator = HttpContext.Current.Session["UserId"].ToString();
            qcMainModel.LastUpdateTime = System.DateTime.Now;
            qcMainModel.MaterialGroup = "Fabric";
            qcMainModel.CreateDate = System.DateTime.Now;
            qcMainModel.Pattern = cat.Pttern;
            qcMainModel.DyeMethod = cat.DyeMehtod;
            qcMainModel.Remark = txtAvaRemark.Text;
            qcMainModel.BfGmmm = cat.BWWeight;
            qcMainModel.AfGmmm = cat.AWWeight;
            qcMainModel.AnalysisNo = AnalysisNo.Text;
            qcMainModel.Shrinkage = cat.Shringkage;
            qcMainModel.GmtWashing = cat.GMTWash;
            qcMainModel.Sourcing = Sourcing.SelectedValue;
            qcMainModel.Status = "New";
            qcMainModel.Layout = cat.Layout;
            qcMainModel.ShrinkageTestingMethod = cat.TestMethod;
            qcMainModel.GK_NO = cat.GK_NO;
            qcMainModel.Repeat = cat.REPEAT;
            return qcMainModel;
        }

        private QcmaininfoModel CreateFlagMain(CAttribute cat) //横机主表信息
        { 
             QcmaininfoModel qcMainModel = new QcmaininfoModel();
            qcMainModel.QualityCode = qcCode;//
            qcMainModel.Creator = HttpContext.Current.Session["UserId"].ToString();
            qcMainModel.LastUpdateTime = System.DateTime.Now;
            qcMainModel.MaterialGroup = "Flat Knit Fabric";
            qcMainModel.CreateDate = System.DateTime.Now;
            qcMainModel.Pattern = cat.Pttern;
            qcMainModel.AnalysisNo = AnalysisNo.Text;
            qcMainModel.Sourcing = Sourcing.SelectedValue;
            qcMainModel.Layout = cat.Layout;
            qcMainModel.YarnLength = cat.YarnLength;
            qcMainModel.Status = "New";
            return qcMainModel;    
         
        }
        private QcmaininfoModel CreateTappingMain(CAttribute cat) //带子主表信息
        {
            QcmaininfoModel qcMainModel = new QcmaininfoModel();
            qcMainModel.QualityCode = qcCode;//
            qcMainModel.Creator = HttpContext.Current.Session["UserId"].ToString();
            qcMainModel.LastUpdateTime = System.DateTime.Now;
            qcMainModel.MaterialGroup = "Tapping";
            qcMainModel.CreateDate = System.DateTime.Now;
            qcMainModel.AnalysisNo = AnalysisNo.Text;
            qcMainModel.Sourcing = Sourcing.SelectedValue;
            qcMainModel.TappingType = cat.TappingType;
            qcMainModel.Layout = cat.Layout;
            qcMainModel.Measurement = cat.Size;
            qcMainModel.YarnLength = cat.YarnLength;
            qcMainModel.Status = "New";
            return qcMainModel;

        }
        private QcconstructiondtlModelList CreateConstruction(CAttribute cat)  //产生布种信息
        {
            QcconstructiondtlModelList qcConstructionModelList = new QcconstructiondtlModelList();
            if(cat.Construction!="")
            {
                string[] strTemp = cat.Construction.Split(new string[]{","},StringSplitOptions.None);
                for(int i=0;i<strTemp.Length;i++)
                {
                  if(strTemp[i]!="")
                  {
                      QcconstructiondtlModel qcConstructionModel = new QcconstructiondtlModel();
                      qcConstructionModel.Construction = strTemp[i];
                      qcConstructionModel.QualityCode = qcCode;
                      qcConstructionModelList.Add(qcConstructionModel);
                  }
                }

            }
            return qcConstructionModelList;
        }

        //add by sunny 2017 1025 //add by Sunny 对ratio 字段进行判断，不允许为空，不允许为0 start 
      /*  public int VerificationRatio(CAttribute cat)
        {
            decimal Ratio=0;
            int j = 0;

            if (cat.ListYarn == null)
            {
                return j;
            }
            else 
            {
                for (int i = 0; i < cat.ListYarn.Count; i++)
                {
                    if (cat.ListYarn[i].Radio.ToString() == "")
                    {
                        j = 1;
                        Response.Write("<script language='JavaScript'>alert('纱类Radio不可以为空');</script>");
                        return j;
                    }
                    else
                    {
                        Ratio = Ratio + decimal.Parse(cat.ListYarn[i].Radio.ToString());
                    }                  
                }
            }

            if (Ratio != 100)
            {
                j = 1;
                Response.Write("<script language='JavaScript'>alert('纱类Radio加起来必须等于100');</script>");
                return j;
            }

            return j;
        }*/

        //add by Sunny 对ratio 字段进行判断，不允许为空，不允许为0 end  

        private QcyarndtlModelList CreateYarnDtl(CAttribute cat)  //产生纱信息
        {

            QcyarndtlModelList qcYarnList = new QcyarndtlModelList();
            if (cat.ListYarn == null)
                return qcYarnList;

            
       
            for (int i = 0; i < cat.ListYarn.Count; i++)
            {
                QcyarndtlModel qcYarnModel = new QcyarndtlModel();
                qcYarnModel.YarnRatio = cat.ListYarn[i].Radio;
                qcYarnModel.Threads = cat.ListYarn[i].Threads;
                qcYarnModel.YarnType = cat.ListYarn[i].YarnType;
                qcYarnModel.YarnCount = cat.ListYarn[i].YarnCount;
                qcYarnModel.WarpWeft = cat.ListYarn[i].WarpWeft;
                qcYarnModel.YarnDensity = cat.ListYarn[i].YarnDensity;
                qcYarnModel.YarnComponent = cat.ListYarn[i].YarnComponent;
                qcYarnModel.QualityCode = qcCode;
                qcYarnList.Add(qcYarnModel);

            }
            return qcYarnList;
        }

        private QcfinishdtlModelList CreateFinishing(CAttribute cat)  //产生后整理信息

        {
            QcfinishdtlModelList qcfinishingList = new QcfinishdtlModelList();
            for (int i = 0; i < cat.ListFinishing.Count; i++)
            {
                QcfinishdtlModel qcFinishModel = new QcfinishdtlModel();
                qcFinishModel.QualityCode = qcCode;
                qcFinishModel.FinishingCode = cat.ListFinishing[i];
                qcfinishingList.Add(qcFinishModel);
            }
            return qcfinishingList;
        }

        private QcavailablewidthModelList CreateAvaWidth(List<AvaWidthModel> listAWM) //产生
        {
            QcavailablewidthModelList qcAvaWidthList = new QcavailablewidthModelList();
            for (int i = 0; i < listAWM.Count; i++)
            {
                QcavailablewidthModel qcAvaModel = new QcavailablewidthModel();
                qcAvaModel.Gauge = listAWM[i].Gauge;
                qcAvaModel.Diameter = listAWM[i].Diameter;
                qcAvaModel.QualityCode = qcCode;
                qcAvaModel.TotalNeedles = listAWM[i].TotalNeedles;
                qcAvaModel.MaxWidth = listAWM[i].MaxWidth;
                qcAvaModel.Width = listAWM[i].Width;
                qcAvaModel.UpdatedBy = HttpContext.Current.Session["UserId"].ToString();
                qcAvaModel.UpdatedTime = System.DateTime.Now;
                qcAvaWidthList.Add(qcAvaModel);
            }
            return qcAvaWidthList;
        }

        private QccustomerlibraryModel CreateCustomer() //产生客户信息
        {
            QccustomerlibraryModel qcCustomerModel = new QccustomerlibraryModel();
            qcCustomerModel.CustomerQualityId = CustomerQC.Text;
            qcCustomerModel.BuyerId = customer.Text;
            qcCustomerModel.Brand = brand.Text;
            qcCustomerModel.IsFirstOwner = "Y";
            qcCustomerModel.QualityCode = qcCode;
            qcCustomerModel.MillComments = GEKComments.Text;
            qcCustomerModel.CreateDate = System.DateTime.Now;
            qcCustomerModel.Sales = txtSales.Text;
            qcCustomerModel.SalesGroup = SalesTeam.SelectedValue;
            qcCustomerModel.Creator = HttpContext.Current.Session["UserId"].ToString();

            return qcCustomerModel;
        }


        private QccustomerlibraryModel UpdateCustomer() //产生客户信息
        {
            QccustomerlibraryModel qcCustomerModel = new QccustomerlibraryModel();
            qcCustomerModel.CustomerQualityId = CustomerQC.Text;
            qcCustomerModel.BuyerId = customerHidden.Value;
            qcCustomerModel.BuyerIdNew = customer.Text;
            qcCustomerModel.Brand = brand.Text;
          //  qcCustomerModel.IsFirstOwner = "Y";
            qcCustomerModel.QualityCode = qcCode;
            qcCustomerModel.MillComments = GEKComments.Text;
            qcCustomerModel.CreateDate = System.DateTime.Now;
            qcCustomerModel.Sales = txtSales.Text;
            qcCustomerModel.SalesGroup = SalesTeam.SelectedValue;
            qcCustomerModel.Creator = HttpContext.Current.Session["UserId"].ToString();

            return qcCustomerModel;
        }

        protected void btnSaveQC_Click(object sender, EventArgs e)
        {
            //分圆机、横机和带子进行不同信息的更新

            qcCode = HiddenQC.Value;
            if (qcCode == "")
                return;

          

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
                }
          


            cuManager.SendEmain(qcCode, editReason.Text,"1");
        }

        //Add by sunny  Edit编译保存时判断ratio是否符合逻辑规则
     /*   public bool  EditVerificationRatiRao()
        {
            List<YarnInfo> ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
             decimal Ratio=0;
             for (int i = 0; i < ListYarn.Count; i++)
                {
                    if (ListYarn[i].Radio.ToString() == "")
                    {

                        Response.Write("<script language='JavaScript'>alert('纱类Radio不可以为空');</script>");
                        return false;
                       
                    }
                    else
                    {
                        Ratio = Ratio + int.Parse(ListYarn[i].Radio.ToString());
                    }                  
                }
             if (Ratio != 100)
             {
                 Response.Write("<script language='JavaScript'>alert('纱类Radio加起来必须等于100');</script>");
                 return false;
             }
             else
             {
                 return true;
             }

           


        }*/



                //Add by sunny  Edit编译保存时判断ratio是否符合逻辑规则
        public string EditVerificationRatio()
        {
            List<YarnInfo> ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
             decimal Ratio=0;
             for (int i = 0; i < ListYarn.Count; i++)
                {
                    if (ListYarn[i].Radio.ToString() == "")
                    {
                        return "falseone";
                       
                    }
                    else
                    {
                        Ratio = Ratio + decimal.Parse(ListYarn[i].Radio.ToString());
                    }                  
                }
             if (Ratio != 100)
             {

                 return "falsetwo";
             }
             else
             {
                 return "";
             }

           


        }
        
       


        



        //更新圆机信息
        private void UpdatedFabric()
        {
            DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
            CAttribute cAttribute = attribute.GetCAttributeValue();
            List<AvaWidthModel> listAWM = avaWidth.GetAvaWidth();
            if (HttpContext.Current.Session[SessionName] != null)
            {
                cAttribute.ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
            }

            QcmaininfoModel qcMainModel = CreateMain(cAttribute);
            qcMainModel.GK_NO = txtGkNo.Text;   //by mengjw 2015/08/03           
            QcconstructiondtlModelList qcConstructionList = CreateConstruction(cAttribute);

            QcyarndtlModelList qcYarnList = CreateYarnDtl(cAttribute);
            QcfinishdtlModelList qcFinfishing = CreateFinishing(cAttribute);
            QcavailablewidthModelList qcAvaWidthList = CreateAvaWidth(listAWM);
            QccustomerlibraryModel qcCustomer;
            if (string.IsNullOrEmpty(HiddenCuId.Value)|| HiddenCuId.Value.ToLower()=="no")
            {
                 qcCustomer = CreateCustomer();
            }
            else
            {
                 qcCustomer = UpdateCustomer();
            }

            try
            {
                qcMainManager.UpdateModel(qcMainModel, tran);

                qcConstructionManager.DeleteModel(new QcconstructiondtlModel() { QualityCode = qcCode }, tran);
                qcConstructionManager.AddModels(qcConstructionList, tran);

                qcYarnManager.DeleteModel(new QcyarndtlModel() { QualityCode = qcCode }, tran);
                qcYarnManager.AddModels(qcYarnList, tran);

                qcFinishManager.DeleteModel(new QcfinishdtlModel() { QualityCode = qcCode }, tran);
                qcFinishManager.AddModels(qcFinfishing, tran);

                qcAvaWidthManager.DeleteModel(new QcavailablewidthModel() { QualityCode = qcCode }, tran);
                qcAvaWidthManager.AddModels(qcAvaWidthList, tran);

                if (string.IsNullOrEmpty(HiddenCuId.Value) || HiddenCuId.Value.ToLower() == "no")
                {
                    qcCustomerManager.AddModel(qcCustomer,tran);   
                }
                else
                {
                    qcCustomerManager.UpdateModelTwo(qcCustomer, tran);
                }
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Response.Write("<script language='JavaScript'>alert('" + ex.Message.Replace("'", " ").Replace("\"", " ").Replace("\n", " ").Replace("\r", " ") + "');</script>");
                return;
            }

            Response.Write("<script language='JavaScript'>window.location='CreateQC.aspx?MG=Fabric&AQC=" + qcCode + "';</script>");  
        }

        //更新横机信息
        private void UpdatedFlat()
        {
            DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
            CAttribute cAttribute = flatAttribute.GetCAttributeValue();
            if (HttpContext.Current.Session[SessionName] != null)
            {
                cAttribute.ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
            }

            QcmaininfoModel qcMainModel = CreateFlagMain(cAttribute);
            qcMainModel.GK_NO = txtGkNo.Text;   //by mengjw 2015/08/03
            QcfinishdtlModelList qcFinfishing = CreateFinishing(cAttribute);
            QcconstructiondtlModelList qcConstructionList = CreateConstruction(cAttribute);
            QcyarndtlModelList qcYarnList = CreateYarnDtl(cAttribute);
            QccustomerlibraryModel qcCustomer;
            if (string.IsNullOrEmpty(HiddenCuId.Value) || HiddenCuId.Value.ToLower() == "no")
            {
                qcCustomer = CreateCustomer();
            }
            else
            {
                qcCustomer = UpdateCustomer();
            }

            try
            {
                qcMainManager.UpdateModelFlat(qcMainModel, tran);

                qcFinishManager.DeleteModel(new QcfinishdtlModel() { QualityCode = qcCode }, tran);
                qcFinishManager.AddModels(qcFinfishing, tran);

                qcConstructionManager.DeleteModel(new QcconstructiondtlModel() { QualityCode = qcCode }, tran);
                qcConstructionManager.AddModels(qcConstructionList, tran);

                qcYarnManager.DeleteModel(new QcyarndtlModel() { QualityCode = qcCode }, tran);
                qcYarnManager.AddModels(qcYarnList, tran);

                if (string.IsNullOrEmpty(HiddenCuId.Value) || HiddenCuId.Value.ToLower() == "no")
                {
                    qcCustomerManager.AddModel(qcCustomer, tran);
                }
                else
                {
                    qcCustomerManager.UpdateModelTwo(qcCustomer, tran);
                }
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Response.Write("<script language='JavaScript'>alert('" + ex.Message.Replace("'", " ").Replace("\"", " ").Replace("\n", " ").Replace("\r", " ") + "');</script>");
                return;
            }


            Response.Write("<script language='JavaScript'>window.location='CreateQC.aspx?MG=FlatKnit&AQC=" + qcCode + "';</script>"); 
        }

        //更新带子信息
        private void UpdatedTapping()
        {
            DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
            CAttribute cAttribute = tapAttribute.GetCAttributeValue();
            if (HttpContext.Current.Session[SessionName] != null)
            {
                cAttribute.ListYarn = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
            }

            QcmaininfoModel qcMainModel = CreateTappingMain(cAttribute);
            qcMainModel.GK_NO = txtGkNo.Text;   //by mengjw 2015/08/03
            QcyarndtlModelList qcYarnList = CreateYarnDtl(cAttribute);
            QccustomerlibraryModel qcCustomer;
            if (string.IsNullOrEmpty(HiddenCuId.Value) || HiddenCuId.Value.ToLower() == "no")
            {
                qcCustomer = CreateCustomer();
            }
            else
            {
                qcCustomer = UpdateCustomer();
            }

            try
            {
                qcMainManager.UpdateModelTapping(qcMainModel, tran);

                qcYarnManager.DeleteModel(new QcyarndtlModel() { QualityCode = qcCode }, tran);
                qcYarnManager.AddModels(qcYarnList, tran);


                if (string.IsNullOrEmpty(HiddenCuId.Value) || HiddenCuId.Value.ToLower() == "no")
                {
                    qcCustomerManager.AddModel(qcCustomer, tran);
                }
                else
                {
                    qcCustomerManager.UpdateModelTwo(qcCustomer, tran);
                }
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Response.Write("<script language='JavaScript'>alert('" + ex.Message.Replace("'", " ").Replace("\"", " ").Replace("\n", " ").Replace("\r", " ") + "');</script>");
                return;
            }


            Response.Write("<script language='JavaScript'>window.location='CreateQC.aspx?MG=Tapping&AQC=" + qcCode + "';</script>"); 
        }




        //获取GEKComment
        public string GetGekComment(string codeAndCustomerId)
        {
            CustomerManager cm = new CustomerManager();
            string[] strs = codeAndCustomerId.Split(new string[] { "," }, StringSplitOptions.None);
            return cm.GetGekComment(strs[0], strs[1]);
        }

        protected void cancel_Click(object sender, EventArgs e)
        {
            qcCode = HiddenQC.Value;
            if (qcCode == "")
                return;
            if (HiddenHasPPO.Value == "1") //有开过单，QC的状态设置为Disabled
            {
                QcmaininfoModel main = new QcmaininfoModel();
                main.QualityCode = qcCode;
                main.Status = "Disabled";
                main.LastUpdateBy = HttpContext.Current.Session["UserId"].ToString();
                main.LastUpdateTime = System.DateTime.Now;
                main.ApproveDate = System.DateTime.Now;
                main.Approver = HttpContext.Current.Session["UserId"].ToString();
                qcMainManager.ApproveAndShowDown(main, null);
                Response.Write("<script language='JavaScript'>alert('Successfully Disabled!');window.location='CreateQC.aspx?MG=" + ViewState["MGType"] + "';</script>");
                cuManager.SendEmain(qcCode, editReason.Text, "2");
            }
            else  //没有开过单，可以直接删除

            {
                DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
                try
                {
                    cuManager.SendEmain(qcCode, editReason.Text, "3");

                    qcMainManager.DeleteModel(new QcmaininfoModel() { QualityCode = qcCode }, tran);

                    qcConstructionManager.DeleteModel(new QcconstructiondtlModel() { QualityCode = qcCode }, tran);

                    qcYarnManager.DeleteModel(new QcyarndtlModel() { QualityCode = qcCode }, tran);

                    qcFinishManager.DeleteModel(new QcfinishdtlModel() { QualityCode = qcCode }, tran);

                    qcAvaWidthManager.DeleteModel(new QcavailablewidthModel() { QualityCode = qcCode }, tran);

                    qcCustomerManager.DeleteModel(new QccustomerlibraryModel() { QualityCode = qcCode }, tran);

                    tran.Commit();
                    
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    Response.Write("<script language='JavaScript'>alert('" + ex.Message.Replace("'", " ").Replace("\"", " ").Replace("\n", " ").Replace("\r", " ") + "');</script>");
                    return;
                }

                Response.Write("<script language='JavaScript'>alert('Successfully deleted!');window.location='CreateQC.aspx?MG=" + ViewState["MGType"] + "';</script>"); 
            }
        }
        /////////////////

    }
}