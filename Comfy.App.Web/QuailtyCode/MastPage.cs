using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Comfy.App.Core.QualityCode;
using System.Data;
using System.Text;

namespace Comfy.App.Web.QuailtyCode
{
    public class MastPage : System.Web.UI.Page
    {

        public QcmaininfoManager qcMainManager = new QcmaininfoManager();
        public QcavailablewidthManager qcAvaWidthManager = new QcavailablewidthManager();
        public QcyarndtlManager qcYarnManager = new QcyarndtlManager();
        public QcfinishdtlManager qcFinishManager = new QcfinishdtlManager();
        public QccustomerlibraryManager qcCustomerManager = new QccustomerlibraryManager();
        public QcconstructiondtlManager qcConstructionManager = new QcconstructiondtlManager();

        protected virtual QcyarndtlModelList qcYarnList { get; set; }
        protected virtual QcmaininfoModelList qcMainList { get; set; }
        protected virtual QcavailablewidthModelList qcAvaList { get; set; }
        protected virtual QcfinishdtlModelList qcFinishList { get; set; }
        protected virtual QccustomerlibraryModelList qcCustomerList { get; set; }
        protected virtual QcconstructiondtlModelList qcConstructionList { get; set; }


        public string InitInfo(string code)
        {
            // 把code转化为大写 by LYH 2014/2/25
            code = code.ToUpper();
            qcMainList = qcMainManager.GetModelList(new QcmaininfoModel() { QualityCode = code });
            qcYarnList = qcYarnManager.GetModelList(new QcyarndtlModel() { QualityCode = code });
            qcAvaList = qcAvaWidthManager.GetModelList(new QcavailablewidthModel() { QualityCode = code });
            qcFinishList = qcFinishManager.GetModelList(new QcfinishdtlModel() { QualityCode = code });
            qcCustomerList = qcCustomerManager.GetModelList(new QccustomerlibraryModel() { QualityCode = code });
            qcConstructionList = qcConstructionManager.GetModelList(new QcconstructiondtlModel() { QualityCode = code });


            InitCAttribute();

            if (qcMainList.Count < 1)
            {
                return "No";
            }

            string i = GetCmainInfo() + "<|>" + GetCConstruction() + "<|>" + GetCFinish() + "<|>" + GetCAvaWdith() + "<|>" + qcMainList[0].YarnLength + "<|>" +
                qcMainList[0].Measurement + "<|>" + qcMainList[0].TappingType + "<|>" + qcMainList[0].AnalysisNo + "<|>" + qcMainList[0].Sourcing + "<|>" + qcMainList[0].Status +
                "<|>" + qcMainList[0].MaterialGroup + "<|>" + qcMainList[0].Remark + "<|>" + qcMainList[0].GK_NO + "<|>" + qcMainList[0].QC_Ref_PPO + "<|>" + qcMainList[0].QC_Ref_GP + "<|>" + qcMainList[0].HF_Ref_PPO + "<|>" + qcMainList[0].HF_Ref_GP + "<|>" + qcMainList[0].RF_Remark;

            return GetCmainInfo() + "<|>" + GetCConstruction() + "<|>" + GetCFinish() + "<|>" + GetCAvaWdith() + "<|>" + qcMainList[0].YarnLength + "<|>" +
                qcMainList[0].Measurement + "<|>" + qcMainList[0].TappingType + "<|>" + qcMainList[0].AnalysisNo + "<|>" + qcMainList[0].Sourcing + "<|>" + qcMainList[0].Status +
                "<|>" + qcMainList[0].MaterialGroup + "<|>" + qcMainList[0].Remark + "<|>" + qcMainList[0].GK_NO + "<|>" + qcMainList[0].QC_Ref_PPO + "<|>" + qcMainList[0].QC_Ref_GP + "<|>" + qcMainList[0].HF_Ref_PPO + "<|>" + qcMainList[0].HF_Ref_GP + "<|>" + qcMainList[0].RF_Remark;



        }

