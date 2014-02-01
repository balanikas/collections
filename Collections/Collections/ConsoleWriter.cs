using System.IO;
using System.Text;

namespace Collections
{
    public class ConsoleWriter : TextWriter
    {
        readonly ILogger _output;

        public ConsoleWriter(ILogger output)
        {
            _output = output;
        }

        public override void Write(string value)
        {
            base.Write(value);
            _output.InfoNow(value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
