//#define SHOW_MESSAGEBOX_POPUPS

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

#if SHOW_MESSAGEBOX_POPUPS
using System.Windows.Forms;
#endif

namespace PDCUtility
{
    public class ExecutionHelper
    {
        public static System.TimeSpan TimeAction(System.Action action)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static string MODULE_NAME = typeof(ExecutionHelper).ToString();

        private bool m_bRunning = false;

        private int m_iRetCode = 0;

        private Process m_oProcess = null;

#pragma warning disable IDE0044 // Add readonly modifier

        private string m_strExecError = "";

        private string m_strExecOutput = "";

#pragma warning restore IDE0044 // Add readonly modifier

        private const string TwoBackslashes = "\\\\";

        public ExecutionHelper(string strExecutable,
                                string strCommandLineArgs,
                                string strWorkingDir = "",
                                ProcessWindowStyle eProcessWindowStyle = ProcessWindowStyle.Normal,
                                bool bUseShellExecute = false,
                                bool bCatchOutput = true,
                                bool bLoaduserProfile = false,
                                bool bWaitForResult = true)
        {
            // if the exectuable starts with two backslashes, it is a fully UNC pathed name, so just check if it exists
            if (strExecutable.StartsWith(TwoBackslashes))
            {
                try
                {
                    if (!System.IO.File.Exists(strExecutable))
                        throw new ApplicationException("Executable '" + strExecutable + "' does not exist. (E1)", new Exception(MODULE_NAME));

                    FileInfo fi = new FileInfo(strExecutable);

                    if (string.IsNullOrEmpty(strWorkingDir))
                        strWorkingDir = fi.DirectoryName;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Executable '" + strExecutable + "' does not exist. (E2)", new Exception(MODULE_NAME, ex));
                }
            }
            else
            {
                // the executable DOES NOT start with two backslashes... so we assume it must be in the same dir as the executable (client)
                try
                {
                    FileInfo fi = new FileInfo(strExecutable);
                    strExecutable = fi.DirectoryName + "\\" + fi.Name;

                    if (string.IsNullOrEmpty(strWorkingDir))
                        strWorkingDir = fi.DirectoryName;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Executable '" + strExecutable + "' does not exist. (E3)", new Exception(MODULE_NAME, ex));
                }

                if (!File.Exists(strExecutable))
                    throw new ApplicationException("Executable '" + strExecutable + "' does not exist. (E4)", new Exception(MODULE_NAME));
            }

            // create a processinfo structure with which to start the Ready Machine Interface
            ProcessStartInfo oPSI = null;

            if (string.IsNullOrEmpty(strCommandLineArgs))
                oPSI = new ProcessStartInfo(strExecutable);
            else
                oPSI = new ProcessStartInfo(strExecutable, strCommandLineArgs);

            if (!string.IsNullOrEmpty(strWorkingDir))
                oPSI.WorkingDirectory = strWorkingDir;

            oPSI.WindowStyle = eProcessWindowStyle;

            if (ProcessWindowStyle.Hidden == eProcessWindowStyle)
                oPSI.CreateNoWindow = true;

            oPSI.UseShellExecute = bUseShellExecute;

            if ((!bUseShellExecute) && (bCatchOutput))
            {
                oPSI.RedirectStandardError = true;
                oPSI.RedirectStandardOutput = true;
            }

            oPSI.LoadUserProfile = bLoaduserProfile;

#if SHOW_MESSAGEBOX_POPUPS
            MessageBox.Show(string.Format("FileName=[{0}]\nArguments=[{1}]\nWorkingDir=[{2}]", oPSI.FileName, oPSI.Arguments, oPSI.WorkingDirectory));

            MessageBox.Show("About to execute:" + "\n" +
                            "\tEXEC: " + strExecutable + "\n" +
                            "\tARGS: " + strCommandLineArgs);
#endif

            // start the exec - and wait for its exit
            m_oProcess = Process.Start(oPSI);
            m_bRunning = true;

            if ((!bUseShellExecute) && (bCatchOutput))
            {
                m_strExecError = m_oProcess.StandardError.ReadToEnd();
                m_strExecOutput = m_oProcess.StandardOutput.ReadToEnd();
            }

            if (bWaitForResult)
            {
                m_oProcess.WaitForExit();
                m_bRunning = false;
                m_iRetCode = m_oProcess.ExitCode;
            }
            else
            {
                m_oProcess.Exited += HandleProcessExited;
                m_iRetCode = 0;
            }
        }

        public string ExecutionStandardError { get { return m_strExecError; } }

        public string ExecutionStandardOutput { get { return m_strExecOutput; } }

        public int ExitCode { get { return m_iRetCode; } }

        public bool Running { get { return m_bRunning; } }

        public static void RunHiddenExecutable(string strExecutable,
                                                string strCommandLineArgs,
                                                string strWorkingDir = "",
                                                System.Action<string> fnStdOutLogger = null)
        {
            // if the exectuabel starts with two backslashes, it is a fully UNC pathed name, so just check if it exists
            if (strExecutable.StartsWith(TwoBackslashes))
            {
                try
                {
                    if (!System.IO.File.Exists(strExecutable))
                        throw new ApplicationException("Executable '" + strExecutable + "' does not exist. (E1)", new Exception(MODULE_NAME));

                    FileInfo fi = new FileInfo(strExecutable);

                    if (string.IsNullOrEmpty(strWorkingDir))
                        strWorkingDir = fi.DirectoryName;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Executable '" + strExecutable + "' does not exist. (E2)", new Exception(MODULE_NAME, ex));
                }
            }
            else
            {
                // the executable DOES NOT start with two backslashes... so we assume it must be in the same dir as the executable (client)
                try
                {
                    FileInfo fi = new FileInfo(strExecutable);
                    strExecutable = fi.DirectoryName + "\\" + fi.Name;

                    if (string.IsNullOrEmpty(strWorkingDir))
                        strWorkingDir = fi.DirectoryName;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Executable '" + strExecutable + "' does not exist. (E3)", new Exception(MODULE_NAME, ex));
                }

                if (!File.Exists(strExecutable))
                    throw new ApplicationException("Executable '" + strExecutable + "' does not exist. (E4)", new Exception(MODULE_NAME));
            }

            ProcessStartInfo starter = new ProcessStartInfo(strExecutable, strCommandLineArgs)
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            if (!string.IsNullOrEmpty(strWorkingDir))
                starter.WorkingDirectory = strWorkingDir;

            Process process = new Process()
            {
                StartInfo = starter
            };
            process.Start();

            StringBuilder sbStdOut = new StringBuilder();

            using (StreamReader srStdOut = process.StandardOutput)
            {
                string strStdOutLine = srStdOut.ReadLine();

                while (null != strStdOutLine)
                {
                    fnStdOutLogger?.Invoke(strStdOutLine);

                    sbStdOut.Append(strStdOutLine);
                    sbStdOut.Append(Environment.NewLine);

                    strStdOutLine = srStdOut.ReadLine();
                    System.Threading.Thread.Sleep(10);
                }
            }

            if (process.ExitCode != 0)
            {
                throw new Exception(string.Format(@"""{0}"" exited with ExitCode {1}.\nOutput: {2}",
                                                    strExecutable,
                                                    process.ExitCode,
                                                    sbStdOut.ToString()));
            }
        }

        public void Stop()
        {
            try
            {
                m_oProcess.Kill();
            }
            catch { }
        }

        private void HandleProcessExited(object sender, EventArgs e)
        {
            m_iRetCode = m_oProcess.ExitCode;
            m_bRunning = false;
        }
    }
}