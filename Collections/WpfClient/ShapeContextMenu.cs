using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    static class ShapeContextMenu
    {


        internal static ContextMenu Get(RoutedEventHandler closeAction = null, RoutedEventHandler infoAction = null)
        {
            var ctxMenu = new ContextMenu();

            var menus = new List<MenuItem>();
            var menuClose = new MenuItem { Header = "Close" };
            var menuInfo = new MenuItem { Header = "Info" };
            if (closeAction != null)
            {
                menuClose.Click += closeAction;
            }
            if (infoAction != null)
            {
                menuInfo.Click += infoAction;
            }

            menus.Add(menuClose);
            menus.Add(menuInfo);
            ctxMenu.ItemsSource = menus;

            return ctxMenu;
        }

       

    }
}
