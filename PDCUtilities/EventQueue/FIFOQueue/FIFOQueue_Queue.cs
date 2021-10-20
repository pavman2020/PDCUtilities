using System;
using System.Collections.Generic;

namespace PDCUtility.EventQueue
{
    public partial class FIFOQueue
    {
        private volatile List<Task> m_lTaskQueue = null;
        private object m_oLock = new object();

        /// <summary>
        /// will cancel the event (from the GUID)
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public bool CancelEvent(Guid guid)
        {
            bool oRet = false;

            lock (m_oLock)
            {
                for (int i = 0; i < m_lTaskQueue.Count; i++)
                {
                    Task oT = m_lTaskQueue[i];
                    if (oT.GUID.Equals(guid))
                    {
                        m_lTaskQueue.RemoveAt(i);
                        return true;
                    }
                }
            }
            return oRet;
        }

        /// <summary>
        /// will clear from the queue all items which have not yet been started.
        /// </summary>
        public void ClearQueue()
        {
            lock (m_oLock)
            {
                for (bool bCleared = false; !bCleared;)
                {
                    bCleared = true;

                    for (int i = 0; i < m_lTaskQueue.Count; i++)
                    {
                        Task oT = m_lTaskQueue[i];

                        if (!oT.StartTime.HasValue)
                        {
                            oT.SetEndTime(DateTime.Now);
                            m_lTaskQueue.RemoveAt(i);
                            bCleared = false;
                            i = m_lTaskQueue.Count;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// returns the Task object when suppled with the GUID
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public Task GetTask(Guid g)
        {
            lock (m_oLock)
            {
                foreach (Task oT in m_lTaskQueue)
                    if (oT.GUID.Equals(g))
                        return oT;
            }

            return (Task)null;
        }

        /// <summary>
        /// will queue a Task
        /// </summary>
        /// <param name="oTask"></param>
        /// <returns></returns>
        public Guid QueueTask(Task oTask)
        {
            lock (m_oLock)
            {
                m_lTaskQueue.Add(oTask);
            }

            return oTask.GUID;
        }

        /// <summary>
        /// used by thread to get the next Task and remove it from the queue
        /// </summary>
        /// <returns></returns>
        private Task _NextTask()
        {
            Task oRet = null;

            lock (m_oLock)
            {
                if (0 < m_lTaskQueue.Count)
                {
                    oRet = m_lTaskQueue[0];
                    m_lTaskQueue.RemoveAt(0);
                }
            }

            return oRet;
        }
    }
}
