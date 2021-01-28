using System;
using System.Collections.Generic;
using System.Text;

namespace Comfy.App.Core.QualityCode
{
    public class QcmaininfoModelGen : QcmaininfoModel
    {

        public int PageSize { get; set; }
        public int StartPage { get; set; }
        public string OrderByField { get; set; }
        public string Finishing { get; set; }
        public string Construction { get; set; }
        public string Sales { get; set; }
        public string SalesTeam { get; set; }
        public string CustomerQaulityId { get; set; }
        // 添加客户信息字段  by LYH 2014/2/25
        public string CustomerCode { get; set; }
        public DateTime CreateDate { get; set; }
        //Add by sunny 2017 10 16 
        public DateTime CreateEndDate { get; set; }

    }
}
