using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Comfy.Utils
{
    public interface IFormStatus
    {
        string FormStatus { get; set; }
        Cursor Cursor { get; set; }
    }
}
