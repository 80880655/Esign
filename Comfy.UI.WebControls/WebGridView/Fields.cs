using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Reflection;
using System.Web.Caching;
using System.Text.RegularExpressions;

namespace Comfy.UI.WebControls.WebGridView
{
    [ToolboxItem(false), ParseChildren(true)]
    public class Fields : List<Field>
    {

        #region 定义构造函数

        public Fields() : base()
        {
        }

        #endregion

        /// <summary>
        /// 得到集合元素的个数
        /// </summary>
        public new int Count
        {
            get
            {
                return base.Count;
            }
        }

        /// <summary>
        /// 表示集合是否为只读
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 添加对象到集合
        /// </summary>
        /// <param name="item"></param>
        public new void Add(Field field)
        {
            base.Add(field);
        }

        public new void Insert(Field field)
        {
            base.Insert(0, field);
        }
        /// <summary>
        /// 清空集合
        /// </summary>
        public new void Clear()
        {
            base.Clear();
        }

        /// <summary>
        /// 判断集合中是否包含元素
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public new bool Contains(Field field)
        {
            return base.Contains(field);
        }

        /// <summary>
        /// 移除一个对象
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public new bool Remove(Field field)
        {
            return base.Remove(field);
        }

        /// <summary>
        /// 设置或取得集合索引项
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public new Field this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
            }
        }
    }
}
