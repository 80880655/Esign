using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Comfy.Data.Core;

namespace Comfy.Data
{
    public sealed class SelectSqlSection : ISqlSection
    {
        #region Private Members

        private readonly Database db;
        private readonly string tableName;
        private string[] columnNames = new string[] { "*" };
        private DbTransaction tran;
        private WhereClip whereClip;
        private int topItemCount = int.MaxValue;
        private int skipItemCount = 0;
        private string identyColumnName;
        private bool identyColumnIsNumber = false;

        private DbCommand PrepareCommand()
        {
            DbCommand cmd = db.QueryFactory.CreateSelectRangeCommand(whereClip, columnNames, topItemCount,
                skipItemCount, identyColumnName, identyColumnIsNumber);
            if (cmd != null)
            {
                string topDistinctPrefix = "SELECT TOP " + this.topItemCount.ToString() + " DISTINCT";
                if (this.topItemCount > 0 && this.skipItemCount == 0 && cmd.CommandText.StartsWith(topDistinctPrefix))
                {
                    cmd.CommandText = cmd.CommandText.Replace(topDistinctPrefix, "SELECT DISTINCT TOP " + this.topItemCount.ToString());
                }
            }
            return cmd;
        }

        public string[] ColumnNames
        {
            get { return columnNames; }
            set { columnNames = value; }
        }

        #endregion

        #region Constructors

