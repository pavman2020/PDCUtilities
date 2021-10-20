using System;
using System.Threading;

namespace PDCUtility.EventQueue
{
    public partial class FIFOQueue
    {
        private bool m_bRunThreadLoop = true;
        private bool m_bThreadIsRunning = false;
        private int m_iThreadSleepMilliseconds = 1000;
        private volatile Task m_oCurrentTask = null;
        private Thread m_oThread = null;
        private OkToRunDelegate m_fnOkToRunDelegate = null;

        public bool CancelCurrentTask()
        {
            if (null != m_oCurrentTask)
            {
                m_oCurrentTask.TaskThread.Abort();
                return true;
            }

            return false;
        }

        private void _DoTask(object o)
        {
            {
                Task oNextTask = null;
                try
                {
                    oNextTask = (Task)o;

                    oNextTask.SetStartTime(DateTime.Now);
                    oNextTask.Execute(oNextTask.Sender, oNextTask.EventArgs, oNextTask);
                    oNextTask.SetExecutedToCompletion(true);
                }
                finally
                {
                    try
                    {
                        oNextTask.SetEndTime(DateTime.Now);
                    }
                    catch { }
                }
            }
        }

        private void _DoThread(object o)
        {
            try
            {
                bool bStop = false;
                while ((!bStop) && (m_bRunThreadLoop))
                    try
                    {
                        m_bThreadIsRunning = true;

                        while ((!bStop) && (m_bRunThreadLoop) && (m_bRunning) && (0 < m_lTaskQueue.Count))
                            try
                            {
                                m_oCurrentTask = _NextTask();

                                if (null != m_oCurrentTask)
                                {
                                    if (null != m_oCurrentTask.Execute)
                                    {
                                        m_oCurrentTask.SetTaskThread(new _TaskThread(_DoTask));
                                        try
                                        {
                                            try
                                            {
                                                m_oCurrentTask.TaskThread.ExecuteTask(m_oCurrentTask);
                                            }
                                            finally
                                            {
                                                m_oCurrentTask.SetElapsedTime(m_oCurrentTask.TaskThread.Elapsed);
                                                m_oCurrentTask.TaskThread.Dispose();
                                                m_oCurrentTask.SetTaskThread(null);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _ThrowError(new ErrorEventArgs()
                                            {
                                                ErrorMessage = ex.Message,
                                                Task = m_oCurrentTask,
                                                GUID = m_oCurrentTask.GUID,
                                                Exception = ex
                                            });
                                        }
                                    }       //  if (null != oNextTask.Task.Execute)
                                    else
                                    {
                                        _ThrowError(new ErrorEventArgs()
                                        {
                                            ErrorMessage = "My Bad",
                                            Task = m_oCurrentTask,
                                            GUID = m_oCurrentTask.GUID
                                        });
                                    }       //  if (null != oNextTask.Task.Execute) else
                                }       //  if (null != oNextTask)
                            }       //  if (m_bRunning)
                            catch { }

                        Thread.Sleep(m_iThreadSleepMilliseconds);
                    }
                    catch { }
                    finally
                    {
                        if (null != m_fnOkToRunDelegate)
                            try
                            {
                                bStop = m_fnOkToRunDelegate();
                            }
                            catch //(Exception ex)
                            {
                                //string s = ex.Message;
                            }
                    }
            }
            catch { }

            m_bThreadIsRunning = false;
        }

        private void _StartThread(int? niThreadSleepMilliseconds)
        {
            if (niThreadSleepMilliseconds.HasValue) m_iThreadSleepMilliseconds = niThreadSleepMilliseconds.Value;

            ParameterizedThreadStart oPS = new ParameterizedThreadStart(_DoThread);
            m_oThread = new Thread(oPS);
            m_oThread.Start(this);
        }

        private void _ThrowError(ErrorEventArgs e)
        {
            try
            {
                if (null == m_fnErrorEventHandler)
                    return;

                m_fnErrorEventHandler(this, e);
            }
            catch { }
        }
    }
}