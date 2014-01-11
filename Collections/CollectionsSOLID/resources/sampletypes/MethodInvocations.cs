using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    

    public class MethodInvocations
    {
        private delegate void CalculateDelegate(); 
        public void InvokeStaticMethod()
        {
            this.InstanceCalculate();
        }

        public void InvokeInstanceMethod()
        {
            MethodInvocations.StaticCalculate();
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
       
    }
}
