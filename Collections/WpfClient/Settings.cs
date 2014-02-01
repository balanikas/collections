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

        public static RunnerType ThreadingType { get; set; }

    }
}