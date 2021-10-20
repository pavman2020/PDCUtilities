using System;
using static PDCUtility.TypeConverters;
using static PDCUtility.FractionConverter;

namespace PDCUtilitiesTester
{
    class Program
    {
        static void Main(string[] args)
        {

            foreach (double d in new double[] { 123.375, 99.5, 99.51, 12.625, 673.432, 66.231, 0.998, 3.14159 })
            {
                Console.WriteLine(string.Format("{0} = {1} = '{2}' = '{3}'", nameof(d), d, d.GetFractionStringFromDouble(), ((decimal)d).ConvertDecimalToFraction(true)));
            }


            Console.WriteLine("Hello World!");
        }
    }
}
