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
        class LogMessage
        {
            public string Message { get; set; }
            public bool IsError { get; set; }
        }

        Queue<LogMessage> _logBuffer;
        RichTextBox _textOutput;
        int _errorCount = 0;

        public TextboxLogger(RichTextBox textBox)
        {
            _textOutput = textBox;
            FlowDocument doc = new FlowDocument();
            _textOutput.Document = doc;
            _logBuffer = new Queue<LogMessage>();
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
            _logBuffer.Enqueue(new LogMessage { IsError = false, Message = message });
        }

        public void InfoNow(string message)
        {
            _textOutput.Dispatcher.BeginInvoke((new Action(delegate()
            {
                var paragraph = new Paragraph(new Run(message));
                paragraph.Foreground = new SolidColorBrush(Colors.White);
                _textOutput.Document.Blocks.Add(paragraph);
                _textOutput.ScrollToEnd();
            })));

            
        }

        public void Error(string message)
        {
            _errorCount++;
            _logBuffer.Enqueue(new LogMessage {  IsError = true, Message = message });
        }

        public void ErrorNow(string message)
        {
            _errorCount++;
            _textOutput.Dispatcher.BeginInvoke((new Action(delegate()
            {
                var paragraph = new Paragraph(new Run(message));
                paragraph.Foreground = new SolidColorBrush(Colors.Red);
                _textOutput.Document.Blocks.Add(paragraph);
                _textOutput.ScrollToEnd();
            })));


        }

        public void Flush()
        {
            return;
            var paragraphs = new List<Paragraph>(_logBuffer.Count);
            
            while (_logBuffer.Count > 0)
            {
                var logMessage = _logBuffer.Dequeue();
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run(logMessage.Message + Environment.NewLine));
                paragraph.Foreground = logMessage.IsError ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.White);

                paragraphs.Add(paragraph);
            }

            _textOutput.Dispatcher.BeginInvoke((new Action(delegate()
            {

                _textOutput.Document.Blocks.AddRange(paragraphs);
                _textOutput.ScrollToEnd();
            })));

        }
    }
}
