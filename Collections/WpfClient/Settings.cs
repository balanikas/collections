using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionsSOLID;

namespace WpfClient
{
    public  enum DrawTypes
    {
        Circle,
        Rectangle
    }

    

    class Settings
    {
        public static int Loops { get; set; }
        public static DrawTypes DrawAs { get; set; }

        public static ObjectType ThreadingType { get; set; }

    }
}
