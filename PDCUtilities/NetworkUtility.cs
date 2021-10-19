using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;

namespace PDCUtility
{
    public class NetworkUtility
    {
        public static string CurrentIPAddress(System.Net.Sockets.AddressFamily? eAddressFamily = null)
        {
            string strRet = string.Empty;

            try
            {
                string strHostName = Dns.GetHostName(); // Retrive the Name of HOST  
                IPHostEntry oIPHostEntry = Dns.GetHostEntry(strHostName);

                for (int i = 0; ((string.IsNullOrEmpty(strRet)) && (i < oIPHostEntry.AddressList.Length)); i++)
                {
                    IPAddress oIPAddress = oIPHostEntry.AddressList[i];

                    string str = oIPAddress.ToString();

                    if ((!eAddressFamily.HasValue) || (eAddressFamily.Value == oIPAddress.AddressFamily))
                        strRet = str;
                }
            }
            catch
            {
            }
            return strRet;
        }
    }

    public class NetworkShareMapper
    {
        public delegate void ErrorLogger(string strMessage);

        public static void MapNetworkDrive(string strDriveLetter,
                                            string strUNCPath,
                                            string strUsername,
                                            string strPassword,
                                            ErrorLogger fnErrorLogger = null
            )
        {
            NetworkDrive oND = new NetworkDrive();
            try
            {
                oND.LocalDrive = strDriveLetter;
                oND.ShareName = strUNCPath;
                oND.MapDrive(strUsername, strPassword);
            }
            catch (Exception ex)
            {
                if (null != fnErrorLogger)
                    fnErrorLogger(string.Format("MAPPING NETWORK DRIVE ERROR: {0}", ex.Message));
            }
            oND = null;
        }

