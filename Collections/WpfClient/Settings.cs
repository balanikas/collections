using Collections;

namespace WpfClient
{
    public enum DrawTypes
    {
        Circle,
        Rectangle
    }


    internal class Settings
    {
        public static int Loops { get; set; }
        public static DrawTypes DrawAs { get; set; }

        public static ObjectType ThreadingType { get; set; }

        public static bool OptimizeRunners { get; set; }
    }
}