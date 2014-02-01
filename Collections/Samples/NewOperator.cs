namespace Samples
{
    internal class NewOperator
    {
        public static void NoNewOperator()
        {
            for (int i = 0; i < 100; i++)
            {
                int a;
                a = 100;
            }
        }

        public static void UsingNewOperator()
        {
            for (int i = 0; i < 100; i++)
            {
                var a = new int();
                a = 100;
            }
        }
    }
}