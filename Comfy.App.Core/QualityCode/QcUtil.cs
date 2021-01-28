using System;
using System.Collections.Generic;
using System.Text;

namespace Comfy.App.Core.QualityCode
{
  public  class QcUtil
    {
      public static string GetQualityCode(string source)
      {
          
          string sor;
          if (source == "Fabric")
          {
              sor = "C";
          }
          else if (source == "Flat Knit")
          {
              sor = "F";
          }
          else if (source == "Tapping")
          {
              sor = "T";
          }
          else if (source == "External")
          {
              sor = "E";
          }
          else return null;

          string strYear = System.DateTime.Now.Year.ToString().Substring(2);

          string strSeq = DataAccess.DefaultDB.CustomSql("select Create_Year||'+'||SEQ  from QCCoding where Material_Group='" + source + "' ").ToScalar<string>();
          string[] strYearSeq = strSeq.Split(new string[] { "+" }, StringSplitOptions.None);
          string ResStr = "";
          if (strYearSeq[0] != System.DateTime.Now.Year.ToString())
          {
              DataAccess.DefaultDB.CustomSql("update QCCoding set Create_Year=" + System.DateTime.Now.Year.ToString() + ",SEQ=0 where Material_Group='" + source + "' ").ExecuteNonQuery();
              ResStr= sor + strYear + "0";
          }
          else
          {
              int strLength = strYearSeq[1].Length;
              string strReturn = strYearSeq[1];
              for (int i = 1; i <= (5 - strLength); i++)
              {
                  strReturn = "0" + strReturn;
              }
              DataAccess.DefaultDB.CustomSql("update QCCoding set SEQ="+(Convert.ToInt32(strYearSeq[1])+1).ToString()+" where Material_Group='" + source + "' ").ExecuteNonQuery();
              ResStr= sor + strYear + strReturn;
          }
          int j = DataAccess.DefaultDB.CustomSql("select count(1) from QCMainInfo where Quality_Code='" + ResStr + "' ").ToScalar<int>();
          if (j == 1)
          {
              ResStr= GetQualityCode(source);
          }
          return ResStr;

      }
    }
}
