using System;
using System.Diagnostics;


[Serializable]
public class SampleUtils
{
    public static string ConcatenateStrings(string s1, string s2)
    {
        return s1 + s2;
    }

    public static void Calculate()
    {
        int x = 0;
        for (int i = 0; i < 1000; i++)
        {
            x++;
        }
    }

    public void NonStaticCalculate()
    {
        int x = 0;
        for (int i = 0; i < 1000; i++)
        {
            x++;
        }
    }

    private void Secret()
    {

    }

    public string AddDifferentTypes(sbyte sb, byte by, Int16 i16, UInt16 ui16, Int32 i32, UInt32 ui32, Int64 i64, UInt64 ui64, 
        float f, double d, bool bo, char c, object o, string s)
    {

        return "";
    }

    public string ConcatenateObjects(object o1, object o2)
    {
        return o1.ToString() + o2.ToString();
    }


}

