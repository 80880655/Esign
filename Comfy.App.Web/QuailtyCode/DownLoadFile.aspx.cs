using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
//using System.Windows.Forms;

namespace YsYarnWHAutoRejection
{
    public partial class DownLoadFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               

                string realName = Request.QueryString["realName"] == null ? string.Empty : Request.QueryString["realName"].ToString();
                string outName = Request.QueryString["outName"] == null ? string.Empty : Request.QueryString["outName"].ToString();
                Dowload(realName, outName);
               
            }
        }

        /// <summary>
        /// xlsx文件下载方式
        /// </summary>
        /// <param name="realFileName"></param>
        /// <param name="outFileName"></param>
        private void Dowload(string realFileName, string outFileName)
        {
            string fileName = Server.MapPath("~/TemparyFile/") + realFileName;
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "utf-8";

                //SaveFileDialog exe = new SaveFileDialog();

                //Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.xlsx", outFileName));
                //Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.xls", outFileName));

                if (outFileName == "QC export file template")
                {
                    //Response.ContentType = "application/vnd.ms-excel.numberformat:@;";
                    //Response.AddHeader("Content-disposition", string.Format("attachment;filename={0}.csv", HttpUtility.UrlEncode(outFileName + "_" + DateTime.Now.ToString("yyyy-MM-dd"), System.Text.Encoding.UTF8)));
                    //Response.AddHeader("Content-disposition", string.Format("attachment;filename={0}.csv", HttpUtility.UrlEncode(outFileName +"_"+DateTime.Now.ToString("yyyy-MM-dd"))));

                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.csv", outFileName));
                    Response.ContentEncoding = Encoding.UTF8;

                }
                else {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.xls", outFileName));
                    Response.ContentEncoding = Encoding.UTF8;
                }

                long dataLengthToRead = file.Length;
                byte[] buff = new byte[5000];
                while (dataLengthToRead > 0 && Response.IsClientConnected)
                {
                    int lengthRead = file.Read(buff, 0, 5000);
                    Response.OutputStream.Write(buff, 0, lengthRead);
                    Response.Flush();
                    dataLengthToRead = dataLengthToRead - lengthRead;
                }
            }
            if (File.Exists(fileName)) File.Delete(fileName);
            Response.End();
        }
    }
}