using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace PDCUtility
{
    public class ApplicationHelper
    {
#if false

        public delegate void LogAppPropertyDelegate(string str);

        public static void LogApplicationProperties(SettingsPropertyValueCollection oSPVC,
                                                    LogAppPropertyDelegate fnLogger)
        {
            try
            {
                fnLogger("Application settings:");

                Dictionary<string, SettingsPropertyValue> oDic = new Dictionary<string, SettingsPropertyValue>();
                List<string> oList = new List<string>();

                foreach (SettingsPropertyValue oSPV in oSPVC)
                {
                    oList.Add(oSPV.Name);
                    oDic.Add(oSPV.Name, oSPV);
                }

                oList.Sort();

                foreach (string strKey in oList)
                {
                    SettingsPropertyValue oSPV = oDic[strKey];
                    {
                        if (typeof(StringCollection).ToString() == oSPV.Property.PropertyType.ToString())
                        {
                            fnLogger(string.Format("\t{0} ({1}) = ...", oSPV.Name, oSPV.Property.PropertyType.ToString()));

                            StringCollection oSC = (StringCollection)oSPV.PropertyValue;
                            foreach (string str in oSC)
                                fnLogger(string.Format("\t\t[{0}]", str));
                        }
                        else
                            fnLogger(string.Format("\t{0} ({1}) = [{2}]", oSPV.Name, oSPV.Property.PropertyType.ToString(), oSPV.PropertyValue.ToString()));
                    }
                }
            }
            catch { }
        }
#endif

    }
}
