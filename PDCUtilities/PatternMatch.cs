using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PDCUtility
{
    public class PatternMatch
    {
        public static bool IsMatch(string strWhatToFind, string strInWhat)
        {
            bool bRet = false;
            try
            {
                bRet = Regex.IsMatch(strInWhat, strWhatToFind);
            }
            catch { }
            finally { }

            return bRet;
        }
    }
}
