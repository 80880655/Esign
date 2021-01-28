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

namespace Comfy.App.Web.QuailtyCode
{
    public partial class Edit_Ref : MastPage
    {
        Attribute attribute = new Attribute();
        FlagAttribute flatAttribute = new FlagAttribute();
        TappingAttribute tapAttribute = new TappingAttribute();
        // CustomerEditForm gridCustomer = new CustomerEditForm();
        AvaWidth avaWidth = new AvaWidth();
        TextBox GekComments = new TextBox();
        protected override string SessionName
        {
            get
            {
                return "EditSession";
            }
        }
        public string UserID
        {
            get
            {
                return HttpContext.Current.Session["UserId"].ToString();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HttpContext.Current.Session[SessionName] = null;
                if (HttpContext.Current.Session["UserPower"] != null)
                {
                    List<string> sl = (List<string>)HttpContext.Current.Session["UserPower"];
                    if (!sl.Contains("3"))
                    {
                        Response.Write("<script language='JavaScript'>window.location='NoPower.aspx';</script>");
                        return;
                    }
                    if (!sl.Contains("1"))
                    {
                        btnSaveAs.Visible = false;
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
            GekComments.Height = 50;
            GekComments.ReadOnly = true;
            GekComments.ID = "GekComments";
            GekComments.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            GekComments.Attributes.Add("style", "border:1px solid #FFA200");
            GekComments.TextMode = TextBoxMode.MultiLine;
            GekComments.ClientIDMode = ClientIDMode.Static;
            string strMG = Request.QueryString["MG"] == null ? "Fabric" : Request.QueryString["MG"].ToString();
            if (strMG == "Fabric")  //圆机
            {
                ViewState["MGType"] = "Fabric";
                attribute = (Attribute)Page.LoadControl("Attribute.ascx");
                //  gridCustomer = (CustomerEditForm)Page.LoadControl("CustomerEditForm.ascx");

                avaWidth = (AvaWidth)Page.LoadControl("AvaWidth.ascx");
                this.CFTAttribute.Controls.Add(attribute);
                this.AvaPanel.Controls.Add(avaWidth);
                this.gridCustomerPanle.Controls.Add(GekComments);
            }
            else if (strMG == "FlatKnit" || strMG == "Flat Knit Fabric") //横机
            {
                strMG = "FlatKnit";
                GekComments.Height = 125;
                GekComments.Width = 487;
                SalesComments.Height = 170;
                avaRemark.Visible = false;
                ViewState["MGType"] = "FlatKnit";
                flatAttribute = (FlagAttribute)Page.LoadControl("FlagAttribute.ascx");
                // gridCustomer = (CustomerEditForm)Page.LoadControl("CustomerEditForm.ascx");
                this.CFTAttribute.Controls.Add(flatAttribute);
                this.labelGekComment.Visible = false;
                this.AWPanel.Text = "GEK Comments";
                this.AvaPanel.Controls.Add(GekComments);
                this.labelGekComment.Visible = false;
            }
            else if (strMG == "Tapping")  //带子
            {
                GekComments.Height = 175;
                GekComments.Width = 487;
                SalesComments.Height = 95;
                ViewState["MGType"] = "Tapping";
                tapAttribute = (TappingAttribute)Page.LoadControl("TappingAttribute.ascx");
                //  gridCustomer = (CustomerEditForm)Page.LoadControl("CustomerEditForm.ascx");
                this.CFTAttribute.Controls.Add(tapAttribute);
                this.labelGekComment.Visible = false;
                this.AWPanel.Text = "GEK Comments";
                avaRemark.Visible = false;
                this.AvaPanel.Controls.Add(GekComments);
                this.labelGekComment.Visible = false;
            }


            if (Request.QueryString["QC"] != null)
            {
                //如果是"另存为"的情况（即页面地址中QC的参数不为NUll），还需要初始化界面，初始化的方式：调用前台的setGMType 进行初始化。


                ClientScript.RegisterClientScriptBlock(typeof(string), "js", "setGMType('" + strMG + "','" + Request.QueryString["QC"].ToString() + "','" + (Request.QueryString["customerId"] == null ? "" : Request.QueryString["customerId"].ToString()) + "');", true);
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(typeof(string), "js", "setGMType('" + strMG + "','','');", true);
            }

        }
        //将Status状态变为Disabled
        public string DisableQC(string QC)
        {
            try
            {
                QcmaininfoModel main = new QcmaininfoModel();
                main.QualityCode = QC;
                main.Status = "Disabled";
                main.LastUpdateBy = HttpContext.Current.Session["UserId"].ToString();
                main.LastUpdateTime = System.DateTime.Now;
                main.ApproveDate = System.DateTime.Now;
                main.Approver = HttpContext.Current.Session["UserId"].ToString();
                qcMainManager.ApproveAndShowDown(main, null);
                return "1";
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
            }
        }
        //保存的时候，如果是圆机则更新AvaWidth和GEKComment的信息；带子和横机都只更新GEKComment信息
        protected void btnSave_Click(object sender, EventArgs e)
        {
            DbTransaction tran = DataAccess.DefaultDB.BeginTransaction();
            try
            {
                if (ViewState["MGType"].ToString() == "Fabric")
                {
                    List<AvaWidthModel> listAWM = avaWidth.GetAvaWidth();
                    QcavailablewidthModelList qcAvaWidthList = CreateAvaWidth(listAWM);

                    QcmaininfoModel mainModel = new QcmaininfoModel();
                    mainModel.QualityCode = tQC.Text;
                    mainModel.Remark = txtAvaRemark.Text;

                    if (!string.IsNullOrEmpty(HidArributeValue.Value))
                    {
                        string[] strArray = HidArributeValue.Value.Replace("<>", "#").Split('#');
                        //todo by mengjw
                        mainModel.QC_Ref_PPO = strArray[10];

                        mainModel.QC_Ref_GP = strArray[11] == "null" ? "" : strArray[11]; ;
                        mainModel.HF_Ref_PPO = strArray[12];
                        mainModel.HF_Ref_GP = strArray[13] == "null" ? "" : strArray[13]; ;
                        mainModel.RF_Remark = strArray[14];

                        //add by zheng zhou 2016-8-3 保存QC,HF的修改日志
                        if (Request.Form["hd_HF_Ref_GP_Old"] != (strArray[13] == "null" ? "" : strArray[13])
                            || Request.Form["hd_HF_Ref_PPO_Old"] != (strArray[12] == "null" ? "" : strArray[12])
                            || Request.Form["hd_QC_Ref_GP_Old"] != (strArray[11] == "null" ? "" : strArray[11])
                            || Request.Form["hd_QC_Ref_PPO_Old"] != (strArray[10] == "null" ? "" : strArray[10]))
                        {
                            QC_HF_ChangeLogModel changeModel = new QC_HF_ChangeLogModel();
                            QC_HF_ChangeLogManager changeManager = new QC_HF_ChangeLogManager();

                            changeModel.QualityCode = tQC.Text.Trim();
                            changeModel.HF_Ref_GP_New = strArray[13] == "null" ? "" : strArray[13];
                            changeModel.HF_Ref_PPO_New = strArray[12] == "null" ? "" : strArray[12];
                            changeModel.QC_Ref_GP_New = strArray[11] == "null" ? "" : strArray[11];
                            changeModel.QC_Ref_PPO_New = strArray[10] == "null" ? "" : strArray[10];
                            changeModel.HF_Ref_GP_Old = Request.Form["hd_HF_Ref_GP_Old"];
                            changeModel.HF_Ref_PPO_Old = Request.Form["hd_HF_Ref_PPO_Old"];
                            changeModel.QC_Ref_GP_Old = Request.Form["hd_QC_Ref_GP_Old"];
                            changeModel.QC_Ref_PPO_Old = Request.Form["hd_QC_Ref_PPO_Old"];
                            changeModel.CreateDate = System.DateTime.Now;
                            changeModel.Creator = HttpContext.Current.Session["UserId"].ToString();

                            changeManager.AddModel(changeModel, tran);
                        }
                        ////////////////////////////////////////////////
                    }

                    qcAvaWidthManager.DeleteModel(new QcavailablewidthModel() { QualityCode = tQC.Text }, tran);
                    qcAvaWidthManager.AddModels(qcAvaWidthList, tran);

                    qcMainManager.UpdateModelRemark(mainModel, tran);


                }
                // add by zheng zhou 2016-8-2 按照是否点击选择ppono，分不同的方法保留qcCustomer对象
                QccustomerlibraryModel qcCustomer = CreateCustomer();
                if (qcCustomer.BuyerId == "**X")
                    qcCustomerManager.UpdateModelThree(qcCustomer, tran);
                else
                    qcCustomerManager.UpdateModelOne(qcCustomer, tran);
                ////////////////////////////////////////////////////////////////////////////////////

                tran.Commit();
                Response.Write("<script language='JavaScript'>alert('Success')</script>");
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Response.Write("<script language='JavaScript'>alert('" + ex.Message.Replace("'", " ").Replace("\"", " ").Replace("\n", " ").Replace("\r", " ") + "');</script>");
                return;
            }
            //  Response.Write("<script language='JavaScript'>alert('Successfully updated!');window.location='Edit_Ref.aspx?MG=" + ViewState["MGType"].ToString() + "&QC=" + tQC.Text + "&customerId=" + tCustomerId.Text + "';</script>");    

        }

        private QcavailablewidthModelList CreateAvaWidth(List<AvaWidthModel> listAWM)
        {

            QcavailablewidthModelList qcAvaWidthList = new QcavailablewidthModelList();
            for (int i = 0; i < listAWM.Count; i++)
            {
                QcavailablewidthModel qcAvaModel = new QcavailablewidthModel();
                qcAvaModel.Gauge = listAWM[i].Gauge;
                qcAvaModel.Diameter = listAWM[i].Diameter;
                qcAvaModel.QualityCode = tQC.Text;
                qcAvaModel.TotalNeedles = listAWM[i].TotalNeedles;
                qcAvaModel.MaxWidth = listAWM[i].MaxWidth;
                qcAvaModel.Width = listAWM[i].Width;
                qcAvaModel.UpdatedBy = HttpContext.Current.Session["UserId"].ToString();
                qcAvaModel.UpdatedTime = System.DateTime.Now;
                qcAvaWidthList.Add(qcAvaModel);
            }
            return qcAvaWidthList;
        }


        //分派的时候，在客户表中增多一条记录

        public string AssignQC(string param)
        {
            try
            {
                string[] parameters = param.Split(new string[] { "(?$)" }, StringSplitOptions.None);
                QccustomerlibraryModel model = new QccustomerlibraryModel();
                model.QualityCode = parameters[0];
                model.BuyerId = parameters[1];
                model.MillComments = parameters[2];
                model.CustomerQualityId = parameters[3];
                model.SalesGroup = parameters[4];
                model.Sales = parameters[5];
                model.Brand = parameters[6];
                model.IsFirstOwner = "N";
                model.CreateDate = System.DateTime.Now;
                model.Creator = HttpContext.Current.Session["UserId"].ToString();
                qcCustomerManager.AddModel(model, null);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private QccustomerlibraryModel CreateCustomer()
        {
            QccustomerlibraryModel qcCustomerModel = new QccustomerlibraryModel();
            qcCustomerModel.QualityCode = tQC.Text;
            qcCustomerModel.BuyerId = string.IsNullOrEmpty(tCustomerId.Text) ? "**X" : tCustomerId.Text;
            //qcCustomerModel.BuyerId = tCustomerId.Text;
            //qcCustomerModel.MillComments = GekComments.Text; Request.Form["TextBox1"].Trim();
            qcCustomerModel.MillComments = Request.Form["GekComments"].Trim();
            qcCustomerModel.CustomerQualityId = Request.Form["tCustomerQualityId"].Trim();
            return qcCustomerModel;
        }


        public string GetUserID(string a)
        {
            return HttpContext.Current.Session["UserId"].ToString();
        }

        public string GetQCStatus(string qc)
        {
            QcmaininfoManager qmm = new QcmaininfoManager();
            QcmaininfoModel qm = new QcmaininfoModel();
            qm.QualityCode = tQC.Text.Trim();

            QcmaininfoModelList qml = qmm.GetModelList(qm);
            if (qml.Count > 0)
            {
                return qml[0].Status.ToLower();
            }
            else
            {
                return string.Empty;
            }
        }


        #region ""

        public string QCTrasferToAX(string sQC)
        {
            try
            {
                if (sQC.Contains("E"))
                {
                    return "The code is not internal, don't need to call AX";
                }

                QccustomerlibraryManager cumanager = new QccustomerlibraryManager();
                QcmaininfoManager qcMainManager = new QcmaininfoManager();
                QcconstructiondtlManager qcConManager = new QcconstructiondtlManager();
                QcyarndtlManager yarnManager = new QcyarndtlManager();
                QcfinishdtlManager finishManager = new QcfinishdtlManager();
                QcavailablewidthManager avaManager = new QcavailablewidthManager();


                QcmaininfoModelList qcMainModelList = qcMainManager.GetModelList(new QcmaininfoModel() { QualityCode = sQC });
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

                QcconstructiondtlModelList qcConstructionList = qcConManager.GetModelList(new QcconstructiondtlModel() { QualityCode = sQC });
                QcyarndtlModelList qcYarnList = yarnManager.GetModelList(new QcyarndtlModel() { QualityCode = sQC });
                QcfinishdtlModelList qcFinfishing = finishManager.GetModelList(new QcfinishdtlModel() { QualityCode = sQC });
                QcavailablewidthModelList qcAvaWidthList = avaManager.GetModelList(new QcavailablewidthModel() { QualityCode = sQC });
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
                        IsFirstOwnerStr = (customerList[i].IsFirstOwner == "Y" ? "Yes" : "No"),

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

                if (finish.Count > 0)
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
                data.InventStatus = TEX_InventStatus.Approved;
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


        #endregion

    }
}