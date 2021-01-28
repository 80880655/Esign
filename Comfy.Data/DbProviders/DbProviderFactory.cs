using System;
using System.Collections.Generic;
using System.Configuration;

namespace Comfy.Data.DbProviders
{
    public sealed class DbProviderFactory
    {
        private static Dictionary<string, DbProvider> providerCache = new Dictionary<string, DbProvider>();

        private DbProviderFactory()
        {
        }

        public static DbProvider CreateDbProvider(string assemblyName, string className, string connectionString)
        {
            string myConnectionString = connectionString.Trim();
            string myClassName = className.ToLower().Trim();

            if (myConnectionString.IndexOf("microsoft.jet.oledb",StringComparison.OrdinalIgnoreCase) != -1 
                || myConnectionString.IndexOf(".db3", StringComparison.OrdinalIgnoreCase) != -1)
            {
                string mdbPath = myConnectionString.Substring(myConnectionString.IndexOf("data source", StringComparison.OrdinalIgnoreCase) + "data source".Length + 1).TrimStart(' ', '=');
                if (mdbPath.StartsWith("|datadirectory|", StringComparison.OrdinalIgnoreCase))
                    mdbPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\App_Data" + mdbPath.Substring("|datadirectory|".Length);
                else if (mdbPath.StartsWith("~/") || mdbPath.StartsWith("~\\"))
                    mdbPath = mdbPath.Replace("/", "\\").Replace("~\\", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\");
                else if (mdbPath.StartsWith("./") || mdbPath.StartsWith(".\\"))
                    mdbPath = mdbPath.Replace("/", "\\").Replace(".\\", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\");
                myConnectionString = myConnectionString.Substring(0, myConnectionString.IndexOf("data source", StringComparison.OrdinalIgnoreCase)) + "Data Source=" + mdbPath;
            }

            //by default, using sqlserver db provider
            if (string.IsNullOrEmpty(myClassName))
            {
                className = typeof(SqlServer.SqlDbProvider).ToString();
            }
            else if (myClassName.Contains("system.data.sqlclient")
                || myClassName == "sql"
                || myClassName == "sqlserver")
            {
                className = typeof(SqlServer.SqlDbProvider).ToString();
            }
            else if (myClassName == "sql9"
                || myClassName == "sqlserver9"
                || myClassName == "sqlserver2005"
                || myClassName == "sql2005")
            {
                className = typeof(SqlServer.SqlDbProvider9).ToString();
            }
            else if (myClassName.Contains("oracle"))
            {
                className = typeof(Oracle.OracleDbProvider).ToString();
            }
            else if (myClassName.Contains("access"))
            {
                className = typeof(MsAccess.AccessDbProvider).ToString();
            }
            else if (myClassName.Contains("mysql"))
            {
                assemblyName = "Comfy.Data.AdditionalDbProviders";
                className = "Comfy.Data.DbProviders.MySql.MySqlDbProvider";
            }
            else if (myClassName.Contains("sqlite"))
            {
                assemblyName = "Comfy.Data.AdditionalDbProviders";
                className = "Comfy.Data.DbProviders.Sqlite.SqliteDbProvider";
            }
            else if (myClassName.Contains("postgresql"))
            {
                assemblyName = "Comfy.Data.AdditionalDbProviders";
                className = "Comfy.Data.DbProviders.PostgreSql.PostgreSqlDbProvider";
            }
            else if (myClassName.Contains("db2"))
            {
                assemblyName = "Comfy.Data.AdditionalDbProviders";
                className = "Comfy.Data.DbProviders.DB2.DB2DbProvider";
            }

            string cacheKey = string.Concat(assemblyName, myClassName, myConnectionString.ToLower());
            lock (providerCache)
            {
                if (providerCache.ContainsKey(cacheKey))
                {
                    return providerCache[cacheKey];
                }
                else
                {
                    System.Reflection.Assembly myAssembly;

                    if (assemblyName == null)
                        myAssembly = typeof(DbProvider).Assembly;
                    else
                        myAssembly = System.Reflection.Assembly.Load(assemblyName);

                    DbProvider retProvider = myAssembly.CreateInstance(className, false,
                        System.Reflection.BindingFlags.Default, null, new object[] { myConnectionString }, null, null) as DbProvider;
                    providerCache.Add(cacheKey, retProvider);
                    return retProvider;
                }
            }
        }

        public static DbProvider Default
        {
            get
            {
                try
                {
                    ConnectionStringSettings connStrSetting =
                        ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1];
                    string[] assAndClass = connStrSetting.ProviderName.Split(',');
                    if (assAndClass.Length > 1)
                        return CreateDbProvider(assAndClass[1].Trim(), assAndClass[0].Trim(), connStrSetting.ConnectionString);
                    else
                        return CreateDbProvider(null, assAndClass[0].Trim(), connStrSetting.ConnectionString);
                }
                catch
                {
                    return null;
                }
            }
        }

        public static DbProvider CreateDbProvider(string connStrName)
        {
            ConnectionStringSettings connStrSetting = ConfigurationManager.ConnectionStrings[connStrName];
            string[] assAndClass = connStrSetting.ProviderName.Split(',');
            if (assAndClass.Length > 1)
            {
                return CreateDbProvider(assAndClass[0].Trim(), assAndClass[1].Trim(), connStrSetting.ConnectionString);
            }
            else
            {
                return CreateDbProvider(null, assAndClass[0].Trim(), connStrSetting.ConnectionString);
            }
        }
    }
}
