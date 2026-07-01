using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using WorkMonitor_360.Helpers;

using WorkMonitor_360.Models;
using WorkMonitor_360.Models.WorkMonitor_360.Models;
using WorkMonitor_360.Services;

using Serilog;  


namespace WorkMonitor_360.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly IdleDetectionService _idleService;
        private readonly NotificationService _notificationService;
        private readonly SyncService _syncService;
        private readonly ActivityTrackerService _activityTracker;

        private DispatcherTimer? _timer;
        private WorkSessionModel? _currentSession;
        private NotifyIcon? _notifyIcon;

        private bool _isCheckedIn;

        public ObservableCollection<WorkSessionModel> Sessions { get; }


        private ILogger _logger;

        public RelayCommand CheckInCommand { get; }
        public RelayCommand CheckOutCommand { get; }
        public RelayCommand SyncCommand { get; }

        public MainViewModel()
        {
            _databaseService = new DatabaseService();
            _idleService = new IdleDetectionService();
            _notificationService = new NotificationService();
            _syncService = new SyncService(_databaseService);
            _activityTracker = new ActivityTrackerService();

            Sessions = new ObservableCollection<WorkSessionModel>();

            CheckInCommand = new RelayCommand(CheckIn);
            CheckOutCommand = new RelayCommand(CheckOut);
            SyncCommand = new RelayCommand(async () => await SyncData());

            InitializeNotifyIcon();
            StartIdleMonitor();

            LoadSessions();

            _logger = Log.ForContext<RelayCommand>();

            _logger.Information("Started");
        }


        private void InitializeNotifyIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Application,
                Visible = true,
                Text = "WorkTrack Lite"
            };
        }

        private async void LoadSessions()
        {
            var data = await _databaseService.GetSessionsAsync();

            Sessions.Clear();

            foreach (var session in data)
            {
                Sessions.Add(session);
            }
        }

        private void CheckIn()
        {

            try

            {
                _logger.Information("Check in started");
                _isCheckedIn = true;

                _currentSession = new WorkSessionModel
                {
                    CheckInTime = DateTime.Now,
                    UserName = Environment.UserName,
                    MachineName = Environment.MachineName,
                    Status = "Checked In",
                    Synced = false
                };

                Sessions.Insert(0, _currentSession);

                _activityTracker.Start();

                System.Windows.Application.Current?.MainWindow?.Hide();

                if (_notifyIcon != null)
                {
                    _notifyIcon.Visible = true;

                    _notifyIcon.ShowBalloonTip(
                        3000,
                        "WorkTrack Lite",
                        "Checked in successfully. Activity tracking is running in the background.",
                        ToolTipIcon.Info);
                }
                _logger.Information("Check in finished");

            }
            catch (Exception ex)
            {
                _logger.Error("Error occured while Check in", ex);

            }      
            
        }

        private async void CheckOut()
        {

            try
            {
                _logger.Information("Check out started");
                if (!_isCheckedIn || _currentSession == null)
                    return;

                _isCheckedIn = false;

                _currentSession.CheckOutTime = DateTime.Now;
                _currentSession.Status = "Checked Out";

                _activityTracker.Stop();

                await _databaseService.InsertSessionAsync(_currentSession);

                LoadSessions();
                _logger.Information("Check out finished");
            }

            catch( Exception ex)
            {
                _logger.Error("error occured while check out",ex);              

            }           
            
        }

        private async Task SyncData()
        {
            try
            {
                _logger.Information("Sync data started");
                await _syncService.SyncAsync();                
            }
            catch (Exception ex)
            {

                _logger.Error("Error occured while Syncing the data", ex);

            }
        }


        private void StartIdleMonitor()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };

            _timer.Tick += Timer_Tick;
            _timer.Start();
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            var idleTime = _idleService.GetIdleTime();

            if (_isCheckedIn && idleTime.TotalMinutes >= 5)
            {
                CheckOut();

                _notificationService.ShowNotification(
                    "Automatically checked out due to inactivity");
            }

            if (!_isCheckedIn && idleTime.TotalSeconds < 5)
            {
                _notificationService.ShowNotification(
                    "Welcome back. Please check in again.");
            }
        }


    }
}