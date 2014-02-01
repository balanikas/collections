namespace Samples
{
    internal class Conditionals
    {
        public static bool IsValidIf(uint i)
        {
            i = i%6;

            if (i == 0 || i == 1)
            {
                return true;
            }
            if (i == 2 || i == 3)
            {
                return false;
            }
            if (i == 4 || i == 5)
            {
                return true;
            }
            return false;
        }

        public static bool IsValidSwitch(int i)
        {
            i = i%6;

            switch (i)
            {
                case 0:
                case 1:
                    return true;
                case 2:
                case 3:
                    return false;
                case 4:
                case 5:
                    return true;
                default:
                    return false;
            }
        }
    }
}