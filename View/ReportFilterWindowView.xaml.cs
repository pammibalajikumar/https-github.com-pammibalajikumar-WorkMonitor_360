using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
using WorkMonitor_360.Services;

namespace WorkMonitor_360.View
{
    /// <summary>
    /// Interaction logic for ReportFilterWindow.xaml
    /// </summary>
    public partial class ReportFilterWindow : Window
    {

        private NotificationService notificationService;
        private ReportService reportervice;

        public ReportFilterWindow()
        {
            InitializeComponent();
            notificationService = new NotificationService();
            reportervice=new ReportService();

        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {


            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                notificationService.ShowNotification("Please select both start and end dates.");
                return;
            }
            //reportervice.GenerateEmployeeReport(dpStartDate.SelectedDate.Value, dpEndDate.SelectedDate.Value);

            DataTable dt = reportervice.GetEmployeeData(dpStartDate.SelectedDate.Value, dpEndDate.SelectedDate.Value);          
            
            
            if (dt.Rows.Count == 0)
            {                
                notificationService.ShowNotification("No data found for the selected date range.");
                return;
            }
            reportervice.GenerateExpenseReport(dt);            
        
        }
    }    
}
