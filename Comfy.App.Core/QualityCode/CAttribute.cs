using System;
using System.Collections.Generic;
using System.Text;

namespace Comfy.App.Core.QualityCode
{
  public class CAttribute
    {
      public List<YarnInfo> ListYarn { get; set; }
      public string Construction { get; set; }
      public int BWWeight { get; set; }
      public int AWWeight { get; set; }
      public string DyeMehtod { get; set; }
      public string Pttern { get; set; }
      public List<string> ListFinishing{get;set;}
      public string Layout{get;set;}
      public string Shringkage { get; set; }
      public string TestMethod { get; set; }
      public string GMTWash{get;set;}
      public string YarnLength { get; set; }
      public string TappingType { get; set; }
      public string WarpWeft { get; set; }
      public string Size { get; set; }
      public string YarnDensity { get; set; }
      public string HeavyCollar { get; set; }


      public string GK_NO { get; set; }
      public string QC_Ref_PPO { get; set; }
      public string QC_Ref_GP { get; set; }
      public string HF_Ref_PPO { get; set; }      
      public string HF_Ref_GP { get; set; }
      public string RF_Remart { get; set; }

      public string REPEAT { get; set; } 
    }
}
