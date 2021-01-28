
namespace Comfy.Data
{
    public abstract class QueryTable
    {
        string _tableAlias;

        bool _hasAlias;

        public bool HasAlias { get { return _hasAlias; } }

        public abstract string TableName { get; }

        /// <summary>
        /// Default value is TableName
        /// </summary>
        public string TableAlias { get { return _tableAlias; } }

        public QueryTable(string alias)
        {
            _tableAlias = alias;
            _hasAlias = true;
        }

        public QueryTable()
        {
            _tableAlias = TableName;
            _hasAlias = false;
        }

        public QueryColumn AllColumns()
        {
            return new QueryColumn(string.Format("[{0}].*", TableAlias), System.Data.DbType.Int32);
        }
    }
}
