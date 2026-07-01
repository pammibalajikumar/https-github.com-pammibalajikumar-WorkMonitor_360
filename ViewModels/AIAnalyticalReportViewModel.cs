using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
//using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WorkMonitor_360.Models;
using WorkMonitor_360.Services;
using WorkMonitor_360.View;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WorkMonitor_360.ViewModels
{
        public class AIAnalyticalReportViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> HostNames { get; set; }
        public ObservableCollection<string> UserNames { get; set; }

        //private  NotificationService? notificationService;


        private string _selectedHostName=string.Empty;

        public AIAnalyticalReportViewModel()
        {
            HostNames = new ObservableCollection<string>();
            GetHostNames();
            UserNames = new ObservableCollection<string>();
            //NotificationService notificationService = new NotificationService();

        }
        public string SelectedHostName
        {
            get => _selectedHostName;
            set
            {
                _selectedHostName = value;
                OnPropertyChanged();

                GetUserNames(_selectedHostName);

                //GetUserNames();
            }
        }
        private string _selectedUserName= string.Empty;
        public string SelectedUserName
        {
            get => _selectedUserName;
            set
            {
                _selectedUserName = value;
                OnPropertyChanged();                
            }
        }

        private DateTime? _fromDate;
        [Required(ErrorMessage = "From Date is required.")]        

        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                _fromDate = value;               
                OnPropertyChanged();
            }
        }

        private DateTime? _toDate;

        [Required(ErrorMessage = "To Date is required.")]
        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                _toDate = value;
                //ValidateProperty(value, nameof(ToDate));
                OnPropertyChanged();
            }
        }

        // public string AISummary { get; set; }

        private string _aiSummary = string.Empty;

        public string AISummary
        {
            get => _aiSummary;
            set
            {
                _aiSummary = value;
                OnPropertyChanged();
            }
        }

      

            private void GetHostNames()

            {
                DatabaseService service = new DatabaseService();
                foreach (var host in service.LoadHostNames())
                {
                    HostNames.Add(host);
                }
            }
      
        private void GetUserNames(string machineName )

        {
            DatabaseService service = new DatabaseService();
            foreach (var host in service.LoadUserNames(machineName))
            {
                UserNames.Add(host);
            }
        }       


        //public void GenerateAIReport(string hostname,string username,DateTime fromDate, DateTime toDate)
        public async Task GenerateAIReport(string hostname, string username, DateTime fromDate, DateTime toDate)
        {            

            AISummary = string.Empty;
            if (!ValidateFields())
            {
                return;            
            }

            DatabaseService ds = new DatabaseService();
            AIAnalyticalReportModel? aiReport = new AIAnalyticalReportModel();
            aiReport = ds.GetEmployeeWorkSummary(hostname, username, fromDate, toDate);
            AIService ais = new AIService();

            if (aiReport == null)
             {
                NotificationService notificationService = new NotificationService();
                notificationService.ShowNotification("No data found.");
                return;
            }
            
            AISummary = await ais.GenerateReportAsync(aiReport.HostName, aiReport.UserName, aiReport.TotalTimeSpent, aiReport.IdleTime);
        }

        public bool ValidateFields()
        {
            NotificationService notificationService = new NotificationService();
            if (string.IsNullOrWhiteSpace(SelectedHostName))
            {
                notificationService.ShowNotification("Please select a machine.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(SelectedUserName))
            {
                notificationService.ShowNotification("Please select a user.");
                return false;
            }

            if (FromDate == null)
            {
                notificationService.ShowNotification("Please select From Date.");
                return false;
            }

            if (ToDate == null)
            {
                notificationService.ShowNotification("Please select To Date.");
                return false;
            }

            if (FromDate > ToDate)
            {
                notificationService.ShowNotification("From Date cannot be greater than To Date.");
                return false;
            }

            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string? propertyName = null)

        { 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));        
        
        }

    }
}
