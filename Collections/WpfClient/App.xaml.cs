using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace WpfClient
{
    public partial class App : Application
    {

        static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");
        public App()
        {
            DispatcherUnhandledException +=App_DispatcherUnhandledException;
            
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "Fatal Error");

        }

        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {

            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("en");
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en");

                var window = new MainWindow();
                window.Show();

                base.OnStartup(e);
            }
            else
            {
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
                Shutdown();
                
            }
        }
    }
}