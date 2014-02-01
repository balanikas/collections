using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Collections;

namespace WpfClient
{
    internal class TextboxLogger : ILogger
    {
        private readonly Queue<LogMessage> _logBuffer;
        private readonly RichTextBox _textOutput;
        private int _errorCount;

        public TextboxLogger(RichTextBox textBox)
        {
            _textOutput = textBox;
            var doc = new FlowDocument();
            _textOutput.Document = doc;
            _logBuffer = new Queue<LogMessage>();
        }

        public int Count
        {
            get { return _logBuffer.Count; }
        }

        public int ErrorCount
        {
            get { return _errorCount; }
        }

        public void Info(string message)
        {
            _logBuffer.Enqueue(new LogMessage {IsError = false, Message = message});
        }

        public void InfoNow(string message)
        {
            _textOutput.Dispatcher.BeginInvoke((new Action(delegate
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
            _logBuffer.Enqueue(new LogMessage {IsError = true, Message = message});
        }

        public void ErrorNow(string message)
        {
            _errorCount++;
            _textOutput.Dispatcher.BeginInvoke((new Action(delegate
            {
                var paragraph = new Paragraph(new Run(message));
                paragraph.Foreground = new SolidColorBrush(Colors.Red);
                _textOutput.Document.Blocks.Add(paragraph);
                _textOutput.ScrollToEnd();
            })));
        }

        public void Flush()
        {
            var paragraph = new Paragraph();
            while (_logBuffer.Count > 0)
            {
                LogMessage logMessage = _logBuffer.Dequeue();

                string error = logMessage.IsError ? "ERROR" : "";

                var run = new Run(error + logMessage.Message + Environment.NewLine);
                run.Foreground = logMessage.IsError
                    ? new SolidColorBrush(Colors.Red)
                    : new SolidColorBrush(Colors.White);


                paragraph.Inlines.Add(run);
            }

            _textOutput.Dispatcher.BeginInvoke((new Action(delegate
            {
                _textOutput.Document.Blocks.Add(paragraph);
                _textOutput.ScrollToEnd();
            })));
        }

        private class LogMessage
        {
            public string Message { get; set; }
            public bool IsError { get; set; }
        }
    }
}