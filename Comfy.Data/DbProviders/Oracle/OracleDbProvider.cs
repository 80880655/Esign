using System.Data.OracleClient;
using Comfy.Data.Core;

namespace Comfy.Data.DbProviders.Oracle
{
    /// <summary>
    /// <para>Represents an Oracle Database.</para>
    /// </summary>
    /// <remarks> 
    /// <para>
    /// Internally uses Oracle .NET Managed Provider from Microsoft (System.Data.OracleClient) to connect to the database.
    /// </para>  
    /// </remarks>
    public class OracleDbProvider : DbProvider
    {
        #region Public Members

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDbProvider"/> class.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        public OracleDbProvider(string connStr)
            : base(connStr, OracleClientFactory.Instance)
        {
            CheckOptionsConfiguration(new OracleDbProviderOptions(), "OracleProviderOptions");
        }

        public override ISqlQueryFactory GetQueryFactory()
        {
            return new DbProviders.Oracle.OracleQueryFactory();
        }

        #endregion
    }
}
