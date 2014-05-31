using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient.Shapes
{
    internal static class ShapeContextMenu
    {
        internal static ContextMenu Create(
            RoutedEventHandler closeAction = null,
            RoutedEventHandler infoAction = null,
            RoutedEventHandler showCodeAction = null)
        {
            var ctxMenu = new ContextMenu();

            var menus = new List<MenuItem>();
            var menuClose = new MenuItem {Header = "Close"};
            var menuInfo = new MenuItem {Header = "Info"};
            var menuShowCode = new MenuItem {Header = "Show code"};

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