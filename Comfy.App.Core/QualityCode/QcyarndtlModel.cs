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
    public class QcyarndtlModelList : Comfy.Data.DataObjectList<QcyarndtlModel>
    {
    }

    [System.Serializable()]
    public partial class QcyarndtlModel : Comfy.Data.DataObject<QcyarndtlModel>
    {

        #region Member Field Region
        private int _Iden;

        private string _QualityCode;

        private string _YarnType;

        private string _YarnCount;

        private int _Threads;

        private decimal _YarnRatio;

        private string _WarpWeft;


        public string YarnComponent { get; set; }

        private decimal _YarnDensity;
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
        /// <para>纱支</para>
        /// </summary>
        public string YarnCount
        {
            get
            {
                return this._YarnCount;
            }
            set
            {
                base.SetValue(ref this._YarnCount, value);
            }
        }

        /// <summary>
        /// <para>股数</para>
        /// </summary>
        public int Threads
        {
            get
            {
                return this._Threads;
            }
            set
            {
                base.SetValue(ref this._Threads, value);
            }
        }

        /// <summary>
        /// <para>纱比</para>
        /// </summary>
        public decimal YarnRatio
        {
            get
            {
                return this._YarnRatio;
            }
            set
            {
                base.SetValue(ref this._YarnRatio, value);
            }
        }

        /// <summary>
        /// <para>经纬向</para>
        /// </summary>
        public string WarpWeft
        {
            get
            {
                return this._WarpWeft;
            }
            set
            {
                base.SetValue(ref this._WarpWeft, value);
            }
        }

        /// <summary>
        /// <para>密度</para>
        /// </summary>
        public decimal YarnDensity
        {
            get
            {
                return this._YarnDensity;
            }
            set
            {
                base.SetValue(ref this._YarnDensity, value);
            }
        }
        #endregion
    }
}
