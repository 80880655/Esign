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


    public class PbknitdyemethodManager
    {

        public PbknitdyemethodModelList GetModelList(PbknitdyemethodModel model)
        {
            PbknitdyemethodTable table = new PbknitdyemethodTable();
            SelectSqlSection sql = DataAccess.DefaultDB.Select(table, table.AllColumns())
                ;
            using (SafeDataReader sdr = new SafeDataReader(sql.ToDataReader()))
            {
                PbknitdyemethodModelList result = new PbknitdyemethodModelList();
                while (sdr.Read())
                {
                    PbknitdyemethodModel m = new PbknitdyemethodModel();
                    m.DyeMethod = sdr.GetString(table.DyeMethod);
                    m.DyeType = sdr.GetString(table.DyeType);
                    m.Description = sdr.GetString(table.Description);
                    result.Add(m);
                }
                return result;
            }
        }

        public void AddModel(PbknitdyemethodModel model)
        {
            //model.CreateTime = System.DateTime.Now;
            PbknitdyemethodTable table = new PbknitdyemethodTable();
            DataAccess.DefaultDB.Insert(table)
                .AddColumn(table.DyeMethod, model.DyeMethod)
                .AddColumn(table.DyeType, model.DyeType)
                .AddColumn(table.Description, model.Description)
                .Execute();
        }

        public void UpdateModel(PbknitdyemethodModel model)
        {
            //model.UpdateTime = System.DateTime.Now;
            PbknitdyemethodTable table = new PbknitdyemethodTable();
            DataAccess.DefaultDB.Update(table)
                .AddColumn(table.DyeMethod, model.DyeMethod)
                .AddColumn(table.DyeType, model.DyeType)
                .AddColumn(table.Description, model.Description)
                .Execute();
        }

        public void DeleteModel(PbknitdyemethodModel model)
        {
            PbknitdyemethodTable table = new PbknitdyemethodTable();
            DataAccess.DefaultDB.Delete(table)
                .Execute();
        }

        protected bool Exists(PbknitdyemethodModel model, bool isNew)
        {
            PbknitdyemethodTable table = new PbknitdyemethodTable();
            SelectSqlSection sql = DataAccess.DefaultDB.Select(table, QueryColumn.All().Count())
                .Where(table.DyeMethod == model.DyeMethod
                && table.DyeType == model.DyeType
                && table.Description == model.Description
                );
            return sql.ToScalar<int>() > 0;
        }

        public void CheckModel(PbknitdyemethodModel model, bool isNew)
        {
            Validator v = new Validator();
            //Check model's data here.
            if (!v.IsValid)
                throw new ValidationException(v);
        }
    }
}
