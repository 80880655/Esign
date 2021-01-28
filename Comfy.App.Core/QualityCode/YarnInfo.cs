using System;
using System.Collections.Generic;
using System.Text;

namespace Comfy.App.Core.QualityCode
{
   public class YarnInfo
    {
       public string Seq { get; set; }
       public Decimal Radio { get; set; }
       public int Threads { get; set; }
       public string YarnType { get; set; }
       public string YarnCount { get; set; }
       public string WarpWeft { get; set; }
       public Decimal YarnDensity { get; set; }
       public string YarnComponent { get; set; }
    }
}
