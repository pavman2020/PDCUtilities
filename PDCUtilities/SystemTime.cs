using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PDCUtility
{
    public class SystemTime
    {
        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool SetSystemTime(ref SYSTEMTIME st);

            [StructLayout(LayoutKind.Sequential)]
            public struct SYSTEMTIME
            {
                public short wYear;
                public short wMonth;
                public short wDayOfWeek;
                public short wDay;
                public short wHour;
                public short wMinute;
                public short wSecond;
                public short wMilliseconds;
            }
        }

        public static void SetSystemTime(DateTime dtInput)
        {
            {
                DateTime dt = dtInput.ToUniversalTime();

                NativeMethods.SYSTEMTIME st = new NativeMethods.SYSTEMTIME();
                st.wYear = Convert.ToInt16(dt.Year);
                st.wMonth = Convert.ToInt16(dt.Month);
                st.wDay = Convert.ToInt16(dt.Day);
                st.wHour = Convert.ToInt16(dt.Hour);
                st.wMinute = Convert.ToInt16(dt.Minute);
                st.wSecond = Convert.ToInt16(dt.Second);

                NativeMethods.SetSystemTime(ref st); // invoke this method.
            }
        }
    }
}
