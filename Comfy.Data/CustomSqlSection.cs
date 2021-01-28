using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Comfy.Data.Core;

namespace Comfy.Data
{
    public sealed class CustomSqlSection
    {
        #region Private Members

        private readonly Database db;
        internal string sql;
        private DbTransaction tran;
        private List<string> inputParamNames = new List<string>();
        private List<DbType> inputParamTypes = new List<DbType>();
        private List<object> inputParamValues = new List<object>();

        private IDataReader FindDataReader()
        {
            DbCommand cmd = PrepareCommand();
            return tran == null ? db.ExecuteReader(cmd) : db.ExecuteReader(cmd, tran);
        }

        private DbCommand PrepareCommand()
        {
            DbCommand cmd = db.QueryFactory.CreateCustomSqlCommand(sql, inputParamNames.ToArray(),
                inputParamTypes.ToArray(), inputParamValues.ToArray());
            return cmd;
        }

        private DataSet FindDataSet()
        {
            DbCommand cmd = PrepareCommand();
            return tran == null ? db.ExecuteDataSet(cmd) : db.ExecuteDataSet(cmd, tran);
        }

        #endregion

        #region Constructors

        public CustomSqlSection(Database db, string sql)
        {
            //Check.Require(db != null, "db could not be null.");
            //Check.Require(sql != null, "sql could not be null.");

            if (System.Configuration.ConfigurationManager.AppSettings[sql] != null)
                sql = System.Configuration.ConfigurationManager.AppSettings[sql];

            this.db = db;
            this.sql = sql;
        }

        #endregion

        #region Public Members

        public CustomSqlSection AddInputParameter(string name, DbType type, object value)
        {
            //Check.Require(!string.IsNullOrEmpty(name), "name could not be null or empty!");

            inputParamNames.Add(name);
            inputParamTypes.Add(type);
            inputParamValues.Add(value);

            return this;
        }

        public CustomSqlSection SetTransaction(DbTransaction tran)
        {
            this.tran = tran;

            return this;
        }

        public int ExecuteNonQuery()
        {
            DbCommand cmd = db.QueryFactory.CreateCustomSqlCommand(sql, inputParamNames.ToArray(),
                inputParamTypes.ToArray(), inputParamValues.ToArray());
            return tran == null ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, tran);
        }

        public object ToScalar()
        {
            DbCommand cmd = PrepareCommand();
            return tran == null ? db.ExecuteScalar(cmd) : db.ExecuteScalar(cmd, tran);
        }

        public object ToScalar(Type returnType)
        {
            //Check.Require(returnType, "returnType");

            object retValue = ToScalar();

            if (retValue == null || retValue == DBNull.Value)
                return CommonUtils.DefaultValue(returnType);

            if (returnType == typeof(Guid))
            {
                return DataUtils.ToGuid(ToScalar());
            }

            return Convert.ChangeType(retValue, returnType);
        }

        public ReturnType ToScalar<ReturnType>()
        {
            return (ReturnType)ToScalar(typeof(ReturnType));
        }

        public SubQuery ToSubQuery()
        {
            DbCommand cmd = PrepareCommand();
            SubQuery expr = new SubQuery(this.db);
            expr.Sql = "(" + cmd.CommandText + ")";
            for (int i = 0; i < cmd.Parameters.Count; ++i)
            {
                expr.Parameters.Add('@' + cmd.Parameters[i].ParameterName.TrimStart(SqlQueryUtils.PrefixCharArray), new KeyValuePair<DbType, object>(cmd.Parameters[i].DbType, cmd.Parameters[i].Value));
            }
            return expr;
        }

        //public T ToSingleObject<T>()
        //    where T : class
        //{
        //    T retObj = default(T);

        //    using (IDataReader reader = this.ToDataReader())
        //    {
        //        if (reader == null)
        //            return retObj;

        //        if (reader.Read())
        //            retObj = NBear.Mapping.ObjectConvertor.ToObject<IDataReader, T>(reader);

        //        reader.Close();
        //    }

        //    return retObj;
        //}

        //public T ToSingleObject<T>(string viewName)
        //    where T : class
        //{
        //    Check.Require(viewName, "viewName", Check.NotNullOrEmpty);

        //    T retObj = default(T);

        //    using (IDataReader reader = this.ToDataReader())
        //    {
        //        if (reader == null)
        //            return retObj;

        //        if (reader.Read())
        //            retObj = NBear.Mapping.ObjectConvertor.ToObject<IDataReader, T>(reader, viewName);

        //        reader.Close();
        //    }

        //    return retObj;
        //}

        //public T[] ToArray<T>()
        //    where T : class
        //{
        //    T[] retObjs = null;

        //    using (IDataReader reader = this.ToDataReader())
        //    {
        //        if (reader == null)
        //            return retObjs;

        //        retObjs = NBear.Mapping.ObjectConvertor.ToArray<IDataReader, T>(reader);

        //        reader.Close();
        //    }

        //    return retObjs;
        //}

        //public List<T> ToList<T>()
        //    where T : class
        //{
        //    return CommonUtils.ConvertArrayToList<T>(ToArray<T>());
        //}

        //public T[] ToArray<T>(string viewName)
        //    where T : class
        //{
        //    //Check.Require(viewName, "viewName", Check.NotNullOrEmpty);

        //    T[] retObjs = null;

        //    using (IDataReader reader = this.ToDataReader())
        //    {
        //        if (reader == null)
        //            return retObjs;

        //        retObjs = NBear.Mapping.ObjectConvertor.ToArray<IDataReader, T>(reader, viewName);

        //        reader.Close();
        //    }

        //    return retObjs;
        //}

        //public List<T> ToList<T>(string viewName)
        //    where T : class
        //{
        //    return CommonUtils.ConvertArrayToList<T>(ToArray<T>(viewName));
        //}

        public IDataReader ToDataReader()
        {
            return FindDataReader();
        }

        public DataSet ToDataSet()
        {
            return FindDataSet();
        }

        #endregion
    }
}
