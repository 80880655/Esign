using System;
using System.Windows.Forms;

namespace Comfy.Utils
{
    public class StatusBusy : IDisposable
    {
        private string _oldStatus;
        private Cursor _oldCursor;
        private IFormStatus _form;

        public StatusBusy(string statusText, IFormStatus form)
        {
            _form = form;
            _oldStatus = form.FormStatus;
            _form.FormStatus = statusText;
            _oldCursor = form.Cursor;
            _form.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
        }

        // IDisposable
        private bool _disposedValue = false; // To detect redundant calls

        protected void Dispose(bool disposing)
        {
            if (!_disposedValue)
                if (disposing)
                {
                    _form.FormStatus = _oldStatus;
                    _form.Cursor = _oldCursor;
                }
            _disposedValue = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