        public string InitInfoOne(string allParam)
        {
            try
            {
                CustomerManager cm = new CustomerManager();
                qcFinishList = new QcfinishdtlModelList();
                qcMainList = new QcmaininfoModelList();
                qcConstructionList = new QcconstructiondtlModelList();
                qcFinishList = new QcfinishdtlModelList();
                DataSet ds = cm.GetInfoFromKmis(allParam);
                DataTable dtMain = ds.Tables[0];
                InitMain(ds.Tables[0]);
                InitConstruction(ds.Tables[1]);
                InitFinish(ds.Tables[2]);
                InitYarnInfo(ds.Tables[3]);

                InitCAttribute();

                return GetCmainInfo() + "<|>" + GetCConstruction() + "<|>" + GetCFinish()+ "<|><|>" + qcMainList[0].YarnLength + "<|>" +
                qcMainList[0].Measurement + "<|>" + qcMainList[0].TappingType + "<|><|><|>" + "<|>" + qcMainList[0].QC_Ref_PPO + "<|>" + qcMainList[0].HF_Ref_PPO + "<|>" + qcMainList[0].QC_Ref_GP + "<|>" + qcMainList[0].HF_Ref_GP + "<|>" + qcMainList[0].RF_Remark;


            }
            catch (Exception ex)
            {
                return "error:"+ex.Message;
            }

        }

        public string InitInfoGkNo(string analysisNO)
        {
            try
            {
                CustomerManager cm = new CustomerManager();
                qcFinishList = new QcfinishdtlModelList();
                qcMainList = new QcmaininfoModelList();
                qcConstructionList = new QcconstructiondtlModelList();
                DataSet ds = cm.GetInfoByGKNo(analysisNO);
                if (ds.Tables[0].Rows.Count == 0)
                    return "0";
                InitMainByGkNo(ds.Tables[0]);
                InitConstructionByGkNo(ds.Tables[1]);
                InitYarnInfoByGkNo(ds.Tables[2]);
                InitFinish(ds.Tables[3]);        

                InitCAttribute();
               // qcFinishList = new QcfinishdtlModelList();

                return GetCmainInfo() + "<|>" + GetCConstruction() + "<|>" + GetCFinish() + "<|><|>" + qcMainList[0].YarnLength + "<|>" +
                qcMainList[0].Measurement + "<|>" + qcMainList[0].TappingType + "<|><|><|><|>" + qcMainList[0].TempComments;
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
            }
        }

        public string InitInfoAN(string analysisNO)
        {
            try
            {
                CustomerManager cm = new CustomerManager();
                qcFinishList = new QcfinishdtlModelList();
                qcMainList = new QcmaininfoModelList();
                qcConstructionList = new QcconstructiondtlModelList();
                DataSet ds = cm.GetInfoByAnalysisNo(analysisNO);
                if (ds.Tables[0].Rows.Count == 0)
                    return "0";
                InitMainOne(ds.Tables[0]);
                InitConstructionOne(ds.Tables[0]);
                InitYarnInfoOne(ds.Tables[1]);



                InitCAttribute();
                qcFinishList = new QcfinishdtlModelList();

                return GetCmainInfo() + "<|>" + GetCConstruction() + "<|>" + GetCFinish();
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
            }
        }

        public void InitMainOne(DataTable dt)
        {
            QcmaininfoModel model = new QcmaininfoModel();
            if (dt.Rows.Count == 0) return;
            qcMainList = new QcmaininfoModelList();
            model.DyeMethod = dt.Rows[0]["Dye_Method"].ToString();
            model.Pattern = dt.Rows[0]["Pattern"].ToString(); ;
            model.YarnLength = dt.Rows[0]["Yarn_Length"].ToString();
            qcMainList.Add(model);
        }

