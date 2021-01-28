using System;
using System.Collections.Generic;
using System.Data;

namespace Comfy.Data.Core
{
    public sealed class DataUtils
    {
        public DataUtils() { }

        public static string ToString(DbType dbType, object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return "NULL";
            }

            Type type = val.GetType();

            if (dbType == DbType.AnsiString || dbType == DbType.AnsiStringFixedLength)
            {
                return string.Format("'{0}'", val.ToString().Replace("'", "''"));
            }
            else if (dbType == DbType.String || dbType == DbType.StringFixedLength)
            {
                return string.Format("N'{0}'", val.ToString().Replace("'", "''"));
            }
            else if (type == typeof(DateTime) || type == typeof(Guid))
            {
                return string.Format("'{0}'", val);
            }
            else if (type == typeof(TimeSpan))
            {
                DateTime baseTime = new DateTime(2006, 11, 23);
                return string.Format("(CAST('{0}' AS datetime) - CAST('{1}' AS datetime))", baseTime + ((TimeSpan)val), baseTime);
            }
            else if (type == typeof(bool))
            {
                return ((bool)val) ? "1" : "0";
            }
            else if (type == typeof(byte[]))
            {
                return "0x" + BitConverter.ToString((byte[])val).Replace("-", string.Empty);
            }
            else if (val is ExpressionClip)
            {
                return ToString((ExpressionClip)val);
            }
            else if (type.IsEnum)
            {
                return Convert.ToInt32(val).ToString();
            }
            else if (type.IsValueType)
            {
                return val.ToString();
            }
            else
            {
                return string.Format("'{0}'", val.ToString().Replace("'", "''"));
            }
        }

        public static string ToString(IExpression expr)
        {
            if (expr == null)
            {
                return null;
            }

            string sql = expr.ToString();

            if (!string.IsNullOrEmpty(sql))
            {
                Dictionary<string, KeyValuePair<DbType, object>>.Enumerator en = expr.Parameters.GetEnumerator();

                while (en.MoveNext())
                {
                    sql = sql.Replace('@' + en.Current.Key.TrimStart(SqlQueryUtils.PrefixCharArray), ToString(en.Current.Value.Key, en.Current.Value.Value));
                }
            }

            return sql.Replace("= NULL", "IS NULL");
        }

        public static string ToString(System.Data.Common.DbCommand cmd)
        {
            if (cmd == null)
            {
                return null;
            }

            string sql = cmd.CommandText;

            if (!string.IsNullOrEmpty(sql))
            {
                System.Collections.IEnumerator en = cmd.Parameters.GetEnumerator();

                while (en.MoveNext())
                {
                    System.Data.Common.DbParameter p = (System.Data.Common.DbParameter)en.Current;
                    sql = sql.Replace(p.ParameterName, ToString(p.DbType, p.Value));
                }
            }

            return sql.Replace("= NULL", "IS NULL");
        }

        public static Guid ToGuid(object obj)
        {
            if (obj != null && obj != DBNull.Value)
            {
                if (obj is Guid)
                    return (Guid)obj;
                if (obj is string)
                    return new Guid(obj as string);
                if (obj is byte[])
                    return new Guid(obj as byte[]);
            }

            return default(Guid);
        }
    }
}
