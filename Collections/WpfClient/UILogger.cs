using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Collections;

namespace WpfClient
{
    class UILogger :ILogSubscriber
    {
        private FlowDocument _doc;

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
                    paragraph.Foreground = new SolidColorBrush(Colors.Red);
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