        public void InitMainByGkNo(DataTable dt)
        {
            QcmaininfoModel model = new QcmaininfoModel();
            if (dt.Rows.Count == 0) return;
            qcMainList = new QcmaininfoModelList();
            model.DyeMethod = dt.Rows[0]["Dye_Method"].ToString();
            model.Pattern = dt.Rows[0]["Pattern"].ToString(); ;
            model.YarnLength = dt.Rows[0]["YarnLength"].ToString();
            model.ShrinkageTestingMethod = dt.Rows[0]["Test_DryMode"].ToString();
            model.GmtWashing = dt.Rows[0]["GMT_Wash"].ToString();
            model.Shrinkage = dt.Rows[0]["Shringkage"].ToString();
            model.BfGmmm = dt.Rows[0]["DensityMin"].ToString() == "" ? 0 : (int) Convert.ToDecimal(dt.Rows[0]["DensityMin"]);
            model.AfGmmm = dt.Rows[0]["AfterWashKemm"].ToString() == "" ? 0 :(int) Convert.ToDecimal(dt.Rows[0]["AfterWashKemm"]);
            model.TappingType = dt.Rows[0]["TappingType"].ToString();
            model.YarnLength = dt.Rows[0]["yarnLength"].ToString();
            model.Measurement = dt.Rows[0]["Width"].ToString();
            model.TempComments = dt.Rows[0]["GekComments"].ToString();
            qcMainList.Add(model);
        }

        public void InitConstructionByGkNo(DataTable dt)
        {
            qcConstructionList = new QcconstructiondtlModelList();
            if (dt.Rows.Count == 0) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                QcconstructiondtlModel cm = new QcconstructiondtlModel();
                cm.Construction = dt.Rows[i]["Construction_abbr"].ToString();
                qcConstructionList.Add(cm);
            }       
        }

