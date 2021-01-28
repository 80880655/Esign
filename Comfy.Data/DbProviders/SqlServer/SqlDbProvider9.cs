using Comfy.Data.Core;

namespace Comfy.Data.DbProviders.SqlServer
{
    /// <summary>
    /// Db provider implementation for SQL Server 9.X (2005)
    /// </summary>
    public class SqlDbProvider9 : SqlServer.SqlDbProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SqlDatabase"/> class.
        /// </summary>
        /// <param name="connStr"></param>
        public SqlDbProvider9(string connStr)
            : base(connStr)
        {
        }

        public override ISqlQueryFactory GetQueryFactory()
        {
            return new DbProviders.SqlServer.SqlServer9QueryFactory();
        }
    }
}
