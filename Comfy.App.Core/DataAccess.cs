using Comfy.Data;

namespace Comfy.App.Core
{
    public class DataAccess
    {
        public static Database CreateDatabase()
        {
//#if MSSQL
            return new Database("ConnectionString");
//#else
            //return new Database("DB_Oracle");
//#endif
        }

        public static Database CreateSqlServerDatabase()
        {

            return new Database("SqlServer");
        }

        public static Database DefaultDB;

        static DataAccess()
        {
            DefaultDB = CreateDatabase();
        }
    }
}
