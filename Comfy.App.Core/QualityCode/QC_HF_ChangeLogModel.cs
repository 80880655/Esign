//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。

//     运行库版本:2.0.50727.832
//
//     对此文件的更改可能会导致不正确的行为，并且如果

//     重新生成代码，这些更改将会丢失。

// </auto-generated>
//------------------------------------------------------------------------------

namespace Comfy.App.Core.QualityCode
{


    [System.Serializable()]
    public class QC_HF_ChangeLogModelList : Comfy.Data.DataObjectList<QC_HF_ChangeLogModel>
    {
    }

    [System.Serializable()]
    public partial class QC_HF_ChangeLogModel : Comfy.Data.DataObject<QC_HF_ChangeLogModel>
    {

        #region Member Field Region

        private int _Iden { get; set; }
        private string _QualityCode { get; set; }
        private string _QC_Ref_PPO_Old { get; set; }
        private string _QC_Ref_GP_Old { get; set; }
        private string _HF_Ref_PPO_Old { get; set; }
        private string _HF_Ref_GP_Old { get; set; }
        private string _QC_Ref_PPO_New { get; set; }
        private string _QC_Ref_GP_New { get; set; }
        private string _HF_Ref_PPO_New { get; set; }
        private string _HF_Ref_GP_New { get; set; }
        private System.DateTime _CreateDate;
        private string _Creator;

        #endregion

        #region Member Property Region

        /// <summary>
        /// 序列号
        /// </summary>
        public int Iden
        {
            get { return _Iden; }
            set { _Iden = value; }
        }
        /// <summary>
        /// QualityCode
        /// </summary>
        public string QualityCode
        {
            get { return _QualityCode; }
            set { _QualityCode = value; }
        }
        /// <summary>
        /// QC_Ref_PPO旧值
        /// </summary>
        public string QC_Ref_PPO_Old
        {
            get { return _QC_Ref_PPO_Old; }
            set { _QC_Ref_PPO_Old = value; }
        }
        /// <summary>
        /// QC_Ref_GP旧值
        /// </summary>
        public string QC_Ref_GP_Old
        {
            get { return _QC_Ref_GP_Old; }
            set { _QC_Ref_GP_Old = value; }
        }
        /// <summary>
        /// HF_Ref_PPO旧值
        /// </summary>
        public string HF_Ref_PPO_Old
        {
            get { return _HF_Ref_PPO_Old; }
            set { _HF_Ref_PPO_Old = value; }
        }
        /// <summary>
        /// HF_Ref_GP_Old旧值
        /// </summary>
        public string HF_Ref_GP_Old
        {
            get { return _HF_Ref_GP_Old; }
            set { _HF_Ref_GP_Old = value; }
        }
        /// <summary>
        /// QC_Ref_PPO新值
        /// </summary>
        public string QC_Ref_PPO_New
        {
            get { return _QC_Ref_PPO_New; }
            set { _QC_Ref_PPO_New = value; }
        }
        /// <summary>
        /// QC_Ref_GP新值
        /// </summary>
        public string QC_Ref_GP_New
        {
            get { return _QC_Ref_GP_New; }
            set { _QC_Ref_GP_New = value; }
        }
        /// <summary>
        /// HF_Ref_PPO新值
        /// </summary>
        public string HF_Ref_PPO_New
        {
            get { return _HF_Ref_PPO_New; }
            set { _HF_Ref_PPO_New = value; }
        }
        /// <summary>
        /// HF_Ref_GP新值
        /// </summary>
        public string HF_Ref_GP_New
        {
            get { return _HF_Ref_GP_New; }
            set { _HF_Ref_GP_New = value; }
        }
        /// <summary>
        /// <para>创建日期</para>
        /// </summary>
        public System.DateTime CreateDate
        {
            get
            {
                return this._CreateDate;
            }
            set
            {
                base.SetValue(ref this._CreateDate, value);
            }
        }

        /// <summary>
        /// <para>创建人</para>
        /// </summary>
        public string Creator
        {
            get
            {
                return this._Creator;
            }
            set
            {
                base.SetValue(ref this._Creator, value);
            }
        }

        #endregion
    }
}
