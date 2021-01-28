using System;
using System.Collections.Generic;
using System.Data;

namespace Comfy.Data.Core
{
    public class CustomQueryTable : QueryTable, IExpression
    {
        private readonly string tableName;
        private readonly Dictionary<string, KeyValuePair<DbType, object>> parameters = new Dictionary<string, KeyValuePair<DbType, object>>();

        public CustomQueryTable(string tableName):base(tableName)
        {
            this.tableName = tableName;
        }

        public CustomQueryTable(SubQuery subQuery)
            : base(subQuery.ToString())
        {
            this.tableName = subQuery.ToString();
            SqlQueryUtils.AddParameters(this.parameters, subQuery);
        }

        #region IQueryTable Members

        public override string TableName
        {
            get { return tableName; }
        }

        #endregion

        #region IExpression Members

        public string Sql
        {
            get
            {
                return tableName;
            }
            set
            {
                throw new Exception("Could not change table name of a CustomQueryTable, you can only specify the table name in constructor.");
            }
        }

        public Dictionary<string, KeyValuePair<DbType, object>> Parameters
        {
            get
            {
                return parameters;
            }
        }

        #endregion
    }
}
