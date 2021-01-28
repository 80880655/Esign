using System.Collections.Generic;

namespace Comfy.Data.DbProviders.MsAccess
{
    public class MsAccessDbProviderOptions : DbProviders.DbProviderOptions
    {
        public override string GetSelectLastInsertAutoIncrementIDSql(string tableName, string columnName, Dictionary<string, string> options)
        {
            return "SELECT @@IDENTITY";
        }

        public override bool SupportADO20Transaction
        {
            get
            {
                return false;
            }
        }

        public override bool SupportMultiSqlStatementInOneCommand
        {
            get
            {
                return false;
            }
        }
    }
}
