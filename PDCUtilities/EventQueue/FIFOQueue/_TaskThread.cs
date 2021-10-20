using System;
using System.Diagnostics;
using System.Threading;

namespace PDCUtility.EventQueue
{
    public partial class FIFOQueue
    {
        internal class _TaskThread : IDisposable
        {
            private Stopwatch m_oStopWatch = null;
            private Thread m_oWorkThread = null;

            public _TaskThread(ParameterizedThreadStart target)
            {
                ParameterizedThreadStart oPS = new ParameterizedThreadStart(target);
                m_oWorkThread = new Thread(oPS);
                m_oStopWatch = new Stopwatch();
            }

            public TimeSpan Elapsed { get { return m_oStopWatch.Elapsed; } }

            public void Abort()
            {
                if (null != m_oWorkThread)
                    try { m_oWorkThread.Abort(); }
                    catch { }
            }

            public void Dispose()
            {
                Abort();

                m_oWorkThread = null;
                GC.Collect();
            }

            public void ExecuteTask(object o)
            {
                m_oStopWatch.Start();
                try
                {
                    m_oWorkThread.Start(o);
                    m_oWorkThread.Join();
                }
                finally
                {
                    m_oStopWatch.Stop();
                }
            }
        }
    }
}