        public SelectSqlSection(Database db, QueryTable table, params ExpressionClip[] columns)
        {
            this.db = db;
            this.tableName = table.TableName;
            string aliasName = table.TableAlias;
            this.whereClip = new WhereClip(new FromClip(this.tableName, string.IsNullOrEmpty(aliasName) ? this.tableName : aliasName));

            if (table is IExpression)
                SqlQueryUtils.AddParameters(this.whereClip.Parameters, table as IExpression);

            if (columns != null && columns.Length > 0)
            {
                this.columnNames = new string[columns.Length];
                for (int i = 0; i < columns.Length; ++i)
                {
                    this.columnNames[i] = columns[i].ToString();

                    //add parameters in column to whereClip
                    if (columns[i].Parameters.Count > 0)
                    {
                        SqlQueryUtils.AddParameters(this.whereClip.Parameters, columns[i]);
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public SelectSqlSection SetTransaction(DbTransaction tran)
        {
            this.tran = tran;

            return this;
        }

        public SelectSqlSection Where(WhereClip where)
        {
            whereClip.And(where);

            return this;
        }

        public SelectSqlSection Having(WhereClip where)
        {
            if (!WhereClip.IsNullOrEmpty(where) && where.Sql.Length > 0)
            {
                string tempSql = where.Sql.ToString();
                Dictionary<string, KeyValuePair<DbType, object>>.Enumerator en = where.Parameters.GetEnumerator();
                while (en.MoveNext())
                {
                    object value = en.Current.Value.Value;
                    if (value != null && value != DBNull.Value && value is ICloneable)
                    {
                        value = ((ICloneable)value).Clone();
                    }

                    string newParamName = ExpressionBase.MakeUniqueParamNameWithoutPrefixToken();
                    tempSql = tempSql.Replace('@' + en.Current.Key.TrimStart(SqlQueryUtils.PrefixCharArray), '@' + newParamName);
                    whereClip.Parameters.Add('@' + newParamName, new KeyValuePair<DbType, object>(en.Current.Value.Key, value));
                }

                if (whereClip.Having.Length > 0)
                {
                    whereClip.Having.Append(" AND ");
                }

                if (where.IsNot && tempSql.Length > 0)
                {
                    whereClip.Having.Append("NOT ");
                }

                if (tempSql.Length > 0)
                {
                    whereClip.Having.Append('(');
                    whereClip.Having.Append(tempSql);
                    whereClip.Having.Append(')');
                }
            }

            return this;
        }

        public SelectSqlSection OrderBy(params OrderByClip[] orderBys)
        {
            if (orderBys != null && orderBys.Length > 0)
            {
                if (orderBys.Length == 1)
                {
                    whereClip.SetOrderBy(orderBys[0].OrderBys.ToArray());
                }
                else
                {
                    OrderByClip combinedOrderBy = new OrderByClip();
                    for (int i = 0; i < orderBys.Length; ++i)
                    {
                        combinedOrderBy = combinedOrderBy & orderBys[i];
                    }
                    whereClip.SetOrderBy(combinedOrderBy.OrderBys.ToArray());
                }
            }

            return this;
        }

        public SelectSqlSection GroupBy(params QueryColumn[] columns)
        {
            if (columns != null && columns.Length > 0)
            {
                string[] columnNames = new string[columns.Length];
                for (int i = 0; i < columns.Length; ++i)
                {
                    columnNames[i] = columns[i].Name;
                }
                this.whereClip.SetGroupBy(columnNames);
            }

            return this;
        }

        public SelectSqlSection SetSelectRange(int topItemCount, int skipItemCount, QueryColumn identyColumn)
        {
            this.topItemCount = topItemCount;
            this.skipItemCount = skipItemCount;
            this.identyColumnName = identyColumn.Name;
            this.identyColumnIsNumber =
                (identyColumn.DbType == DbType.Int32) ||
                (identyColumn.DbType == DbType.Int16) ||
                (identyColumn.DbType == DbType.Int64) ||
                (identyColumn.DbType == DbType.Byte) ||
                (identyColumn.DbType == DbType.Double) ||
                (identyColumn.DbType == DbType.Currency) ||
                (identyColumn.DbType == DbType.Decimal) ||
                (identyColumn.DbType == DbType.SByte) ||
                (identyColumn.DbType == DbType.Single) ||
                (identyColumn.DbType == DbType.UInt16) ||
                (identyColumn.DbType == DbType.UInt32) ||
                (identyColumn.DbType == DbType.UInt64);

            return this;
        }

        public SelectSqlSection Join(QueryTable joinTable, string joinTableAliasName, WhereClip joinOnWhere)
        {
            this.whereClip.From.Join(joinTable.TableName, joinTableAliasName, joinOnWhere);
            if(joinOnWhere.Parameters.Count > 0)
                SqlQueryUtils.AddParameters(whereClip.Parameters, joinOnWhere);
            return this;
        }

        public SelectSqlSection Join(QueryTable joinTable, WhereClip joinOnWhere)
        {
            return Join(joinTable, joinTable.TableAlias, joinOnWhere);
        }

        public SelectSqlSection LeftJoin(QueryTable joinTable, string joinTableAliasName, WhereClip joinOnWhere)
        {
            this.whereClip.From.LeftJoin(joinTable.TableName, joinTableAliasName, joinOnWhere);
            if (joinOnWhere.Parameters.Count > 0)
                SqlQueryUtils.AddParameters(whereClip.Parameters, joinOnWhere);
            return this;
        }

        public SelectSqlSection LeftJoin(QueryTable joinTable, WhereClip joinOnWhere)
        {
            return LeftJoin(joinTable, joinTable.TableAlias, joinOnWhere);
        }

        public SelectSqlSection RightJoin(QueryTable joinTable, string joinTableAliasName, WhereClip joinOnWhere)
        {
            this.whereClip.From.RightJoin(joinTable.TableName, joinTableAliasName, joinOnWhere);
            if (joinOnWhere.Parameters.Count > 0)
                SqlQueryUtils.AddParameters(whereClip.Parameters, joinOnWhere);
            return this;
        }

        public SelectSqlSection RightJoin(QueryTable joinTable, WhereClip joinOnWhere)
        {
            return RightJoin(joinTable, joinTable.TableAlias, joinOnWhere);
        }

        public object ToScalar()
        {
            DbCommand cmd = PrepareCommand();
            return tran == null ? db.ExecuteScalar(cmd) : db.ExecuteScalar(cmd, tran);
        }

        public object ToScalar(Type returnType)
        {
            object retValue = ToScalar();

            if (retValue == null || retValue == DBNull.Value)
                return CommonUtils.DefaultValue(returnType);

            if (returnType == typeof(Guid))
                return DataUtils.ToGuid(retValue);

            return Convert.ChangeType(retValue, returnType);
        }

        public T ToScalar<T>()
        {
            return (T)ToScalar(typeof(T));
        }

        public T ToScalar<T>(T defaultValue)
        {
            object retValue = ToScalar();

            if (retValue == null || retValue == DBNull.Value)
                return defaultValue;

            if (typeof(T) == typeof(Guid))
                return (T)(object)DataUtils.ToGuid(retValue);

            return (T)Convert.ChangeType(retValue, typeof(T));
        }

        public IDataReader ToDataReader()
        {
            DbCommand cmd = PrepareCommand();
            return tran == null ? db.ExecuteReader(cmd) : db.ExecuteReader(cmd, tran);
        }

        public DataSet ToDataSet()
        {
            DbCommand cmd = PrepareCommand();
            return tran == null ? db.ExecuteDataSet(cmd) : db.ExecuteDataSet(cmd, tran);
        }

        public DbCommand ToDbCommand()
        {
            return PrepareCommand();
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
                    System.Collections.IEnumerator en = cmd.Parameters.GetEnumerator();
                    int i = 0;
                    while (en.MoveNext())
                    {
                        System.Data.Common.DbParameter p = (System.Data.Common.DbParameter)en.Current;
                        sql = sql.Replace(p.ParameterName, p.ParameterName[0] + parameterNames[i].TrimStart(p.ParameterName[0]));
                        ++i;
                    }
                }

                return sql;
            }
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

        #endregion
    }
}
