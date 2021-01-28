using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;

namespace Comfy.UI.WebControls.Transformation
{
    public class ImportXls
    {
        public static DataTable ReadXlsToDataTable(string xlsFile, string sheetName,string range, bool hasHeader)
        {
            using (OleDbConnection conn = new OleDbConnection())
            {
                conn.ConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties = 'Excel 8.0;HDR={1};IMEX=1'", xlsFile, hasHeader ? "YES" : "NO");
                conn.Open();
                string cmd = " SELECT * FROM [" + sheetName + "$" + range + "] ";
                using (OleDbDataAdapter oleAdper = new OleDbDataAdapter(cmd, conn))
                {
                    DataTable result = new DataTable("table");
                    oleAdper.Fill(result);
                    return result;
                }
            }
        }

        public static OleDbDataReader ReadXlsToDataReader(string xlsFile, string sheetName, string range, bool hasHeader)
        {
            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties = 'Excel 8.0;HDR={1};IMEX=1'", xlsFile, hasHeader ? "YES" : "NO");
            conn.Open();
            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = " SELECT * FROM [" + sheetName + "$" + range + "] ";
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
    }
}
