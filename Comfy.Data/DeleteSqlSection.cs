﻿using System.Data.Common;
using Comfy.Data.Core;

namespace Comfy.Data
{
    public sealed class DeleteSqlSection : ISqlSection
    {
        #region Private Members

        private readonly Database db;
        private readonly string tableName;
        private DbTransaction tran;
        private WhereClip whereClip = new WhereClip();

        #endregion

        #region Constructors

        public DeleteSqlSection(Database db, QueryTable table)
        {
            this.db = db;
            this.tableName = table.TableName;
        }

        #endregion

        #region Public Methods

        public DeleteSqlSection SetTransaction(DbTransaction tran)
        {
            this.tran = tran;

            return this;
        }

        public DeleteSqlSection Where(WhereClip where)
        {
            whereClip.And(where);

            return this;
        }

        public int Execute()
        {
            DbCommand cmd = db.QueryFactory.CreateDeleteCommand(tableName, whereClip);
            return tran == null ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, tran);
        }

        public DbCommand ToDbCommand()
        {
            return db.QueryFactory.CreateDeleteCommand(tableName, whereClip);
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
