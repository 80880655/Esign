using System;
using System.Collections.Generic;
using System.Text;

namespace Comfy.App.Core.QualityCode
{
   public class AvaWidthModel
    {
       public string QualityCode { get; set; }
       public int Gauge { get; set; }
       public int Diameter { get; set; }
       public int TotalNeedles { get; set; }
       public decimal Width { get; set; }
       public decimal MaxWidth { get; set; }
       public string UpdatedBy { get; set; }
       public DateTime UpdateTime { get; set; }
    }
}
