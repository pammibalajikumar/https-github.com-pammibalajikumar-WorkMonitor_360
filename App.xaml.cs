using System.Configuration;
using System.Data;
using System.Windows;
using QuestPDF.Infrastructure;
using Serilog;

namespace WorkMonitor_360
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {            
            
            QuestPDF.Settings.License=LicenseType.Community;

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()           
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt",
            rollingInterval: RollingInterval.Day
            ,retainedFileCountLimit:31)
            .CreateLogger();
            Log.Information("Application Started");
            
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("application closed");
            base.OnExit(e);
        }
    }
}