using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDCUtility
{
	public class Comparers
	{
		public static bool Equal(double? nd1, double? nd2, double? ndAbsoluteTolerance)
		{
			try
			{
				if (nd1.HasValue == nd2.HasValue)
				{
					if (!nd1.HasValue)
						return true;

					if (ndAbsoluteTolerance.HasValue)
					{
						double? ndDiff = PDCUtility.Math.Abs(nd1 - nd2);
						return (ndAbsoluteTolerance > ndDiff);
					}
					else
						return (nd1.Value == nd2.Value);
				}
			}
			catch { }
			return false;
		}
	}
}
