using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace PDCUtility
{
    public class ServiceHelper
    {
        public delegate void ErrorHandlerDelegate(string strMessage);

        public static bool StartService(string strServiceName
                                        , bool bWaitForStartComplete = true
                                        , ErrorHandlerDelegate fnErrorHandlerDelegate = null
                                        )
        {
            try
            {
                ServiceController[] aServices = ServiceController.GetServices();

                bool bServiceFound = false;

                foreach (ServiceController oSC in aServices)

                    if (oSC.ServiceName == strServiceName)
                    {
                        bServiceFound = true;

                        if (oSC.Status == ServiceControllerStatus.Stopped)
                        {
                            oSC.Start();

                            if (bWaitForStartComplete)
                                while (oSC.Status != ServiceControllerStatus.Running)
                                {
                                    System.Threading.Thread.Sleep(1000);
                                    oSC.Refresh();
                                }

                            return true;
                        }
                    }

                if (bServiceFound)
                    fnErrorHandlerDelegate?.Invoke(string.Format("Service [{0}] is not stopped - cannot start", strServiceName));
                else
                    fnErrorHandlerDelegate?.Invoke(string.Format("Service [{0}] does not exist", strServiceName));
            }
            catch (Exception ex)
            {
                if (null != fnErrorHandlerDelegate)
                    try
                    {
                        fnErrorHandlerDelegate(ex.Message);
                    }
                    catch { }
            }

            return false;
        }

        public static bool StopService(string strServiceName
                                        , bool bWaitForStopComplete = true
                                        , ErrorHandlerDelegate fnErrorHandlerDelegate = null
                                        )
        {
            try
            {
                ServiceController[] aServices = ServiceController.GetServices();

                bool bServiceFound = false;

                foreach (ServiceController oSC in aServices)

                    if (oSC.ServiceName == strServiceName)
                    {
                        bServiceFound = true;

                        if (oSC.Status == ServiceControllerStatus.Running)
                        {
                            oSC.Stop();

                            if (bWaitForStopComplete)
                                while (oSC.Status != ServiceControllerStatus.Stopped)
                                {
                                    System.Threading.Thread.Sleep(1000);
                                    oSC.Refresh();
                                }

                            return true;
                        }
                    }

                if (bServiceFound)
                    fnErrorHandlerDelegate?.Invoke(string.Format("Service [{0}] is not running - cannot stop", strServiceName));
                else
                    fnErrorHandlerDelegate?.Invoke(string.Format("Service [{0}] does not exist", strServiceName));
            }
            catch (Exception ex)
            {
                if (null != fnErrorHandlerDelegate)
                    try
                    {
                        fnErrorHandlerDelegate(ex.Message);
                    }
                    catch { }
            }

            return false;
        }
    }
}