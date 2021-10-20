using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDCUtility
{
    public static class TypeConverters
    {
        public static double? GetDoubleFromFractionString(this string str)
        {
            try
            {
                str = str.Trim();
                if (0 == str.Length)
                    return null;

                if (str.Contains("/"))
                {
                    int i = str.IndexOf('/');
                    string strNumerator = str.Substring(0, i);
                    string strDenominator = str.Substring(i + 1);

                    double dNumerator = System.Convert.ToDouble(strNumerator);
                    double dDenominator = System.Convert.ToDouble(strDenominator);

                    return dNumerator / dDenominator;
                }
                else
                {
                    return System.Convert.ToDouble(str);
                }
            }
            catch { return null; }
        }

        public static string GetFractionStringFromDouble(this double d)
        {
            return ((double?)d).GetFractionStringFromDouble();
        }

        public static string GetFractionStringFromDouble(this double? d)
        {
            double[] dDenominators = { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 
                                        3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 
                                        101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 
                                        211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293,
                                        307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397,
                                        401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499,
                                        503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599,
                                        601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691,
                                        701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797,
                                        809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887,
                                        907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997,
            };
            if (d.HasValue)
            {
                string str = d.ToString();
                int iDecimalPoint = str.IndexOf('.');

                if (iDecimalPoint >= 0)
                {
                    string strWhole = str.Substring(0, iDecimalPoint);
                    string strDecimal = str.Substring(iDecimalPoint + 1);
                    double dDecimal = Convert.ToDouble("0." + strDecimal);

                    foreach (double dDenominator in dDenominators)
                    {
                        double dMultiplied = dDecimal * dDenominator;

                        string strMulitplied = dMultiplied.ToString();

                        if (0 > strMulitplied.IndexOf('.'))
                            return (("0" == strWhole) ? "" : strWhole + " ") + strMulitplied + "/" + dDenominator.ToString();
                    }
                }

                return d.ToString();
            }
            else
                return string.Empty;
        }

#if false
        public static double ToDouble(string str)
        {
            if (0 < str.Trim().Length)
                if (str.Contains("/"))  // handle fractions!
                {
                    int i = str.IndexOf("/");
                    string s1 = str.Substring(0, i);
                    string s2 = str.Substring(i + 1);

                    double d1 = ToDouble(s1);
                    double d2 = ToDouble(s2);

                    return d1 / d2;
                }
                else
                    try
                    {
                        double d = Convert.ToDouble(str);
                        return d;
                    }
                    catch
                    {
                    }

            return 0;
        }

        public static short ToShort(string str)
        {
            try
            {
                return Convert.ToInt16(str);
            }
            catch { }
            return 0;
        }
#endif
    }
}