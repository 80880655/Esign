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
    public class QcconstructiondtlModelList : Comfy.Data.DataObjectList<QcconstructiondtlModel>
    {
    }

    [System.Serializable()]
    public partial class QcconstructiondtlModel : Comfy.Data.DataObject<QcconstructiondtlModel>
    {

        #region Member Field Region
        private int _Iden;

        private string _QualityCode;

        private string _Construction;
        #endregion

        #region Member Property Region
        /// <summary>
        /// <para>Iden</para>
        /// </summary>
        public int Iden
        {
            get
            {
                return this._Iden;
            }
            set
            {
                base.SetValue(ref this._Iden, value);
            }
        }

        /// <summary>
        /// <para>Quality Code</para>
        /// </summary>
        public string QualityCode
        {
            get
            {
                return this._QualityCode;
            }
            set
            {
                base.SetValue(ref this._QualityCode, value);
            }
        }

        /// <summary>
        /// <para>组织结构ID</para>
        /// </summary>
        public string Construction
        {
            get
            {
                return this._Construction;
            }
            set
            {
                base.SetValue(ref this._Construction, value);
            }
        }
        #endregion
    }
}