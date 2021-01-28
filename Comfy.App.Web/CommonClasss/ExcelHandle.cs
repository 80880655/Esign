using System;
using System.Data;
using System.IO;
using System.Collections;
using NPOI.SS.UserModel;
using NPOI.SS.Formula.Eval;
//using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Text;
using NPOI.XSSF.UserModel;

namespace YsYarnWHAutoRejection.CommonClass
{
    public class ExcelHandle
    {
        /// <summary>
        /// 生成excel文件流
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static void DataTableToExcel(DataTable dt, string filePath, int sheetIndex = 0)
        {
            IWorkbook workbook = null;
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {

                    //workbook = new XSSFWorkbook(file);
                    workbook = new HSSFWorkbook(file);

                    ISheet sheet = workbook.GetSheetAt(sheetIndex);
                    ICellStyle headerCellStyle = workbook.CreateCellStyle();
                    headerCellStyle.BorderBottom = BorderStyle.Thin;
                    headerCellStyle.BorderLeft = BorderStyle.Thin;
                    headerCellStyle.BorderRight = BorderStyle.Thin;
                    headerCellStyle.BorderTop = BorderStyle.Thin;

                    headerCellStyle.Alignment = HorizontalAlignment.Center;

                    //modify:gaofeng  时间：2021年1月19日21:06:10
                    headerCellStyle.VerticalAlignment = VerticalAlignment.Center;

                    IFont headerFont = workbook.CreateFont();
                    headerFont.Boldweight = (short)FontBoldWeight.Bold;
                    headerCellStyle.SetFont(headerFont);

                    int colIndex = 0;
                    IRow headerRow = sheet.CreateRow(0);
                    foreach (DataColumn dc in dt.Columns)
                    {
                        ICell cell = headerRow.CreateCell(colIndex);
                        cell.SetCellValue(dc.ColumnName);
                        cell.CellStyle = headerCellStyle;
                        colIndex++;
                    }

                    ICellStyle cellStyle = workbook.CreateCellStyle();

                    cellStyle.BorderBottom = BorderStyle.Thin;
                    cellStyle.BorderLeft = BorderStyle.Thin;
                    cellStyle.BorderRight = BorderStyle.Thin;
                    cellStyle.BorderTop = BorderStyle.Thin;

                    IFont cellFont = workbook.CreateFont();
                    cellFont.Boldweight = (short)FontBoldWeight.Normal;
                    cellStyle.SetFont(cellFont);


                    // 创建行
                    int rowIndex = 1;

                    int cellIndex = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        IRow dataRow = sheet.CreateRow(rowIndex);
                        foreach (DataColumn dc in dt.Columns)
                        {
                            ICell cell = dataRow.CreateCell(cellIndex);
                            cell.SetCellValue(dr[dc].ToString());
                            cell.CellStyle = cellStyle;
                            cellIndex++;
                        }
                        cellIndex = 0;
                        rowIndex++;
                    }

                    //for (int i = 0; i < colIndex; i++)
                    //{
                    //    sheet.AutoSizeColumn(i);
                    //}
                    MemoryStream ms = new MemoryStream();
      
                    workbook.Write(ms);
                    
                    ms.Flush();

                    using (FileStream saveFile = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        byte[] data = ms.ToArray();
                        saveFile.Write(data, 0, data.Length);

                        saveFile.Flush();
                        data = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                workbook = null;
            }
        }




        public static void DataTabletoExcelCSV(DataTable dt, string filePath, int sheetIndex = 0) {






        }



    }
}