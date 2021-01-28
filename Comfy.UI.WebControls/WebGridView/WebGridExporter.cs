using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Comfy.UI.WebControls.WebGridView;
using Comfy.UI.WebControls.Transformation;
namespace Comfy.UI.WebControls
{
    public class WebGridExporter
    {
        #region style

        private ExportCacheCellStyle GetHeaderStyle()
        {
            ExportCacheCellStyle style = new ExportCacheCellStyle();
            style.BottomBorder = new ExportCacheCellBorderStyle() { Color_ = Color.Black, Width = 1 };
            style.LeftBorder = new ExportCacheCellBorderStyle() { Color_ = Color.Black, Width = 1 };
            style.RightBorder = new ExportCacheCellBorderStyle() { Color_ = Color.Black, Width = 1 };
            style.TopBorder = new ExportCacheCellBorderStyle() { Color_ = Color.Black, Width = 1 };
            style.TextFont = new Font("Arial", 10, FontStyle.Bold);
            style.BkColor = Color.FromArgb(153, 204, 255);
            style.BrushStyle_ = BrushStyle.Solid;
            return style;
        }

        private ExportCacheCellStyle GetCellStyle()
        {
            ExportCacheCellStyle style = new ExportCacheCellStyle();
            style.BottomBorder = new ExportCacheCellBorderStyle() { Color_ = Color.Black, Width = 1 };
            style.LeftBorder = new ExportCacheCellBorderStyle() { Color_ = Color.Black, Width = 1 };
            style.RightBorder = new ExportCacheCellBorderStyle() { Color_ = Color.Black, Width = 1 };
            style.TopBorder = new ExportCacheCellBorderStyle() { Color_ = Color.Black, Width = 1 };
            style.TextFont = new Font("Arial", 10);
            return style;
        }

        private ExportCacheCellStyle GetFormatCellStyle(string format)
        {
            ExportCacheCellStyle style = new ExportCacheCellStyle();
            style.BottomBorder = new ExportCacheCellBorderStyle() { Color_ = Color.Black, Width = 1 };
            style.LeftBorder = new ExportCacheCellBorderStyle() { Color_ = Color.Black, Width = 1 };
            style.RightBorder = new ExportCacheCellBorderStyle() { Color_ = Color.Black, Width = 1 };
            style.TopBorder = new ExportCacheCellBorderStyle() { Color_ = Color.Black, Width = 1 };
            style.TextFont = new Font("Arial", 9);
            style.FormatString = format;
            return style;
        }

        #endregion

        private void CreateHeaderCells(ExportXlsProvider xls, Fields fields, List<int> width, int styleIndex)
        {
            int colIndex = 0;
            foreach (Field field  in fields)
            {
                string value = string.IsNullOrEmpty(field.Caption) ? field.FieldName : field.Caption;
                xls.SetCellString(colIndex, 0, value);
                int w = GetFontWidth(value, xls.GetStyle(styleIndex).TextFont);
                width.Add(w);
                xls.SetColumnWidth(colIndex, w);
                xls.SetCellStyle(colIndex, 0, styleIndex);
                colIndex++;
            }
        }

        private void CreateCells(ExportXlsProvider xls, Fields fields, List<int> width, int styleIndex, int rowIndex, object obj,Comfy.UI.WebControls.WebGridView.WebGridView gridView)
        {
            int colIndex = 0;
            Dictionary<string, int> formatStyles = new Dictionary<string, int>();
            foreach (Field field in fields)
            {
                string value = null;
                if (obj is DataRow)
                    value = (obj as DataRow)[field.FieldName].ToString();
                else
                    value = obj.GetType().GetProperty(field.FieldName).GetValue(obj, null) == null ? string.Empty : obj.GetType().GetProperty(field.FieldName).GetValue(obj, null).ToString();

                value =  gridView.GetLabelText(field, value);

                xls.SetCellData(colIndex, rowIndex, value);

                xls.SetCellStyle(colIndex, rowIndex, styleIndex);

                int w = GetFontWidth(value, xls.GetStyle(styleIndex).TextFont);
                if (w > width[colIndex])
                    xls.SetColumnWidth(colIndex, w);
                colIndex++;
            }
        }

        public void WriteToXls(ICollection source, Fields fields, MemoryStream ms,Comfy.UI.WebControls.WebGridView.WebGridView gridView)
        {
            Dictionary<string, int> formatStyles = new Dictionary<string, int>();
            List<int> width = new List<int>();
            ExportXlsProvider xls = new ExportXlsProvider(ms);
            xls.SetRange(fields.Count, source.Count + 1, true);
            int cellStyleIndex = xls.RegisterStyle(GetCellStyle());
            int headerStyleIndex = xls.RegisterStyle(GetHeaderStyle());
            CreateHeaderCells(xls, fields, width, headerStyleIndex);
            int rowIndex = 1;
            foreach (object obj in source)
            {
                CreateCells(xls, fields, width, cellStyleIndex, rowIndex, obj,gridView);
                rowIndex++;
            }
            xls.Commit();
        }

        public void ExportToXls(object source, Fields fields, string fileName,Comfy.UI.WebControls.WebGridView.WebGridView gridView)
        {
            ICollection data = null;
            if (source is DataSet)
                data = (source as DataSet).Tables[0].Rows;
            else if (source is DataTable)
                data = (source as DataTable).Rows;
            else if (source is ICollection)
                data = source as ICollection;
            else
                throw new NotSupportedException("source type:" + source.GetType());

            MemoryStream ms = new MemoryStream();
            WriteToXls(data, fields, ms,gridView);

            //HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", fileName));
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            HttpContext.Current.Response.End();
        }

        public void ExportToXls(object source, Fields fields, Comfy.UI.WebControls.WebGridView.WebGridView gridView)
        {
            ExportToXls(source, fields, "data_" + System.DateTime.Now.ToString("yyyyMMddHHmmss"),gridView);
        }

        XmlDocument ConvertToXml(object source)
        {
            XmlDocument xml = new XmlDocument();
            System.Data.DataTable dt = source as System.Data.DataTable;
            if (dt != null)
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    dt.WriteXml(ms);
                    ms.Flush();
                    ms.Seek(0, System.IO.SeekOrigin.Begin);
                    xml.Load(ms);
                }
            }
            else
            {
                XmlSerializer serializer = new XmlSerializer(source.GetType());
                using (StringWriter sw = new StringWriter())
                {
                    serializer.Serialize(sw, source);
                    sw.Flush();
                    string o = sw.ToString();
                    xml.LoadXml(o);
                }
            }
            return xml;
        }

        int GetFontWidth(string text, System.Drawing.Font font)
        {
            Graphics gr = Graphics.FromHwnd(System.IntPtr.Zero);
            gr.PageUnit = GraphicsUnit.Point;
            return (int)(gr.MeasureString(text, font, new SizeF(0f, 0f)).Width * 1.5);
        }
    }
}
