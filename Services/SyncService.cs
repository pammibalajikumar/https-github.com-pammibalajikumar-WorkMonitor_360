using System.Net.Http;
using System.Text;
using System.Text.Json;
using WorkMonitor_360.Services;
using WorkMonitor_360.Models;

namespace WorkMonitor_360.Services
{
    public class SyncService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        private readonly DatabaseService _databaseService;
        private NotificationService? _notificationService;

        public SyncService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _notificationService = new NotificationService();
        }

        public async Task SyncAsync()
        {
            var unsyncedSessions =
                await _databaseService.GetUnsyncedSessionsAsync();
            //write code here
            if (unsyncedSessions.Count > 0)
            {

                foreach (var session in unsyncedSessions)
                {
                    try
                    {
                        string json = JsonSerializer.Serialize(session);

                        var content = new StringContent(
                            json,
                            Encoding.UTF8,
                            "application/json");
                        //dummy API 
                        var response = await _httpClient.PostAsync(
                            "https://jsonplaceholder.typicode.com/posts",
                            content);

                        if (response.IsSuccessStatusCode)
                        {
                            await _databaseService.MarkAsSyncedAsync(session.SerialNo);
                        }
                    }
                    catch
                    {
                        // retry later
                    }
                }
                _notificationService?.ShowNotification("Data synced successfully");
            }
            else
            { 
                _notificationService?.ShowNotification("All data is already synced.");
                return;

            }
        }
    }
}