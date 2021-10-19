using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace PDCUtilities
{
    public class ClassHelper
    {
        public delegate void FindClassHandlerDelegate(System.Type oClassType);

        public delegate void FindMethodHandlerDelegate(System.Type oClassType,
                                                System.Type oAttributeType,
                                                object oAttribute,
                                                System.Reflection.MethodInfo oMethod);

        public static void FindClassesWithAttributes(Assembly oAss,
                                                    Type oAttributeType,
                                                    FindClassHandlerDelegate fnHandler
                                                    //BindingFlags? eBindingFlags = BindingFlags.Public | BindingFlags.Static
                                                    )
        {
            try
            {
                foreach (Type t in oAss.GetTypes())
                {
                    foreach (object oAttribute in t.GetCustomAttributes())
                        try
                        {
                            string strType = oAttribute.GetType().ToString();

                            if (strType.ToString() == oAttributeType.ToString())
                                fnHandler(t);
                        }
                        catch { }
                }
            }
            catch { }
        }

        public static void FindMethodsWithAttributes(Type oClassType,
                                                    Type oAttributeType,
                                                    FindMethodHandlerDelegate fnHandler,
                                                    BindingFlags? eBindingFlags = BindingFlags.Public | BindingFlags.Static)
        {
            try
            {
                MethodInfo[] lMI = (eBindingFlags.HasValue) ? oClassType.GetMethods(eBindingFlags.Value) : oClassType.GetMethods();

                foreach (MethodInfo oMI in lMI)
                {
                    //string strName = oMI.Name;

                    object[] oArrayOfCustomAttributes = oMI.GetCustomAttributes(false);

                    if (null != oArrayOfCustomAttributes)
                        if (0 < oArrayOfCustomAttributes.Length)
                            foreach (object oAttribute in oArrayOfCustomAttributes)
                                try
                                {
                                    string strType = oAttribute.GetType().ToString();

                                    if (oAttributeType.ToString() == strType)
                                        fnHandler(oClassType, oAttributeType, oAttribute, oMI);
                                }
                                catch { }
                }
            }
            catch { }
        }

        public static bool IsDerivedFromClass(Type oT, Type oBaseType)
        {
            return IsDerivedFromClass(oT, oBaseType.ToString());
        }

        public static bool IsDerivedFromClass(Type oT, string strBaseType)
        {
            if (oT.ToString().Equals(strBaseType))
                return true;

            if (null != oT.BaseType)
                return IsDerivedFromClass(oT.BaseType, strBaseType);

            return false;
        }

        public static void ObjectShallowCopy(object oFrom, object oTo)
        {
            if ((null == oFrom) || (null == oTo))
                return;

            Type oFromType = oFrom.GetType();
            Type oToType = oTo.GetType();

            if ((null == oFromType) || (null == oToType))
                return;

            MemberInfo[] oFromMIL = oFromType.GetMembers();
            MemberInfo[] oToMIL = oToType.GetMembers();

            if ((null == oFromMIL) || (null == oToMIL))
                return;

            foreach (MemberInfo oToMI in oToMIL)
                if (null != oToMI)
                    switch (oToMI.MemberType)
                    {
                        case MemberTypes.Property:

                            #region "Property"

                            try
                            {
                                PropertyInfo oFromProperty = oFromType.GetProperty(oToMI.Name);

                                if (__CopyProperty(oFromProperty))
                                {
                                    PropertyInfo oToProperty = oToType.GetProperty(oToMI.Name);

                                    if ((oFromProperty.CanRead) && (oToProperty.CanWrite))
                                    {
                                        MethodInfo oFromGetMethod = oFromProperty.GetGetMethod();
                                        MethodInfo oToSetMethod = oToProperty.GetSetMethod();

                                        object[] oParams = { oFromGetMethod.Invoke(oFrom, null) };

                                        oToSetMethod.Invoke(oTo, oParams);
                                    }
                                }
                            }
                            catch { }

                            #endregion "Property"

                            break;

                        case MemberTypes.Field:

                            #region "Field"

                            try
                            {
                                FieldInfo oFromFieldInfo = oFromType.GetField(oToMI.Name, BindingFlags.Instance | BindingFlags.Public);

                                if (__CopyField(oFromFieldInfo))
                                {
                                    FieldInfo oToFieldInfo = oToType.GetField(oToMI.Name, BindingFlags.Instance | BindingFlags.Public);

                                    if (oFromFieldInfo.FieldType.BaseType.ToString().Contains("Array"))
                                    {
                                        string strOFFI = __Get_FriendlyArrayTypeName_From_FieldInfo(oFromFieldInfo);

                                        if (typeof(short?[]).ToString() == strOFFI)
                                        {
                                            short?[] oFromArray = (short?[])oFromFieldInfo.GetValue(oFrom);
                                            short?[] oToArray = (short?[])oToFieldInfo.GetValue(oTo);

                                            for (int i = 0; i < oFromArray.Length; i++)
                                                oToArray[i] = oFromArray[i];
                                        }
                                        else if (typeof(double?[]).ToString() == strOFFI)
                                        {
                                            double?[] oFromArray = (double?[])oFromFieldInfo.GetValue(oFrom);
                                            double?[] oToArray = (double?[])oToFieldInfo.GetValue(oTo);

                                            for (int i = 0; i < oFromArray.Length; i++)
                                                oToArray[i] = oFromArray[i];
                                        }
                                        else if (typeof(string[]).ToString() == strOFFI)
                                        {
                                            string[] oFromArray = (string[])oFromFieldInfo.GetValue(oFrom);
                                            string[] oToArray = (string[])oToFieldInfo.GetValue(oTo);

                                            for (int i = 0; i < oFromArray.Length; i++)
                                                oToArray[i] = oFromArray[i];
                                        }
                                        else
                                            throw new ArgumentException(string.Format("Unhandled case for type {0} for field {1}", strOFFI, oToMI.Name));
                                    }
                                    else
                                        oToFieldInfo.SetValue(oTo, oFromFieldInfo.GetValue(oFrom));
                                }
                            }
                            catch (ArgumentException /* ex */) { throw; }
                            catch { }

                            #endregion "Field"

                            break;
                    }       //  switch (oToMI.MemberType)
        }

        public static void ObjectShallowCopyPropertiesOnly(object oFrom, object oTo)
        {
            if ((null == oFrom) || (null == oTo))
                return;

            Type oFromType = oFrom.GetType();
            Type oToType = oTo.GetType();

            if ((null == oFromType) || (null == oToType))
                return;

            MemberInfo[] oFromMIL = oFromType.GetMembers();
            MemberInfo[] oToMIL = oToType.GetMembers();

            if ((null == oFromMIL) || (null == oToMIL))
                return;

            foreach (MemberInfo oToMI in oToMIL)
                if (null != oToMI)
                    switch (oToMI.MemberType)
                    {
                        case MemberTypes.Property:

                            #region "Property"

                            try
                            {
                                PropertyInfo oFromProperty = oFromType.GetProperty(oToMI.Name);

                                if (__CopyProperty(oFromProperty))
                                {
                                    PropertyInfo oToProperty = oToType.GetProperty(oToMI.Name);

                                    if ((oFromProperty.CanRead) && (oToProperty.CanWrite))
                                    {
                                        MethodInfo oFromGetMethod = oFromProperty.GetGetMethod();
                                        MethodInfo oToSetMethod = oToProperty.GetSetMethod();

                                        object[] oParams = { oFromGetMethod.Invoke(oFrom, null) };

                                        oToSetMethod.Invoke(oTo, oParams);
                                    }
                                }
                            }
                            catch { }

                            #endregion "Property"

                            break;
                    }       //  switch (oToMI.MemberType)
        }

        public static void SetDefaultValues(object oObject)
        {
            PropertyInfo[] olPI = oObject.GetType().GetProperties();

            foreach (PropertyInfo oPI in olPI)
            {
                try
                {
                    object[] o = oPI.GetCustomAttributes(typeof(System.ComponentModel.DefaultValueAttribute), false);
                    if (null != o)
                        if (0 < o.Length)
                        {
                            System.ComponentModel.DefaultValueAttribute oPN = (System.ComponentModel.DefaultValueAttribute)o[0];

                            MethodInfo oMI = oPI.GetSetMethod();

                            if (null != oMI)
                            {
                                object[] oParams = { oPN.Value };
                                oMI.Invoke(oObject, oParams);
                            }
                        }
                }
                catch { }
            }
        }

        private static bool __CopyField(FieldInfo oFI)
        {
            try
            {
                object[] o = oFI.GetCustomAttributes(typeof(NoShallowCopyField), true);

                if (null != o)
                    if (0 < o.Length)
                        return false;
            }
            catch { }
            return true;
        }

        private static bool __CopyProperty(PropertyInfo oPI)
        {
            try
            {
                object[] o = oPI.GetCustomAttributes(typeof(NoShallowCopyProperty), true);

                if (null != o)
                    if (0 < o.Length)
                        return false;
            }
            catch { }
            return true;
        }

        private static string __Get_FriendlyArrayTypeName_From_FieldInfo(FieldInfo oFI)
        {
            string strRet = string.Empty;

            strRet = oFI.FieldType.FullName;
            int iComma = strRet.IndexOf(',');

            if (0 > iComma)
                return strRet;

            int iClose = strRet.IndexOf(']', iComma);
            if (0 > iClose)
                return strRet;

            strRet = strRet.Substring(0, iComma).Trim() + strRet.Substring(iClose);

            int iDoubleBacketOpen = strRet.IndexOf("[[");
            if (0 > iDoubleBacketOpen)
                return strRet;

            int iDoubleBacketClose = strRet.IndexOf("]]", iDoubleBacketOpen);
            if (0 > iDoubleBacketClose)
                return strRet;

            strRet = strRet.Substring(0, iDoubleBacketOpen) + strRet.Substring(iDoubleBacketOpen + 1);

            strRet = strRet.Substring(0, iDoubleBacketClose - 1) + strRet.Substring(iDoubleBacketClose);
            return strRet;
        }

        //private static MemberInfo __Get_MemberInfo_From_MemberInfoList(string strName, MemberInfo[] oFromMIL)
        //{
        //    if (null == strName)
        //        return null;
        //
        //    if (null == oFromMIL)
        //        return null;
        //
        //    foreach (MemberInfo oMI in oFromMIL)
        //        if (null != oMI)
        //            if (strName == oMI.Name)
        //                return oMI;
        //
        //    return null;
        //}

        [System.AttributeUsage(AttributeTargets.Field)]
        public class NoShallowCopyField : System.Attribute
        {
        }

        [System.AttributeUsage(AttributeTargets.Property)]
        public class NoShallowCopyProperty : System.Attribute
        {
        }
    }
}
