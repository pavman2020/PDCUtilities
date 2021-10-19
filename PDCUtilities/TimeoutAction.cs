using System;
using System.Threading;

namespace PDCUtility
{
    public class TimeoutAction
    {
        public static bool DidActionTimeout(TimeSpan ts, Action action)
        {
            return DidActionTimeout(action, ts);
        }

        public static bool DidActionTimeout(Action action, TimeSpan ts)
        {
            bool bRet = false;
            Thread t1Sleeper = null;
            Thread t2Action = null;
            object m_oLock = new object();
            try
            {
                t2Action = new Thread(() =>
                {
                    try
                    {
                        if (null != action)
                            try
                            {
                                action();
                                bRet = false;
                            }
                            finally
                            {
                                lock (m_oLock)
                                {
                                    t2Action = null;
                                    if (null != t1Sleeper)
                                        t1Sleeper.Abort();
                                }
                            }
                    }
                    catch (ThreadAbortException)
                    {
                    }
                    finally
                    {
                        lock (m_oLock)
                        {
                            t2Action = null;

                            if (null != t1Sleeper)
                                t1Sleeper.Abort();
                        }
                    }
                })
                { Name = "Action" };

                t1Sleeper = new Thread(() =>
                {
                    try
                    {
                        #region "waiting"

                        System.Threading.Thread.Sleep(ts);
                        bRet = true;

                        #endregion "waiting"
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                    }
                    finally
                    {
                        lock (m_oLock)
                        {
                            t1Sleeper = null;

                            if (null != t2Action)
                                t2Action.Abort();
                        }
                    }
                })
                { Name = "Sleeper" };

                t1Sleeper.Start();
                t2Action.Start();

                TimeSpan ds = new TimeSpan(0, 0, 1);

                while ((null != t1Sleeper) || (null != t2Action))
                {
                    if ((null == t1Sleeper) && (null != t2Action))
                        t2Action.Abort();

                    Thread.Sleep(ds);
                }
            }
            finally { m_oLock = null; }

            return bRet;
        }
    }
}