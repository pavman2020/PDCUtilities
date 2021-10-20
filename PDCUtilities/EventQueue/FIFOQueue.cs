using System;
using System.Collections.Generic;
using System.Threading;

namespace PDCUtility.EventQueue
{
    public sealed partial class FIFOQueue : IDisposable
    {
        public delegate bool OkToRunDelegate();
        #region "Construction / Destruction"

        public FIFOQueue(bool bStarted = true,
                            ErrorEventArgs.ErrorEventHandler fnErrorHandler = null,
                            OkToRunDelegate fnOkToRunDelegate = null,
                            int? niThreadSleepMilliseconds = 1000)
        {
            Running = bStarted;
            m_fnErrorEventHandler = fnErrorHandler;
            m_fnOkToRunDelegate = fnOkToRunDelegate;
            m_lTaskQueue = new List<Task>();

            _StartThread(niThreadSleepMilliseconds);
        }

        public void Dispose()
        {
            try
            {
                m_bRunThreadLoop = false;

                while (m_bThreadIsRunning)
                    Thread.Sleep(100);

                m_fnErrorEventHandler = null;

                try
                {
                    m_lTaskQueue = null;
                }
                catch { }

                if (null != m_oThread)
                {
                    try { m_oThread.Abort(); }
                    catch { }

                    m_oThread = null;
                }
            }
            catch { }
        }

        #endregion "Construction / Destruction"

        private volatile bool m_bRunning = false;

        private ErrorEventArgs.ErrorEventHandler m_fnErrorEventHandler = null;

        public bool Running
        {
            get
            {
                return m_bRunning;
            }
            set
            {
                m_bRunning = value;
            }
        }
    }
}
