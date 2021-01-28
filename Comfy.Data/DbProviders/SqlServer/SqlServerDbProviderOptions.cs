using System.Collections.Generic;

namespace Comfy.Data.DbProviders.SqlServer
{
    public class SqlServerDbProviderOptions : DbProviders.DbProviderOptions
    {
        public override string GetSelectLastInsertAutoIncrementIDSql(string tableName, string columnName, Dictionary<string, string> options)
        {
            return "SELECT SCOPE_IDENTITY()";
        }

        public override bool SupportADO20Transaction
        {
            get
            {
                return true;
            }
        }

        public override bool SupportMultiSqlStatementInOneCommand
        {
            get
            {
                return true;
            }
        }
    }
}
