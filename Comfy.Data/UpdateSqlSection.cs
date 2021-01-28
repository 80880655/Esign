using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Comfy.Data.Core;

namespace Comfy.Data
{
    public sealed class UpdateSqlSection : ISqlSection
    {
        #region Private Members

        private readonly Database db;
        private readonly string tableName;
        private List<string> columnNames = new List<string>();
        private List<DbType> columnTypes = new List<DbType>();
        private List<object> columnValues = new List<object>();
        private DbTransaction tran;
        private WhereClip whereClip = new WhereClip();

        #endregion

        #region Constructors

        public UpdateSqlSection(Database db, QueryTable table)
        {
            //Check.Require(db != null, "db could not be null.");
            //Check.Require(table != null, "table could not be null.");

            this.db = db;
            this.tableName = table.TableName;
        }

        #endregion

        #region Public Methods

        public UpdateSqlSection SetTransaction(DbTransaction tran)
        {
            this.tran = tran;

            return this;
        }

        public UpdateSqlSection AddColumn(QueryColumn column, object value)
        {
            //Check.Require(!QueryColumn.IsNullOrEmpty(column), "column could not be null.");

            columnNames.Add(column.Name.IndexOf('.') > 0 ? column.Name.Split('.')[1] : column.Name);
            columnTypes.Add(column.DbType);
            columnValues.Add(value);

            return this;
        }

        public UpdateSqlSection Where(WhereClip where)
        {
            whereClip.And(where);

            return this;
        }

        public int Execute()
        {
            DbCommand cmd = db.QueryFactory.CreateUpdateCommand(tableName, whereClip, columnNames.ToArray(),
                columnTypes.ToArray(), columnValues.ToArray());
            return tran == null ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, tran);
        }

        public DbCommand ToDbCommand()
        {
            return db.QueryFactory.CreateUpdateCommand(tableName, whereClip, columnNames.ToArray(),
                columnTypes.ToArray(), columnValues.ToArray());
        }

        public string ToDbCommandText()
        {
            return ToDbCommandText(true);
        }

        /// <summary>
        /// If fillParameterValues == false, you must specify the parameter names you want to be in the returning sql.
        /// </summary>
        /// <param name="fillParameterValues"></param>
        /// <param name="parameterNames"></param>
        /// <returns></returns>
        public string ToDbCommandText(bool fillParameterValues, params string[] parameterNames)
        {
            if (fillParameterValues)
                return DataUtils.ToString(ToDbCommand());
            else
            {
                DbCommand cmd = ToDbCommand();
                string sql = cmd.CommandText;

                if (!string.IsNullOrEmpty(sql) && parameterNames != null)
                {
                    //Check.Require(parameterNames.Length == cmd.Parameters.Count, "The Specified count of parameter names does not equal the count of parameter names in the query.");

                    System.Collections.IEnumerator en = cmd.Parameters.GetEnumerator();
                    int i = 0;
                    while (en.MoveNext())
                    {
                        //Check.Require(parameterNames[i], "parameterNames[" + i + "]", Check.NotNullOrEmpty);

                        System.Data.Common.DbParameter p = (System.Data.Common.DbParameter)en.Current;
                        sql = sql.Replace(p.ParameterName, p.ParameterName[0] + parameterNames[i].TrimStart(p.ParameterName[0]));
                        ++i;
                    }
                }

                return sql;
            }
        }

        #endregion
    }
}
