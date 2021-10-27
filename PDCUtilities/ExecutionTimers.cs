using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDCUtility
{
    public class ExecutionTimers
    {
        public static System.TimeSpan TimeAction(System.Action action
                                                , System.Action<Exception> fnExceptionHandler = null
                                                )
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            try { action(); } catch (Exception ex) { fnExceptionHandler?.Invoke(ex); }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static System.TimeSpan TimeAction<T1>(System.Action<T1> action
                                                    , T1 p1
                                                    , System.Action<Exception> fnExceptionHandler = null
                                                    )
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            try { action(p1); } catch (Exception ex) { fnExceptionHandler?.Invoke(ex); }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static System.TimeSpan TimeAction<T1, T2>(System.Action<T1, T2> action
                                                        , T1 p1
                                                        , T2 p2
                                                        , System.Action<Exception> fnExceptionHandler = null
                                                        )
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            try
            {
                action(p1, p2);
            }
            catch (Exception ex) { fnExceptionHandler?.Invoke(ex); }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static System.TimeSpan TimeAction<T1, T2, T3>(System.Action<T1, T2, T3> action
                                                            , T1 p1
                                                            , T2 p2
                                                            , T3 p3
                                                            , System.Action<Exception> fnExceptionHandler = null
                                                            )
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            try
            {
                action(p1, p2, p3);
            }
            catch (Exception ex) { fnExceptionHandler?.Invoke(ex); }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static System.TimeSpan TimeFunction<R>(System.Func<R> fn
                                                    , ref R ret
                                                    , System.Action<Exception> fnExceptionHandler = null
                                                    )
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            try { ret = fn(); } catch (Exception ex) { fnExceptionHandler?.Invoke(ex); }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static System.TimeSpan TimeFunction<R, T1>(System.Func<T1, R> fn
                                                        , ref R ret
                                                        , T1 p1
                                                        , System.Action<Exception> fnExceptionHandler = null
                                                        )
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            try { ret = fn(p1); } catch (Exception ex) { fnExceptionHandler?.Invoke(ex); }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static System.TimeSpan TimeFunction<R, T1, T2>(System.Func<T1, T2, R> fn
                                                            , ref R ret
                                                            , T1 p1
                                                            , T2 p2
                                                            , System.Action<Exception> fnExceptionHandler = null
                                                            )
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            try { ret = fn(p1, p2); } catch (Exception ex) { fnExceptionHandler?.Invoke(ex); }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static System.TimeSpan TimeFunction<R, T1, T2, T3>(System.Func<T1, T2, T3, R> fn
                                                                , ref R ret
                                                                , T1 p1
                                                                , T2 p2
                                                                , T3 p3
                                                                , System.Action<Exception> fnExceptionHandler = null
                                                                )
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            try { ret = fn(p1, p2, p3); } catch (Exception ex) { fnExceptionHandler?.Invoke(ex); }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}