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
    public class PbknitconstructionModelList : Comfy.Data.DataObjectList<PbknitconstructionModel>
    {
    }

    [System.Serializable()]
    public partial class PbknitconstructionModel : Comfy.Data.DataObject<PbknitconstructionModel>
    {

        #region Member Field Region
        private string _Construction;

        private string _Description;

        private string _DescriptionCn;

        private string _SingleDouble;

        private string _IsSpecial;

        private string _IsActive;
        #endregion

        #region Member Property Region
        /// <summary>
        /// <para>布种ID</para>
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

        /// <summary>
        /// <para>布种描述</para>
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
        /// <para>中文描述</para>
        /// </summary>
        public string DescriptionCn
        {
            get
            {
                return this._DescriptionCn;
            }
            set
            {
                base.SetValue(ref this._DescriptionCn, value);
            }
        }

        /// <summary>
        /// <para>单双面</para>
        /// </summary>
        public string SingleDouble
        {
            get
            {
                return this._SingleDouble;
            }
            set
            {
                base.SetValue(ref this._SingleDouble, value);
            }
        }

        /// <summary>
        /// <para>是否特殊布种</para>
        /// </summary>
        public string IsSpecial
        {
            get
            {
                return this._IsSpecial;
            }
            set
            {
                base.SetValue(ref this._IsSpecial, value);
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
