using System;

namespace Samples
{
    internal class Exceptions
    {
        public static void NoCheck(uint i)
        {
            if (i < int.MaxValue / 100)
            {
                throw new Exception();
            }
        }

        public static void GenericCheck(uint i)
        {
            try
            {
                if (i < int.MaxValue / 100)
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
            }
        }

        public static void GenericCheckWithFinally(uint i)
        {
            try
            {
                if (i < int.MaxValue / 100)
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
            }
        }
    }
}