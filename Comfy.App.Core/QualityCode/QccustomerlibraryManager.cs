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
    using System.Data;
    using System;


    public class QccustomerlibraryManager
    {
        public QccustomerlibraryModelList GetModelList(QccustomerlibraryModel model)
        {
            QccustomerlibraryTable table = new QccustomerlibraryTable();
            QcmaininfoTable main = new QcmaininfoTable();
            SelectSqlSection sql = DataAccess.DefaultDB.Select(table, table.AllColumns(),main.Status);
            sql.Join(main, table.QualityCode == main.QualityCode);
            if (model.QualityCode != "")
            {
                sql.Where(table.QualityCode == model.QualityCode);
            }

            using (SafeDataReader sdr = new SafeDataReader(sql.ToDataReader()))
            {
                QccustomerlibraryModelList result = new QccustomerlibraryModelList();
                while (sdr.Read())
                {
                    QccustomerlibraryModel m = new QccustomerlibraryModel();
                    m.Status = sdr.GetString(main.Status);
                    m.QualityCode = sdr.GetString(table.QualityCode);
                    m.BuyerId = sdr.GetString(table.BuyerId);
                    m.Brand = sdr.GetString(table.Brand);
                    m.CustomerQualityId = sdr.GetString(table.CustomerQualityId);
                    m.Sales = sdr.GetString(table.Sales);
                    m.SalesGroup = sdr.GetString(table.SalesGroup);
                    m.MillComments = sdr.GetString(table.MillComments);
                    m.IsFirstOwner = sdr.GetString(table.IsFirstOwner);
                    m.CreateDate = sdr.GetDateTime(table.CreateDate);
                    m.Creator = sdr.GetString(table.Creator);
                    result.Add(m);
                }
                return result;
            }
        }


        public QccustomerlibraryModelList GetModelListOne(string QcNo)
        {

            QccustomerlibraryModelList result = new QccustomerlibraryModelList();
            CustomSqlSection sql = DataAccess.DefaultDB.CustomSql(@"select a.*,b.Name as customerName,c.Name as salesName from QCCustomerLibrary a left join 
                              gen_customer b on a.Buyer_ID=b.Customer_CD 
                              left join GEN_USERS c on  a.Sales=c.USER_ID 
                              where a.Quality_Code='" + QcNo + "'");
            using (DataSet ds = sql.ToDataSet())
            {
                DataTable dt = ds.Tables[0];
                QccustomerlibraryModel m;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    m = new QccustomerlibraryModel();
                    m.Iden = Convert.ToInt32(dt.Rows[i]["Iden"]);
                    m.QualityCode = dt.Rows[i]["Quality_Code"].ToString();
                    m.BuyerId = dt.Rows[i]["customerName"].ToString();
                    m.Brand = dt.Rows[i]["Brand"].ToString();
                    m.CustomerQualityId = dt.Rows[i]["Customer_Quality_ID"].ToString();
                    m.Sales = dt.Rows[i]["salesName"].ToString();
                    m.SalesGroup = dt.Rows[i]["Sales_Group"].ToString();
                    m.MillComments = dt.Rows[i]["Mill_Comments"].ToString();
                    m.IsFirstOwner = dt.Rows[i]["Is_First_Owner"].ToString();
                    m.CreateDate = (DateTime)dt.Rows[i]["Create_Date"];
                    m.Creator = dt.Rows[i]["Creator"].ToString();
                    result.Add(m);
                }
            }
            return result;
        }

        public QccustomerlibraryModelList GetModelListTwo(string QcNo)
        {

            QccustomerlibraryModelList result = new QccustomerlibraryModelList();
            string strSql = "";
            if (!QcNo.Contains("@"))
            {
                strSql = @"select a.*,b.Name as customerName,c.Name as salesName from QCCustomerLibrary a left join 
                              gen_customer b on a.Buyer_ID=b.Customer_CD 
                              left join GEN_USERS c on  a.Sales=c.USER_ID 
                              where  a.Is_First_Owner='Y' and a.Quality_Code='" + QcNo + "'";
            }
            else
            {
                strSql = @"select a.*,b.Name as customerName,c.Name as salesName from QCCustomerLibrary a left join 
                              gen_customer b on a.Buyer_ID=b.Customer_CD 
                              left join GEN_USERS c on  a.Sales=c.USER_ID 
                              where  a.Quality_Code='" + QcNo.Split(new char[] { '@' }, StringSplitOptions.None)[0] + "' and  a.Buyer_ID='" + QcNo.Split(new char[] { '@' }, StringSplitOptions.None)[1] + "'";
            }
            CustomSqlSection sql = DataAccess.DefaultDB.CustomSql(strSql);
            using (DataSet ds = sql.ToDataSet())
            {
                DataTable dt = ds.Tables[0];
                QccustomerlibraryModel m;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    m = new QccustomerlibraryModel();
                    m.Iden = Convert.ToInt32(dt.Rows[i]["Iden"]);
                    m.QualityCode = dt.Rows[i]["Quality_Code"].ToString();
                    m.BuyerName = dt.Rows[i]["customerName"].ToString();
                    m.SalesName = dt.Rows[i]["salesName"].ToString();
                    m.BuyerId = dt.Rows[i]["Buyer_Id"].ToString();
                    m.Brand = dt.Rows[i]["Brand"].ToString();
                    m.CustomerQualityId = dt.Rows[i]["Customer_Quality_ID"].ToString();
                    m.Sales = dt.Rows[i]["sales"].ToString();
                    m.SalesGroup = dt.Rows[i]["Sales_Group"].ToString();
                    m.MillComments = dt.Rows[i]["Mill_Comments"].ToString();
                    m.IsFirstOwner = dt.Rows[i]["Is_First_Owner"].ToString();
                    m.CreateDate = (DateTime)dt.Rows[i]["Create_Date"];
                    m.Creator = dt.Rows[i]["Creator"].ToString();
                    result.Add(m);
                }
            }
            return result;
        }

        public QccustomerlibraryModelList GetModelListOne(string QcNo,string BuId)
        {

            QccustomerlibraryModelList result = new QccustomerlibraryModelList();
            CustomSqlSection sql = DataAccess.DefaultDB.CustomSql("select a.*,b.Name as customerName,c.USER_ID as salesName from QCCustomerLibrary a left join gen_customer b on  a.Buyer_ID=b.Customer_CD left join GEN_USERS c on  a.Sales=c.USER_ID " +
                " where  a.Quality_Code='" + QcNo + "' and a.Buyer_ID='"+BuId+"'");
            using (DataSet ds = sql.ToDataSet())
            {
                DataTable dt = ds.Tables[0];
                QccustomerlibraryModel m;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    m = new QccustomerlibraryModel();
                    m.Iden = Convert.ToInt32(dt.Rows[i]["Iden"]);
                    m.QualityCode = dt.Rows[i]["Quality_Code"].ToString();
                    m.BuyerId =dt.Rows[i]["Buyer_ID"]+"<|>" +dt.Rows[i]["customerName"].ToString();
                    m.Brand = dt.Rows[i]["Brand"].ToString();
                    m.CustomerQualityId = dt.Rows[i]["Customer_Quality_ID"].ToString();
                    m.Sales = dt.Rows[i]["salesName"].ToString();
                    m.SalesGroup = dt.Rows[i]["Sales_Group"].ToString();
                    m.MillComments = dt.Rows[i]["Mill_Comments"].ToString();
                    m.IsFirstOwner = dt.Rows[i]["Is_First_Owner"].ToString();
                    m.CreateDate = (DateTime)dt.Rows[i]["Create_Date"];
                    m.Creator = dt.Rows[i]["Creator"].ToString();
                    result.Add(m);
                }
            }
            return result;
        }

        public void AddModel(QccustomerlibraryModel model, DbTransaction tran)
        {
            //model.CreateTime = System.DateTime.Now;
            QccustomerlibraryTable table = new QccustomerlibraryTable();
            InsertSqlSection insertSql = DataAccess.DefaultDB.Insert(table);
            if (tran != null)
            {
                insertSql.SetTransaction(tran);
            }
            int iden = DataAccess.DefaultDB.CustomSql("select QCCUSTOMERLIBRARYIDEN.nextval from dual").ToScalar<int>();
            insertSql.AddColumn(table.QualityCode, model.QualityCode)
                .AddColumn(table.BuyerId, model.BuyerId)
                .AddColumn(table.Brand, model.Brand)
                .AddColumn(table.CustomerQualityId, model.CustomerQualityId)
                .AddColumn(table.Sales, model.Sales)
                .AddColumn(table.SalesGroup, model.SalesGroup)
                .AddColumn(table.MillComments, model.MillComments)
                .AddColumn(table.IsFirstOwner, model.IsFirstOwner)
                .AddColumn(table.CreateDate, model.CreateDate)
                .AddColumn(table.Creator, model.Creator)
                .AddColumn(table.Iden,iden)
                .Execute();
        }
        public void AddModels(QccustomerlibraryModelList models, DbTransaction tran)
        {
            for (int i = 0; i < models.Count; i++)
            {
                this.AddModel(models[i], tran);
            }
        }
        public void UpdateModel(QccustomerlibraryModel model, DbTransaction tran)
        {
            QccustomerlibraryTable table = new QccustomerlibraryTable();
            UpdateSqlSection sql = DataAccess.DefaultDB.Update(table);
            if (tran != null)
            {
                sql.SetTransaction(tran);
            }
            sql.AddColumn(table.MillComments, model.MillComments).Where(table.Iden==model.Iden)
            .Execute();
        }
        public void UpdateModelOne(QccustomerlibraryModel model, DbTransaction tran)
        {
            QccustomerlibraryTable table = new QccustomerlibraryTable();
            UpdateSqlSection sql = DataAccess.DefaultDB.Update(table);
            if (tran != null)
            {
                sql.SetTransaction(tran);
            }
            sql.AddColumn(table.MillComments, model.MillComments).Where(table.QualityCode == model.QualityCode && table.BuyerId == model.BuyerId)
            .Execute();
        }
        public void UpdateModelTwo(QccustomerlibraryModel model, DbTransaction tran)
        {
            QccustomerlibraryTable table = new QccustomerlibraryTable();
            UpdateSqlSection sql = DataAccess.DefaultDB.Update(table);
            if (tran != null)
            {
                sql.SetTransaction(tran);
            }
            sql.AddColumn(table.MillComments, model.MillComments)
                .AddColumn(table.CustomerQualityId,model.CustomerQualityId)
                .AddColumn(table.Brand,model.Brand)
                .AddColumn(table.BuyerId,model.BuyerIdNew)
                .AddColumn(table.Sales,model.Sales)
                .AddColumn(table.SalesGroup,model.SalesGroup)
                .Where(table.QualityCode == model.QualityCode && table.BuyerId == model.BuyerId)
            .Execute();
        }
        public void UpdateModelThree(QccustomerlibraryModel model, DbTransaction tran)
        {
            QccustomerlibraryTable table = new QccustomerlibraryTable();
            UpdateSqlSection sql = DataAccess.DefaultDB.Update(table);
            if (tran != null)
            {
                sql.SetTransaction(tran);
            }
            sql.AddColumn(table.MillComments, model.MillComments)
                .AddColumn(table.CustomerQualityId, model.CustomerQualityId)
                .Where(table.QualityCode == model.QualityCode)
            .Execute();
        }
        public void DeleteModel(QccustomerlibraryModel model,DbTransaction tran)
        {
            QccustomerlibraryTable table = new QccustomerlibraryTable();
            DeleteSqlSection sql = DataAccess.DefaultDB.Delete(table);
            if (tran != null)
            {
                sql.SetTransaction(tran);
            }
            sql.Where(table.QualityCode == model.QualityCode)
                .Execute();
        }

        protected bool Exists(QccustomerlibraryModel model, bool isNew)
        {
            QccustomerlibraryTable table = new QccustomerlibraryTable();
            SelectSqlSection sql = DataAccess.DefaultDB.Select(table, QueryColumn.All().Count())
                .Where(table.Iden == model.Iden
                && table.QualityCode == model.QualityCode
                && table.BuyerId == model.BuyerId
                && table.Brand == model.Brand
                && table.CustomerQualityId == model.CustomerQualityId
                && table.Sales == model.Sales
                && table.SalesGroup == model.SalesGroup
                && table.MillComments == model.MillComments
                && table.IsFirstOwner == model.IsFirstOwner
                && table.CreateDate == model.CreateDate
                && table.Creator == model.Creator
                );
            return sql.ToScalar<int>() > 0;
        }

        public void CheckModel(QccustomerlibraryModel model, bool isNew)
        {
            Validator v = new Validator();
            //Check model's data here.
            if (!v.IsValid)
                throw new ValidationException(v);
        }
    }
}
