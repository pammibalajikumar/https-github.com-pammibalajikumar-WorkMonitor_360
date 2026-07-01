using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using WorkMonitor_360.ViewModels;
using System.IO;
using WorkMonitor_360.Services;
using WorkMonitor_360.View;

namespace WorkMonitor_360
{
    public partial class MainWindow : Window
    {        
        private readonly NotifyIcon _notifyIcon = new();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {

            string iconPath = Path.Combine(
           AppDomain.CurrentDomain.BaseDirectory,
           "Resources",
           "app.ico");

            _notifyIcon.Icon = new System.Drawing.Icon(iconPath);

            _notifyIcon.Visible = true;

            _notifyIcon.Text = "WorkTrack Lite";


            _notifyIcon.DoubleClick += (s, e) =>
            {
                System.Windows.Application.Current.MainWindow.Show();
                System.Windows.Application.Current.MainWindow.WindowState = WindowState.Normal;
                System.Windows.Application.Current.MainWindow.Activate();
            };
        }


        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _notifyIcon.Dispose();

            base.OnClosing(e);
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {           
            ReportFilterWindow reportFilterWindow=new ReportFilterWindow();
            reportFilterWindow.ShowDialog();        }


        private void AIREportButton_Click(object sender, RoutedEventArgs e)
        { AIAnalyticalFilterReportWindow aiAnalyticalReportWindow =new AIAnalyticalFilterReportWindow();
            aiAnalyticalReportWindow.ShowDialog();       
        
        }
    }
}