using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDCUtility
{
    public class TypeConverters
    {
        public static double? GetDoubleFromFractionString(string str)
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

        public static string GetFractionStringFromDouble(double? d)
        {
            double[] dDenominators = { 2, 4, 8, 16, 32, 64 };
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
    }
}
