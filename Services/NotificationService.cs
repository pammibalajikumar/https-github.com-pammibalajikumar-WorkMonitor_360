using System.Windows;

namespace WorkMonitor_360.Services
{
    public class NotificationService
    {
        public void ShowNotification(string message)
        {
            System.Windows.MessageBox.Show(message,
                "WorkTrackMonitor 360°",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
