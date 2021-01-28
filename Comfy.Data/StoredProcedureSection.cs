using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Comfy.Data
{
    public sealed class StoredProcedureSection
    {
        #region Private Members

        private Database db;
        private string spName;
        private DbTransaction tran;

        private List<string> inputParamNames = new List<string>();
        private List<DbType> inputParamTypes = new List<DbType>();
        private List<object> inputParamValues = new List<object>();

        private List<string> outputParamNames = new List<string>();
        private List<DbType> outputParamTypes = new List<DbType>();
        private List<int> outputParamSizes = new List<int>();

        private List<string> inputOutputParamNames = new List<string>();
        private List<DbType> inputOutputParamTypes = new List<DbType>();
        private List<object> inputOutputParamValues = new List<object>();
        private List<int> inputOutputParamSizes = new List<int>();

        private string returnValueParamName;
        private DbType returnValueParamType;
        private int returnValueParamSize;

        private IDataReader FindDataReader()
        {
            DbCommand cmd = db.QueryFactory.CreateStoredProcedureCommand(spName,
                inputParamNames.ToArray(), inputParamTypes.ToArray(), inputParamValues.ToArray(),
                outputParamNames.ToArray(), outputParamTypes.ToArray(), outputParamSizes.ToArray(),
                inputOutputParamNames.ToArray(), inputOutputParamTypes.ToArray(), inputOutputParamSizes.ToArray(), inputOutputParamValues.ToArray(),
                returnValueParamName, returnValueParamType, returnValueParamSize);
            return tran == null ? db.ExecuteReader(cmd) : db.ExecuteReader(cmd, tran);
        }

        private DataSet FindDataSet()
        {
            DbCommand cmd = db.QueryFactory.CreateStoredProcedureCommand(spName,
                inputParamNames.ToArray(), inputParamTypes.ToArray(), inputParamValues.ToArray(),
                outputParamNames.ToArray(), outputParamTypes.ToArray(), outputParamSizes.ToArray(),
                inputOutputParamNames.ToArray(), inputOutputParamTypes.ToArray(), inputOutputParamSizes.ToArray(), inputOutputParamValues.ToArray(),
                returnValueParamName, returnValueParamType, returnValueParamSize);
            return tran == null ? db.ExecuteDataSet(cmd) : db.ExecuteDataSet(cmd, tran);
        }

        private DataSet FindDataSet(out Dictionary<string, object> outValues)
        {
            DbCommand cmd = db.QueryFactory.CreateStoredProcedureCommand(spName,
                inputParamNames.ToArray(), inputParamTypes.ToArray(), inputParamValues.ToArray(),
                outputParamNames.ToArray(), outputParamTypes.ToArray(), outputParamSizes.ToArray(),
                inputOutputParamNames.ToArray(), inputOutputParamTypes.ToArray(), inputOutputParamSizes.ToArray(), inputOutputParamValues.ToArray(),
                returnValueParamName, returnValueParamType, returnValueParamSize);
            DataSet ds = (tran == null ? db.ExecuteDataSet(cmd) : db.ExecuteDataSet(cmd, tran));
            outValues = GetOutputParameterValues(cmd);
            return ds;
        }

        private static Dictionary<string, object> GetOutputParameterValues(DbCommand cmd)
        {
            Dictionary<string, object> outValues;
            outValues = new Dictionary<string, object>();
            for (int i = 0; i < cmd.Parameters.Count; ++i)
            {
                if (cmd.Parameters[i].Direction == ParameterDirection.InputOutput || cmd.Parameters[i].Direction == ParameterDirection.Output || cmd.Parameters[i].Direction == ParameterDirection.ReturnValue)
                {
                    outValues.Add(cmd.Parameters[i].ParameterName.TrimStart('@',':','?'),
                        cmd.Parameters[i].Value == DBNull.Value ? null : cmd.Parameters[i].Value);
                }
            }
            return outValues;
        }

        #endregion

        #region Constructors

        public StoredProcedureSection(Database db, string spName)
            : base()
        {
            //Check.Require(db != null, "db could not be null.");
            //Check.Require(spName != null, "spName could not be null.");

            this.db = db;
            this.spName = spName;
        }

        #endregion

        #region Public Members

        public StoredProcedureSection AddInputParameter(string name, DbType type, object value)
        {
            //Check.Require(!string.IsNullOrEmpty(name), "name could not be null or empty!");

            inputParamNames.Add(name);
            inputParamTypes.Add(type);
            inputParamValues.Add(value);

            return this;
        }

        public StoredProcedureSection AddOutputParameter(string name, DbType type, int size)
        {
            //Check.Require(!string.IsNullOrEmpty(name), "name could not be null or empty!");

            outputParamNames.Add(name);
            outputParamTypes.Add(type);
            outputParamSizes.Add(size);

            return this;
        }

        public StoredProcedureSection AddInputOutputParameter(string name, DbType type, int size, object value)
        {
            //Check.Require(!string.IsNullOrEmpty(name), "name could not be null or empty!");

            inputOutputParamNames.Add(name);
            inputOutputParamTypes.Add(type);
            inputOutputParamSizes.Add(size);
            inputOutputParamValues.Add(value);

            return this;
        }

        public StoredProcedureSection SetReturnParameter(string name, DbType type, int size)
        {
            //Check.Require(!string.IsNullOrEmpty(name), "name could not be null or empty!");

            returnValueParamName = name;
            returnValueParamType = type;
            returnValueParamSize = size;

            return this;
        }

        public StoredProcedureSection SetTransaction(DbTransaction tran)
        {
            this.tran = tran;

            return this;
        }

        public int ExecuteNonQuery()
        {
            DbCommand cmd = db.QueryFactory.CreateStoredProcedureCommand(spName,
                inputParamNames.ToArray(), inputParamTypes.ToArray(), inputParamValues.ToArray(),
                outputParamNames.ToArray(), outputParamTypes.ToArray(), outputParamSizes.ToArray(),
                inputOutputParamNames.ToArray(), inputOutputParamTypes.ToArray(), inputOutputParamSizes.ToArray(), inputOutputParamValues.ToArray(),
                returnValueParamName, returnValueParamType, returnValueParamSize);
            return tran == null ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, tran);
        }

        public int ExecuteNonQuery(out Dictionary<string, object> outValues)
        {
            DbCommand cmd = db.QueryFactory.CreateStoredProcedureCommand(spName,
                inputParamNames.ToArray(), inputParamTypes.ToArray(), inputParamValues.ToArray(),
                outputParamNames.ToArray(), outputParamTypes.ToArray(), outputParamSizes.ToArray(),
                inputOutputParamNames.ToArray(), inputOutputParamTypes.ToArray(), inputOutputParamSizes.ToArray(), inputOutputParamValues.ToArray(),
                returnValueParamName, returnValueParamType, returnValueParamSize);
            int affactRows = (tran == null ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, tran));
            outValues = GetOutputParameterValues(cmd);
            return affactRows;
        }

        public object ToScalar()
        {
            IDataReader reader = FindDataReader();
            object retObj = null;
            if (reader.Read())
            {
                retObj = reader.GetValue(0);
            }
            reader.Close();
            reader.Dispose();

            return retObj;
        }

        public IDataReader ToDataReader()
        {
            return FindDataReader();
        }

        public DataSet ToDataSet()
        {
            return FindDataSet();
        }

        public object ToScalar(out Dictionary<string, object> outValues)
        {
            DataSet ds = FindDataSet(out outValues);
            object retObj = null;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retObj = ds.Tables[0].Rows[0][0];
            }
            ds.Dispose();

            return retObj;
        }

        public DataSet ToDataSet(out Dictionary<string, object> outValues)
        {
            return FindDataSet(out outValues);
        }

        #endregion
    }
}
