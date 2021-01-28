
namespace Comfy.Data.Core
{
    public class SubQuery : ExpressionClip
    {
        public readonly Database Db;

        internal SubQuery(Database db)
        {
            this.Db = db;
        }

        public new SubQuery Alias(string aliasName)
        {
            this.sql.Append(' ');
            SqlQueryUtils.AppendColumnName(this.sql, aliasName);

            return this;
        }

        public SelectSqlSection Select(params ExpressionClip[] columns)
        {
            SelectSqlSection select = this.Db.Select(new CustomQueryTable(this), columns);
            return select;
        }
    }
}
