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
    using System.Data.Common;


    public class QcavailablewidthManager
    {

        public QcavailablewidthModelList GetModelList(QcavailablewidthModel model)
        {
            QcavailablewidthTable table = new QcavailablewidthTable();
            SelectSqlSection sql = DataAccess.DefaultDB.Select(table, table.AllColumns());
            if (model.QualityCode != "")
            {
                sql.Where(table.QualityCode == model.QualityCode);
            }
            using (SafeDataReader sdr = new SafeDataReader(sql.ToDataReader()))
            {
                QcavailablewidthModelList result = new QcavailablewidthModelList();
                while (sdr.Read())
                {
                    QcavailablewidthModel m = new QcavailablewidthModel();
                    m.Iden = sdr.GetInt32(table.Iden);
                    m.QualityCode = sdr.GetString(table.QualityCode);
                    m.Gauge = sdr.GetInt32(table.Gauge);
                    m.Diameter = sdr.GetInt32(table.Diameter);
                    m.TotalNeedles = sdr.GetInt32(table.TotalNeedles);
                    m.Width = sdr.GetInt32(table.Width);
                    m.MaxWidth = sdr.GetInt32(table.MaxWidth);
                    m.UpdatedBy = sdr.GetString(table.UpdatedBy);
                    m.UpdatedTime = sdr.GetDateTime(table.UpdatedTime);
                    result.Add(m);
                }
                return result;
            }
        }

        public void AddModel(QcavailablewidthModel model, DbTransaction tran)
        {
            //model.CreateTime = System.DateTime.Now;
            QcavailablewidthTable table = new QcavailablewidthTable();
            int iden = DataAccess.DefaultDB.CustomSql("select QCAVAILABLEWIDTHIDEN.nextval from dual").ToScalar<int>();
            InsertSqlSection insertSql = DataAccess.DefaultDB.Insert(table);
            if (tran != null)
            {
                insertSql.SetTransaction(tran);
            }
            insertSql.AddColumn(table.QualityCode, model.QualityCode)
            .AddColumn(table.Gauge, model.Gauge)
            .AddColumn(table.Iden,iden)
            .AddColumn(table.Diameter, model.Diameter)
            .AddColumn(table.TotalNeedles, model.TotalNeedles)
            .AddColumn(table.Width, model.Width)
            .AddColumn(table.MaxWidth, model.MaxWidth)
            .AddColumn(table.UpdatedBy, model.UpdatedBy)
            .AddColumn(table.UpdatedTime, model.UpdatedTime)
            .Execute();
        }

        public void AddModels(QcavailablewidthModelList models, DbTransaction tran)
        {
            for (int i = 0; i < models.Count; i++)
            {
                this.AddModel(models[i], tran);
            }
        }
        public void UpdateModel(QcavailablewidthModel model, DbTransaction tran)
        {
            QcavailablewidthTable table = new QcavailablewidthTable();
            UpdateSqlSection sql = DataAccess.DefaultDB.Update(table);
            if (tran != null)
            {
                sql.SetTransaction(tran);
            }
            sql.AddColumn(table.Gauge, model.Gauge)
            .AddColumn(table.Diameter, model.Diameter)
            .AddColumn(table.TotalNeedles, model.TotalNeedles)
            .AddColumn(table.Width, model.Width)
            .AddColumn(table.MaxWidth, model.MaxWidth)
            .AddColumn(table.UpdatedBy, model.UpdatedBy)
            .AddColumn(table.UpdatedTime, model.UpdatedTime)
            .Execute();
        }

        public void DeleteModel(QcavailablewidthModel model,DbTransaction tran)
        {
            QcavailablewidthTable table = new QcavailablewidthTable();
            DeleteSqlSection sql = DataAccess.DefaultDB.Delete(table);
            if (tran != null)
            {
                sql.SetTransaction(tran);
            }
            sql.Where(table.QualityCode == model.QualityCode)
                .Execute();
        }

        protected bool Exists(QcavailablewidthModel model, bool isNew)
        {
            QcavailablewidthTable table = new QcavailablewidthTable();
            SelectSqlSection sql = DataAccess.DefaultDB.Select(table, QueryColumn.All().Count())
                .Where(table.Iden == model.Iden
                && table.QualityCode == model.QualityCode
                && table.Gauge == model.Gauge
                && table.Diameter == model.Diameter
                && table.TotalNeedles == model.TotalNeedles
                && table.Width == model.Width
                && table.MaxWidth == model.MaxWidth
                && table.UpdatedBy == model.UpdatedBy
                && table.UpdatedTime == model.UpdatedTime
                );
            return sql.ToScalar<int>() > 0;
        }

        public void CheckModel(QcavailablewidthModel model, bool isNew)
        {
            Validator v = new Validator();
            //Check model's data here.
            if (!v.IsValid)
                throw new ValidationException(v);
        }
    }
}