using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Comfy.Data;



namespace Comfy.Utils.Core
{
    public class MessageHelper
    {
        public MessageHelper() { }

        /// <summary>
        /// For Win App.
        /// </summary>
        /// <param name="text"></param>
        public void ShowError(string text)
        {
            ShowError(text, "");
        }

        /// <summary>
        /// For Win App.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        public void ShowError(string text, string caption)
        {
            Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowError(Exception  exc, string caption)
        {
            Show(exc.ToString(), caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowError(Exception exc)
        {
            Show(exc.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// For Win App.
        /// </summary>
        /// <param name="text"></param>
        public void ShowInfo(string text)
        {
            ShowInfo(text, "");
        }

        /// <summary>
        /// For Win App.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        public void ShowInfo(string text, string caption)
        {
            Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// For Win App.
        /// </summary>
        /// <param name="text"></param>
        public void ShowWarn(string text)
        {
            ShowWarn(text, "");
        }

        /// <summary>
        /// For Win App.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        public void ShowWarn(string text, string caption)
        {
            Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// For Win App.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <param name="buttons"></param>
        /// <param name="icon"></param>
        public void Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
           // XtraMessageBox.Show(text, caption, buttons, icon);
        }

        
       

        
    }
}
