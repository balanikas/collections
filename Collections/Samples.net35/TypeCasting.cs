namespace Samples
{
    internal class TypeCasting
    {
        public static void ImplicitTypeCast()
        {
            var derived = new Derive();
            Base b = derived;
        }

        public static void ExplicitTypeCast()
        {
            var derived = new Derive();
            Base b = derived;
        }

        private class Base
        {
        }

        private class Derive : Base
        {
        }
    }
}