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


        internal static ContextMenu Get(
            RoutedEventHandler closeAction = null,
            RoutedEventHandler infoAction = null,
            RoutedEventHandler showCodeAction = null)
        {
            var ctxMenu = new ContextMenu();

            var menus = new List<MenuItem>();
            var menuClose = new MenuItem { Header = "Close" };
            var menuInfo = new MenuItem { Header = "Info" };
            var menuShowCode = new MenuItem { Header = "Show code" };
           
            if (closeAction != null)
            {
                menuClose.Click += closeAction;
            }
            if (infoAction != null)
            {
                menuInfo.Click += infoAction;
            }

            if (showCodeAction != null)
            {
                menuShowCode.Click += showCodeAction;
            }
           
            menus.Add(menuInfo);
            menus.Add(menuShowCode);
            menus.Add(menuClose);

            ctxMenu.ItemsSource = menus;

            return ctxMenu;
        }

       

    }
}
