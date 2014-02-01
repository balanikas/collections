namespace Samples
{
    public class MethodInvocations
    {
        public void InvokeStaticMethod()
        {
            InstanceCalculate();
        }

        public void InvokeInstanceMethod()
        {
            StaticCalculate();
        }


        public void InvokeDelegate()
        {
            var calcDelegate = new CalculateDelegate(InstanceCalculate);
            calcDelegate();
        }


        private static void StaticCalculate()
        {
            int i = 0;
            int x = i++;
        }

        private void InstanceCalculate()
        {
            int i = 0;
            int x = i++;
        }

        private delegate void CalculateDelegate();
    }
}