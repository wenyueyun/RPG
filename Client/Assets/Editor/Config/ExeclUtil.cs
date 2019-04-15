using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/************************* 
* 作者： wenyueyun 
* 时间： 2019/3/26 16:16:52 
* 描述： MethodUtil 
*************************/
public class ExeclUtil
{
    public static string GetMethod(string type)
    {
        if (type == "string")
            return "GetString";
        else if (type.ToLower() == "short")
            return "GetInt16";
        else if (type.ToLower() == "ushort")
            return "GetUInt16";
        else if (type.ToLower() == "int")
            return "GetInt32";
        else if (type.ToLower() == "uint")
            return "GetUInt32";
        else if (type.ToLower() == "long")
            return "GetInt64";
        else if (type.ToLower() == "ulong")
            return "GetUInt64";
        else if (type.ToLower() == "float")
            return "GetSingle";
        else if (type.ToLower() == "double")
            return "GetDouble";
        else if (type.ToLower() == "bool")
            return "GetBoolean";
        else if (type.ToLower() == "byte")
            return "GetByte";
        return string.Empty;
    }

    public static string FirstCharToUpper(string value)
    {
        char[] a = value.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }
}
