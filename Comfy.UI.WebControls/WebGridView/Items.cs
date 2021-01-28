using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;

namespace Comfy.UI.WebControls.WebGridView
{
   [ToolboxItem(false), ParseChildren(true)]
   public  class Items:List<Item>
    {

        #region 定义构造函数

       public Items():base()
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
        public new void Add(Item item)
        {
            base.Add(item);
        }

        public new void Insert(Item item)
        {
            base.Insert(0, item);
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
        public new bool Contains(Item item)
        {
            return base.Contains(item);
        }

        /// <summary>
        /// 移除一个对象
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public new bool Remove(Item item)
        {
            return base.Remove(item);
        }

        /// <summary>
        /// 设置或取得集合索引项
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public new Item this[int index]
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
