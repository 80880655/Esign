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
    using Comfy.Data;
    using Comfy.App.Core.QualityCode;


    public class PbknitfinishManager
    {

        public PbknitfinishModelList GetModelList(PbknitfinishModel model)
        {
            PbknitfinishTable table = new PbknitfinishTable();
            SelectSqlSection sql = DataAccess.DefaultDB.Select(table, table.AllColumns())
                ;
            using (SafeDataReader sdr = new SafeDataReader(sql.ToDataReader()))
            {
                PbknitfinishModelList result = new PbknitfinishModelList();
                while (sdr.Read())
                {
                    PbknitfinishModel m = new PbknitfinishModel();
                    m.FinishingCode = sdr.GetString(table.FinishingCode);
                    m.FinishingName = sdr.GetString(table.FinishingName);
                    m.Description = sdr.GetString(table.Description);
                    m.WashingFlag = sdr.GetString(table.WashingFlag);
                    m.IsActive = sdr.GetString(table.IsActive);
                    result.Add(m);
                }
                return result;
            }
        }

        public void AddModel(PbknitfinishModel model)
        {
            //model.CreateTime = System.DateTime.Now;
            PbknitfinishTable table = new PbknitfinishTable();
            DataAccess.DefaultDB.Insert(table)
                .AddColumn(table.FinishingCode, model.FinishingCode)
                .AddColumn(table.FinishingName, model.FinishingName)
                .AddColumn(table.Description, model.Description)
                .AddColumn(table.WashingFlag, model.WashingFlag)
                .AddColumn(table.IsActive, model.IsActive)
                .Execute();
        }

        public void UpdateModel(PbknitfinishModel model)
        {
            //model.UpdateTime = System.DateTime.Now;
            PbknitfinishTable table = new PbknitfinishTable();
            DataAccess.DefaultDB.Update(table)
                .AddColumn(table.FinishingCode, model.FinishingCode)
                .AddColumn(table.FinishingName, model.FinishingName)
                .AddColumn(table.Description, model.Description)
                .AddColumn(table.WashingFlag, model.WashingFlag)
                .AddColumn(table.IsActive, model.IsActive)
                .Execute();
        }

        public void DeleteModel(PbknitfinishModel model)
        {
            PbknitfinishTable table = new PbknitfinishTable();
            DataAccess.DefaultDB.Delete(table)
                .Execute();
        }

        protected bool Exists(PbknitfinishModel model, bool isNew)
        {
            PbknitfinishTable table = new PbknitfinishTable();
            SelectSqlSection sql = DataAccess.DefaultDB.Select(table, QueryColumn.All().Count())
                .Where(table.FinishingCode == model.FinishingCode
                && table.FinishingName == model.FinishingName
                && table.Description == model.Description
                && table.WashingFlag == model.WashingFlag
                && table.IsActive == model.IsActive
                );
            return sql.ToScalar<int>() > 0;
        }

        public void CheckModel(PbknitfinishModel model, bool isNew)
        {
            Validator v = new Validator();
            //Check model's data here.
            if (!v.IsValid)
                throw new ValidationException(v);
        }
    }
}