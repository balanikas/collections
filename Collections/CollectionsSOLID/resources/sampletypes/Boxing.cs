using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    class Boxing
    {
        public void Box()
        {
            int i = 1;
            object j = (object)i;
        }

        public void UnBox()
        {
            int i = 1;
            object j = i;
        }

    }
}
  

