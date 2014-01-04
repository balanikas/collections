using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionsSOLID;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfClient
{
    class TextboxLogger : ILogger
    {
        Queue<string> _logBuffer;
        RichTextBox _textOutput;
        int _errorCount = 0;

        public TextboxLogger(RichTextBox textBox)
        {
            _textOutput = textBox;
            FlowDocument doc = new FlowDocument();
            _textOutput.Document = doc;
            _logBuffer = new Queue<string>();
        }

        public int Count { 
            get {
                return _logBuffer.Count;
            } 
        }

        public int ErrorCount
        {
            get
            {
                return _errorCount;
            }
        }

        public void Info(string message)
        {
            _logBuffer.Enqueue(message);
        }

        public void InfoNow(string message)
        {
            _textOutput.Dispatcher.BeginInvoke((new Action(delegate()
            {
                var paragraph = new Paragraph(new Run(message));
                _textOutput.Document.Blocks.Add(paragraph);

            })));

            
        }

        public void Error(string message)
        {
            _errorCount++;
            _logBuffer.Enqueue(message);
        }

        public void ErrorNow(string message)
        {
            _errorCount++;
            _textOutput.Dispatcher.BeginInvoke((new Action(delegate()
            {
                var paragraph = new Paragraph(new Run(message));
                _textOutput.Document.Blocks.Add(paragraph);

            })));


        }

        public void Flush()
        {
            var contentToFlush = new StringBuilder();

            while(_logBuffer.Count > 0)
            {
                contentToFlush.Append(_logBuffer.Dequeue() + Environment.NewLine);
            }
            

            _textOutput.Dispatcher.BeginInvoke((new Action(delegate()
            {
                var paragraph = new Paragraph(new Run(contentToFlush.ToString()));
                paragraph.Foreground = Utils.PickBrush();
                _textOutput.Document.Blocks.Add(paragraph);
                _textOutput.ScrollToEnd();
            })));
            
        }
    }
}
