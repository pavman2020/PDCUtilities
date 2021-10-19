using System;
using System.Collections.Generic;
using System.IO;

namespace PDCUtility
{
    public class DirectoryHelper
    {

        public static string PathForFilename(string strFilename)
        {
            try
            {
                return System.IO.Path.GetDirectoryName(strFilename);
            }
            catch { return strFilename; }

            //try
            //{
            //    int i = strFilename.LastIndexOf("\\");
            //    return strFilename.Substring(0, i);
            //}
            //catch
            //{
            //    return strFilename;
            //}
        }

        public static void EnsureDirExists(string strDir)
        {
            try
            {
                if (Directory.Exists(strDir))
                    return;

                int i = strDir.LastIndexOf("\\");
                string strParent = strDir.Substring(0, i);

                EnsureDirExists(strParent);

                Directory.CreateDirectory(strDir);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to ensure [{0}] exists", strDir), ex);
            }
        }

        public static string EnsureValidFilenameCharacters(string strFileName, List<Tuple<string, string>> lTuples)
        {
            string strRet = strFileName;

            foreach (Tuple<string, string> Tuple in lTuples)
                try
                {
                    strRet = strRet.Replace(Tuple.Item1, Tuple.Item2);
                }
                catch { }

            return EnsureValidFilenameCharacters(strRet);
        }

        public static string EnsureValidFilenameCharacters(string strFileName)
        {
            char cReplacementChar = '_';

            char[] acBadChars = { '\\', '/', ':', '*', '"', '<', '>', '|' };

            string strRet = strFileName;

            foreach (char c in acBadChars)
                strRet = strRet.Replace(c, cReplacementChar);

            return strRet;
        }
    }
}
