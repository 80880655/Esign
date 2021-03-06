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


    public class PbknityarntypeManager
    {

        public PbknityarntypeModelList GetModelList(PbknityarntypeModel model)
        {
            PbknityarntypeTable table = new PbknityarntypeTable();
            SelectSqlSection sql = DataAccess.DefaultDB.Select(table, table.AllColumns()).Where(table.YarnType==model.YarnType)
                ;
            using (SafeDataReader sdr = new SafeDataReader(sql.ToDataReader()))
            {
                PbknityarntypeModelList result = new PbknityarntypeModelList();
                while (sdr.Read())
                {
                    PbknityarntypeModel m = new PbknityarntypeModel();
                    m.YarnType = sdr.GetString(table.YarnType);
                    m.Description = sdr.GetString(table.Description);
                    m.IeDescription = sdr.GetString(table.IeDescription);
                    m.CustomerDescription = sdr.GetString(table.CustomerDescription);
                    m.IsActive = sdr.GetString(table.IsActive);
                    result.Add(m);
                }
                return result;
            }
        }

        public void AddModel(PbknityarntypeModel model)
        {
            //model.CreateTime = System.DateTime.Now;
            PbknityarntypeTable table = new PbknityarntypeTable();
            DataAccess.DefaultDB.Insert(table)
                .AddColumn(table.YarnType, model.YarnType)
                .AddColumn(table.Description, model.Description)
                .AddColumn(table.IeDescription, model.IeDescription)
                .AddColumn(table.CustomerDescription, model.CustomerDescription)
                .AddColumn(table.IsActive, model.IsActive)
                .Execute();
        }

        public void UpdateModel(PbknityarntypeModel model)
        {
            //model.UpdateTime = System.DateTime.Now;
            PbknityarntypeTable table = new PbknityarntypeTable();
            DataAccess.DefaultDB.Update(table)
                .AddColumn(table.YarnType, model.YarnType)
                .AddColumn(table.Description, model.Description)
                .AddColumn(table.IeDescription, model.IeDescription)
                .AddColumn(table.CustomerDescription, model.CustomerDescription)
                .AddColumn(table.IsActive, model.IsActive)
                .Execute();
        }

        public void DeleteModel(PbknityarntypeModel model)
        {
            PbknityarntypeTable table = new PbknityarntypeTable();
            DataAccess.DefaultDB.Delete(table)
                .Execute();
        }

        protected bool Exists(PbknityarntypeModel model, bool isNew)
        {
            PbknityarntypeTable table = new PbknityarntypeTable();
            SelectSqlSection sql = DataAccess.DefaultDB.Select(table, QueryColumn.All().Count())
                .Where(table.YarnType == model.YarnType
                && table.Description == model.Description
                && table.IeDescription == model.IeDescription
                && table.CustomerDescription == model.CustomerDescription
                && table.IsActive == model.IsActive
                );
            return sql.ToScalar<int>() > 0;
        }

        public void CheckModel(PbknityarntypeModel model, bool isNew)
        {
            Validator v = new Validator();
            //Check model's data here.
            if (!v.IsValid)
                throw new ValidationException(v);
        }
    }
}
