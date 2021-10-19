using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDCUtility
{
    public class StackTraceHelper
    {
        private const string FullStackTraceFormat = "{2} ({0} Line:{1})";
        private const string BriefStackTraceFormat = "{0}";

        public static string WhoCalledMe(bool bDetailedStack = false)
        {
            string str = string.Empty;

            System.Diagnostics.StackTrace oStackTrace = new System.Diagnostics.StackTrace(true);
            if (null != oStackTrace)
            {
                bool bFound = false;
                bool bDone = false;

                System.Diagnostics.StackFrame[] oStackFrames = oStackTrace.GetFrames();

                for (int i = 0; ((!bDone) && (i < oStackFrames.Length)); i++)
                {
                    System.Diagnostics.StackFrame oStackFrame = oStackFrames[i];

                    if (null != oStackFrame)
                    {
                        System.Reflection.MethodBase oMethod = oStackFrame.GetMethod();

                        if (null != oMethod)
                            if (bFound)
                            {
                                if (bDetailedStack)
                                    str = string.Format(FullStackTraceFormat,
                                                                oStackFrame.GetFileName(),
                                                                oStackFrame.GetFileLineNumber(),
                                                                oMethod.Name);
                                else
                                    str = string.Format(BriefStackTraceFormat,
                                                            oMethod.Name);

                                bDone = true;
                            }
                            else if ("WhoCalledMe" == oMethod.Name)
                            {
                                bFound = true; i++;
                            }
                    }
                }
            }

            return str;
        }

        public static string FullStack(bool bDetailedStack = false, string strLinePrepend = "")
        {
            string strRet = string.Empty;

            System.Diagnostics.StackTrace oStackTrace = new System.Diagnostics.StackTrace(true);
            if (null != oStackTrace)
            {
                bool bFound = false;
                bool bDone = false;

                System.Diagnostics.StackFrame[] oStackFrames = oStackTrace.GetFrames();

                for (int i = 0; ((!bDone) && (i < oStackFrames.Length)); i++)
                {
                    System.Diagnostics.StackFrame oStackFrame = oStackFrames[i];

                    if (null != oStackFrame)
                    {
                        System.Reflection.MethodBase oMethod = oStackFrame.GetMethod();

                        if (null != oMethod)
                            if (bFound)
                            {
                                string str = string.Empty;

                                if (bDetailedStack)
                                    str = string.Format(FullStackTraceFormat,
                                                                oStackFrame.GetFileName(),
                                                                oStackFrame.GetFileLineNumber(),
                                                                oMethod.Name);
                                else
                                    str = string.Format(BriefStackTraceFormat,
                                                            oMethod.Name);

                                //bDone = true;
                                strRet += string.Format("{0}{2}{1}",
                                                        (string.IsNullOrEmpty(strRet) ? string.Empty : "\r\n"),
                                                        str, strLinePrepend);
                            }
                            else if ("FullStack" == oMethod.Name)
                            {
                                bFound = true; i++;
                            }
                    }
                }
            }

            return strRet;
        }

        public static string FileLocationLogInfo(int iFramesBackwardStart, string strStopAtMethod, bool bDetailedStack = false)
        {
            string str = string.Empty;

            System.Diagnostics.StackTrace oStackTrace = new System.Diagnostics.StackTrace(true);
            if (null != oStackTrace)
            {
                System.Diagnostics.StackFrame[] oStackFrames = oStackTrace.GetFrames();

                if ((iFramesBackwardStart - 1) < oStackFrames.Length)
                {
                    int iEnd = oStackFrames.Length;

                    for (int i = iFramesBackwardStart; i < iEnd; i++)
                    {
                        System.Diagnostics.StackFrame oStackFrame = oStackFrames[i];
                        if (null != oStackFrame)
                        {
                            System.Reflection.MethodBase oMethod = oStackFrame.GetMethod();

                            if (null != oMethod)
                            {
                                if (bDetailedStack)
                                    str += ((0 < str.Length) ? "  <--  " : "") +
                                                string.Format(FullStackTraceFormat,
                                                                oStackFrame.GetFileName(),
                                                                oStackFrame.GetFileLineNumber(),
                                                                oMethod.Name);
                                else
                                    str += ((0 < str.Length) ? "  <--  " : "") +
                                            string.Format(BriefStackTraceFormat,
                                                            oMethod.Name);

                                if (strStopAtMethod == oMethod.Name)
                                    i = iEnd;
                            }
                        }
                    }
                }
            }
            return str;
        }

        /*
        public static string FileLocationLogInfo(int iFramesBackward = 0, bool bDetailedStack = false)
        {
            string strValue = string.Empty;

            System.Diagnostics.StackTrace oStackTrace = new System.Diagnostics.StackTrace(true);
            if (null != oStackTrace)
            {
                System.Diagnostics.StackFrame[] oStackFrames = oStackTrace.GetFrames();

                if ((iFramesBackward - 1) < oStackFrames.Length)
                {
                    System.Diagnostics.StackFrame oStackFrame = oStackTrace.GetFrame(iFramesBackward + 1);
                    if (null != oStackFrame)
                    {
                        System.Reflection.MethodBase oMethod = oStackFrame.GetMethod();

                        if (null != oMethod)
                            if (bDetailedStack)
                                strValue = string.Format(FullStackTraceFormat,
                                                    oStackFrame.GetFileName(),
                                                    oStackFrame.GetFileLineNumber(),
                                                    oMethod.Name);
                            else
                                strValue = string.Format(BriefStackTraceFormat,
                oMethod.Name);
                    }
                }
            }

            return strValue;
        }
        */

    }
}
