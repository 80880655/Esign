﻿//------------------------------------------------------------------------------
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
    public class PbknityarntypeModelList : Comfy.Data.DataObjectList<PbknityarntypeModel>
    {
    }

    [System.Serializable()]
    public partial class PbknityarntypeModel : Comfy.Data.DataObject<PbknityarntypeModel>
    {

        #region Member Field Region
        private string _YarnType;

        private string _Description;

        private string _IeDescription;

        private string _CustomerDescription;

        private string _IsActive;
        #endregion

        #region Member Property Region
        /// <summary>
        /// <para>纱类</para>
        /// </summary>
        public string YarnType
        {
            get
            {
                return this._YarnType;
            }
            set
            {
                base.SetValue(ref this._YarnType, value);
            }
        }

        /// <summary>
        /// <para>纱类描述</para>
        /// </summary>
        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                base.SetValue(ref this._Description, value);
            }
        }

        /// <summary>
        /// <para>报关描述</para>
        /// </summary>
        public string IeDescription
        {
            get
            {
                return this._IeDescription;
            }
            set
            {
                base.SetValue(ref this._IeDescription, value);
            }
        }

        /// <summary>
        /// <para>客户报告描述</para>
        /// </summary>
        public string CustomerDescription
        {
            get
            {
                return this._CustomerDescription;
            }
            set
            {
                base.SetValue(ref this._CustomerDescription, value);
            }
        }

        /// <summary>
        /// <para>是否有效</para>
        /// </summary>
        public string IsActive
        {
            get
            {
                return this._IsActive;
            }
            set
            {
                base.SetValue(ref this._IsActive, value);
            }
        }
        #endregion
    }
}