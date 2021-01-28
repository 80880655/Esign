using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Comfy.UI.WebControls.WebButton
{
   public class FaceStyle:TableItemStyle,IStateManager
    {
       private bool _blnOK;

       public FaceStyle()
       {
           _blnOK = false;
       }

       public bool OK
       {
           get
           {
               return _blnOK;
           }
           set
           {
               _blnOK = value;
           }
       }

       bool IStateManager.IsTrackingViewState
       {
           get
           {
               return base.IsTrackingViewState;
           }
       }

       object IStateManager.SaveViewState()
       {
           object[] state = new object[2];
           state[0] = base.SaveViewState();
           state[1] = (object)OK;
           return state;
       }

       void IStateManager.LoadViewState(object state)
       {
           if (state == null)
           {
               return;
           }
           object[] myState = (object[])state;
           base.LoadViewState(myState[0]);
           OK = (bool)myState[1];
       }
    }

}
