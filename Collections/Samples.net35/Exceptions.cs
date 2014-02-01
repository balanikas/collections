using System;

namespace Samples
{
    internal class Exceptions
    {
        public static void NoCheck(int i)
        {
            if (i == 0)
            {
                throw new Exception();
            }
        }

        public static void GenericCheck(int i)
        {
            try
            {
                if (i == 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
            }
        }

        public static void GenericCheckWithFinally(int i)
        {
            try
            {
                if (i == 0)
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