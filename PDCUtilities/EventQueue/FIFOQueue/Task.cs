using System;


namespace PDCUtility.EventQueue
{
    public partial class FIFOQueue
    {
        public class Task
        {
            #region "Execution"

            public delegate void TaskEventHandler(object o, EventArgs e, Task t);

            /// <summary>
            /// passed to the callback (from queueing call)
            /// </summary>
            public EventArgs EventArgs { get; private set; }

            /// <summary>
            /// this is called when the queue is processed. passed the Sender and EventArgs parameters that were passed when the Task was Queued
            /// this call is made on a separate thread - so it can be aborted to stop it, and it is NOT the windows event thread so, interactions with forms must be invoked
            /// </summary>
            public TaskEventHandler Execute { get; private set; }

            /// <summary>
            /// passed to the callback (from queueing call)
            /// </summary>
            public object Sender { get; private set; }

            #endregion "Execution"

            public Task(TaskEventHandler fnExecute, object oSender, EventArgs oEArgs)
            {
                Execute = fnExecute;
                Sender = oSender;
                EventArgs = oEArgs;
                Queued = DateTime.Now;
                ElapsedTime = null;
                GUID = Guid.NewGuid();
            }

            /// <summary>
            /// how much time elapsed executing the command
            /// </summary>
            public TimeSpan? ElapsedTime { get; private set; }

            /// <summary>
            /// after the Task is processed (or cancelled) this is when the Task was finished
            /// </summary>
            public DateTime? EndTime { get; private set; }

            /// <summary>
            /// will be false if EndTime.Hasvalue and the Task was cancelled before execution.
            /// </summary>
            public bool ExecutedToCompletion { get; private set; }

            /// <summary>
            /// uniquely identifies the Task
            /// </summary>
            public Guid GUID { get; private set; }

            /// <summary>
            /// when was the Task Queued
            /// </summary>
            public DateTime Queued { get; private set; }

            /// <summary>
            /// when was execution of the Task started
            /// </summary>
            public DateTime? StartTime { get; private set; }

            #region "Internals"

            internal _TaskThread TaskThread { get; private set; }

            internal void SetElapsedTime(TimeSpan ts)
            {
                ElapsedTime = ts;
            }

            internal void SetEndTime(DateTime dt)
            {
                EndTime = dt;
            }

            internal void SetExecutedToCompletion(bool b)
            {
                ExecutedToCompletion = b;
            }

            internal void SetStartTime(DateTime dt)
            {
                StartTime = dt;
            }

            internal void SetTaskThread(_TaskThread tt)
            {
                TaskThread = tt;
            }

            #endregion
        }
    }
}
