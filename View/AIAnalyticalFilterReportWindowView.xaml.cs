using Microsoft.Extensions.Diagnostics.Metrics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WorkMonitor_360.Models;
using WorkMonitor_360.Services;
using WorkMonitor_360.ViewModels;
using Serilog;

namespace WorkMonitor_360.View
{
    /// <summary>
    /// Interaction logic for AIAnalyticalReport.xaml
    /// </summary>
    public partial class AIAnalyticalFilterReportWindow : Window
    {

        private ILogger _logger;
        // public ObservableCollection<string> HostNames = new ObservableCollection<string>();
        //public ObservableCollection<string> Hostnames { get;set;}
        public AIAnalyticalFilterReportWindow()
        {
            InitializeComponent();
            DataContext = new AIAnalyticalReportViewModel();
            _logger = Log.Logger; ;
        }       

        private async void BtnGenerateAIReport_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                 
                _logger.Information("Ai Report Processing Started");
                if (FromDate.SelectedDate == null || ToDate.SelectedDate == null)
                {
                    NotificationService ns = new NotificationService();
                    ns.ShowNotification("Please select both From Date and To Date.");
                    return;
                }

                if (DataContext is AIAnalyticalReportViewModel aiAnalyticalReport)
            {
                await aiAnalyticalReport.GenerateAIReport(
                    hostName.Text,
                    userName.Text,
                    FromDate.SelectedDate.Value,
                    ToDate.SelectedDate.Value);
                _logger.Information("Ai Report Processing Finished");

            }

            }

            catch(Exception ex)
            {
                _logger.Information("Error Occured generating the Ai Report");
                _logger.Error(ex.ToString());
            }
        }
    }
}
