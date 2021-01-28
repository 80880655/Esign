using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Comfy.Data.Core
{
    public abstract class ExpressionBase : IExpression
    {
        #region Protected Members

        protected StringBuilder sql = new StringBuilder();
        protected readonly Dictionary<string, KeyValuePair<DbType, object>> parameters = new Dictionary<string, KeyValuePair<DbType, object>>();

        internal protected static string MakeUniqueParamNameWithoutPrefixToken()
        {
            return CommonUtils.MakeUniqueKey(15, "p");
        }

        #endregion

        #region IExpression Members

        public string Sql
        {
            get
            {
                return sql.ToString();
            }
            set
            {
                sql = new StringBuilder(value);
            }
        }

        public Dictionary<string, KeyValuePair<DbType, object>> Parameters
        {
            get
            {
                return parameters;
            }
        }

        public override string ToString()
        {
            return sql.ToString();
        }

        #endregion
    }

    public class ExpressionClip : ExpressionBase, ICloneable
    {
        #region Protected Members

        protected DbType dbType;

        #endregion

        #region Properties

        public DbType DbType
        {
            get
            {
                return dbType;
            }
            set
            {
                dbType = value;
            }
        }

        #endregion

        #region Constructors & factory methods

        public ExpressionClip()
        {
        }

        protected void InitColumnExpression(string columnName, DbType type)
        {
            if (this.sql.Length > 0)
            {
                this.sql = new StringBuilder();
            }
            SqlQueryUtils.AppendColumnName(this.sql, columnName);
            this.DbType = type;
        }

        /// <summary>
        /// Initializes a new Column Expression instance of the <see cref="ExpressionClip"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="type">The type.</param>
        public ExpressionClip(string columnName, DbType type)
        {
            InitColumnExpression(columnName, type);
        }

        /// <summary>
        /// Initializes a new Parameter Expression instance of the <see cref="ExpressionClip"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        public ExpressionClip(DbType type, object value)
        {
            string paramName = MakeUniqueParamNameWithoutPrefixToken();
            this.sql.Append('@');
            this.sql.Append(paramName);
            this.parameters.Add('@' + paramName, new KeyValuePair<DbType, object>(type, value));
            this.dbType = type;
        }

        /// <summary>
        /// Initializes a new Custom Expression instance of the <see cref="ExpressionClip"/> class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="type">The type.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="types">The types.</param>
        /// <param name="values">The values.</param>
        public ExpressionClip(string sql, DbType type, string[] paramNames, DbType[] types, object[] values)
        {
            if (System.Configuration.ConfigurationManager.AppSettings[sql] != null)
                sql = System.Configuration.ConfigurationManager.AppSettings[sql];

            this.sql.Append(sql);
            this.dbType = type;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; ++i)
                {
                    this.parameters.Add('@' + paramNames[i].TrimStart(SqlQueryUtils.PrefixCharArray), new KeyValuePair<DbType, object>(types[i], values[i]));
                }
            }
        }

        public ExpressionClip Append(QueryOperator op, ExpressionClip right)
        {
            this.sql.Append(SqlQueryUtils.ToString(op));
            this.sql.Append(SqlQueryUtils.ToString(right));
            SqlQueryUtils.AddParameters(this.parameters, right);
            return this;
        }

        #endregion

        #region ICloneable Members

        public virtual object Clone()
        {
            ExpressionClip newExpr = new ExpressionClip();
            newExpr.dbType = this.dbType;
            string tempSql = this.sql.ToString();

            Dictionary<string, KeyValuePair<DbType, object>>.Enumerator en = this.parameters.GetEnumerator();
            while (en.MoveNext())
            {
                object value = en.Current.Value.Value;
                if (value != null && value != DBNull.Value && value is ICloneable)
                {
                    value = ((ICloneable)value).Clone();
                }

                string newParamName = MakeUniqueParamNameWithoutPrefixToken();
                tempSql = tempSql.Replace('@' + en.Current.Key.TrimStart(SqlQueryUtils.PrefixCharArray), '@' + newParamName);
                newExpr.Parameters.Add('@' + newParamName, new KeyValuePair<DbType, object>(en.Current.Value.Key, value));
            }
            newExpr.sql.Append(tempSql);
            return newExpr;
        }

        #endregion

        #region Operators & Database Functions

        public static bool IsNullOrEmpty(ExpressionClip expr)
        {
            return ((object)expr) == null || expr.sql.Length == 0;
        }

        public ExpressionClip Alias(string aliasName)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            if (expr.parameters.Count > 0)
                expr.Sql = "(" + expr.Sql + ")";
            expr.sql.Append(' ');
            SqlQueryUtils.AppendColumnName(expr.sql, aliasName);

            return expr;
        }

        #region String Functions

        public WhereClip Like(string right)
        {
            return new WhereClip().And(this, QueryOperator.Like, new ExpressionClip(this.DbType, right));
        }

        public WhereClip Contains(string subString)
        {
            return new WhereClip().And(this, QueryOperator.Like, new ExpressionClip(this.dbType, '%' + subString.Replace("%", "[%]").Replace("_", "[_]") + '%'));
        }

        public WhereClip StartsWith(string prefix)
        {
            return new WhereClip().And(this, QueryOperator.Like, new ExpressionClip(this.dbType, prefix.Replace("%", "[%]").Replace("_", "[_]") + '%'));
        }

        public WhereClip EndsWith(string suffix)
        {
            return new WhereClip().And(this, QueryOperator.Like, new ExpressionClip(this.dbType, '%' + suffix.Replace("%", "[%]").Replace("_", "[_]")));
        }

        public ExpressionClip Length
        {
            get
            {
                ExpressionClip expr = (ExpressionClip)this.Clone();
                expr.Sql = ColumnFormatter.Length(expr.Sql);
                expr.dbType = DbType.Int32;

                return expr;
            }
        }

        public ExpressionClip ToUpper()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.ToUpper(expr.Sql);

            return expr;
        }

        public ExpressionClip ToLower()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.ToLower(expr.Sql);

            return expr;
        }

        public ExpressionClip Trim()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Trim(expr.Sql);

            return expr;
        }

        public ExpressionClip SubString(int start)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            ExpressionClip cloneExpr = (ExpressionClip)this.Clone();
            StringBuilder sb = new StringBuilder("SUBSTRING(");
            SqlQueryUtils.AppendColumnName(sb, expr.Sql);
            sb.Append(',');
            sb.Append(start + 1);
            sb.Append(",LEN(");
            SqlQueryUtils.AppendColumnName(sb, cloneExpr.Sql);
            sb.Append(')');
            sb.Append(')');

            expr.sql = sb;
            SqlQueryUtils.AddParameters(expr.parameters, cloneExpr);

            return expr;
        }

        public ExpressionClip SubString(int start, int length)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.SubString(expr.Sql, start, length);
            return expr;
        }

        public ExpressionClip IndexOf(string subString)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            StringBuilder sb = new StringBuilder();
            sb.Append("CHARINDEX(@");
            string paramName = MakeUniqueParamNameWithoutPrefixToken();
            sb.Append(paramName);
            sb.Append(',');
            SqlQueryUtils.AppendColumnName(sb, expr.Sql);
            sb.Append(')');
            sb.Append("-1");

            expr.sql = sb;
            expr.dbType = DbType.Int32;
            expr.parameters.Add('@' + paramName, new KeyValuePair<DbType, object>(this.dbType, subString));

            return expr;
        }

        public ExpressionClip Replace(string subString, string replaceString)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            StringBuilder sb = new StringBuilder();
            sb.Append("REPLACE(");
            SqlQueryUtils.AppendColumnName(sb, expr.Sql);
            sb.Append(",@");
            string paramName = MakeUniqueParamNameWithoutPrefixToken();
            sb.Append(paramName);
            sb.Append(",@");
            string paramName2 = MakeUniqueParamNameWithoutPrefixToken();
            sb.Append(paramName2);
            sb.Append(')');

            expr.sql = sb;
            expr.dbType = DbType.Int32;
            expr.parameters.Add('@' + paramName, new KeyValuePair<DbType, object>(this.dbType, subString));
            expr.Parameters.Add('@' + paramName2, new KeyValuePair<DbType, object>(this.dbType, replaceString));

            return expr;
        }

        public ExpressionClip ToNumber()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.ToNumber(expr.Sql);

            return expr;
        }

        #endregion

        #region DateTime Functions

        public ExpressionClip GetYear()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.DatePart(expr.Sql, ColumnFormatter.DatePartType.Year);
            expr.dbType = DbType.Int32;

            return expr;
        }

        public ExpressionClip GetMonth()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.DatePart(expr.Sql, ColumnFormatter.DatePartType.Month);
            expr.dbType = DbType.Int32;

            return expr;
        }

        public ExpressionClip GetDay()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.DatePart(expr.Sql, ColumnFormatter.DatePartType.Day);
            expr.dbType = DbType.Int32;

            return expr;
        }

        public static ExpressionClip GetCurrentDate()
        {
            ExpressionClip expr = new ExpressionClip();
            expr.Sql = ColumnFormatter.GetCurrentDate();
            expr.dbType = DbType.DateTime;
            return expr;
        }

        public static ExpressionClip GetCurrentUtcDate()
        {
            ExpressionClip expr = new ExpressionClip();
            expr.Sql = ColumnFormatter.GetCurrentUtcDate();
            expr.dbType = DbType.DateTime;
            return expr;
        }

        #endregion

        #region Aggregation Functions

        public ExpressionClip Distinct
        {
            get
            {
                ExpressionClip expr = (ExpressionClip)this.Clone();
                expr.Sql = "DISTINCT " + expr.Sql;

                return expr;
            }
        }

        public ExpressionClip Count()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Count(expr.Sql);
            expr.dbType = DbType.Int32;

            return expr;
        }

        public ExpressionClip Count(bool isDistinct)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Count(expr.Sql, isDistinct);
            expr.dbType = DbType.Int32;

            return expr;
        }

        public ExpressionClip Sum()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Sum(expr.Sql);

            return expr;
        }

        public ExpressionClip Min()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Min(expr.Sql);

            return expr;
        }

        public ExpressionClip Max()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Max(expr.Sql);

            return expr;
        }

        public ExpressionClip Avg()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Avg(expr.Sql);

            return expr;
        }

        #endregion

        #region Equals and Not Equals

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static WhereClip operator ==(ExpressionClip left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (ExpressionClip.IsNullOrEmpty(right))
            {
                where.And(left, QueryOperator.IsNULL, null);
            }
            else if (ExpressionClip.IsNullOrEmpty(left))
            {
                where.And(right, QueryOperator.IsNULL, null);
            }
            else
            {
                where.And(left, QueryOperator.Equal, right);
            }

            return where;
        }

        public static WhereClip operator !=(ExpressionClip left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (ExpressionClip.IsNullOrEmpty(right))
            {
                where = where.And(left, QueryOperator.IsNULL, null).Not();
            }
            else if (ExpressionClip.IsNullOrEmpty(left))
            {
                where = where.And(right, QueryOperator.IsNULL, null).Not();
            }
            else
            {
                where.And(left, QueryOperator.NotEqual, right);
            }

            return where;
        }

        public static WhereClip operator ==(ExpressionClip left, object right)
        {
            WhereClip where = new WhereClip();
            if (right == null || right == DBNull.Value)
            {
                where.And(left, QueryOperator.IsNULL, null);
            }
            else
            {
                where.And(left, QueryOperator.Equal,
                    new ExpressionClip(left.DbType, right));
            }

            return where;
        }

        public static WhereClip operator !=(ExpressionClip left, object right)
        {
            WhereClip where = new WhereClip();
            if (right == null || right == DBNull.Value)
            {
                where = where.And(left, QueryOperator.IsNULL, null).Not();
            }
            else
            {
                where.And(left, QueryOperator.NotEqual,
                    new ExpressionClip(left.DbType, right));
            }

            return where;
        }

        public static WhereClip operator ==(object left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (left == null || left == DBNull.Value)
            {
                where.And(right, QueryOperator.IsNULL, null);
            }
            else
            {
                where.And(new ExpressionClip(right.DbType, left),
                    QueryOperator.Equal, right);
            }

            return where;
        }

        public static WhereClip operator !=(object left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (left == null || left == DBNull.Value)
            {
                where = where.And(right, QueryOperator.IsNULL, null).Not();
            }
            else
            {
                where.And(new ExpressionClip(right.DbType, left),
                    QueryOperator.NotEqual, right);
            }

            return where;
        }

        public WhereClip In(params object[] objs)
        {
            WhereClip where = new WhereClip();
            foreach (object obj in objs)
            {
                where.Or(this == obj);
            }

            return where;
        }

        public WhereClip In(ExpressionClip subQuery)
        {
            WhereClip where = new WhereClip();
            where.Sql = string.Format("{0} IN ({1})", this.ToString(), subQuery.ToString());
            if (subQuery.Parameters.Count > 0)
            {
                Dictionary<string, KeyValuePair<DbType, object>>.Enumerator en = subQuery.Parameters.GetEnumerator();
                while (en.MoveNext())
                {
                    where.Parameters.Add('@' + en.Current.Key.TrimStart(SqlQueryUtils.PrefixCharArray), new KeyValuePair<DbType, object>(en.Current.Value.Key, en.Current.Value.Value));
                }
            }
            return where;
        }

        public WhereClip NotIn(params object[] objs)
        {
            WhereClip where = new WhereClip();
            foreach (object obj in objs)
            {
                where.Or(this == obj);
            }

            return where.Not();
        }

        public WhereClip NotIn(ExpressionClip subQuery)
        {
            WhereClip where = new WhereClip();
            where.Sql = string.Format("{0} NOT IN ({1})", this.ToString(), subQuery.ToString());
            if (subQuery.Parameters.Count > 0)
            {
                Dictionary<string, KeyValuePair<DbType, object>>.Enumerator en = subQuery.Parameters.GetEnumerator();
                while (en.MoveNext())
                {
                    where.Parameters.Add('@' + en.Current.Key.TrimStart(SqlQueryUtils.PrefixCharArray), new KeyValuePair<DbType, object>(en.Current.Value.Key, en.Current.Value.Value));
                }
            }
            return where;
        }

        #endregion

        #region Greater and Less

        public static WhereClip operator >(ExpressionClip left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (ExpressionClip.IsNullOrEmpty(right))
            {
                where.And(left, QueryOperator.Greater, null);
            }
            else if (ExpressionClip.IsNullOrEmpty(left))
            {
                where.And(right, QueryOperator.Less, null);
            }
            else
            {
                where.And(left, QueryOperator.Greater, right);
            }

            return where;
        }

        public static WhereClip operator <(ExpressionClip left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (ExpressionClip.IsNullOrEmpty(right))
            {
                where = where.And(left, QueryOperator.Less, null);
            }
            else if (ExpressionClip.IsNullOrEmpty(left))
            {
                where = where.And(right, QueryOperator.Greater, null);
            }
            else
            {
                where.And(left, QueryOperator.Less, right);
            }

            return where;
        }

        public static WhereClip operator >(ExpressionClip left, object right)
        {
            WhereClip where = new WhereClip();
            if (right == null || right == DBNull.Value)
            {
                where.And(left, QueryOperator.Greater, null);
            }
            else
            {
                where.And(left, QueryOperator.Greater,
                    new ExpressionClip(left.DbType, right));
            }

            return where;
        }

        public static WhereClip operator <(ExpressionClip left, object right)
        {
            WhereClip where = new WhereClip();
            if (right == null || right == DBNull.Value)
            {
                where = where.And(left, QueryOperator.Less, null);
            }
            else
            {
                where.And(left, QueryOperator.Less,
                    new ExpressionClip(left.DbType, right));
            }

            return where;
        }

        public static WhereClip operator >(object left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (left == null || left == DBNull.Value)
            {
                where.And(right, QueryOperator.Less, null);
            }
            else
            {
                where.And(new ExpressionClip(right.DbType, left),
                    QueryOperator.Greater, right);
            }

            return where;
        }

        public static WhereClip operator <(object left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (left == null || left == DBNull.Value)
            {
                where = where.And(right, QueryOperator.Greater, null);
            }
            else
            {
                where.And(new ExpressionClip(right.DbType, left),
                    QueryOperator.Less, right);
            }

            return where;
        }

        #endregion

        #region Greater or Equals and Less or Equals

        public static WhereClip operator >=(ExpressionClip left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (ExpressionClip.IsNullOrEmpty(right))
            {
                where.And(left, QueryOperator.GreaterOrEqual, null);
            }
            else if (ExpressionClip.IsNullOrEmpty(left))
            {
                where.And(right, QueryOperator.LessOrEqual, null);
            }
            else
            {
                where.And(left, QueryOperator.GreaterOrEqual, right);
            }

            return where;
        }

        public static WhereClip operator <=(ExpressionClip left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (ExpressionClip.IsNullOrEmpty(right))
            {
                where = where.And(left, QueryOperator.LessOrEqual, null);
            }
            else if (ExpressionClip.IsNullOrEmpty(left))
            {
                where = where.And(right, QueryOperator.GreaterOrEqual, null);
            }
            else
            {
                where.And(left, QueryOperator.LessOrEqual, right);
            }

            return where;
        }

        public static WhereClip operator >=(ExpressionClip left, object right)
        {
            WhereClip where = new WhereClip();
            if (right == null || right == DBNull.Value)
            {
                where.And(left, QueryOperator.GreaterOrEqual, null);
            }
            else
            {
                where.And(left, QueryOperator.GreaterOrEqual,
                    new ExpressionClip(left.DbType, right));
            }

            return where;
        }

        public static WhereClip operator <=(ExpressionClip left, object right)
        {
            WhereClip where = new WhereClip();
            if (right == null || right == DBNull.Value)
            {
                where = where.And(left, QueryOperator.LessOrEqual, null);
            }
            else
            {
                where.And(left, QueryOperator.LessOrEqual,
                    new ExpressionClip(left.DbType, right));
            }

            return where;
        }

        public static WhereClip operator >=(object left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (left == null || left == DBNull.Value)
            {
                where.And(right, QueryOperator.LessOrEqual, null);
            }
            else
            {
                where.And(new ExpressionClip(right.DbType, left),
                    QueryOperator.GreaterOrEqual, right);
            }

            return where;
        }

        public static WhereClip operator <=(object left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (left == null || left == DBNull.Value)
            {
                where = where.And(right, QueryOperator.GreaterOrEqual, null);
            }
            else
            {
                where.And(new ExpressionClip(right.DbType, left),
                    QueryOperator.LessOrEqual, right);
            }

            return where;
        }

        public WhereClip Between(object left, object right)
        {
            return (this >= left).And(this <= right);
        }

        #endregion

        #region + - * / %

        public static ExpressionClip operator +(ExpressionClip left, ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Add, right);
            return expr;
        }

        public static ExpressionClip operator +(object left, ExpressionClip right)
        {
            ExpressionClip expr = new ExpressionClip(right.dbType, left);
            expr.Append(QueryOperator.Add, right);
            return expr;
        }

        public static ExpressionClip operator +(ExpressionClip left, object right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Add,
                new ExpressionClip(left.dbType, right));
            return expr;
        }

        public static ExpressionClip operator -(ExpressionClip left, ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Subtract, right);
            return expr;
        }

        public static ExpressionClip operator -(object left, ExpressionClip right)
        {
            ExpressionClip expr = new ExpressionClip(right.dbType, left);
            expr.Append(QueryOperator.Subtract, right);
            return expr;
        }

        public static ExpressionClip operator -(ExpressionClip left, object right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Subtract,
                new ExpressionClip(left.dbType, right));
            return expr;
        }

        public static ExpressionClip operator *(ExpressionClip left, ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Multiply, right);
            return expr;
        }

        public static ExpressionClip operator *(object left, ExpressionClip right)
        {
            ExpressionClip expr = new ExpressionClip(right.dbType, left);
            expr.Append(QueryOperator.Multiply, right);
            return expr;
        }

        public static ExpressionClip operator *(ExpressionClip left, object right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Multiply,
                new ExpressionClip(left.dbType, right));
            return expr;
        }

        public static ExpressionClip operator /(ExpressionClip left, ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Divide, right);
            return expr;
        }

        public static ExpressionClip operator /(object left, ExpressionClip right)
        {
            ExpressionClip expr = new ExpressionClip(right.dbType, left);
            expr.Append(QueryOperator.Divide, right);
            return expr;
        }

        public static ExpressionClip operator /(ExpressionClip left, object right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Divide,
                new ExpressionClip(left.dbType, right));
            return expr;
        }

        public static ExpressionClip operator %(ExpressionClip left, ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Modulo, right);
            expr.dbType = DbType.Int32;
            return expr;
        }

        public static ExpressionClip operator %(object left, ExpressionClip right)
        {
            ExpressionClip expr = new ExpressionClip(right.dbType, left);
            expr.Append(QueryOperator.Modulo, right);
            expr.dbType = DbType.Int32;
            return expr;
        }

        public static ExpressionClip operator %(ExpressionClip left, object right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Modulo,
                new ExpressionClip(left.dbType, right));
            expr.dbType = DbType.Int32;
            return expr;
        }

        #endregion

        #region Bitwise

        public static ExpressionClip operator !(ExpressionClip left)
        {
            ExpressionClip expr = new ExpressionClip().Append(QueryOperator.BitwiseNOT,
                left);
            return expr;
        }

        public ExpressionClip BitwiseAnd(ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Append(QueryOperator.BitwiseAND, right);

            return expr;
        }

        public ExpressionClip BitwiseAnd(object right)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Append(QueryOperator.BitwiseAND,
                new ExpressionClip(this.dbType, right));

            return expr;
        }

        public ExpressionClip BitwiseOr(ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Append(QueryOperator.BitwiseOR,
                right);

            return expr;
        }

        public ExpressionClip BitwiseOr(object right)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Append(QueryOperator.BitwiseOR,
                new ExpressionClip(this.dbType, right));

            return expr;
        }

        public ExpressionClip BitwiseXOr(ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Append(QueryOperator.BitwiseXOR,
                right);

            return expr;
        }

        public ExpressionClip BitwiseXOr(object right)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Append(QueryOperator.BitwiseXOR,
                new ExpressionClip(this.dbType, right));

            return expr;
        }

        #endregion

        #endregion
    }
}
