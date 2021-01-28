using System;
using System.Collections.Generic;
using System.Text;
using Comfy.Data;
using System.Collections;
namespace Comfy.Utils
{
    /// <summary>
    /// 該類用反射機制,匹配model和table的字段，這樣就不用寫繁瑣的AddColumn方法
    /// by zhanghe
    /// </summary>
    public class TableMapModel
    {
        /// <summary>
        /// 增加表操作
        /// </summary>
        /// <param name="database">Comfy.Data.Database</param>
        /// <param name="table">table 類</param>
        /// <param name="model">lmodel 類</param>
        public static void AddTable(Database database, QueryTable table, Object model)
        {
            Type modelType = model.GetType();
            Type tableType = table.GetType();
            String s = "";
            Comfy.Data.QueryColumn cq = null;
            InsertSqlSection inSql = database.Insert(table);
            foreach (System.Reflection.PropertyInfo info in modelType.GetProperties())
            {
                s = info.Name;

                if (table.GetType().GetProperty(s) == null)
                    continue;
                cq = (Comfy.Data.QueryColumn)table.GetType().GetProperty(s).GetValue(table, null);
                inSql.AddColumn(cq, info.GetValue(model, null));

            }
            inSql.Execute();

        }



        /// <summary>
        /// 更新表
        /// </summary>
        /// <param name="database">Comfy.Data.Database</param>
        /// <param name="table">table類</param>
        /// <param name="model">model類</param>
        /// <param name="updateFields">要更新的字段，名字要和model中的屬性相同。如果要更新所有的字段，此參數為null</param>
        /// <param name="whereStrs">條件字段</param>
        public static void UpdateTable(Database database, QueryTable table, Object model, Comfy.Data.QueryColumn[] updateFields, Comfy.Data.QueryColumn[] whereQc)
        {
            Type modelType = model.GetType();
            Type tableType = table.GetType();
            String s = "";
            Comfy.Data.QueryColumn wQc = null;
            UpdateSqlSection updateSql = database.Update(table);
            if (updateFields == null)   //更新所有字段
            {
                foreach (System.Reflection.PropertyInfo info in modelType.GetProperties())  //更新所有字段
                {
                    s = info.Name;
                    int i = 0;
                    foreach (QueryColumn wqc in whereQc)
                    {

                        if (wqc.Name == s)
                        {
                            i++;
                            break;
                        }
                    }
                    if (i == 0)
                    {
                        QueryColumn qc = (Comfy.Data.QueryColumn)table.GetType().GetProperty(s).GetValue(table, null);
                        updateSql.AddColumn(qc, info.GetValue(model, null));

                    }

                }
            }
            else //更新部分字段
            {

                foreach (QueryColumn qc in updateFields)
                {


                    foreach (System.Reflection.PropertyInfo info in modelType.GetProperties())  //更新所有字段
                    {
                        if (qc.Name == info.Name)
                        {
                            updateSql.AddColumn(qc, info.GetValue(model, null));
                            break;
                        }
                    }


                }

            }

            foreach (QueryColumn wqc in whereQc)
            {

                foreach (System.Reflection.PropertyInfo wInfo in modelType.GetProperties()) //條件語句
                {

                    if (wqc.Name == wInfo.Name)
                    {
                        updateSql.Where(wqc == wInfo.GetValue(model, null));
                        break;

                    }

                }
            }
            updateSql.Execute();

        }
        /// <summary>
        /// 獲得表信息
        /// </summary>
        /// <param name="modelList">modelList</param>
        /// <param name="table">table</param>
        /// <param name="model">model</param>
        /// <param name="sdr">SafeDataReader sdr</param>
        public static void GetTable(IList modelList, QueryTable table, Object model, SelectSqlSection sql)
        {
            Type modelType = model.GetType();  //model的類型
            Type tableType = table.GetType();  //table的類型
            string proName = "";  //屬性名稱
            using (SafeDataReader sdr = new SafeDataReader(sql.ToDataReader()))
            {
                while (sdr.Read())
                {
                    object modelObj = Activator.CreateInstance(modelType);  //創建新對象
                    Type modelObjType = modelObj.GetType();   //獲得對象的類型

                    foreach (System.Reflection.PropertyInfo modelInfo in modelObjType.GetProperties())  //遍曆model的所有屬性
                    {
                        proName = modelInfo.Name;
                        if (table.GetType().GetProperty(proName) == null)
                            continue;
                        Comfy.Data.QueryColumn queryColumn = (Comfy.Data.QueryColumn)table.GetType().GetProperty(proName).GetValue(table, null);
                        //匹配類型
                        if (modelInfo.PropertyType.Equals(typeof(string)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetString(queryColumn), null);
                        }

                        else if (modelInfo.PropertyType.Equals(typeof(DateTime)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetDateTime(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(int)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetInt32(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(long)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetInt64(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(short)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetInt16(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(bool)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetBoolean(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(float)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetFloat(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(double)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetDouble(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(decimal)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetDecimal(queryColumn), null);

                        }
                        else
                        {
                            throw new Exception("沒有匹配中類型");
                        }


                    }
                    modelList.Add(modelObj);

                }
            }

        }

        /// <summary>
        /// 獲得表信息
        /// </summary>
        /// <param name="modelList">modelList</param>
        /// <param name="table">table</param>
        /// <param name="model">model</param>
        /// <param name="sdr">SafeDataReader sdr</param>
        public static void GetTable(IList modelList, QueryTable table, Object model, SelectSqlSection sql, int sum)
        {
            Type modelType = model.GetType();  //model的類型
            Type tableType = table.GetType();  //table的類型
            string proName = "";  //屬性名稱
            using (SafeDataReader sdr = new SafeDataReader(sql.ToDataReader()))
            {
                while (sdr.Read())
                {
                    object modelObj = Activator.CreateInstance(modelType);  //創建新對象
                    Type modelObjType = modelObj.GetType();   //獲得對象的類型
                    foreach (System.Reflection.PropertyInfo modelInfo in modelObjType.GetProperties())  //遍曆model的所有屬性
                    {
                        proName = modelInfo.Name;
                        if (proName.Equals("GridRowCount"))
                        {
                            modelInfo.SetValue(modelObj, sum, null);
                            continue;
                        }

                        if (table.GetType().GetProperty(proName) == null)
                            continue;
                        Comfy.Data.QueryColumn queryColumn = (Comfy.Data.QueryColumn)table.GetType().GetProperty(proName).GetValue(table, null);

                        //匹配類型
                        if (modelInfo.PropertyType.Equals(typeof(string)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetString(queryColumn), null);
                        }

                        else if (modelInfo.PropertyType.Equals(typeof(DateTime)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetDateTime(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(int)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetInt32(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(long)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetInt64(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(short)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetInt16(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(bool)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetBoolean(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(float)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetFloat(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(double)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetDouble(queryColumn), null);
                        }
                        else if (modelInfo.PropertyType.Equals(typeof(decimal)))
                        {
                            modelInfo.SetValue(modelObj, sdr.GetDecimal(queryColumn), null);

                        }
                        else
                        {
                            throw new Exception("沒有匹配中類型");
                        }


                    }
                    modelList.Add(modelObj);

                }
            }

        }
        /// <summary>
        /// 獲取屬性值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static Comfy.Data.QueryColumn GetTableColumnByProName(string name, QueryTable table)
        {
            foreach (System.Reflection.PropertyInfo tableInfo in table.GetType().GetProperties())
            {
                if (tableInfo.Name.Equals(name))
                {
                    return (Comfy.Data.QueryColumn)tableInfo.GetValue(table, null);
                }
            }

            return null;
        }

        public static int SetRangeSql(ref SelectSqlSection sql, QueryTable table, QueryColumn IkeyColumn, int startPage, int pageSize, string orderByField)
        {

            string[] origanalStr = sql.ColumnNames;
            sql.ColumnNames = new string[] { "COUNT(*)" };
            int count = Convert.ToInt32(sql.ToScalar());
            if (!string.IsNullOrEmpty(orderByField))
            {
                string[] orderStr = orderByField.Split(new char[] { '*' });
                try
                {
                    QueryColumn qc = TableMapModel.GetTableColumnByProName(orderStr[0], table);

                    if (orderStr[1].Contains("Asc"))  //升序
                    {
                        sql.OrderBy(qc.Asc, IkeyColumn.Asc);
                    }
                    if (orderStr[1].Contains("Desc"))  //降序
                    {
                        sql.OrderBy(qc.Desc, IkeyColumn.Asc);
                    }

                }
                catch (Exception e)
                {
                    // throw e;
                }
            }

            int wPageSize = pageSize;
            if (pageSize * startPage > count)
            {
                wPageSize = count - pageSize * (startPage - 1);
            }
            if (pageSize > 0 && startPage > 0 && wPageSize > 0)
            {
                sql.SetSelectRange(wPageSize, pageSize * (startPage - 1), IkeyColumn);
            }

            sql.ColumnNames = origanalStr;

            return count;
        }

    }

}
