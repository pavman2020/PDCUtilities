using System.Collections.Generic;

namespace PDCUtility
{
    public class StringHelper
    {
        public static string CatenateStrings(string[] astr, string strCat)
        {
            string strRet = string.Empty;

            try
            {
                foreach (string s in astr)
                    strRet += string.Format("{0}{1}", s.Replace(strCat, " "), strCat);
            }
            catch { }

            return strRet;
        }

        public static string CatenateStrings(List<string> lstr, string strCat)
        {
            return CatenateStrings(lstr.ToArray(), strCat);
        }

        public static List<string> Chunkify(string str, int iNumChars)
        {
            List<string> lRet = new List<string>();

            try
            {
                for (int i = 0; i < str.Length;)
                {
                    string s = string.Empty;
                    for (int j = 0; j < iNumChars; j++)
                    {
                        s += str[i++];
                        if (i == str.Length)
                            break;
                    }
                    lRet.Add(s);
                }
            }
            catch { }

            return lRet;
        }

        public static string ClipString(string strInput, int iLen)
        {
            string str = IfNull(strInput);

            if (str.Length <= iLen)
                return str;

            return str.Substring(0, iLen);
        }

        public static string ExtractLeadingDigits(string strInput)
        {
            string strRet = string.Empty;

            char[] cInput = strInput.ToCharArray();

            for (int i = 0; i < cInput.Length; i++)
            {
                if (('1' <= cInput[i]) && (cInput[i] <= '9'))
                {
                    strRet = strInput.Substring(i);
                    return strRet;
                }
            }

            return strRet;
        }

        public static string IfNull(string str, string strReplacement = null)
        {
            if (null == str)
                return (null == strReplacement) ? string.Empty : strReplacement;
            else
                return str;
        }

        public static bool MatchesPattern(string strTestMe, string strPattern, char cSingleWildChar = '.', bool bIgnoreLengthMismatch = false)
        {
            if (!bIgnoreLengthMismatch)
                if (strTestMe.Length != strPattern.Length)
                    return false;

            if (strPattern.Length > strTestMe.Length)
                return false;

            char[] caTestMe = strTestMe.ToCharArray();
            char[] caPattern = strPattern.ToCharArray();

            int iMax = strPattern.Length;

            for (int i = 0; i < iMax; i++)
            {
                if (caPattern[i] != cSingleWildChar)
                    if (caPattern[i] != caTestMe[i])
                        return false;
            }

            return true;
        }

        public static string PadLeft(string str, int iWidth)
        {
            return IfNull(str).PadLeft(iWidth);
        }

        public static string PadRight(string str, int iWidth)
        {
            return IfNull(str).PadRight(iWidth);
        }

        public static string RemoveLeadingChars(string strText, string strChar)
        {
            string str = IfNull(strText);

            while (str.StartsWith(strChar))
                str = str.Substring(strChar.Length);

            return str;
        }

        public static string Repeat(string str, int iNumTimes)
        {
            string strRet = string.Empty;
            for (int i = 0; i < iNumTimes; i++)
                strRet += str;
            return strRet;
        }

        public static string Repeat(char c, int iNum)
        {
            string strRet = string.Empty;

            for (int i = 0; i < iNum; i++) strRet += c;

            return strRet;
        }

        public static string ReplaceChar(string strText, int iOneBasedCharPos, char newChar, char OldChar)
        {
            string str = IfNull(strText);

            if (iOneBasedCharPos < str.Length)
            {
                char[] c = str.ToCharArray();

                if (OldChar == c[iOneBasedCharPos])
                    c[iOneBasedCharPos] = newChar;

                str = new string(c);
            }

            return str;
        }

        public static int? StringArrayMatchedIndex(string[,] strArray, int iMatchCol, string strMatchText, int? iStartChar = null, int? iEndChar = null)
        {
            if (iMatchCol < strArray.GetLength(1))
            {
                for (int i = 0; i < strArray.GetLength(0); i++)
                {
                    string strToMatch = strArray[i, iMatchCol];

                    if (null != iStartChar)
                        if (iStartChar.HasValue)
                            if (strToMatch.Length < iStartChar.Value)
                                strToMatch = string.Empty;
                            else
                                strToMatch = strToMatch.Substring(iStartChar.Value);

                    if (null != iEndChar)
                        if (iEndChar.HasValue)
                            if (strToMatch.Length >= iEndChar.Value)
                                strToMatch = strToMatch.Substring(0, iEndChar.Value);

                    if (strToMatch == strMatchText)
                        return i;
                }
            }

            return null;
        }

        public static string Substring1Based(string str, int iStart, int? iLength = null)
        {
            if ((iStart > str.Length) || (1 > iStart))
                return string.Empty;

            string strTemp = str.Substring(iStart - 1);

            if (iLength.HasValue)
                if (iLength.Value < strTemp.Length)
                    return strTemp.Substring(0, iLength.Value);
                else
                    return strTemp;
            else
                return strTemp;
        }

        public static string[] TrimStrings(string[] astr)
        {
            try
            {
                for (int i = 0; i < astr.Length; i++)
                    if (string.IsNullOrEmpty(astr[i]))
                        astr[i] = string.Empty;
                    else
                        astr[i] = astr[i].Trim();
            }
            catch { }
            return astr;
        }

        public static List<string> TrimStrings(List<string> astr)
        {
            try
            {
                for (int i = 0; i < astr.Count; i++)
                    astr[i] = astr[i].Trim();
            }
            catch { }
            return astr;
        }
    }
}