        public void InitConstructionOne(DataTable dt)
        {
            qcConstructionList = new QcconstructiondtlModelList();
            if (dt.Rows.Count == 0) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                QcconstructiondtlModel cm = new QcconstructiondtlModel();
                cm.Construction = dt.Rows[i]["Construction"].ToString();
                qcConstructionList.Add(cm);
            }
        }

        public void InitMain(DataTable dt)
        {
            QcmaininfoModel model = new QcmaininfoModel();
            if (dt.Rows.Count == 0) return;
            qcMainList = new QcmaininfoModelList();
            model.Pattern = dt.Rows[0]["Pattern"].ToString();
            model.DyeMethod = dt.Rows[0]["Dye_Method"].ToString();
            model.BfGmmm = dt.Rows[0]["BF_Gmmm"].ToString() == "" ? 0 : Int32.Parse(dt.Rows[0]["BF_Gmmm"].ToString());
            model.AfGmmm = dt.Rows[0]["AF_Gmmm"].ToString() == "" ? 0 : Int32.Parse(dt.Rows[0]["AF_Gmmm"].ToString());
            model.Shrinkage = dt.Rows[0]["Shrinkage"].ToString();
            model.ShrinkageTestingMethod = dt.Rows[0]["Test_DryMode"].ToString();
            model.GmtWashing = dt.Rows[0]["GMT_Washing"].ToString();
            model.YarnLength = dt.Rows[0]["Yarn_Length"].ToString();
            model.TappingType = dt.Rows[0]["Tapping_Type"].ToString();
            model.Measurement = dt.Rows[0]["Measurement"].ToString();
            qcMainList.Add(model);

        }

        public void InitConstruction(DataTable dt)
        {
            qcConstructionList = new QcconstructiondtlModelList();
            if (dt.Rows.Count == 0) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                QcconstructiondtlModel cm = new QcconstructiondtlModel();
                cm.Construction = dt.Rows[i]["Construction"].ToString();
                qcConstructionList.Add(cm);
            }
        }


        public void InitFinish(DataTable dt)
        {
            qcFinishList = new QcfinishdtlModelList();
            if (dt.Rows.Count == 0) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                QcfinishdtlModel cm = new QcfinishdtlModel();
                cm.FinishingCode = dt.Rows[i]["Finishing_Code"].ToString();
                cm.Description = dt.Rows[i]["Finishing_Name"].ToString();
                qcFinishList.Add(cm);
            }
        }


        public void InitYarnInfo(DataTable dt)
        {
            qcYarnList = new QcyarndtlModelList();
            if (dt.Rows.Count == 0) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                QcyarndtlModel cm = new QcyarndtlModel();
                cm.YarnType = dt.Rows[i]["Yarn_Type"].ToString();
                cm.YarnCount = dt.Rows[i]["Yarn_Count"].ToString();
                cm.Threads = dt.Rows[i]["Threads"].ToString() == "" ? 0 : (int)dt.Rows[i]["Threads"];
                cm.YarnRatio = dt.Rows[i]["Yarn_Ratio"].ToString() == "" ? 0 : (decimal)dt.Rows[i]["Yarn_Ratio"];
                cm.WarpWeft = dt.Rows[i]["Warp_Weft"].ToString();
                cm.YarnDensity = dt.Rows[i]["Yarn_Density"].ToString() == "" ? 0 : (decimal)dt.Rows[i]["Yarn_Density"];
                qcYarnList.Add(cm);
            }
        }

        public void InitYarnInfoOne(DataTable dt)
        {
            qcYarnList = new QcyarndtlModelList();
            if (dt.Rows.Count == 0) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                QcyarndtlModel cm = new QcyarndtlModel();
                cm.YarnType = dt.Rows[i]["Yarn_Type"].ToString();
                cm.YarnCount = dt.Rows[i]["Yarn_Count"].ToString();
                object o = dt.Rows[i]["Threads"];
                cm.Threads = dt.Rows[i]["Threads"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[i]["Threads"]);
                cm.YarnRatio = dt.Rows[i]["Yarn_Ratio"].ToString() == "" ? 0 : Convert.ToDecimal(dt.Rows[i]["Yarn_Ratio"]);
                qcYarnList.Add(cm);
            }
        }
        public void InitYarnInfoByGkNo(DataTable dt)
        {
            qcYarnList = new QcyarndtlModelList();
            if (dt.Rows.Count == 0) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                QcyarndtlModel cm = new QcyarndtlModel();
                cm.YarnType = dt.Rows[i]["Yarn_Type"].ToString();
                cm.YarnCount = dt.Rows[i]["Yarn_Count"].ToString();
                cm.Threads = dt.Rows[i]["Threads"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[i]["Threads"]);
                cm.YarnRatio = dt.Rows[i]["Yarn_Percent"].ToString() == "" ? 0 : Convert.ToDecimal(dt.Rows[i]["Yarn_Percent"]);
                cm.YarnComponent = dt.Rows[i]["Remark"].ToString();
                qcYarnList.Add(cm);
            }
        }

        /// <summary>
        /// 创建由搜索Quality Code产生的FabricModel字符串

        /// by LYH
        /// </summary>
        /// <param name="code">Quality Code</param>
        /// <returns></returns>
        public string InitFabricInfo(string code)
        {
            if (code == null || code == "")
            {
                return "error: no input!";
            }

            string spliterField = "<|>";
            //string spliterRecord = "<?>";
            StringBuilder strB = null;
            CustomerManager cm = null;
            List<FabricCodeModel> listFabricCodeModel = null;

            try
            {
                strB = new StringBuilder();
                cm = new CustomerManager();
                listFabricCodeModel = cm.GetFabricCodeListOne(code);
                if (listFabricCodeModel == null) return "no result.";
                if (listFabricCodeModel.Count < 1) return "";

                    strB.Append(listFabricCodeModel[0].PPONO + spliterField);
                    strB.Append(listFabricCodeModel[0].QualityCode + spliterField);
                    strB.Append(listFabricCodeModel[0].FabricPart + spliterField);
                    //strB.Append(listFabricCodeModel[0].FabricCode + spliterField);
                    //strB.Append(listFabricCodeModel[0].ComboCode + spliterField);
                    strB.Append(listFabricCodeModel[0].ComboName + spliterField);
                    //strB.Append(listFabricCodeModel[0].SampleApprove + spliterField);
                    strB.Append(listFabricCodeModel[0].CustomerComment + spliterField);
                    //strB.Append(listFabricCodeModel[0].FabricWidth + spliterField);
                    //strB.Append(listFabricCodeModel[0].LastModiDate.ToString() + spliterField);
                    //strB.Append(listFabricCodeModel[0].LastModiUserId + spliterField);
                    strB.Append(listFabricCodeModel[0].ViewFlag + spliterField);
                    //strB.Append(listFabricCodeModel[0].CustomerName + spliterField);
                    strB.Append(listFabricCodeModel[0].CustomerId + spliterField);
                    strB.Append(listFabricCodeModel[0].Status);
                    //strB.Append(listFabricCodeModel[0].Iden + spliterField);
                    //strB.Append(spliterRecord);

                return strB.ToString();
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
            }
        }

        protected virtual string SessionName
        {
            get
            {
                return "";
            }
        }

        public List<YarnInfo> listInfo;

        public void InitCAttribute()
        {
            List<YarnInfo> listInfo = new List<YarnInfo>();
            for (int i = 0; i < qcYarnList.Count; i++)
            {
                YarnInfo yarnInfo = new YarnInfo();
                yarnInfo.Seq = (i + 1).ToString();
                yarnInfo.Threads = qcYarnList[i].Threads;
                yarnInfo.Radio = qcYarnList[i].YarnRatio;
                yarnInfo.YarnCount = qcYarnList[i].YarnCount;
                yarnInfo.YarnType = qcYarnList[i].YarnType;
                yarnInfo.WarpWeft = qcYarnList[i].WarpWeft;
                yarnInfo.YarnDensity = qcYarnList[i].YarnDensity;
                yarnInfo.YarnComponent = qcYarnList[i].YarnComponent;
                listInfo.Add(yarnInfo);
            }
            HttpContext.Current.Session[SessionName] = listInfo;
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        public List<YarnInfo> GetYarnInfo(string orderByField)
        {
            try
            {
                if (HttpContext.Current.Session[SessionName] != null)
                {
                    listInfo = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
                }
                else
                {
                    listInfo = new List<YarnInfo>();
                }
                return listInfo;
            }
            catch (Exception exc)
            {
                return null;
            }
        }
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Insert)]
        public void AddYarnInfo(YarnInfo model)
        {
            try
            {
                if (HttpContext.Current.Session[SessionName] != null)
                {
                    listInfo = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
                    model.Seq = (listInfo.Count + 1).ToString();
                }
                else
                {
                    listInfo = new List<YarnInfo>();
                    model.Seq = "1";
                }
                listInfo.Add(model);
                HttpContext.Current.Session[SessionName] = listInfo;

            }
            catch (Exception exc)
            {
                // throw GetError(exc);
            }
        }

        public void EditYarnInfo(YarnInfo model)
        {
            try
            {
                listInfo = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
                for (int i = 0; i < listInfo.Count; i++)
                {
                    if (listInfo[i].Seq == model.Seq)
                    {
                      //  listInfo.RemoveAt(i);
                        listInfo[i].Radio = model.Radio;
                        listInfo[i].Threads = model.Threads;
                        listInfo[i].YarnType = model.YarnType;
                        listInfo[i].YarnCount = model.YarnCount;
                        listInfo[i].WarpWeft = model.WarpWeft;
                        listInfo[i].YarnDensity = model.YarnDensity;
                        listInfo[i].YarnComponent = model.YarnComponent;
                        HttpContext.Current.Session[SessionName] = listInfo;
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                // throw GetError(exc);
            }
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Delete)]
        public void DeleteYarnInfo(YarnInfo model)
        {
            try
            {
                listInfo = (List<YarnInfo>)HttpContext.Current.Session[SessionName];
                for (int i = 0; i < listInfo.Count; i++)
                {
                    if (listInfo[i].Seq == model.Seq)
                    {
                        listInfo.RemoveAt(i);
                        HttpContext.Current.Session[SessionName] = listInfo;
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                // throw GetError(exc);
            }
        }

        public string GetCAvaWdith()
        {
            string strTemp = "";
            for (int i = 0; i < qcAvaList.Count; i++)
            {
                if (i != qcAvaList.Count - 1)
                {
                    strTemp = strTemp + qcAvaList[i].Gauge + "," + qcAvaList[i].Diameter + "," + qcAvaList[i].TotalNeedles + "," + qcAvaList[i].Width + "," + qcAvaList[i].MaxWidth + "<?>";
                }
                else
                {
                    strTemp = strTemp + qcAvaList[i].Gauge + "," + qcAvaList[i].Diameter + "," + qcAvaList[i].TotalNeedles + "," + qcAvaList[i].Width + "," + qcAvaList[i].MaxWidth;
                }
            }
            return strTemp;
        }

        public string GetCmainInfo()
        {
            if (qcMainList.Count > 0)
            {
                return qcMainList[0].BfGmmm + "<|>" + qcMainList[0].AfGmmm + "<|>" +
                    qcMainList[0].DyeMethod + "<|>" + qcMainList[0].Pattern + "<|>" +
                    qcMainList[0].Layout + "<|>" + qcMainList[0].Shrinkage + "<|>" +
                    qcMainList[0].ShrinkageTestingMethod + "<|>" + qcMainList[0].GmtWashing;
            }
            return "null";
        }

        public string GetCConstruction()
        {
            string strTemp = "";
            for (int i = 0; i < qcConstructionList.Count; i++)
            {
                if (i != qcConstructionList.Count - 1)
                {
                    strTemp = strTemp + qcConstructionList[i].Construction + ",";
                }
                else
                {
                    strTemp = strTemp + qcConstructionList[i].Construction;
                }
            }
            return strTemp;
        }

        public string GetCFinish()
        {
            string strTemp = "";
            for (int i = 0; i < qcFinishList.Count; i++)
            {
                if (i != qcFinishList.Count - 1)
                {
                    strTemp = strTemp + qcFinishList[i].Description + "[" + qcFinishList[i].FinishingCode + "]" + "<4>";
                }
                else
                {
                    strTemp = strTemp + qcFinishList[i].Description + "[" + qcFinishList[i].FinishingCode + "]";
                }
            }
            return strTemp;
        }

        public string GetCGekComment()
        {
            string strTemp = "";
            for (int i = 0; i < qcCustomerList.Count; i++)
            {
                if (i != qcCustomerList.Count - 1)
                {
                    strTemp = strTemp + qcCustomerList[i].MillComments + ",";
                }
                else
                {
                    strTemp = strTemp + qcCustomerList[i].MillComments;
                }
            }
            return strTemp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetQC_Ref_GP(string strppo_no)
        { 
            CustomerManager custManager=new CustomerManager();
            string strResult= custManager.GetFabric_type_cd(strppo_no);
            return strResult;
        }
        
        public int IsEffective(string strppo_no)
        {
            CustomerManager custManager = new CustomerManager();

            int iResult = custManager.IsEffective(strppo_no);
            return iResult;
        }
        //2019-01-15 add by linyob Quality code自动带入质量标准.
        public string GetPhyWebTestData(string allParam)
        {
            CustomerManager cm = null;

            string rtn = "";
            try
            {

                cm = new CustomerManager();
                DataTable dt = cm.GetPhyWebTest(allParam);
                if (dt != null && dt.Rows.Count > 0)
                {


                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        if (dr["Item_Name"].ToString().Trim().ToUpper() != "Repeat".ToUpper())
                        {
                            if (dr["Gk_Stand1"].ToString().Trim() != "")
                            {
                                string str = dr["Gk_Stand1"].ToString().Trim();
                                if (str.Substring(str.Length - 1) != "%")
                                    dr["Gk_Stand1"] += "%";


                            }

                            if (dr["Gk_Stand2"].ToString().Trim() != "")
                            {
                                string str = dr["Gk_Stand2"].ToString().Trim();
                                if (str.Substring(str.Length - 1) != "%")
                                    dr["Gk_Stand2"] += "%";
                            }
                        }
                        if (dr["Gk_Stand2"].ToString().Trim() != "")
                            rtn += "<|>" + dr["Item_Name"].ToString() + ":" + dr["Gk_Stand1"].ToString() + "&-&" + dr["Gk_Stand2"].ToString();
                        else
                            rtn += "<|>" + dr["Item_Name"].ToString() + ":" + dr["Gk_Stand1"].ToString();
                    }
                }
                if (rtn != "")
                    rtn = rtn.Substring(3).Replace("<|>", "\n");

                return rtn;

            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
            }
        }

    }
}