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


    public class PbknityarncountManager
    {

        public PbknityarncountModelList GetModelList(PbknityarncountModel model)
        {
            PbknityarncountTable table = new PbknityarncountTable();
            SelectSqlSection sql = DataAccess.DefaultDB.Select(table, table.AllColumns())
                ;
            using (SafeDataReader sdr = new SafeDataReader(sql.ToDataReader()))
            {
                PbknityarncountModelList result = new PbknityarncountModelList();
                while (sdr.Read())
                {
                    PbknityarncountModel m = new PbknityarncountModel();
                    m.YarnCount = sdr.GetString(table.YarnCount);
                    m.IsActive = sdr.GetString(table.IsActive);
                    result.Add(m);
                }
                return result;
            }
        }

        public void AddModel(PbknityarncountModel model)
        {
            //model.CreateTime = System.DateTime.Now;
            PbknityarncountTable table = new PbknityarncountTable();
            DataAccess.DefaultDB.Insert(table)
                .AddColumn(table.YarnCount, model.YarnCount)
                .AddColumn(table.IsActive, model.IsActive)
                .Execute();
        }

        public void UpdateModel(PbknityarncountModel model)
        {
            //model.UpdateTime = System.DateTime.Now;
            PbknityarncountTable table = new PbknityarncountTable();
            DataAccess.DefaultDB.Update(table)
                .AddColumn(table.YarnCount, model.YarnCount)
                .AddColumn(table.IsActive, model.IsActive)
                .Execute();
        }

        public void DeleteModel(PbknityarncountModel model)
        {
            PbknityarncountTable table = new PbknityarncountTable();
            DataAccess.DefaultDB.Delete(table)
                .Execute();
        }

        protected bool Exists(PbknityarncountModel model, bool isNew)
        {
            PbknityarncountTable table = new PbknityarncountTable();
            SelectSqlSection sql = DataAccess.DefaultDB.Select(table, QueryColumn.All().Count())
                .Where(table.YarnCount == model.YarnCount
                && table.IsActive == model.IsActive
                );
            return sql.ToScalar<int>() > 0;
        }

        public void CheckModel(PbknityarncountModel model, bool isNew)
        {
            Validator v = new Validator();
            //Check model's data here.
            if (!v.IsValid)
                throw new ValidationException(v);
        }
    }
}
