using System;
using System.Windows;

namespace AI_NEWS
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Set up unhandled exception handling
            this.DispatcherUnhandledException += (s, args) =>
            {
                System.Windows.MessageBox.Show(
                    $"An error occurred:\n\n{args.Exception.Message}\n\nStack Trace:\n{args.Exception.StackTrace}",
                    "Application Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                args.Handled = true;
            };
        }
    }
}
