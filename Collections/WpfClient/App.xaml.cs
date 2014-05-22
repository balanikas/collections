using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;

namespace WpfClient
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            App.Current.Properties.Add("startup",DateTime.Now);
            Debug.WriteLine("App ctor");
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("en");
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en");
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en");
        }



        protected override void OnStartup(StartupEventArgs e)
        {
            //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("zh-CN");
            base.OnStartup(e);

            var window = new MainWindow();

            
            window.Show();
          
        }

       
    }
}