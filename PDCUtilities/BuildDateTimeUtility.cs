using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace PDCUtility
{
    public class BuildDateTimeUtility
    {
#pragma warning disable 649

        struct _IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalheader;
            public ushort Characteristics;
        }

#pragma warning restore 649

        public static string GetVersionString(Assembly assembly)
        {
            string strRet = string.Empty;

            if (null != assembly)
            {
                string strFN = assembly.FullName;

                if (strFN.Contains("Version="))
                {
                    string strLeft = strFN.Substring(strFN.IndexOf("Version="));

                    if (strLeft.Contains(","))
                        strRet = strLeft.Substring(0, strLeft.IndexOf(","));
                }
            }

            return strRet;
        }

        public static DateTime GetBuildDateTime(Assembly assembly)
        {
            DateTime dtRet = new DateTime();

            if (File.Exists(assembly.Location))
            {
                var buffer = new byte[Math.Max(Marshal.SizeOf(typeof(_IMAGE_FILE_HEADER)), 4)];

                using (var filestream = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read))
                {
                    filestream.Position = 0x3C;
                    filestream.Read(buffer, 0, 4);
                    filestream.Position = BitConverter.ToUInt32(buffer, 0);
                    filestream.Read(buffer, 0, 4); // "PE\0\0"
                    filestream.Read(buffer, 0, buffer.Length);
                }

                var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                try
                {
                    var coffHeader = (_IMAGE_FILE_HEADER)Marshal.PtrToStructure(pinnedBuffer.AddrOfPinnedObject(), typeof(_IMAGE_FILE_HEADER));

                    return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1) + new TimeSpan(coffHeader.TimeDateStamp * TimeSpan.TicksPerSecond));
                }
                finally { pinnedBuffer.Free(); }
            }


            return dtRet;
        }


    }
}