        private class NetworkDrive
        {
            #region API

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "1")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "2")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("mpr.dll")]
            private static extern int WNetAddConnection2A(ref structNetResource pstNetRes, string psPassword, string psUsername, int piFlags);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "0")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("mpr.dll")]
            private static extern int WNetCancelConnection2A(string psName, int piFlags, int pfForce);

            //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "0")]
            //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            //[DllImport("mpr.dll")]
            //private static extern int WNetConnectionDialog(int phWnd, int piType);

            //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "0")]
            //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            //[DllImport("mpr.dll")]
            //private static extern int WNetDisconnectDialog(int phWnd, int piType);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "1")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "0")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("mpr.dll")]
            private static extern int WNetRestoreConnectionW(int phWnd, string psLocalDrive);

            [StructLayout(LayoutKind.Sequential)]
            private struct structNetResource
            {
                public int iScope;
                public int iType;
                public int iDisplayType;
                public int iUsage;
                public string sLocalName;
                public string sRemoteName;
                public string sComment;
                public string sProvider;
            }

            private const int RESOURCETYPE_DISK = 0x1;

            //Standard
            private const int CONNECT_INTERACTIVE = 0x00000008;

            private const int CONNECT_PROMPT = 0x00000010;
            private const int CONNECT_UPDATE_PROFILE = 0x00000001;

            //IE4+
            private const int CONNECT_REDIRECT = 0x00000080;

            //NT5 only
            private const int CONNECT_COMMANDLINE = 0x00000800;

            private const int CONNECT_CMD_SAVECRED = 0x00001000;

            #endregion API

            #region Propertys and options

            private bool lf_SaveCredentials = false;

            /// <summary>
            /// Option to save credentials are reconnection...
            /// </summary>
            public bool SaveCredentials
            {
                get { return (lf_SaveCredentials); }
                set { lf_SaveCredentials = value; }
            }

            private bool lf_Persistent = false;

            /// <summary>
            /// Option to reconnect drive after log off / reboot ...
            /// </summary>
            public bool Persistent
            {
                get { return (lf_Persistent); }
                set { lf_Persistent = value; }
            }

            private bool lf_Force = false;

            /// <summary>
            /// Option to force connection if drive is already mapped...
            /// or force disconnection if network path is not responding...
            /// </summary>
            public bool Force
            {
                get { return (lf_Force); }
                set { lf_Force = value; }
            }

            private bool ls_PromptForCredentials = false;

            /// <summary>
            /// Option to prompt for user credintals when mapping A drive
            /// </summary>
            public bool PromptForCredentials
            {
                get { return (ls_PromptForCredentials); }
                set { ls_PromptForCredentials = value; }
            }

            private string ls_Drive = "d:";

            /// <summary>
            /// Drive to be used in mapping / unmapping...
            /// </summary>
            public string LocalDrive
            {
                get { return (ls_Drive); }
                set
                {
                    if (value.Length >= 1)
                    {
                        ls_Drive = value.Substring(0, 1) + ":";
                    }
                    else
                    {
                        ls_Drive = "";
                    }
                }
            }

            private string ls_ShareName = "\\\\Computer\\C$";

            /// <summary>
            /// Share address to map drive to.
            /// </summary>
            public string ShareName
            {
                get { return (ls_ShareName); }
                set { ls_ShareName = value; }
            }

            #endregion Propertys and options

            #region Function mapping

            /// <summary>
            /// Map network drive
            /// </summary>
            public void MapDrive()
            {
                __MapDrive(null, null);
            }

            /// <summary>
            /// Map network drive (using supplied Password)
            /// </summary>
            public void MapDrive(string Password)
            {
                __MapDrive(null, Password);
            }

            /// <summary>
            /// Map network drive (using supplied Username and Password)
            /// </summary>
            public void MapDrive(string Username, string Password)
            {
                __MapDrive(Username, Password);
            }

            /// <summary>
            /// Unmap network drive
            /// </summary>
            public void UnMapDrive()
            {
                __UnMapDrive(this.lf_Force);
            }

            /// <summary>
            /// Check / restore persistent network drive
            /// </summary>
            public void RestoreDrives()
            {
                __RestoreDrive();
            }

            #endregion Function mapping

            #region Core functions

            // Map network drive
            private void __MapDrive(string psUsername, string psPassword)
            {
                //create struct data
                structNetResource stNetRes = new structNetResource();
                stNetRes.iScope = 2;
                stNetRes.iType = RESOURCETYPE_DISK;
                stNetRes.iDisplayType = 3;
                stNetRes.iUsage = 1;
                stNetRes.sRemoteName = ls_ShareName;
                stNetRes.sLocalName = ls_Drive;
                //prepare params
                int iFlags = 0;
                if (lf_SaveCredentials) { iFlags += CONNECT_CMD_SAVECRED; }
                if (lf_Persistent) { iFlags += CONNECT_UPDATE_PROFILE; }
                if (ls_PromptForCredentials) { iFlags += CONNECT_INTERACTIVE + CONNECT_PROMPT; }
                if (psUsername == "") { psUsername = null; }
                if (psPassword == "") { psPassword = null; }
                //if force, unmap ready for new connection
                if (lf_Force) { try { __UnMapDrive(true); } catch { } }
                //call and return
                int i = WNetAddConnection2A(ref stNetRes, psPassword, psUsername, iFlags);
                if (i > 0) { throw new System.ComponentModel.Win32Exception(i); }
            }

            // Unmap network drive
            private void __UnMapDrive(bool pfForce)
            {
                //call unmap and return
                int iFlags = 0;
                if (lf_Persistent) { iFlags += CONNECT_UPDATE_PROFILE; }
                int i = WNetCancelConnection2A(ls_Drive, iFlags, Convert.ToInt32(pfForce));
                if (i != 0) i = WNetCancelConnection2A(ls_ShareName, iFlags, Convert.ToInt32(pfForce));  //disconnect if localname was null
                if (i > 0) { throw new System.ComponentModel.Win32Exception(i); }
            }

            // Check / Restore A network drive
            private void __RestoreDrive()
            {
                //call restore and return
                int i = WNetRestoreConnectionW(0, null);
                if (i > 0) { throw new System.ComponentModel.Win32Exception(i); }
            }

            #endregion Core functions
        }
    }
}