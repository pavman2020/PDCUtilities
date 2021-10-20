using System;


namespace PDCUtility.EventQueue
{
    public partial class FIFOQueue
    {
        public class ErrorEventArgs : EventArgs
        {
            public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

            public string ErrorMessage { get; set; }

            public Exception Exception { get; set; }

            public Guid GUID { get; set; }

            public Task Task { get; set; }
        }
    }
}
