using System;
using System.Windows.Documents;
using System.Windows.Media;
using Collections;
using Collections.Logging;

namespace WpfClient
{
    internal class UILogger : ILogSubscriber
    {
        private readonly FlowDocument _doc;

        public UILogger(FlowDocument doc)
        {
            _doc = doc;
        }

        public void Notify(LogMessage message)
        {
            if (message.IsError)
            {
                _doc.Dispatcher.BeginInvoke((new Action(delegate
                {
                    var paragraph = new Paragraph(new Run(message.Message));
                    paragraph.Foreground = new SolidColorBrush(Colors.Yellow);
                    _doc.Blocks.Add(paragraph);
                    //_rtb.ScrollToEnd();
                })));
            }
            else
            {
                _doc.Dispatcher.BeginInvoke((new Action(delegate
                {
                    var paragraph = new Paragraph(new Run(message.Message));
                    paragraph.Foreground = new SolidColorBrush(Colors.White);
                    _doc.Blocks.Add(paragraph);
                    //_rtb.ScrollToEnd();
                })));
            }
        }
    }
}