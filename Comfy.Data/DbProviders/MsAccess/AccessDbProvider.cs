using System.Data.OleDb;
using Comfy.Data.Core;

namespace Comfy.Data.DbProviders.MsAccess
{
    /// <summary>
    /// <para>Represents a MsAccess Database Provider.</para>
    /// </summary>
    /// <remarks> 
    /// <para>
    /// Internally uses MsAccess .NET Managed Provider from Microsoft (System.Data.OleDb) to connect to the database.
    /// </para>  
    /// </remarks>
    public class AccessDbProvider : DbProvider
    {
        #region Public Members

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDbProvider"/> class.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        public AccessDbProvider(string connStr)
            : base(connStr, OleDbFactory.Instance)
        {
            CheckOptionsConfiguration(new MsAccessDbProviderOptions(), "MsAccessProviderOptions");
        }

        public override ISqlQueryFactory GetQueryFactory()
        {
            return new DbProviders.MsAccess.MsAccessQueryFactory();
        }

        #endregion
    }
}
