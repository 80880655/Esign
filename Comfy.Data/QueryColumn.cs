using System.Data;
using Comfy.Data.Core;

namespace Comfy.Data
{
    public sealed class QueryColumn : ExpressionClip
    {
        public static QueryColumn All()
        {
            return new QueryColumn("*", DbType.Int32);
        }

        public static QueryColumn All(QueryTable table)
        {
            if (table == null)
                return new QueryColumn("*", DbType.Int32);
            return new QueryColumn(string.Format("[{0}].*", table.TableAlias), DbType.Int32);
        }

        public string Name
        {
            get
            {
                return this.ToString().Replace("[", string.Empty).Replace("]", string.Empty);
            }
        }

        public string ColumnName
        {
            get
            {
                return this.ToString().Substring(this.ToString().LastIndexOf('.') + 1).Trim('[', ']');
            }
        }

        public OrderByClip Desc
        {
            get
            {
                return new OrderByClip(this, true);
            }
        }

        public OrderByClip Asc
        {
            get
            {
                return new OrderByClip(this, false);
            }
        }

        public QueryColumn(string name, DbType type)
        {
            SqlQueryUtils.AppendColumnName(this.sql, name);
            this.DbType = type;
        }
    }
}
