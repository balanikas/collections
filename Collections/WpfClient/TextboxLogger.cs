using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionsSOLID;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WpfClient
{
    class TextboxLogger : ILogger
    {
        RichTextBox _textOutput;
        public TextboxLogger(RichTextBox textBox)
        {
            _textOutput = textBox;
            FlowDocument myFlowDoc = new FlowDocument();
            _textOutput.Document = myFlowDoc;

        }

        public void Write(string message)
        {
           
            _textOutput.Document.Blocks.Add(new Paragraph(new Run(message)));
            
            
        }
    }
}
